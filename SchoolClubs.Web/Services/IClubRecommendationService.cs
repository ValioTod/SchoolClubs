using SchoolClubs.Web.Models;

namespace SchoolClubs.Web.Services
{
    public interface IClubRecommendationService
    {
        Task<List<Club>> GetRecommendations(string userId, int count = 4);
    }
}
