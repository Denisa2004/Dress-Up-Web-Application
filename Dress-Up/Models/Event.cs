namespace Dress_Up.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsGoing { get; set; } = true;
        public ICollection<User> Users { get; set; }

    }
}
