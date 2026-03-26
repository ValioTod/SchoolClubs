using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;

        public DateTime PostedOn { get; set; } = DateTime.UtcNow;

        public bool IsPinned { get; set; } = false;

        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public string AuthorId { get; set; } = null!;
        public ApplicationUser Author { get; set; } = null!;
    }

    public enum AnnouncementPriority
    {
        Low,
        Normal,
        High,
        Urgent
    }
}
