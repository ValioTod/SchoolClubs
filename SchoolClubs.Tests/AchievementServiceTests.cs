using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Services;
using Xunit;

namespace SchoolClubs.Tests
{
    public class AchievementServiceTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CheckAndAward_ClubsJoined_AwardsWhenThresholdMet()
        {
            var ctx = GetInMemoryContext("ach_test_1");

            ctx.Achievements.Add(new Achievement
            {
                Id = 1,
                Name = "First Club",
                Type = AchievementType.ClubsJoined,
                Threshold = 1,
                PointsAwarded = 10
            });
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "u1", ClubId = 100 });
            ctx.Clubs.Add(new Club { Id = 100, Name = "Test", Description = "Test", IsActive = true });
            await ctx.SaveChangesAsync();

            var service = new AchievementService(ctx);
            await service.CheckAndAwardAchievements("u1");

            var earned = await ctx.UserAchievements.Where(ua => ua.UserId == "u1").ToListAsync();
            Assert.Single(earned);
            Assert.Equal(1, earned.First().AchievementId);
        }

        [Fact]
        public async Task CheckAndAward_DoesNotDuplicateAchievements()
        {
            var ctx = GetInMemoryContext("ach_test_2");

            ctx.Achievements.Add(new Achievement
            {
                Id = 2,
                Name = "First Club",
                Type = AchievementType.ClubsJoined,
                Threshold = 1,
                PointsAwarded = 10
            });
            ctx.ClubMemberships.Add(new ClubMembership { UserId = "u2", ClubId = 200 });
            ctx.Clubs.Add(new Club { Id = 200, Name = "Test", Description = "Test", IsActive = true });
            await ctx.SaveChangesAsync();

            var service = new AchievementService(ctx);
            await service.CheckAndAwardAchievements("u2");
            await service.CheckAndAwardAchievements("u2");

            var count = await ctx.UserAchievements.CountAsync(ua => ua.UserId == "u2");
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckAndAward_EventsAttended_RequiresHasAttended()
        {
            var ctx = GetInMemoryContext("ach_test_3");

            ctx.Achievements.Add(new Achievement
            {
                Id = 3,
                Name = "Event Goer",
                Type = AchievementType.EventsAttended,
                Threshold = 1,
                PointsAwarded = 15
            });
            ctx.EventAttendances.Add(new EventAttendance { UserId = "u3", EventId = 1, HasAttended = false });
            await ctx.SaveChangesAsync();

            var service = new AchievementService(ctx);
            await service.CheckAndAwardAchievements("u3");

            var earned = await ctx.UserAchievements.CountAsync(ua => ua.UserId == "u3");
            Assert.Equal(0, earned);
        }

        [Fact]
        public async Task GetTotalPoints_SumsCorrectly()
        {
            var ctx = GetInMemoryContext("ach_test_4");

            ctx.Achievements.AddRange(
                new Achievement { Id = 10, Name = "A", PointsAwarded = 10, Type = AchievementType.ClubsJoined, Threshold = 1 },
                new Achievement { Id = 11, Name = "B", PointsAwarded = 25, Type = AchievementType.EventsAttended, Threshold = 1 }
            );
            ctx.UserAchievements.AddRange(
                new UserAchievement { UserId = "u4", AchievementId = 10 },
                new UserAchievement { UserId = "u4", AchievementId = 11 }
            );
            await ctx.SaveChangesAsync();

            var service = new AchievementService(ctx);
            var points = await service.GetTotalPoints("u4");

            Assert.Equal(35, points);
        }

        [Fact]
        public async Task GetLeaderboard_OrdersByPointsDescending()
        {
            var ctx = GetInMemoryContext("ach_test_5");

            var user1 = new ApplicationUser { Id = "leader1", FullName = "Alice", UserName = "a@a.com", Email = "a@a.com" };
            var user2 = new ApplicationUser { Id = "leader2", FullName = "Bob", UserName = "b@b.com", Email = "b@b.com" };
            ctx.Users.AddRange(user1, user2);

            ctx.Achievements.AddRange(
                new Achievement { Id = 20, Name = "Small", PointsAwarded = 5, Type = AchievementType.ClubsJoined, Threshold = 1 },
                new Achievement { Id = 21, Name = "Big", PointsAwarded = 50, Type = AchievementType.EventsAttended, Threshold = 1 }
            );
            ctx.UserAchievements.AddRange(
                new UserAchievement { UserId = "leader1", AchievementId = 20 },
                new UserAchievement { UserId = "leader2", AchievementId = 20 },
                new UserAchievement { UserId = "leader2", AchievementId = 21 }
            );
            await ctx.SaveChangesAsync();

            var service = new AchievementService(ctx);
            var board = await service.GetLeaderboard(10);

            Assert.Equal("Bob", board.First().FullName);
            Assert.Equal(55, board.First().Points);
        }
    }
}
