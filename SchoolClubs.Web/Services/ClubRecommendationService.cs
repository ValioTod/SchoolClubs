using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;

namespace SchoolClubs.Web.Services
{
    public class ClubRecommendationService : IClubRecommendationService
    {
        private readonly AppDbContext _db;

        public ClubRecommendationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Club>> GetRecommendations(string userId, int count = 4)
        {
            var myClubIds = await _db.ClubMemberships
                .Where(m => m.UserId == userId)
                .Select(m => m.ClubId)
                .ToListAsync();

            if (!myClubIds.Any())
            {
                return await _db.Clubs
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.Members.Count)
                    .Take(count)
                    .ToListAsync();
            }

            var myCategories = await _db.Clubs
                .Where(c => myClubIds.Contains(c.Id))
                .Select(c => c.Category)
                .Distinct()
                .ToListAsync();

            var recommended = await _db.Clubs
                .Where(c => c.IsActive
                    && !myClubIds.Contains(c.Id)
                    && myCategories.Contains(c.Category))
                .OrderByDescending(c => c.Members.Count)
                .Take(count)
                .ToListAsync();

            if (recommended.Count < count)
            {
                var remaining = count - recommended.Count;
                var alreadyIncluded = recommended.Select(r => r.Id).ToList();

                var extra = await _db.Clubs
                    .Where(c => c.IsActive
                        && !myClubIds.Contains(c.Id)
                        && !alreadyIncluded.Contains(c.Id))
                    .OrderByDescending(c => c.Members.Count)
                    .Take(remaining)
                    .ToListAsync();

                recommended.AddRange(extra);
            }

            return recommended;
        }
    }
}
