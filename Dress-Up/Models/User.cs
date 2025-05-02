using Microsoft.AspNetCore.Identity;

namespace Dress_Up.Models
{
    public class User : IdentityUser
    {
        public ICollection<Outfit> Outfits { get; set; }
        public ICollection<UserEvent> UserEvents { get; set; }
    }
}
