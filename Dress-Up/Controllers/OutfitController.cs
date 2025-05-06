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

            // elimin outfit-ul din lista userului
            user.Outfits.Remove(outfit);

            // sterg outfitul din baza de date
            db.Outfits.Remove(outfit);

            db.SaveChanges();
        }

        return RedirectToAction("Index", "User", new { id = user.Id });
    }

}
