
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dress_Up.Controllers;

public class OutfitController(ApplicationDbContext context, UserManager<User> userManager) : Controller
{
    private readonly ApplicationDbContext db = context;
    private readonly UserManager<User> _userManager = userManager;

    //vizualizare intgrala a unui outfit
    [HttpGet]
    public IActionResult Show(int id)
    {
        var outfit = db.Outfits.Include(o => o.User).FirstOrDefault(o => o.Id == id);
        ViewBag.Outfit = outfit;
        return View();
    }

    // editarea nmelui, descrierii si vizibilitatii unui outfit
    public ActionResult Edit(int id)
    {
        var otd = db.Outfits.Find(id);

        return View(otd);
    }

    [HttpPost]
    public ActionResult Edit(int id, Outfit outfit)
    {
        if (ModelState.IsValid)
        {
            // obtin obiectul din DB
            var existingOutfit = db.Outfits.Include(o => o.User).FirstOrDefault(o => o.Id == outfit.Id);

            //  doar campurile editabile
            existingOutfit.Name = outfit.Name;
            existingOutfit.Description = outfit.Description;
            existingOutfit.IsPublic = outfit.IsPublic;

            db.SaveChanges();
            return RedirectToAction("Index", "User", new { id = existingOutfit.User.Id });
        }

        return View(outfit);
    }

    //stergere outfit
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var outfit = db.Outfits.Find(id);
        var user = _userManager.GetUserAsync(User).Result;

        if (outfit != null)
        {
            // sterg toate UserEvents asociate acestui outfit
            var relatedEvents = db.UserEvents.Where(ue => ue.OutfitId == id).ToList();
            db.UserEvents.RemoveRange(relatedEvents);

            // sterg toate comentariile asociate acestui outfit
            var comms = db.Comments.Where(c => c.OutfitId == id).ToList();
            db.Comments.RemoveRange(comms);

            // sterg toate voturile asociate acestui outfit
            var votes = db.Votes.Where(v => v.OutfitId == id).ToList();
            db.Votes.RemoveRange(votes);

            // elimin outfit-ul din lista userului
            user.Outfits.Remove(outfit);

            // sterg outfitul din baza de date
            db.Outfits.Remove(outfit);

            db.SaveChanges();
        }

        return RedirectToAction("Index", "User", new { id = user.Id });
    }

    /*────────── AddComment  ──────────*/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int outfitId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return RedirectToAction("HomeGuest", "Home");

        var outfit = db.Outfits.Find(outfitId);
        if (outfit == null) return NotFound();

        string userId = User.Identity.IsAuthenticated
            ? _userManager.GetUserId(User)
            : null;

        db.Comments.Add(new Comment
        {
            OutfitId = outfitId,
            UserId = userId,
            Content = content,
            Date_created = DateTime.Now,
            Date_updated = DateTime.Now
        });
        db.SaveChanges();

        return RedirectToAction("HomeGuest", "Home");
    }


    /*────────── Like post  ──────────*/

    [HttpPost("Like/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Like(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(); // Blochează like‑ul pentru guest

        var outfit = db.Outfits.Find(id);
        if (outfit == null) return NotFound();

        string userId = _userManager.GetUserId(User);

        var existingLike = db.Votes.FirstOrDefault(v =>
            v.OutfitId == id &&
            v.EventId == null &&
            v.UserId == userId);

        if (existingLike != null)
        {
            db.Votes.Remove(existingLike);
        }
        else
        {
            db.Votes.Add(new Vote
            {
                OutfitId = id,
                UserId = userId,
                EventId = null,
                Date_Voted = DateTime.Now
            });
        }
        db.SaveChanges();

        var referer = Request.Headers["Referer"].ToString();
        return !string.IsNullOrWhiteSpace(referer)
               ? Redirect(referer)
               : RedirectToAction("HomeGuest", "Home");
    }

    /*────────────────  DELETE COMMENT  ────────────────*/
    [HttpPost("DeleteComment/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteComment(int id)
    {
        var comment = db.Comments.Include(c => c.User).FirstOrDefault(c => c.Id == id);
        if (comment == null) return NotFound();

        bool isOwner = comment.UserId == _userManager.GetUserId(User);
        bool isAdmin = User.IsInRole("Admin");

        if (!isOwner && !isAdmin) return Forbid();   // 403

        db.Comments.Remove(comment);
        db.SaveChanges();

        return Redirect(Request.Headers["Referer"].ToString());
    }

    /*────────────────  EDIT COMMENT  (POST) ───────────*/
    [HttpPost("EditComment")]
    [ValidateAntiForgeryToken]
    public IActionResult EditComment(int id, string content)
    {
        var comment = db.Comments.Include(c => c.User).FirstOrDefault(c => c.Id == id);
        if (comment == null) return NotFound();

        bool isOwner = comment.UserId == _userManager.GetUserId(User);
        bool isAdmin = User.IsInRole("Admin");
        if (!isOwner && !isAdmin) return Forbid();

        if (!string.IsNullOrWhiteSpace(content))
        {
            comment.Content = content.Trim();
            comment.Date_updated = DateTime.Now;
            db.SaveChanges();
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }


}
