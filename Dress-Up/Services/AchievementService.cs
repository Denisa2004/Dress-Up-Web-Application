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
        var user = await _context.Users
            .Include(u => u.UserAchievements)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var achievement = await _context.Achievements
            .FirstOrDefaultAsync(a => a.Code == code);

        if (user == null || achievement == null)
            return null;

        bool alreadyHas = user.UserAchievements.Any(ua => ua.AchievementId == achievement.Id);

        if (!alreadyHas)
        {
            user.UserAchievements.Add(new UserAchievement
            {
                UserId = user.Id,
                AchievementId = achievement.Id,
                DateEarned = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return achievement;
        }

        return null;
    }


    public async Task<List<Achievement>> GetUserAchievementsAsync(string userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.Achievement)
            .ToListAsync();
    }
}
