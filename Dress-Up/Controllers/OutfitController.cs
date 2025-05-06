using System;
using System.Linq;
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dress_Up.Controllers
{
    public class OutfitController : Controller
    {
        private readonly ApplicationDbContext _context;    
        private readonly UserManager<User> _userManager;

        public OutfitController(ApplicationDbContext context,
                                UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /*────────── Edit ──────────*/
        public IActionResult Edit(int id)
        {
            var outfit = _context.Outfits.Find(id);
            return View(outfit);
        }

        [HttpPost]
        public IActionResult Edit(int id, Outfit outfit)
        {
            if (!ModelState.IsValid) return View(outfit);

            var existing = _context.Outfits
                                   .Include(o => o.User)
                                   .FirstOrDefault(o => o.Id == outfit.Id);

            if (existing == null) return NotFound();

            existing.Name = outfit.Name;
            existing.Description = outfit.Description;
            existing.IsPublic = outfit.IsPublic;

            _context.SaveChanges();
            return RedirectToAction("Index", "User", new { id = existing.User.Id });
        }

        /*────────── Delete ──────────*/
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var outfit = _context.Outfits.Find(id);
            var user = _userManager.GetUserAsync(User).Result;

            if (outfit != null)
            {
                user.Outfits.Remove(outfit);
                _context.Outfits.Remove(outfit);
                _context.SaveChanges();
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

            var outfit = _context.Outfits.Find(outfitId);
            if (outfit == null) return NotFound();

            string userId = User.Identity.IsAuthenticated
                ? _userManager.GetUserId(User)
                : null;

            _context.Comments.Add(new Comment
            {
                OutfitId = outfitId,
                UserId = userId,
                Content = content,
                Date_created = DateTime.Now,
                Date_updated = DateTime.Now
            });
            _context.SaveChanges();

            return RedirectToAction("HomeGuest", "Home");
        }


        /*────────── Like post  ──────────*/

        [HttpPost("Like/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Like(int id)
        {
            var outfit = _context.Outfits.Find(id);
            if (outfit == null) return NotFound();

            string userId = User.Identity.IsAuthenticated
                ? _userManager.GetUserId(User)      // id-ul userului logat
                : null;                             // guest → null

            bool already = _context.Votes.Any(v =>
                v.OutfitId == id &&
                v.EventId == null &&               // like, nu vot 
                v.UserId == userId);

            if (!already)
            {
                _context.Votes.Add(new Vote
                {
                    OutfitId = id,
                    UserId = userId,
                    EventId = null,              // diferențiator „like”
                    Date_Voted = DateTime.Now
                });
                _context.SaveChanges();
            }

            var referer = Request.Headers["Referer"].ToString();   // pagina anterioară
            return !string.IsNullOrWhiteSpace(referer)
                   ? Redirect(referer)
                   : RedirectToAction("HomeGuest", "Home");
        }

        /*────────────────  DELETE COMMENT  ────────────────*/
        [HttpPost("DeleteComment/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            if (comment == null) return NotFound();

            bool isOwner = comment.UserId == _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");

            if (!isOwner && !isAdmin) return Forbid();   // 403

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        /*────────────────  EDIT COMMENT  (POST) ───────────*/
        [HttpPost("EditComment")]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(int id, string content)
        {
            var comment = _context.Comments.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            if (comment == null) return NotFound();

            bool isOwner = comment.UserId == _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");
            if (!isOwner && !isAdmin) return Forbid();

            if (!string.IsNullOrWhiteSpace(content))
            {
                comment.Content = content.Trim();
                comment.Date_updated = DateTime.Now;
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }


    }
}
