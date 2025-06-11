namespace Dress_Up.Models
{
    public class UserAchievement
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int AchievementId { get; set; }
        public Achievement Achievement { get; set; }

        public DateTime DateEarned { get; set; }
    }

}
