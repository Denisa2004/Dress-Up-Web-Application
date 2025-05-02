using Microsoft.AspNetCore.Mvc;
using Dress_Up.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dress_Up.Data;
using Microsoft.AspNetCore.Identity;

namespace Dress_Up.Controllers
{

    public class AvatarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AvatarController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var avatars = _context.Avatars.ToList(); // parcurge toate avatarele
            return View(avatars); // trimite lista în View
        }

        [HttpGet]
        public IActionResult Personalizare(int id)
        {
            ViewBag.AvatarId = id;
            return View(); // aici personalizezi avatarul selectat
        }

        [HttpPost]
        public IActionResult Save([FromBody] string imageBase64)
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


            return Ok(new { imagePath = $"/avatars/{fileName}" });
        }

    }
}
