using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Models.ViewModels;
using SchoolClubs.Web.Services;
using System.Text;

namespace SchoolClubs.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AchievementService _achievementService;
        private readonly IWebHostEnvironment _env;

        public ProfileController(AppDbContext db, UserManager<ApplicationUser> userManager,
            AchievementService achievementService, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _achievementService = achievementService;
            _env = env;
        }

        public async Task<IActionResult> Index(string? id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var userId = id ?? currentUserId;
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound();

            var memberships = await _db.ClubMemberships
                .Where(m => m.UserId == userId)
                .Include(m => m.Club)
                .ToListAsync();

            var achievements = await _db.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Achievement)
                .OrderByDescending(ua => ua.EarnedOn)
                .ToListAsync();

            var eventsAttended = await _db.EventAttendances
                .CountAsync(ea => ea.UserId == userId && ea.HasAttended);

            var totalPoints = await _achievementService.GetTotalPoints(userId!);

            var leaderboardData = await _achievementService.GetLeaderboard(100);
            var rank = leaderboardData.FindIndex(e => e.UserId == userId) + 1;

            var vm = new ProfileViewModel
            {
                User = user,
                Memberships = memberships,
                Achievements = achievements,
                TotalPoints = totalPoints,
                EventsAttendedCount = eventsAttended,
                Rank = rank > 0 ? rank : leaderboardData.Count + 1,
                IsOwnProfile = userId == currentUserId
            };

            return View(vm);
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var vm = new ProfileEditViewModel
            {
                FullName = user.FullName,
                Grade = user.Grade,
                GradeSection = user.GradeSection,
                Bio = user.Bio,
                CurrentProfilePicture = user.ProfilePicturePath
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel vm, IFormFile? profilePicture)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FullName = vm.FullName;
            user.Grade = vm.Grade;
            user.GradeSection = vm.GradeSection;
            user.Bio = vm.Bio;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    TempData["Error"] = "Invalid file format. Use JPG, PNG or WebP.";
                    return View(vm);
                }
                if (profilePicture.Length > 2 * 1024 * 1024)
                {
                    TempData["Error"] = "File too large (max 2MB).";
                    return View(vm);
                }

                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePicturePath = $"/uploads/profiles/{fileName}";
            }

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Leaderboard()
        {
            var data = await _achievementService.GetLeaderboard(50);
            var entries = data.Select((e, idx) => new LeaderboardEntry
            {
                UserId = e.UserId,
                FullName = e.FullName,
                ProfilePicturePath = e.Picture,
                TotalPoints = e.Points,
                AchievementCount = e.Count,
                Rank = idx + 1
            }).ToList();

            return View(entries);
        }

        public async Task<IActionResult> ExportMembers(int clubId)
        {
            var club = await _db.Clubs.FindAsync(clubId);
            if (club == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var members = await _db.ClubMemberships
                .Where(m => m.ClubId == clubId)
                .Include(m => m.User)
                .OrderBy(m => m.User.FullName)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Grade,Section,Role,JoinedOn");
            foreach (var m in members)
            {
                sb.AppendLine($"\"{m.User.FullName}\",\"{m.User.Email}\",{m.User.Grade},\"{m.User.GradeSection}\",\"{m.Role}\",\"{m.JoinedOn:yyyy-MM-dd}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"{club.Name}_members.csv");
        }

        public async Task<IActionResult> Statistics(int clubId)
        {
            var club = await _db.Clubs
                .Include(c => c.Members).ThenInclude(m => m.User)
                .Include(c => c.Events).ThenInclude(e => e.Attendees)
                .FirstOrDefaultAsync(c => c.Id == clubId);
            if (club == null) return NotFound();

            var totalAttendances = club.Events.Sum(e => e.Attendees.Count(a => a.HasAttended));
            var totalPossible = club.Events.Sum(e => e.Attendees.Count);
            var avgRate = totalPossible > 0 ? (double)totalAttendances / totalPossible * 100 : 0;

            string? mostActiveUserId = null;
            int mostActivePoints = 0;
            foreach (var member in club.Members)
            {
                var pts = await _achievementService.GetTotalPoints(member.UserId);
                if (pts > mostActivePoints)
                {
                    mostActivePoints = pts;
                    mostActiveUserId = member.UserId;
                }
            }

            var vm = new ClubStatisticsViewModel
            {
                Club = club,
                TotalMembers = club.Members.Count,
                TotalEvents = club.Events.Count,
                TotalAnnouncements = await _db.Announcements.CountAsync(a => a.ClubId == clubId),
                TotalPhotos = await _db.GalleryPhotos.CountAsync(p => p.ClubId == clubId),
                TotalAttendances = totalAttendances,
                AverageAttendanceRate = Math.Round(avgRate, 1),
                MostActiveUser = mostActiveUserId != null ? await _userManager.FindByIdAsync(mostActiveUserId) : null,
                MostActiveUserPoints = mostActivePoints
            };

            return View(vm);
        }
    }
}
