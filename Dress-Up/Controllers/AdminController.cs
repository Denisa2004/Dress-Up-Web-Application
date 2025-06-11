using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dress_Up.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult SendMessage()
        {
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("SendMessage");

            var message = new AlertMessage
            {
                UserId = userId,
                Content = content,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.AlertMessages.Add(message);
            _context.SaveChanges();

            TempData["Msg"] = "Mesaj trimis cu succes!";
            return RedirectToAction("SendMessage");
        }
    }
}
