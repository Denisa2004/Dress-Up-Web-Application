namespace Dress_Up.Models
{
    public class Outfit
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int HairId { get; set; }
        public Hair Hair { get; set; }

        public int ShoesId { get; set; }
        public Shoe Shoes { get; set; }


        public ICollection<Clothes> Clothes { get; set; }
        public ICollection<Accessory> Accessories { get; set; }

        public string Description { get; set; }

        public int Rarity { get; set; }
        public DateTime Date_added { get; set; } = DateTime.Now;
        public string Image { get; set; }
    }
}
