namespace Dress_Up.Models
{
    public class Accessory
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Gender { get; set; }
        public int Price { get; set; }
        public int Rarity { get; set; }
        public DateTime Date_added { get; set; } = DateTime.Now;
        public string Image { get; set; }
    }
}
