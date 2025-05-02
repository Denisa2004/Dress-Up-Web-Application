using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            return View(user);
        }
        else
        {
            var UserOutfits = _context.Outfits.Where(u => u.User.Id == id && u.IsPublic == true);
            ViewBag.UserOutfits = UserOutfits;

            return View(user);
        }



    }
}
