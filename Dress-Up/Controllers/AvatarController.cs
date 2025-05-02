using Microsoft.AspNetCore.Mvc;
using Dress_Up.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Dress_Up.Controllers
{

    public class AvatarController : Controller
    {
        public IActionResult Index()
        {
            return View(); // afiseaza cele 3 avatare
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

            Outfit outfit = new Outfit();
            //outfit.Id = ? 
            outfit.Image = filePath;
            outfit.Name = fileName; //numele va putea fi editat ulterior
            //outfit.UserId = User.Identity.Name; //aici ar trebui sa fie id ul user ului
            outfit.Date_added = DateTime.Now;
            outfit.IsPublic = false;
            outfit.Description = "Avatar personalizat"; //descrierea o sa poata fi editata ulterior


            return Ok(new { imagePath = $"/avatars/{fileName}" });
        }

    }
}
