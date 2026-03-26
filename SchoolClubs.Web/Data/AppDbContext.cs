using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Models;

namespace SchoolClubs.Web.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<ClubMembership> ClubMemberships { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<GalleryPhoto> GalleryPhotos { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ClubMembership>()
                .HasIndex(cm => new { cm.UserId, cm.ClubId })
                .IsUnique();

            builder.Entity<EventAttendance>()
                .HasIndex(ea => new { ea.UserId, ea.EventId })
                .IsUnique();

            builder.Entity<UserAchievement>()
                .HasIndex(ua => new { ua.UserId, ua.AchievementId })
                .IsUnique();

            builder.Entity<ClubMembership>()
                .HasOne(cm => cm.Club)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClubMembership>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Event>()
                .HasOne(e => e.Club)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventAttendance>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.Attendees)
                .HasForeignKey(ea => ea.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventAttendance>()
                .HasOne(ea => ea.User)
                .WithMany(u => u.EventAttendances)
                .HasForeignKey(ea => ea.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Announcement>()
                .HasOne(a => a.Club)
                .WithMany(c => c.Announcements)
                .HasForeignKey(a => a.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Announcement>()
                .HasOne(a => a.Author)
                .WithMany()
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<GalleryPhoto>()
                .HasOne(g => g.Club)
                .WithMany(c => c.Photos)
                .HasForeignKey(g => g.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GalleryPhoto>()
                .HasOne(g => g.UploadedBy)
                .WithMany()
                .HasForeignKey(g => g.UploadedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Club>()
                .HasOne(c => c.Leader)
                .WithMany()
                .HasForeignKey(c => c.LeaderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Feedback>()
                .HasOne(f => f.Author)
                .WithMany()
                .HasForeignKey(f => f.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
