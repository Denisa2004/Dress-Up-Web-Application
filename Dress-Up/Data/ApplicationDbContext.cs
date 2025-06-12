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

        public DbSet<Outfit> Outfits { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet <Vote> Votes { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet <Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }

        public DbSet<AlertMessage> AlertMessages { get; set; }
        public DbSet<OutfitUser> OutfitUsers { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Event)
                .WithMany(b => b.Votes)
                .HasForeignKey(v => v.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Outfit)
                .WithMany()
                .HasForeignKey(ue => ue.OutfitId)
                .OnDelete(DeleteBehavior.Restrict); // sau .Cascade, cum preferi

            modelBuilder.Entity<UserAchievement>()
                .HasKey(ua => new { ua.UserId, ua.AchievementId });

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.Achievement)
                .WithMany(a => a.UserAchievements)
                .HasForeignKey(ua => ua.AchievementId);

            modelBuilder.Entity<OutfitUser>()
                .HasKey(ou => new { ou.UserId, ou.OutfitId });
    
            modelBuilder.Entity<OutfitUser>()
                .HasOne(ou => ou.User)
                .WithMany(u => u.SavedOutfits)
                .HasForeignKey(ou => ou.UserId);

            modelBuilder.Entity<OutfitUser>()
                .HasOne(ou => ou.Outfit)
                .WithMany(o => o.SavedByUsers)
                .HasForeignKey(ou => ou.OutfitId);
        }

    }
}