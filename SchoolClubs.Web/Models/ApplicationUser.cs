using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public int Grade { get; set; }

        [MaxLength(10)]
        public string GradeSection { get; set; } = "А";

        public string? Bio { get; set; }

        public string? ProfilePicturePath { get; set; }

        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        public ICollection<ClubMembership> Memberships { get; set; } = new List<ClubMembership>();
        public ICollection<EventAttendance> EventAttendances { get; set; } = new List<EventAttendance>();
        public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    }
}
