using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class Club
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името на клуба е задължително")]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Описанието е задължително")]
        public string Description { get; set; } = null!;

        [MaxLength(50)]
        public string Category { get; set; } = "Друго";

        [MaxLength(200)]
        public string? MeetingSchedule { get; set; }

        [MaxLength(100)]
        public string? MeetingLocation { get; set; }

        public int MaxMembers { get; set; } = 30;

        public string? LogoPath { get; set; }

        public string? CoverImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? LeaderId { get; set; }
        public ApplicationUser? Leader { get; set; }

        public ICollection<ClubMembership> Members { get; set; } = new List<ClubMembership>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public ICollection<GalleryPhoto> Photos { get; set; } = new List<GalleryPhoto>();
    }
}
