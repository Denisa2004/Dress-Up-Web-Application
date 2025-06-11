using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dress_Up.Models
{
    public class User : IdentityUser
    {
        public ICollection<Outfit>? Outfits { get; set; }
        public ICollection<UserEvent>? UserEvents { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public ICollection<UserAchievement> UserAchievements { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? About { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
