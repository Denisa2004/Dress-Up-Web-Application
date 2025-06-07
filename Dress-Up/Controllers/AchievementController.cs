using Dress_Up.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AchievementsController : Controller
{
    private readonly AchievementService _achievementService;
    private readonly UserManager<User> _userManager;

    public AchievementsController(AchievementService achievementService, UserManager<User> userManager)
    {
        _achievementService = achievementService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var achievements = await _achievementService.GetUserAchievementsAsync(user.Id);
        return View(achievements);
    }
}
