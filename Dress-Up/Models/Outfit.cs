using System.ComponentModel.DataAnnotations;

namespace Dress_Up.Models
{
    public class Outfit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual User? User { get; set; }
        public ICollection<Clothes>? Clothes { get; set; }
        public string? Description { get; set; }
        public DateTime Date_added { get; set; } = DateTime.Now;
        public string? Image { get; set; }
        public bool IsPublic { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}