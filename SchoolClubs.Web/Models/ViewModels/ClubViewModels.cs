using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models.ViewModels
{
    public class ClubCreateViewModel
    {
        [Required(ErrorMessage = "Моля въведете име на клуба")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Името трябва да е между 3 и 150 символа")]
        [Display(Name = "Име на клуба")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете описание")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = null!;

        [Display(Name = "Категория")]
        public string Category { get; set; } = "Друго";

        [Display(Name = "График на срещите")]
        public string? MeetingSchedule { get; set; }

        [Display(Name = "Място на срещите")]
        public string? MeetingLocation { get; set; }

        [Range(2, 100, ErrorMessage = "Броят членове трябва да е между 2 и 100")]
        [Display(Name = "Макс. членове")]
        public int MaxMembers { get; set; } = 30;
    }

    public class ClubEditViewModel : ClubCreateViewModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ClubDetailsViewModel
    {
        public Club Club { get; set; } = null!;
        public bool IsMember { get; set; }
        public bool IsLeader { get; set; }
        public int MemberCount { get; set; }
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<Announcement> RecentAnnouncements { get; set; } = new();
        public List<GalleryPhoto> RecentPhotos { get; set; } = new();
        public List<ClubMembership> PendingRequests { get; set; } = new();
    }

    public class ClubListViewModel
    {
        public List<Club> Clubs { get; set; } = new();
        public string? SearchQuery { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string> Categories { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
