using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalClubs { get; set; }
        public int TotalStudents { get; set; }
        public int TotalEvents { get; set; }
        public int UpcomingEventsCount { get; set; }

        public List<Club> MyClubs { get; set; } = new();
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<Announcement> RecentAnnouncements { get; set; } = new();
        public List<UserAchievement> RecentAchievements { get; set; } = new();

        // за класацията
        public List<LeaderboardEntry> TopMembers { get; set; } = new();
        public int MyPoints { get; set; }
        public int MyRank { get; set; }

        // препоръчани клубове
        public List<Club> RecommendedClubs { get; set; } = new();
    }

    public class LeaderboardEntry
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfilePicturePath { get; set; }
        public int TotalPoints { get; set; }
        public int AchievementCount { get; set; }
        public int Rank { get; set; }
    }

    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<ClubMembership> Memberships { get; set; } = new();
        public List<UserAchievement> Achievements { get; set; } = new();
        public int TotalPoints { get; set; }
        public int EventsAttendedCount { get; set; }
        public int Rank { get; set; }
        public bool IsOwnProfile { get; set; }
    }

    public class ProfileEditViewModel
    {
        [Required(ErrorMessage = "Пълното име е задължително")]
        [MaxLength(100)]
        [Display(Name = "Пълно име")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Клас")]
        [Range(1, 12)]
        public int Grade { get; set; }

        [MaxLength(10)]
        [Display(Name = "Паралелка")]
        public string GradeSection { get; set; } = "А";

        [Display(Name = "Биография")]
        [MaxLength(500)]
        public string? Bio { get; set; }

        public string? CurrentProfilePicture { get; set; }
    }

    public class AnnouncementCreateViewModel
    {
        [Required(ErrorMessage = "Заглавието е задължително")]
        [MaxLength(250)]
        [Display(Name = "Заглавие")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Съдържанието е задължително")]
        [Display(Name = "Съдържание")]
        public string Content { get; set; } = null!;

        [Display(Name = "Приоритет")]
        public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;

        [Display(Name = "Закачена")]
        public bool IsPinned { get; set; }

        public int ClubId { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalClubs { get; set; }
        public int TotalEvents { get; set; }
        public int TotalAnnouncements { get; set; }
        public int TotalFeedbacks { get; set; }
        public int UnresolvedFeedbacks { get; set; }
        public List<ApplicationUser> RecentUsers { get; set; } = new();
        public List<Club> AllClubs { get; set; } = new();
        public List<Feedback> RecentFeedbacks { get; set; } = new();
    }

    public class FeedbackCreateViewModel
    {
        [Required(ErrorMessage = "Темата е задължителна")]
        [MaxLength(200)]
        [Display(Name = "Тема")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Съобщението е задължително")]
        [Display(Name = "Съобщение")]
        public string Message { get; set; } = null!;

        [Display(Name = "Категория")]
        public FeedbackCategory Category { get; set; } = FeedbackCategory.General;
    }

    public class ClubStatisticsViewModel
    {
        public Club Club { get; set; } = null!;
        public int TotalMembers { get; set; }
        public int TotalEvents { get; set; }
        public int TotalAnnouncements { get; set; }
        public int TotalPhotos { get; set; }
        public int TotalAttendances { get; set; }
        public double AverageAttendanceRate { get; set; }
        public ApplicationUser? MostActiveUser { get; set; }
        public int MostActiveUserPoints { get; set; }
    }
}
