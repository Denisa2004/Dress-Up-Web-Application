using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Dress_Up.Models
{
    public class Clothes
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
    }
}
