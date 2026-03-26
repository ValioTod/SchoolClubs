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
    public class EventsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AchievementService _achievementService;

        public EventsController(AppDbContext db, UserManager<ApplicationUser> userManager, AchievementService achievementService)
        {
            _db = db;
            _userManager = userManager;
            _achievementService = achievementService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _db.Events
                .Include(e => e.Club)
                .Include(e => e.Attendees)
                .Where(e => !e.IsCancelled)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return View(events);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ev = await _db.Events
                .Include(e => e.Club)
                .Include(e => e.Attendees).ThenInclude(a => a.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var attendance = ev.Attendees.FirstOrDefault(a => a.UserId == userId);
            var isMember = await _db.ClubMemberships.AnyAsync(m => m.UserId == userId && m.ClubId == ev.ClubId);

            var vm = new EventDetailsViewModel
            {
                Event = ev,
                IsSignedUp = attendance != null,
                HasAttended = attendance?.HasAttended ?? false,
                AttendeeCount = ev.Attendees.Count,
                CanSignUp = (ev.MaxAttendees == 0 || ev.Attendees.Count < ev.MaxAttendees) && ev.StartDate > DateTime.UtcNow,
                IsClubMember = isMember
            };

            return View(vm);
        }

        [Authorize]
        public IActionResult Create(int clubId)
        {
            var vm = new EventCreateViewModel { ClubId = clubId };
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = _userManager.GetUserId(User);
            var club = await _db.Clubs.FindAsync(vm.ClubId);
            if (club == null) return NotFound();

            if (club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var ev = new Event
            {
                Title = vm.Title,
                Description = vm.Description,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                Location = vm.Location,
                MaxAttendees = vm.MaxAttendees,
                ClubId = vm.ClubId,
                AttendanceCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ev.Id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var ev = await _db.Events.Include(e => e.Attendees).FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();

            if (ev.Attendees.Any(a => a.UserId == user.Id))
            {
                TempData["Error"] = "Вече сте записани за това събитие.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (ev.MaxAttendees > 0 && ev.Attendees.Count >= ev.MaxAttendees)
            {
                TempData["Error"] = "Събитието е пълно.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _db.EventAttendances.Add(new EventAttendance
            {
                UserId = user.Id,
                EventId = id
            });
            await _db.SaveChangesAsync();

            TempData["Success"] = "Записахте се за събитието!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int id, string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var ev = await _db.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();

            if (!string.Equals(ev.AttendanceCode, code, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Грешен код за присъствие.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var attendance = await _db.EventAttendances
                .FirstOrDefaultAsync(a => a.UserId == user.Id && a.EventId == id);

            if (attendance == null)
            {
                attendance = new EventAttendance
                {
                    UserId = user.Id,
                    EventId = id,
                    HasAttended = true,
                    CheckedInAt = DateTime.UtcNow
                };
                _db.EventAttendances.Add(attendance);
            }
            else
            {
                attendance.HasAttended = true;
                attendance.CheckedInAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            await _achievementService.CheckAndAwardAchievements(user.Id);

            TempData["Success"] = "Присъствието ви е отбелязано!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSignUp(int id)
        {
            var userId = _userManager.GetUserId(User);
            var attendance = await _db.EventAttendances
                .FirstOrDefaultAsync(a => a.UserId == userId && a.EventId == id);

            if (attendance != null)
            {
                _db.EventAttendances.Remove(attendance);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Отказахте участието си.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelEvent(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ev = await _db.Events.Include(e => e.Club).FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();

            if (ev.Club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            ev.IsCancelled = true;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event cancelled.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreEvent(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ev = await _db.Events.Include(e => e.Club).FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();

            if (ev.Club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            ev.IsCancelled = false;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event restored.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
