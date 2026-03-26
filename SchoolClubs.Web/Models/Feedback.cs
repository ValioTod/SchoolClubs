using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        public FeedbackCategory Category { get; set; } = FeedbackCategory.General;

        public bool IsResolved { get; set; } = false;

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;

        public string AuthorId { get; set; } = null!;
        public ApplicationUser Author { get; set; } = null!;

        public string? AdminResponse { get; set; }
        public DateTime? RespondedOn { get; set; }
    }

    public enum FeedbackCategory
    {
        General,
        Bug,
        Feature,
        ClubRelated,
        EventRelated
    }
}
