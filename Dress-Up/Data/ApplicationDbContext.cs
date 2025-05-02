using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Dress_Up.Models;

namespace Dress_Up.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany() // dacă nu ai colecție de UserEvent în User
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany()
                .HasForeignKey(ue => ue.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Outfit)
                .WithMany()
                .HasForeignKey(ue => ue.OutfitId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
