using System.ComponentModel.DataAnnotations.Schema;

namespace Dress_Up.Models;

public class OutfitUser
{
    public string UserId { get; set; }
    public User User { get; set; }

    public int OutfitId { get; set; }
    public Outfit Outfit { get; set; }
}
