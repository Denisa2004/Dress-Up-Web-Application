using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dress_Up.Models;
using Microsoft.EntityFrameworkCore;
using Dress_Up.Data;

namespace Dress_Up.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext db;

        public EventController(ApplicationDbContext context)
        {
            db = context;
        }


        public async Task<IActionResult> Index()
        {
            var events = await db.Events
                .Include(e => e.UserEvents)
                .ToListAsync();

            return View(events);
        }


        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            var ev = await db.Events
                .Include(e => e.UserEvents)
                    .ThenInclude(ue => ue.User)
                .Include(e => e.UserEvents)
                    .ThenInclude(ue => ue.Outfit)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
            {
                return NotFound();
            }

            return View(ev);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> New(Event ev)
        {
            if (ModelState.IsValid)
            {
                ev.IsGoing = true; 
                db.Events.Add(ev);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        [HttpPost]
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
            // gasit castigator prin voturi
            var castigator = ev.Votes
                .Where(v => v.Outfit != null)
                .GroupBy(v => v.OutfitId)
                .Select(g => new
                {
                    OutfitId = g.Key,
                    Count = g.Count(),
                    OutfitName = g.First().Outfit.Title
                })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            if (castigator != null)
            {
                ev.Description = "Castigatorul este: {castigator.OutfitName}";
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: /Event/Participate/5
        [HttpGet]
        public async Task<IActionResult> Participate(int eventId)
        {
            var ev = await db.Events.FindAsync(eventId);
            if (ev == null || !ev.IsActive)
                return NotFound();

            var user = await db.Users
                .Include(u => u.Outfits)
                .FirstOrDefaultAsync(u => u.Username == User.Identity.Name);

            if (user == null)
                return Unauthorized();

            ViewBag.EventId = eventId;
            return View(user.Outfits.ToList());
        }

        // POST: /Event/Participate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Participate(int eventId, int outfitId)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            if (user == null)
                return Unauthorized();

            var outfit = await db.Outfits.FirstOrDefaultAsync(o => o.Id == outfitId && o.UserId == user.Id);
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

            return RedirectToAction("Index","Event");
        }

    }

}
