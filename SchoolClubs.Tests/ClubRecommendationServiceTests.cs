using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Services;
using Xunit;

namespace SchoolClubs.Tests
{
    public class ClubRecommendationServiceTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetRecommendations_NoMemberships_ReturnsPopularClubs()
        {
            var ctx = GetInMemoryContext("reco_test_1");

            ctx.Clubs.AddRange(
                new Club { Id = 1, Name = "Клуб А", Description = "Desc", Category = "Технологии", IsActive = true },
                new Club { Id = 2, Name = "Клуб Б", Description = "Desc", Category = "Спорт", IsActive = true },
                new Club { Id = 3, Name = "Клуб В", Description = "Desc", Category = "Изкуство", IsActive = true }
            );
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "other-user", ClubId = 1 });
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "other-user-2", ClubId = 1 });
            await ctx.SaveChangesAsync();

            var service = new ClubRecommendationService(ctx);
            var result = await service.GetRecommendations("new-user", 3);

            Assert.NotEmpty(result);
            Assert.Equal("Клуб А", result.First().Name);
        }

        [Fact]
        public async Task GetRecommendations_WithMemberships_ReturnsSameCategory()
        {
            var ctx = GetInMemoryContext("reco_test_2");

            ctx.Clubs.AddRange(
                new Club { Id = 10, Name = "Програмиране", Description = "Desc", Category = "Технологии", IsActive = true },
                new Club { Id = 11, Name = "Робототехника", Description = "Desc", Category = "Технологии", IsActive = true },
                new Club { Id = 12, Name = "Театър", Description = "Desc", Category = "Изкуство", IsActive = true }
            );
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "user-1", ClubId = 10 });
            await ctx.SaveChangesAsync();

            var service = new ClubRecommendationService(ctx);
            var result = await service.GetRecommendations("user-1", 2);

            Assert.Contains(result, c => c.Name == "Робототехника");
        }

        [Fact]
        public async Task GetRecommendations_ExcludesJoinedClubs()
        {
            var ctx = GetInMemoryContext("reco_test_3");

            ctx.Clubs.AddRange(
                new Club { Id = 20, Name = "Клуб X", Description = "Desc", Category = "Спорт", IsActive = true },
                new Club { Id = 21, Name = "Клуб Y", Description = "Desc", Category = "Спорт", IsActive = true }
            );
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "user-2", ClubId = 20 });
            await ctx.SaveChangesAsync();

            var service = new ClubRecommendationService(ctx);
            var result = await service.GetRecommendations("user-2", 5);

            Assert.DoesNotContain(result, c => c.Id == 20);
        }

        [Fact]
        public async Task GetRecommendations_ExcludesInactiveClubs()
        {
            var ctx = GetInMemoryContext("reco_test_4");

            ctx.Clubs.AddRange(
                new Club { Id = 30, Name = "Активен", Description = "Desc", Category = "Технологии", IsActive = true },
                new Club { Id = 31, Name = "Неактивен", Description = "Desc", Category = "Технологии", IsActive = false }
            );
            await ctx.SaveChangesAsync();

            var service = new ClubRecommendationService(ctx);
            var result = await service.GetRecommendations("user-3", 5);

            Assert.DoesNotContain(result, c => c.Name == "Неактивен");
        }

        [Fact]
        public async Task GetRecommendations_FillsUpWithOtherCategories()
        {
            var ctx = GetInMemoryContext("reco_test_5");

            ctx.Clubs.AddRange(
                new Club { Id = 40, Name = "My Club", Description = "Desc", Category = "Природа", IsActive = true },
                new Club { Id = 41, Name = "Other Sport", Description = "Desc", Category = "Спорт", IsActive = true }
            );
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "user-4", ClubId = 40 });
            await ctx.SaveChangesAsync();

            var service = new ClubRecommendationService(ctx);
            var result = await service.GetRecommendations("user-4", 3);

            Assert.Contains(result, c => c.Name == "Other Sport");
        }
    }
}
