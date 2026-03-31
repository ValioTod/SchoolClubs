namespace SchoolClubs.Web.Services
{
    public interface IAchievementService
    {
        Task CheckAndAwardAchievements(string userId);
        Task<int> GetTotalPoints(string userId);
        Task<List<(string UserId, string FullName, string? Picture, int Points, int Count)>> GetLeaderboard(int top = 10);
    }
}
