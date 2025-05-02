using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Dress_Up.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int OutfitId { get; set; }
        [ForeignKey("OutfitId")]
        public virtual Outfit Outfit { get; set; }
        public DateTime Date_created { get; set; } = DateTime.Now;
        public DateTime Date_updated { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Invalid Comment.")]
        public string Content { get; set; }
    }
}
