using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models.ViewModels
{
    public class EventCreateViewModel
    {
        [Required(ErrorMessage = "Заглавието е задължително")]
        [MaxLength(200)]
        [Display(Name = "Заглавие")]
        public string Title { get; set; } = null!;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Началната дата е задължителна")]
        [Display(Name = "Начало")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(1);

        [Display(Name = "Край")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Локация")]
        public string? Location { get; set; }

        [Display(Name = "Макс. участници (0 = без лимит)")]
        public int MaxAttendees { get; set; } = 0;

        public int ClubId { get; set; }
    }

    public class EventDetailsViewModel
    {
        public Event Event { get; set; } = null!;
        public bool IsSignedUp { get; set; }
        public bool HasAttended { get; set; }
        public int AttendeeCount { get; set; }
        public bool CanSignUp { get; set; }
        public bool IsClubMember { get; set; }
    }
}
