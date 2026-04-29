using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Models.ViewModels;
using SchoolClubs.Web.Services;

namespace SchoolClubs.Web.Controllers
{
    public class ClubsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AchievementService _achievementService;

        public ClubsController(AppDbContext db, UserManager<ApplicationUser> userManager, AchievementService achievementService)
        {
            _db = db;
            _userManager = userManager;
            _achievementService = achievementService;
        }

        public async Task<IActionResult> Index(string? search, string? category)
        {
            var query = _db.Clubs.Where(c => c.IsActive).Include(c => c.Members).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search) || c.Description.Contains(search));

            if (!string.IsNullOrWhiteSpace(category) && category != "Всички")
                query = query.Where(c => c.Category == category);

            var categories = await _db.Clubs
                .Where(c => c.IsActive)
                .Select(c => c.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var clubs = await query.OrderBy(c => c.Name).ToListAsync();

            var vm = new ClubListViewModel
            {
                Clubs = clubs,
                SearchQuery = search,
                SelectedCategory = category,
                Categories = categories,
                TotalCount = clubs.Count
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var club = await _db.Clubs
                .Include(c => c.Leader)
                .Include(c => c.Members).ThenInclude(m => m.User)
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (club == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isLeader = club.LeaderId == userId;
            var isAdmin = User.IsInRole("Admin");

            // Check if club is active - if not, only allow leader and admin to view
            if (!club.IsActive && !isLeader && !isAdmin)
            {
                return NotFound(); // Pretend it doesn't exist for non-authorized users
            }

            var vm = new ClubDetailsViewModel
            {
                Club = club,
                IsMember = club.Members.Any(m => m.UserId == userId && m.Status == MembershipStatus.Active),
                IsLeader = isLeader,
                MemberCount = club.Members.Count(m => m.Status == MembershipStatus.Active),
                UpcomingEvents = await _db.Events
                    .Where(e => e.ClubId == id && e.StartDate > DateTime.UtcNow && !e.IsCancelled)
                    .OrderBy(e => e.StartDate)
                    .Take(5)
                    .ToListAsync(),
                RecentAnnouncements = await _db.Announcements
                    .Where(a => a.ClubId == id)
                    .OrderByDescending(a => a.PostedOn)
                    .Take(5)
                    .ToListAsync(),
                RecentPhotos = await _db.GalleryPhotos
                    .Where(p => p.ClubId == id)
                    .OrderByDescending(p => p.UploadedOn)
                    .Take(8)
                    .ToListAsync(),
                PendingRequests = isLeader 
                    ? club.Members.Where(m => m.Status == MembershipStatus.Pending).ToList()
                    : new List<ClubMembership>()
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Create()
        {
            return View(new ClubCreateViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClubCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var club = new Club
            {
                Name = vm.Name,
                Description = vm.Description,
                Category = vm.Category,
                MeetingSchedule = vm.MeetingSchedule,
                MeetingLocation = vm.MeetingLocation,
                MaxMembers = vm.MaxMembers,
                LeaderId = _userManager.GetUserId(User)
            };

            _db.Clubs.Add(club);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = club.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _db.Clubs.FindAsync(id);
            if (club == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var vm = new ClubEditViewModel
            {
                Id = club.Id,
                Name = club.Name,
                Description = club.Description,
                Category = club.Category,
                MeetingSchedule = club.MeetingSchedule,
                MeetingLocation = club.MeetingLocation,
                MaxMembers = club.MaxMembers,
                IsActive = club.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClubEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var club = await _db.Clubs.FindAsync(vm.Id);
            if (club == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            club.Name = vm.Name;
            club.Description = vm.Description;
            club.Category = vm.Category;
            club.MeetingSchedule = vm.MeetingSchedule;
            club.MeetingLocation = vm.MeetingLocation;
            club.MaxMembers = vm.MaxMembers;
            club.IsActive = vm.IsActive;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = club.Id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var club = await _db.Clubs.Include(c => c.Members).FirstOrDefaultAsync(c => c.Id == id);
            if (club == null) return NotFound();

            if (club.Members.Any(m => m.UserId == user.Id && m.Status == MembershipStatus.Active))
            {
                TempData["Error"] = "Вече сте член на този клуб.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var activeMembers = club.Members.Count(m => m.Status == MembershipStatus.Active);
            if (activeMembers >= club.MaxMembers)
            {
                TempData["Error"] = "Клубът е пълен.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _db.ClubMemberships.Add(new ClubMembership
            {
                UserId = user.Id,
                ClubId = id,
                Status = MembershipStatus.Active
            });
            await _db.SaveChangesAsync();

            await _achievementService.CheckAndAwardAchievements(user.Id);

            TempData["Success"] = "Успешно се записахте в клуба!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMember(int memberId)
        {
            var membership = await _db.ClubMemberships.Include(m => m.Club).FirstOrDefaultAsync(m => m.Id == memberId);
            if (membership == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (membership.Club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            membership.Status = MembershipStatus.Active;
            await _db.SaveChangesAsync();

            await _achievementService.CheckAndAwardAchievements(membership.UserId);

            TempData["Success"] = "Членството е одобрено.";
            return RedirectToAction(nameof(Details), new { id = membership.ClubId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectMember(int memberId, string? reason = null)
        {
            var membership = await _db.ClubMemberships.Include(m => m.Club).FirstOrDefaultAsync(m => m.Id == memberId);
            if (membership == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (membership.Club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            membership.Status = MembershipStatus.Rejected;
            membership.ApprovalNotes = reason;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Заявката е отхвърлена.";
            return RedirectToAction(nameof(Details), new { id = membership.ClubId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            var userId = _userManager.GetUserId(User);
            var membership = await _db.ClubMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.ClubId == id && m.Status == MembershipStatus.Active);

            if (membership == null)
            {
                TempData["Error"] = "Не сте член на този клуб.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _db.ClubMemberships.Remove(membership);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Напуснахте клуба.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
