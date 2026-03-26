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
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var feedbacks = await _db.Feedbacks
                .Where(f => f.AuthorId == userId)
                .OrderByDescending(f => f.SubmittedOn)
                .ToListAsync();

            return View(feedbacks);
        }

        public IActionResult Create()
        {
            return View(new FeedbackCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = _userManager.GetUserId(User);

            var feedback = new Feedback
            {
                Subject = vm.Subject,
                Message = vm.Message,
                Category = vm.Category,
                AuthorId = userId!
            };

            _db.Feedbacks.Add(feedback);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Feedback submitted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
