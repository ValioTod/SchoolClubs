using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolClubs.Web.Controllers;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using SchoolClubs.Web.Services;
using System.Security.Claims;
using Xunit;

namespace SchoolClubs.Tests
{
    public class ClubsControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "test@test.com")
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        [Fact]
        public async Task Index_ReturnsClubList()
        {
            var ctx = GetInMemoryContext("ctrl_test_1");
            ctx.Clubs.AddRange(
                new Club { Id = 1, Name = "Club 1", Description = "Desc", Category = "Спорт", IsActive = true },
                new Club { Id = 2, Name = "Club 2", Description = "Desc", Category = "Технологии", IsActive = true }
            );
            await ctx.SaveChangesAsync();

            var mockUm = GetMockUserManager();
            var achService = new AchievementService(ctx);
            var controller = new ClubsController(ctx, mockUm.Object, achService);

            var result = await controller.Index(null, null) as ViewResult;
            Assert.NotNull(result);
            var model = result.Model as SchoolClubs.Web.Models.ViewModels.ClubListViewModel;
            Assert.NotNull(model);
            Assert.Equal(2, model.TotalCount);
        }

        [Fact]
        public async Task Index_FiltersByCategory()
        {
            var ctx = GetInMemoryContext("ctrl_test_2");
            ctx.Clubs.AddRange(
                new Club { Id = 10, Name = "Sport Club", Description = "D", Category = "Спорт", IsActive = true },
                new Club { Id = 11, Name = "Tech Club", Description = "D", Category = "Технологии", IsActive = true }
            );
            await ctx.SaveChangesAsync();

            var mockUm = GetMockUserManager();
            var achService = new AchievementService(ctx);
            var controller = new ClubsController(ctx, mockUm.Object, achService);

            var result = await controller.Index(null, "Спорт") as ViewResult;
            var model = result!.Model as SchoolClubs.Web.Models.ViewModels.ClubListViewModel;
            Assert.Single(model!.Clubs);
            Assert.Equal("Sport Club", model.Clubs.First().Name);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenClubMissing()
        {
            var ctx = GetInMemoryContext("ctrl_test_3");
            var mockUm = GetMockUserManager();
            mockUm.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("u1");

            var achService = new AchievementService(ctx);
            var controller = new ClubsController(ctx, mockUm.Object, achService);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("u1") }
            };

            var result = await controller.Details(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsClubInfo()
        {
            var ctx = GetInMemoryContext("ctrl_test_4");
            ctx.Clubs.Add(new Club { Id = 50, Name = "Test Club", Description = "D", Category = "Спорт", IsActive = true });
            await ctx.SaveChangesAsync();

            var mockUm = GetMockUserManager();
            mockUm.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("u1");

            var achService = new AchievementService(ctx);
            var controller = new ClubsController(ctx, mockUm.Object, achService);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("u1") }
            };

            var result = await controller.Details(50) as ViewResult;
            Assert.NotNull(result);
            var model = result.Model as SchoolClubs.Web.Models.ViewModels.ClubDetailsViewModel;
            Assert.Equal("Test Club", model!.Club.Name);
        }
    }
}
