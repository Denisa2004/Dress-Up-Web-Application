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
    private readonly SignInManager<User> _signInManager;
    private readonly AchievementService _achievementService;

    public UserController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager , AchievementService achievementService)
    {
        _context = context;
        _userManager = userManager;
        _achievementService = achievementService;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Index(string id)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Msg = TempData["message"].ToString();
        }

        if (user == null)
        {
            return NotFound();
        }

        var messages = _context.AlertMessages
             .Where(m => m.UserId == user.Id && !m.IsRead)
             .OrderByDescending(m => m.SentAt)
             .ToList();

        ViewBag.AlertMessages = messages;

        if (User.IsInRole("Admin"))
        {
            var allUsers = _context.Users.ToList();
            ViewBag.AllUsers = allUsers;
        }


        // Așteptăm rezultatul corect
        var achievements = await _achievementService.GetUserAchievementsAsync(userId);
        ViewBag.UserAchievements = achievements;

        IQueryable<Outfit> UserOutfitsQuery;

        if (userId == id || User.IsInRole("Admin"))
        {
            var UserOutfits = _context.Outfits.Where(u => u.User.Id == id);
            ViewBag.UserOutfits = UserOutfits;
            ViewBag.AfisareButoane = true; //pentru a sti daca afisez butoanele de editare sau nu
            ViewBag.Admin = User.IsInRole("Admin");
            return View(user);
            UserOutfitsQuery = _context.Outfits.Where(u => u.User.Id == id);
            ViewBag.AfisareButoane = true;
        }
        else
        {
            UserOutfitsQuery = _context.Outfits.Where(u => u.User.Id == id && u.IsPublic == true);
            ViewBag.AfisareButoane = false;
        }

        var userOutfits = await UserOutfitsQuery.ToListAsync();
        ViewBag.UserOutfits = userOutfits;

        return View(user);
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

    [HttpPost]
    public async Task<IActionResult> MakeAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Utilizatorul nu a fost găsit.";
            return RedirectToAction("Index");
        }

        var result = await _userManager.AddToRoleAsync(user, "Admin");

        if (!result.Succeeded)
        {
            TempData["Error"] = "Nu s-a putut adăuga rolul de admin.";
            return RedirectToAction("Index", "User", new { id = user.Id });
        }

        TempData["Success"] = "Utilizatorul a devenit admin!";

        return RedirectToAction("Index", "User", new { id = user.Id });
    }

    [HttpPost]
    public async Task<IActionResult> RmvAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Utilizatorul nu a fost găsit.";
            return RedirectToAction("Index");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, "Admin");


        if (!result.Succeeded)
        {
            TempData["Error"] = "Nu s-a putut sterge rolul de admin.";
            return RedirectToAction("Index", "User", new { id = user.Id });
        }

        TempData["Success"] = "Utilizatorul nu mai are rol de admin!";
        if (user.Id == _userManager.GetUserId(User))
            await _signInManager.RefreshSignInAsync(user); //daca userul curent este admin si isi sterge rolul, nu mai afisez butoanele pt rol

        return RedirectToAction("Index", "User", new { id = user.Id });
    }

}
