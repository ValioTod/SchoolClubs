using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(150)]
        public string? Location { get; set; }

        public int MaxAttendees { get; set; } = 0;

        public bool IsCancelled { get; set; } = false;

        public string? AttendanceCode { get; set; }

        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public ICollection<EventAttendance> Attendees { get; set; } = new List<EventAttendance>();
    }
}
