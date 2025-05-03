using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

[Route("event")]
public class EventController : Controller
{
    private readonly ApplicationDbContext db;

    public EventController(ApplicationDbContext context)
    {
        db = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var events = await db.Events
            .Where(e => e.IsActive)
            .Include(e => e.UserEvents)
            .ToListAsync();

        return View(events);
    }



    [HttpGet("show/{id}")]
    public async Task<IActionResult> Show(int id)
    {
        var ev = await db.Events
            .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.User)
            .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.Outfit)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();

        return View(ev); // => /event/show/5
    }

    [HttpGet("new")]
    [Authorize(Roles = "Admin")]
    public IActionResult New()
    {
        return View(); // => /event/new
    }

    [HttpPost("new")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> New(Event ev)
    {
        if (ModelState.IsValid)
        {
            ev.IsActive = true;
            db.Events.Add(ev);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Event");

        }

        return View(new Event());
    }

    [HttpPost("stop/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Stop(int id)
    {
        var ev = await db.Events
            .Include(e => e.Votes)
                .ThenInclude(v => v.Outfit)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();

        ev.IsActive = false;

        var castigator = ev.Votes
            .Where(v => v.Outfit != null)
            .GroupBy(v => v.OutfitId)
            .Select(g => new
            {
                OutfitId = g.Key,
                Count = g.Count(),
                OutfitName = g.First().Outfit.Name
            })
            .OrderByDescending(g => g.Count)
            .FirstOrDefault();

        if (castigator != null)
        {
            ev.Description = $"Castigatorul este: {castigator.OutfitName}";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); // => POST /event/stop/5
    }


    //stergerea unui bookmark
    [HttpPost]
    [Authorize(Roles = "Admin")]

    public ActionResult Delete(int id)
    {
        var @event = db.Events
            .Include(e => e.UserEvents)
            .FirstOrDefault(e => e.Id == id);

        if (@event == null)
        {
            return NotFound();
        }

        if (User.IsInRole("Admin"))
        {

            if (@event.UserEvents != null && @event.UserEvents.Any())
            {
                db.UserEvents.RemoveRange(@event.UserEvents);
            }

            db.Events.Remove(@event);
            db.SaveChanges();

            TempData["message"] = "Concursul a fost eliminat.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }


    [HttpGet("participate/{eventId}")]
    public async Task<IActionResult> Participate(int eventId)
    {
        var ev = await db.Events.FindAsync(eventId);
        if (ev == null || !ev.IsActive)
            return NotFound();

        var user = await db.Users
            .Include(u => u.Outfits)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null)
            return Unauthorized();

        ViewBag.EventId = eventId;
        return View(user.Outfits.ToList()); // => GET /event/participate/5
    }

    [HttpPost("participate/{eventId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Participate(int eventId, int outfitId)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
        if (user == null)
            return Unauthorized();

        var outfit = await db.Outfits
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == outfitId && o.User.Id == user.Id);

        if (outfit == null)
            return BadRequest("Outfit invalid");

        bool alreadyJoined = await db.UserEvents
            .AnyAsync(ue => ue.UserId == user.Id && ue.EventId == eventId);

        if (alreadyJoined)
            return BadRequest("Poti participa cu un singur outfit la un eveniment.");

        var userEvent = new UserEvent
        {
            UserId = user.Id,
            EventId = eventId,
            OutfitId = outfitId
        };

        db.UserEvents.Add(userEvent);
        await db.SaveChangesAsync();

        return RedirectToAction("Index", "Event"); // => POST /event/participate/5
    }
}
