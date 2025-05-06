using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dress_Up.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
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
}
