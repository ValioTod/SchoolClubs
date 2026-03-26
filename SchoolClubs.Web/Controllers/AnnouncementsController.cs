using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Models.ViewModels;

namespace SchoolClubs.Web.Controllers
{
    [Authorize]
    public class AnnouncementsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnnouncementsController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int clubId)
        {
            var club = await _db.Clubs.FindAsync(clubId);
            if (club == null) return NotFound();

            var announcements = await _db.Announcements
                .Where(a => a.ClubId == clubId)
                .Include(a => a.Author)
                .OrderByDescending(a => a.IsPinned)
                .ThenByDescending(a => a.PostedOn)
                .ToListAsync();

            ViewBag.Club = club;
            return View(announcements);
        }

        public IActionResult Create(int clubId)
        {
            return View(new AnnouncementCreateViewModel { ClubId = clubId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = _userManager.GetUserId(User);
            var club = await _db.Clubs.FindAsync(vm.ClubId);
            if (club == null) return NotFound();

            var isMember = await _db.ClubMemberships.AnyAsync(m => m.UserId == userId && m.ClubId == vm.ClubId);
            if (!isMember && club.LeaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var announcement = new Announcement
            {
                Title = vm.Title,
                Content = vm.Content,
                Priority = vm.Priority,
                IsPinned = vm.IsPinned,
                ClubId = vm.ClubId,
                AuthorId = userId!
            };

            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Clubs", new { id = vm.ClubId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _db.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (announcement.AuthorId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var clubId = announcement.ClubId;
            _db.Announcements.Remove(announcement);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Clubs", new { id = clubId });
        }
    }
}
