
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OutfitController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<User> _userManager;
    public OutfitController(
    ApplicationDbContext context,
    UserManager<User> userManager
    )
    {
        db = context;
        _userManager = userManager;
    }

    // GET: Outfit/Edit
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

    [HttpPost]
    public ActionResult Delete(int id)
    {
        var otd = db.Outfits.Find(id);
        var user = _userManager.GetUserAsync(User).Result;
        if (otd != null)
        {
            user.Outfits.Remove(otd);
            db.Outfits.Remove(otd);
            db.SaveChanges();
        }
        return RedirectToAction("Index", "User", new { id = user.Id });
    }
}
