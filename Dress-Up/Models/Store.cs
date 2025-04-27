namespace Dress_Up.Models
{
    public class Store
    {
        public string Name { get; set; }
        public ICollection<Accessory> Accessories { get; set; }
        public ICollection<Clothes> Clothes { get; set; }
        public ICollection<Shoe> Shoes { get; set; }
        public ICollection<Hair> Hair { get; set; }
        public ICollection <Avatar> Avatars { get; set; }
    }
}
