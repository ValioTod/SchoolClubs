using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class ClubMembership
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

        public MemberRole Role { get; set; } = MemberRole.Member;
    }

    public enum MemberRole
    {
        Member = 0,
        Moderator = 1,
        President = 2
    }
}
