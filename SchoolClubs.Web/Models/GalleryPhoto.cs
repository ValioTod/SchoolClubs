using System.ComponentModel.DataAnnotations;

namespace SchoolClubs.Web.Models
{
    public class GalleryPhoto
    {
        public int Id { get; set; }

        [Required]
        public string FilePath { get; set; } = null!;

        [MaxLength(200)]
        public string? Caption { get; set; }

        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public string UploadedById { get; set; } = null!;
        public ApplicationUser UploadedBy { get; set; } = null!;
    }
}
