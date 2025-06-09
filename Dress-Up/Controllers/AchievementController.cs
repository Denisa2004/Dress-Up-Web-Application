using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AchievementController : Controller
{
    private readonly AchievementService _achievementService;
    private readonly UserManager<User> _userManager;

    private readonly ApplicationDbContext _context;

    public AchievementController(ApplicationDbContext context, AchievementService achievementService, UserManager<User> userManager)
    {
        _achievementService = achievementService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        // Obținem lista de UserAchievement pentru user, inclusiv datele Achievement
        var userAchievements = await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == user.Id)
            .ToListAsync();

        ViewBag.FirstName = user.FirstName;
        ViewBag.LastName = user.LastName;

        return View(userAchievements);
    }

}
