namespace Dress_Up.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

        public int Duration { get; set; }
        public ICollection <User> Users { get; set; }

    }
}
