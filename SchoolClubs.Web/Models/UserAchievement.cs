namespace SchoolClubs.Web.Models
{
    public class UserAchievement
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int AchievementId { get; set; }
        public Achievement Achievement { get; set; } = null!;

        public DateTime EarnedOn { get; set; } = DateTime.UtcNow;
    }
}
