using Microsoft.EntityFrameworkCore;
using pl_backend.Models;

namespace pl_backend.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Marker> Markers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLanguage> UserLanguages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
            .HasOne(c => c.InvitingUser)
            .WithMany(u => u.Chats)
            .HasForeignKey(c => c.InvitingUserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
            .HasOne(c => c.InvitedUser)
            .WithMany()
            .HasForeignKey(c => c.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=mahmud.db.elephantsql.com;Database=hioutuxk;Username=hioutuxk;Password=pW9ZpLZM7zP1fU5koEUs0i1za25nR1nz");
        }


    }
}
