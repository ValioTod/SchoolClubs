using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Models.ViewModels;
using SchoolClubs.Web.Services;
using System.Diagnostics;

namespace SchoolClubs.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ClubRecommendationService _recommendationService;
        private readonly AchievementService _achievementService;

        public HomeController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            ClubRecommendationService recommendationService,
            AchievementService achievementService)
        {
            _db = db;
            _userManager = userManager;
            _recommendationService = recommendationService;
            _achievementService = achievementService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                var featuredClubs = await _db.Clubs
                    .Where(c => c.IsActive)
                    .Include(c => c.Members)
                    .OrderByDescending(c => c.Members.Count)
                    .Take(6)
                    .ToListAsync();

                ViewBag.FeaturedClubs = featuredClubs;
                ViewBag.TotalClubs = await _db.Clubs.CountAsync(c => c.IsActive);
                ViewBag.TotalStudents = await _userManager.Users.CountAsync();
                ViewBag.TotalEvents = await _db.Events.CountAsync();

                return View("Landing");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            await _achievementService.CheckAndAwardAchievements(user.Id);

            var vm = new DashboardViewModel
            {
                TotalClubs = await _db.Clubs.CountAsync(c => c.IsActive),
                TotalStudents = await _userManager.Users.CountAsync(),
                TotalEvents = await _db.Events.CountAsync(),
                UpcomingEventsCount = await _db.Events.CountAsync(e => e.StartDate > DateTime.UtcNow && !e.IsCancelled),

                MyClubs = await _db.ClubMemberships
                    .Where(m => m.UserId == user.Id)
                    .Include(m => m.Club)
                    .Select(m => m.Club)
                    .ToListAsync(),

                UpcomingEvents = await _db.Events
                    .Where(e => e.StartDate > DateTime.UtcNow && !e.IsCancelled)
                    .Include(e => e.Club)
                    .OrderBy(e => e.StartDate)
                    .Take(5)
                    .ToListAsync(),

                RecentAnnouncements = await _db.Announcements
                    .Include(a => a.Club)
                    .Include(a => a.Author)
                    .OrderByDescending(a => a.PostedOn)
                    .Take(5)
                    .ToListAsync(),

                RecentAchievements = await _db.UserAchievements
                    .Where(ua => ua.UserId == user.Id)
                    .Include(ua => ua.Achievement)
                    .OrderByDescending(ua => ua.EarnedOn)
                    .Take(3)
                    .ToListAsync(),

                MyPoints = await _achievementService.GetTotalPoints(user.Id),
                RecommendedClubs = await _recommendationService.GetRecommendations(user.Id, 4)
            };

            var leaderboardData = await _achievementService.GetLeaderboard(10);
            vm.TopMembers = leaderboardData.Select((entry, idx) => new LeaderboardEntry
            {
                UserId = entry.UserId,
                FullName = entry.FullName,
                ProfilePicturePath = entry.Picture,
                TotalPoints = entry.Points,
                AchievementCount = entry.Count,
                Rank = idx + 1
            }).ToList();

            var myEntry = vm.TopMembers.FirstOrDefault(t => t.UserId == user.Id);
            vm.MyRank = myEntry?.Rank ?? 0;

            return View("Dashboard", vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
