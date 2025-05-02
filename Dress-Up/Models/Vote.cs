using System.ComponentModel.DataAnnotations.Schema;

namespace Dress_Up.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int OutfitId { get; set; }
        [ForeignKey("OutfitId")]
        public virtual Outfit Outfit { get; set; }
        public DateTime Date_Voted { get; set; } = DateTime.Now;
    }
}
