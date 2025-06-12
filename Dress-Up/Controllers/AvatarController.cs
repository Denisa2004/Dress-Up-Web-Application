using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Dress_Up.Controllers;


public class AvatarController(ApplicationDbContext context, UserManager<User> userManager, AchievementService achievementService) : Controller
{
    private readonly AchievementService _achievementService= achievementService;
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;

    /*
    public IActionResult Index()
    {
        var avatars = _context.Avatars.ToList(); // parcurge toate avatarele
        ViewBag.Avatars = avatars;
        return View(); // trimite lista în View
    }
    */
    public IActionResult Index()
    {
        var dbConnection = _context.Database.GetDbConnection();
        Console.WriteLine("DB Connection: " + dbConnection.ConnectionString);
        Console.WriteLine("DB Name: " + dbConnection.Database);

        var avatars = _context.Avatars.ToList();

        if (!avatars.Any())
        {
            Console.WriteLine("⚠️ Nu există avatare în baza de date. Se inserează automat...");

            _context.Avatars.AddRange(new List<Avatar>
        {
            new Avatar { Name = "Avatar 1", ImageData = "placeholder1" },
            new Avatar { Name = "Avatar 2", ImageData = "placeholder2" },
            new Avatar { Name = "Avatar 3", ImageData = "placeholder3" }
        });
            _context.SaveChanges();

            avatars = _context.Avatars.ToList();
        }

        ViewBag.Avatars = avatars;
        return View();
    }




    [HttpGet]
    public IActionResult Personalizare(int id)
    {
        ViewBag.AvatarId = id;
        return View(); // aici personalizezi avatarul selectat
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] string imageBase64)
    {
        if (string.IsNullOrEmpty(imageBase64))
            return BadRequest("Imaginea lipseste.");

        var base64Data = Regex.Replace(imageBase64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty); //sterg prima parte a path ului pentru a ramane cu un path de tip /avatars/nume
        var imageBytes = Convert.FromBase64String(base64Data); //transform in string

        var fileName = $"avatar_{Guid.NewGuid()}.png";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars", fileName);

        System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        User user = _userManager.GetUserAsync(User).Result;
        Outfit outfit = new Outfit();
        outfit.Image = $"/avatars/{fileName}";
        outfit.Name = fileName; //numele va putea fi editat ulterior
        outfit.User = user; // nu e niciodata null pt ca butonul de salvare apare doar daca user ul este autentificat
        outfit.Date_added = DateTime.Now;
        outfit.IsPublic = false;
        outfit.Description = "Avatar personalizat"; //descrierea o sa poata fi editata ulterior
        _context.Outfits.Add(outfit); //adaug outfit ul in baza de date
        user.Outfits.Add(outfit); //adaug outfit ul in colectia user ului
        _context.SaveChanges(); // aici se generează Id-ul

        var outfitCount = await context.Outfits.CountAsync(o => o.User.Id == user.Id);

        if (outfitCount == 1)
        {
            var achievement = await _achievementService.AddAchievementToUser(user.Id, "FIRST_OUTFIT");
            if (achievement != null)
            {
                TempData["AchievementMessage"] = "Felicitări! Ai creat primul tău outfit!";
            }
        }
        else if (outfitCount == 5)
        {
            var achievement = await _achievementService.AddAchievementToUser(user.Id, "FIVE_OUTFITS");
            if (achievement != null)
            {
                TempData["AchievementMessage"] = "Felicitări! Ai creat 5 outfituri!";
            }
        }

        return Ok(new { imagePath = $"/avatars/{fileName}" });
    }

}
