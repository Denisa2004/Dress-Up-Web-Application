using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.EntityFrameworkCore;


public class AchievementService
{
    private readonly ApplicationDbContext _context;

    public AchievementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Achievement?> AddAchievementToUser(string userId, string code)
    {
        var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Code == code);
        if (achievement == null) return null;

        bool alreadyHas = await _context.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.Id);

        if (alreadyHas) return null;

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievement.Id,
            DateEarned = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();

        return achievement;
    }

    public async Task<List<Achievement>> GetUserAchievementsAsync(string userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.Achievement)
            .ToListAsync();
    }
}
