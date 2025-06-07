using Dress_Up.Models;

namespace Dress_Up.Data
{
    public static class AchievementSeeder
    {
        public static void SeedAchievements(ApplicationDbContext context)
        {
            if (!context.Achievements.Any())
            {
                var achievements = new List<Achievement>
                {
                    new Achievement
                    {
                        Code = "FIRST_OUTFIT",
                        Name = "Primul outfit",
                        Description = "Ai creat primul tău outfit!",
                        IconUrl = "wwwroot/images/achievements/first_outfit.png"
                    },
                    new Achievement
                    {
                        Code = "FIVE_OUTFITS",
                        Name = "5 outfit-uri",
                        Description = "Ai creat 5 outfit-uri!",
                        IconUrl = "wwwroot/images/achievements/5_outfits.png"
                    },
                    new Achievement
                    {
                        Code = "FIRST_EVENT",
                        Name = "Primul concurs",
                        Description = "Ai participat la primul tau concurs!",
                        IconUrl = "wwwroot/images/achievements/concurs.png"
                    },
                    new Achievement
                    {
                        Code = "FIVE_EVENT",
                        Name = "5 concursuri",
                        Description = "Ai creat 5 concursuri!",
                        IconUrl = "wwwroot/images/achievements/5_concurs.png"
                    },
                    new Achievement
                    {
                        Code = "FIRST_WON",
                        Name = "Primul concurs castigat",
                        Description = "Ai castigat primul tau concurs!",
                        IconUrl = "/images/achievements/primul_castigat.png"
                    }
                };

                context.Achievements.AddRange(achievements);
                context.SaveChanges();
            }
        }
    }
}
