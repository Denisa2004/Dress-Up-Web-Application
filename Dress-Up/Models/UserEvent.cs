using System.Collections;

namespace Dress_Up.Models
{
    public class UserEvent
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public int OutfitId { get; set; }
        public User User { get; set; }
        public Event Event { get; set; }
        public Outfit Outfit { get; set; }
    }
}