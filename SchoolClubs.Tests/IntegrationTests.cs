using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolClubs.Web.Data;
using SchoolClubs.Web.Models;
using System.Net;
using Xunit;

namespace SchoolClubs.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {   // swap db for in-memory so we don't need sql server
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("IntegrationTestDb"));
                });
            });
        }

        [Fact]
        public async Task LandingPage_ReturnsSuccess()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task ClubsIndex_ReturnsSuccess()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Clubs");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("html", content, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task EventsIndex_ReturnsSuccess()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Events");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task NonExistentPage_Returns404()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = await client.GetAsync("/this-page-does-not-exist");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ClubDetails_NonExistentClub_Returns404()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Clubs/Details/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateClub_RequiresAuthentication_RedirectsToLogin()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/Clubs/Create");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("Login", response.Headers.Location?.ToString() ?? "");
        }

        [Fact]
        public async Task AdminArea_RequiresAuthentication_RedirectsToLogin()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/Admin");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task StaticFiles_CssLoads()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/css/site.css");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task StaticFiles_JsLoads()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/js/site.js");

            response.EnsureSuccessStatusCode();
        }
    }
}
