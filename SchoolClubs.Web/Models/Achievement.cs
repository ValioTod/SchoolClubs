using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class Achievement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(300)]
        public string? Description { get; set; }

        public string? IconPath { get; set; }

        public AchievementType Type { get; set; }
        public int Threshold { get; set; }

        public int PointsAwarded { get; set; } = 10;

        public ICollection<UserAchievement> EarnedBy { get; set; } = new List<UserAchievement>();
    }

    public enum AchievementType
    {
        EventsAttended,
        ClubsJoined,
        DaysAsMember,
        AnnouncementsMade,
        PhotosUploaded
    }
}
