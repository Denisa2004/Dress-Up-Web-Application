using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Dress_Up.Models
{
    public class Avatar
    {
        /*
         - o sa am 3 avatare disponibile, fiecare cu un id(vor fi 1, 2, 3 si stim ca pe astea nu le mai folosim niciodata), un nume si o imagine
         - user ul cand intra pe "creeaza avatar" va avea afisate cele 3 avatare disponibile, iar cand da click pe unul dintre ele, pe acela il va personaliza mai departe
         - cand da click pe un avatar, se va face un post request catre server cu id ul avatarului selectat
         - server ul va returna un json cu id ul avatarului selectat, numele si imaginea
         - user ul va selecta din dulap hainele pe care vrea sa le adauge avatarului si apoi are un buton de "salveaza avatar" 
         - la apasarea butonului de "salveaza avatar", se va salva imaginea formata din imaginea initiala + hainele, accesoriile, etc. si se salveaza atat in folder ul wwwroot/avatars, dar se creeaza si un obiect nou de tip Outfit, un nume dat de user si imaginea formata si se adauga in colectia user ului(default private)
         
         */
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string ImageData { get; set; }
    }
}
