namespace SchoolClubs.Web.Models
{
    public class EventAttendance
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public DateTime SignedUpOn { get; set; } = DateTime.UtcNow;

        public bool HasAttended { get; set; } = false;
        public DateTime? CheckedInAt { get; set; }
    }
}
