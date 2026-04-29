using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Models.ViewModels;

namespace SchoolClubs.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalClubs = await _db.Clubs.CountAsync(),
                TotalEvents = await _db.Events.CountAsync(),
                TotalAnnouncements = await _db.Announcements.CountAsync(),
                TotalFeedbacks = await _db.Feedbacks.CountAsync(),
                UnresolvedFeedbacks = await _db.Feedbacks.CountAsync(f => !f.IsResolved),
                RecentUsers = await _userManager.Users
                    .OrderByDescending(u => u.DateJoined)
                    .Take(10)
                    .ToListAsync(),
                AllClubs = await _db.Clubs
                    .Include(c => c.Members)
                    .Include(c => c.Leader)
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                RecentFeedbacks = await _db.Feedbacks
                    .Include(f => f.Author)
                    .OrderByDescending(f => f.SubmittedOn)
                    .Take(10)
                    .ToListAsync()
            };

            return View(vm);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.OrderByDescending(u => u.DateJoined).ToListAsync();
            ViewBag.Users = users;

            // Get roles for each user
            var userRoles = new Dictionary<string, List<string>>();
            foreach (var user in users)
            {
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                userRoles[user.Id] = roles;
            }
            ViewBag.UserRoles = userRoles;

            return View();
        }

        public async Task<IActionResult> Clubs()
        {
            var clubs = await _db.Clubs
                .Include(c => c.Leader)
                .Include(c => c.Members)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewBag.Clubs = clubs;

            return View();
        }

        public async Task<IActionResult> Feedbacks()
        {
            var feedbacks = await _db.Feedbacks
                .Include(f => f.Author)
                .OrderByDescending(f => f.SubmittedOn)
                .ToListAsync();
            ViewBag.Feedbacks = feedbacks;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveFeedback(int id)
        {
            var feedback = await _db.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsResolved = true;
            _db.Feedbacks.Update(feedback);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Feedback marked as resolved.";
            return RedirectToAction(nameof(Feedbacks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnresolveFeedback(int id)
        {
            var feedback = await _db.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsResolved = false;
            _db.Feedbacks.Update(feedback);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Feedback marked as pending.";
            return RedirectToAction(nameof(Feedbacks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleClub(int id)
        {
            var club = await _db.Clubs.FindAsync(id);
            if (club == null) return NotFound();

            club.IsActive = !club.IsActive;
            await _db.SaveChangesAsync();

            TempData["Success"] = club.IsActive ? "Club activated." : "Club deactivated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var club = await _db.Clubs.FindAsync(id);
            if (club == null) return NotFound();

            _db.Clubs.Remove(club);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Club deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveFeedback(int id, string? response)
        {
            var feedback = await _db.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsResolved = true;
            feedback.AdminResponse = response;
            feedback.RespondedOn = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Feedback resolved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var validRoles = new[] { "Admin", "Teacher", "Student" };
            if (!validRoles.Contains(role)) return BadRequest();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Грешка при промяна на роля.";
                return RedirectToAction(nameof(Index));
            }

            // If changing the current user's role, refresh the authentication cookie
            var currentUserId = _userManager.GetUserId(User);
            if (userId == currentUserId)
            {
                var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                await signInManager.RefreshSignInAsync(user);
            }

            TempData["Success"] = $"Роля променена на {role}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
