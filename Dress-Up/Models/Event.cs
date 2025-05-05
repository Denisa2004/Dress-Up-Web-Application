using System.ComponentModel.DataAnnotations;

namespace Dress_Up.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele nu poate depăși 100 de caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        public string Description { get; set; }

        [Display(Name = "Data concursului")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Imaginea este obligatorie")]
        [Url(ErrorMessage = "Te rog introdu un URL valid")]
        public string Image { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<User>? Users { get; set; }
        public ICollection<UserEvent>? UserEvents { get; set; }
        public ICollection<Vote>? Votes { get; set; }
    }
}
