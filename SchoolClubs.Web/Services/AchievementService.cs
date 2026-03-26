using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;

namespace SchoolClubs.Web.Services
{
    public class AchievementService
    {
        private readonly AppDbContext _db;

        public AchievementService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CheckAndAwardAchievements(string userId)
        {
            var allAchievements = await _db.Achievements.ToListAsync();
            var alreadyEarned = await _db.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            foreach (var achievement in allAchievements)
            {
                if (alreadyEarned.Contains(achievement.Id))
                    continue;

                bool earned = await CheckCriteria(userId, achievement);
                if (earned)
                {
                    _db.UserAchievements.Add(new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = achievement.Id,
                        EarnedOn = DateTime.UtcNow
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

        private async Task<bool> CheckCriteria(string userId, Achievement achievement)
        {
            switch (achievement.Type)
            {
                case AchievementType.ClubsJoined:
                    var clubCount = await _db.ClubMemberships.CountAsync(m => m.UserId == userId);
                    return clubCount >= achievement.Threshold;

                case AchievementType.EventsAttended:
                    var eventsCount = await _db.EventAttendances
                        .CountAsync(ea => ea.UserId == userId && ea.HasAttended);
                    return eventsCount >= achievement.Threshold;

                case AchievementType.DaysAsMember:
                    var oldest = await _db.ClubMemberships
                        .Where(m => m.UserId == userId)
                        .OrderBy(m => m.JoinedOn)
                        .FirstOrDefaultAsync();
                    if (oldest == null) return false;
                    var days = (DateTime.UtcNow - oldest.JoinedOn).TotalDays;
                    return days >= achievement.Threshold;

                case AchievementType.AnnouncementsMade:
                    var annCount = await _db.Announcements.CountAsync(a => a.AuthorId == userId);
                    return annCount >= achievement.Threshold;

                case AchievementType.PhotosUploaded:
                    var photoCount = await _db.GalleryPhotos.CountAsync(g => g.UploadedById == userId);
                    return photoCount >= achievement.Threshold;

                default:
                    return false;
            }
        }

        public async Task<int> GetTotalPoints(string userId)
        {
            return await _db.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Achievement)
                .SumAsync(ua => ua.Achievement.PointsAwarded);
        }

        public async Task<List<(string UserId, string FullName, string? Picture, int Points, int Count)>> GetLeaderboard(int top = 10)
        {
            var data = await _db.UserAchievements
                .Include(ua => ua.Achievement)
                .Include(ua => ua.User)
                .GroupBy(ua => new { ua.UserId, ua.User.FullName, ua.User.ProfilePicturePath })
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.FullName,
                    g.Key.ProfilePicturePath,
                    Points = g.Sum(x => x.Achievement.PointsAwarded),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Points)
                .Take(top)
                .ToListAsync();

            return data.Select(d =>
                (d.UserId, d.FullName, d.ProfilePicturePath, d.Points, d.Count)
            ).ToList();
        }
    }
}
