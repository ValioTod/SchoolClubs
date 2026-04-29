using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Services;

namespace SchoolClubs.Web.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly AchievementService _achievementService;

        public GalleryController(AppDbContext db, UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env, AchievementService achievementService)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
            _achievementService = achievementService;
        }

        public async Task<IActionResult> Index(int clubId)
        {
            var club = await _db.Clubs.FindAsync(clubId);
            if (club == null) return NotFound();

            var photos = await _db.GalleryPhotos
                .Where(p => p.ClubId == clubId)
                .Include(p => p.UploadedBy)
                .OrderByDescending(p => p.UploadedOn)
                .ToListAsync();

            ViewBag.Club = club;
            return View(photos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int clubId, IFormFile photo, string? caption)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isMember = await _db.ClubMemberships.AnyAsync(m => m.UserId == user.Id && m.ClubId == clubId);
            if (!isMember) return Forbid();

            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Моля изберете снимка.";
                return RedirectToAction(nameof(Index), new { clubId });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                TempData["Error"] = "Невалиден формат на файла.";
                return RedirectToAction(nameof(Index), new { clubId });
            }

            if (photo.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "Файлът е твърде голям (макс. 5MB).";
                return RedirectToAction(nameof(Index), new { clubId });
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "gallery");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            _db.GalleryPhotos.Add(new GalleryPhoto
            {
                FilePath = $"/uploads/gallery/{fileName}",
                Caption = caption,
                ClubId = clubId,
                UploadedById = user.Id
            });
            await _db.SaveChangesAsync();

            await _achievementService.CheckAndAwardAchievements(user.Id);

            TempData["Success"] = "Снимката е качена!";
            return RedirectToAction(nameof(Index), new { clubId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var galleryPhoto = await _db.GalleryPhotos.Include(p => p.Club).FirstOrDefaultAsync(p => p.Id == id);
            if (galleryPhoto == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isUploader = galleryPhoto.UploadedById == userId;
            var isClubLeader = galleryPhoto.Club.LeaderId == userId;
            var isAdmin = User.IsInRole("Admin");

            if (!isUploader && !isClubLeader && !isAdmin)
                return Forbid();

            var physicalPath = Path.Combine(_env.WebRootPath, galleryPhoto.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            var clubId = galleryPhoto.ClubId;
            _db.GalleryPhotos.Remove(galleryPhoto);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Снимката е изтрита.";
            return RedirectToAction(nameof(Index), new { clubId });
        }
    }
}
