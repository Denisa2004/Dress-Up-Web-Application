using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dress_Up.Controllers;


public class AlertController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public AlertController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Send(string userId, string content)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(content))
        {
            TempData["message"] = "Mesajul sau utilizatorul nu sunt valide.";
            return RedirectToAction("Index", "User", new { id = userId });
        }

        var alert = new AlertMessage
        {
            UserId = userId,
            Content = content,
            SentAt = DateTime.Now
        };

        _context.AlertMessages.Add(alert);
        _context.SaveChanges();

        TempData["message"] = "Alerta a fost trimisă cu succes!";
        return RedirectToAction("Index", "User", new { id = userId });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAsRead(int alertId)
    {
        var alert = _context.AlertMessages.FirstOrDefault(a => a.Id == alertId);
        if (alert == null)
        {
            return NotFound();
        }

        // Asigură-te că utilizatorul curent este cel care a primit alerta
        var currentUserId = _userManager.GetUserId(User);
        if (alert.UserId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        alert.IsRead = true;
        _context.SaveChanges();

        TempData["message"] = "Alerta a fost marcată ca citită!";
        return RedirectToAction("Index", "User", new { id = currentUserId });
    }

}
