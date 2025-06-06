using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.Outfit)
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
            .Include(e => e.Votes)
                .ThenInclude(v => v.Outfit)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();

        return View(ev);
    }

    [HttpGet("new")]
    [Authorize(Roles = "Admin")]
    public IActionResult New()
    {
        return View();
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
            return RedirectToAction("Index");
        }

        return View(ev);
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

        ev.Description = castigator != null
            ? $"Castigatorul este: {castigator.OutfitName}"
            : "Nu există castigator.";

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await db.Events
            .Include(e => e.UserEvents)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();

        if (ev.UserEvents != null)
        {
            db.UserEvents.RemoveRange(ev.UserEvents);
        }

        db.Events.Remove(ev);
        await db.SaveChangesAsync();

        TempData["message"] = "Concursul a fost eliminat.";
        TempData["messageType"] = "alert-success";

        return RedirectToAction("Index");
    }



    [HttpGet("participate/{eventId}")]
    [Authorize]
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
        return View(user.Outfits.ToList());
    }



    [HttpPost("participate/{eventId}")]
    [Authorize]
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
        {
            TempData["message"] = "Outfit invalid";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Participate", new { eventId });
        }
            

        bool alreadyJoined = await db.UserEvents
            .AnyAsync(ue => ue.UserId == user.Id && ue.EventId == eventId);

        if (alreadyJoined)
        {
            TempData["message"] = "Poti participa cu un singur outfit la eveniment";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Participate", new { eventId });
        }
            

        var userEvent = new UserEvent
        {
            UserId = user.Id,
            EventId = eventId,
            OutfitId = outfitId
        };

        db.UserEvents.Add(userEvent);
        await db.SaveChangesAsync();

        return RedirectToAction("Index", "Event");
    }


    [HttpGet("vote/{eventId}")]
    [Authorize]
    public async Task<IActionResult> Vote(int eventId)
    {
        var ev = await db.Events
            .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.Outfit)
            .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.User)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null || !ev.IsActive)
            return NotFound();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null)
            return Unauthorized();

        ViewBag.EventId = eventId;
        return View("Vote", ev);  
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SubmitVote(int eventId, int outfitId)
    {
        var ev = await db.Events
            .Include(e => e.Votes)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null || !ev.IsActive)
            return NotFound();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null)
            return Unauthorized();

        var alreadyVoted = await db.Votes
            .AnyAsync(v => v.EventId == eventId && v.UserId == user.Id);

        if (alreadyVoted)
        {
            TempData["message"] = "Fiecare participant poate vota o singura data.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Vote", new { eventId });
        }

        var vote = new Vote
        {
            UserId = user.Id,
            OutfitId = outfitId,
            EventId = eventId,
            Date_Voted = DateTime.Now
        };

        ev.Votes.Add(vote);
        db.Votes.Add(vote);
        await db.SaveChangesAsync();

        return RedirectToAction("Show","Event", new { id = eventId });
    }

}
