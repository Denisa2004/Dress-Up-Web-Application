namespace Dress_Up.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Code { get; set; }          // ex: "FIRST_OUTFIT"
        public string Name { get; set; } 
        public string Description { get; set; }
        public string IconUrl { get; set; } 
        public ICollection<UserAchievement> UserAchievements { get; set; }
    }

}
