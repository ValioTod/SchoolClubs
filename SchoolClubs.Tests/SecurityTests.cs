using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolClubs.Web.Data;
using System.Net;
using Xunit;

namespace SchoolClubs.Tests
{
    public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SecurityTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("SecurityTestDb"));
                });
            });
        }

        // sql injection - make sure EF parameterizes properly

        [Theory]
        [InlineData("'; DROP TABLE Clubs; --")]
        [InlineData("1' OR '1'='1")]
        [InlineData("' UNION SELECT * FROM AspNetUsers --")]
        [InlineData("'; DELETE FROM Events; --")]
        [InlineData("1; EXEC xp_cmdshell('dir') --")]
        public async Task ClubSearch_SqlInjection_IsHandledSafely(string maliciousInput)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/Clubs?search={Uri.EscapeDataString(maliciousInput)}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // should not see raw db errors
            Assert.DoesNotContain("SqlException", content);
            Assert.DoesNotContain("Internal Server Error", content);
        }

        [Theory]
        [InlineData("'; DROP TABLE Clubs; --")]
        [InlineData("1' OR '1'='1")]
        public async Task ClubCategory_SqlInjection_IsHandledSafely(string maliciousInput)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/Clubs?category={Uri.EscapeDataString(maliciousInput)}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain("SqlException", content);
        }

        // xss - razor should encode these automatically

        [Theory]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("<img src=x onerror=alert('xss')>")]
        [InlineData("javascript:alert('xss')")]
        [InlineData("<svg onload=alert('xss')>")]
        [InlineData("'\"><script>alert(document.cookie)</script>")]
        public async Task ClubSearch_XssPayload_IsEncodedInOutput(string xssPayload)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/Clubs?search={Uri.EscapeDataString(xssPayload)}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // raw tags should be html-encoded by razor
            Assert.DoesNotContain("<script>alert", content);
            Assert.DoesNotContain("<svg onload", content);
            Assert.DoesNotContain("<img src=x onerror", content);
        }

        [Fact]
        public async Task EventsPage_XssInQueryString_IsEncodedInOutput()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Events?search=<script>alert('xss')</script>");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain("<script>alert", content);
        }

        // csrf - posting without token should fail

        [Fact]
        public async Task PostEndpoint_WithoutAntiForgeryToken_Fails()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("id", "1")
            });

            var response = await client.PostAsync("/Clubs/Join/1", formContent);

            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // auth - protected pages should redirect to login

        [Theory]
        [InlineData("/Clubs/Create")]
        [InlineData("/Clubs/Edit/1")]
        [InlineData("/Announcements/Create")]
        public async Task ProtectedEndpoints_RequireAuthentication(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("Login", response.Headers.Location?.ToString() ?? "",
                StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Response_DoesNotExposeServerInfo()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");

            Assert.False(response.Headers.Contains("X-Powered-By"));
        }

        // edge cases

        [Fact]
        public async Task ClubSearch_VeryLongInput_DoesNotCrash()
        {
            var client = _factory.CreateClient();
            var longString = new string('A', 10000);
            var response = await client.GetAsync($"/Clubs?search={longString}");

            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest
                || response.StatusCode == HttpStatusCode.RequestUriTooLong);
        }

        [Fact]
        public async Task ClubDetails_NegativeId_DoesNotCrash()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Clubs/Details/-1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
