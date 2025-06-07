using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dress_Up.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly AchievementService _achievementService;

    public UserController(ApplicationDbContext context, UserManager<User> userManager, AchievementService achievementService)
    {
        _context = context;
        _userManager = userManager;
        _achievementService = achievementService;
    }

    public IActionResult Index(string id)
    {
        var userId = _userManager.GetUserId(User);
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Msg = TempData["message"].ToString();
        }

        if (user == null)
        {
            return NotFound();
        }

        var achievements = _achievementService.GetUserAchievementsAsync(id);
        ViewBag.UserAchievements = achievements;


        //daca este contul propriu sau admin afisez si categoriile private
        if (userId == id || User.IsInRole("Admin"))
        {
            var UserOutfits = _context.Outfits.Where(u => u.User.Id == id);
            ViewBag.UserOutfits = UserOutfits;
            ViewBag.AfisareButoane = true; //pentru a sti daca afisez butoanele de editare sau nu
            return View(user);
        }
        else
        {
            var UserOutfits = _context.Outfits.Where(u => u.User.Id == id && u.IsPublic == true);
            ViewBag.UserOutfits = UserOutfits;
            ViewBag.AfisareButoane = false;

            return View(user);
        }


    }

    public IActionResult PublicaOutfit(int id)
    {
        var outfit = _context.Outfits.Include(o => o.User).FirstOrDefault(o => o.Id == id);
        if (outfit != null)
        {
            outfit.IsPublic = true;
            _context.SaveChanges();
            TempData["message"] = "Outfit-ul a fost publicat cu succes!";
        }
        else
        {
            TempData["message"] = "Outfit-ul nu a fost gasit!";
        }
        return Redirect("/User/Index/" + outfit!.User!.Id);
    }

    [HttpGet]
   /* [Authorize(Roles = "Admin,User")]*/
    public async Task<IActionResult> Edit()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("User/Edit")]
    public async Task<IActionResult> Edit(User model)
    {
        Console.WriteLine("MODEL ID: " + model.Id);

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.About = model.About;
            user.ProfilePictureUrl = model.ProfilePictureUrl;

            // Actualizează utilizatorul în baza de date
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "User", new { id = user.Id });


            }

            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(model);
    }

}
