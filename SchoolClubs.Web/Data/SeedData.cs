using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolClubs.Web.Models;

namespace SchoolClubs.Web.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created from the current model
            // This works on any machine — no migrations needed
            await context.Database.EnsureCreatedAsync();

            // If roles already exist, skip all seeding
            if (await roleManager.RoleExistsAsync("Admin"))
                return;

            string[] roles = { "Admin", "Teacher", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (await userManager.FindByEmailAsync("admin@schoolclubs.bg") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@schoolclubs.bg",
                    Email = "admin@schoolclubs.bg",
                    FullName = "Administrator",
                    Grade = 0,
                    GradeSection = "",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Create Teacher user
            if (await userManager.FindByEmailAsync("teacher@schoolclubs.bg") == null)
            {
                var teacher = new ApplicationUser
                {
                    UserName = "teacher@schoolclubs.bg",
                    Email = "teacher@schoolclubs.bg",
                    FullName = "Prof. Maria Nikolova",
                    Grade = 0,
                    GradeSection = "Teacher",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(teacher, "Teacher123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(teacher, "Teacher");
            }

            var studentEmails = new[]
            {
                ("ivan.petrov@school.bg", "Ivan Petrov", 10, "A"),
                ("maria.ivanova@school.bg", "Maria Ivanova", 11, "B"),
                ("georgi.dimitrov@school.bg", "Georgi Dimitrov", 9, "A"),
                ("elena.stoyanova@school.bg", "Elena Stoyanova", 12, "V"),
                ("nikolay.kolev@school.bg", "Nikolay Kolev", 10, "B"),
            };

            foreach (var (email, name, grade, section) in studentEmails)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var student = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = name,
                        Grade = grade,
                        GradeSection = section,
                        EmailConfirmed = true
                    };
                    var res = await userManager.CreateAsync(student, "Student123!");
                    if (res.Succeeded)
                        await userManager.AddToRoleAsync(student, "Student");
                }
            }

            if (await context.Clubs.AnyAsync())
                return;

            var allStudents = await userManager.GetUsersInRoleAsync("Student");
            var leader = allStudents.FirstOrDefault();

            var clubs = new List<Club>
            {
                new Club
                {
                    Name = "Programming Club",
                    Description = "We study C#, Python and web technologies. Preparation for informatics competitions and building our own projects.",
                    Category = "Technology",
                    MeetingSchedule = "Tuesday and Thursday 15:00-17:00",
                    MeetingLocation = "Room 301",
                    MaxMembers = 25,
                    LeaderId = leader?.Id,
                    IsActive = true
                },
                new Club
                {
                    Name = "Debate Club",
                    Description = "We learn to argue our position, speak publicly and think critically. We participate in inter-school tournaments.",
                    Category = "Humanities",
                    MeetingSchedule = "Wednesday 14:30-16:00",
                    MeetingLocation = "Hall 102",
                    MaxMembers = 20,
                    LeaderId = allStudents.Skip(1).FirstOrDefault()?.Id
                },
                new Club
                {
                    Name = "Photography and Video",
                    Description = "We shoot, edit and create visual content. From basics of composition to drone work.",
                    Category = "Art",
                    MeetingSchedule = "Friday 14:00-16:30",
                    MeetingLocation = "Media Room",
                    MaxMembers = 15,
                    LeaderId = allStudents.Skip(2).FirstOrDefault()?.Id
                },
                new Club
                {
                    Name = "Eco Club",
                    Description = "We take care of the school garden, organize clean-ups and recycling campaigns.",
                    Category = "Nature",
                    MeetingSchedule = "Monday 15:00-16:30",
                    MeetingLocation = "School Yard / Room 205",
                    MaxMembers = 30,
                    LeaderId = allStudents.Skip(3).FirstOrDefault()?.Id
                },
                new Club
                {
                    Name = "Robotics",
                    Description = "We build robots with Arduino and Raspberry Pi. Preparation for First Lego League and other competitions.",
                    Category = "Technology",
                    MeetingSchedule = "Monday and Wednesday 15:30-17:00",
                    MeetingLocation = "Lab 401",
                    MaxMembers = 18
                },
                new Club
                {
                    Name = "Theater Troupe",
                    Description = "We prepare performances for school celebrations. Improvisations, acting skills, stage speech.",
                    Category = "Art",
                    MeetingSchedule = "Tuesday and Friday 15:00-17:00",
                    MeetingLocation = "Assembly Hall",
                    MaxMembers = 25
                },
                new Club
                {
                    Name = "Chess Club",
                    Description = "For beginners and advanced players. Weekly tournaments, preparation for regional and national competitions.",
                    Category = "Sports",
                    MeetingSchedule = "Thursday 14:00-16:00",
                    MeetingLocation = "Room 108",
                    MaxMembers = 20
                },
                new Club
                {
                    Name = "Creative Writing Club",
                    Description = "We write stories, poetry, essays. We have our own literary newspaper and do live readings.",
                    Category = "Humanities",
                    MeetingSchedule = "Wednesday 14:30-16:00",
                    MeetingLocation = "Library",
                    MaxMembers = 15
                }
            };

            context.Clubs.AddRange(clubs);
            await context.SaveChangesAsync();

            var programmingClub = clubs[0];
            foreach (var s in allStudents.Take(3))
            {
                context.ClubMemberships.Add(new ClubMembership
                {
                    UserId = s.Id,
                    ClubId = programmingClub.Id,
                    Role = s.Id == leader?.Id ? MemberRole.President : MemberRole.Member
                });
            }

            var debateClub = clubs[1];
            foreach (var s in allStudents.Skip(1).Take(2))
            {
                context.ClubMemberships.Add(new ClubMembership
                {
                    UserId = s.Id,
                    ClubId = debateClub.Id
                });
            }

            await context.SaveChangesAsync();

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Hackathon 2026",
                    Description = "24-hour hackathon on the topic 'Green Technologies'. Teams of 3. Food and drinks provided.",
                    StartDate = DateTime.UtcNow.AddDays(14),
                    EndDate = DateTime.UtcNow.AddDays(15),
                    Location = "Gym",
                    MaxAttendees = 60,
                    ClubId = programmingClub.Id,
                    AttendanceCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
                },
                new Event
                {
                    Title = "Debate: AI in Education",
                    Description = "Public debate on the role of artificial intelligence in education. Open to all.",
                    StartDate = DateTime.UtcNow.AddDays(7),
                    Location = "Hall 102",
                    MaxAttendees = 40,
                    ClubId = debateClub.Id,
                    AttendanceCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
                },
                new Event
                {
                    Title = "Photography Walk Downtown",
                    Description = "Meet in front of the school at 10:00 on Saturday. Bring your cameras/phones. We will explore the architecture.",
                    StartDate = DateTime.UtcNow.AddDays(10),
                    Location = "In front of the school",
                    MaxAttendees = 0,
                    ClubId = clubs[2].Id
                }
            };

            context.Events.AddRange(events);
            await context.SaveChangesAsync();

            if (leader != null)
            {
                context.Announcements.AddRange(new[]
                {
                    new Announcement
                    {
                        Title = "Welcome to the New School Year!",
                        Content = "Club registration is open. Browse available clubs and sign up by October 15.",
                        Priority = AnnouncementPriority.High,
                        IsPinned = true,
                        ClubId = programmingClub.Id,
                        AuthorId = leader.Id
                    },
                    new Announcement
                    {
                        Title = "Schedule Change This Week",
                        Content = "Due to renovation of Room 301, Thursday's meeting will be in Room 205.",
                        Priority = AnnouncementPriority.Normal,
                        ClubId = programmingClub.Id,
                        AuthorId = leader.Id
                    }
                });
            }

            await context.SaveChangesAsync();

            // Seed achievements
            var achievements = new List<Achievement>
            {
                new Achievement { Name = "First Steps", Description = "Joined your first club", Type = AchievementType.ClubsJoined, Threshold = 1, PointsAwarded = 10 },
                new Achievement { Name = "Social Butterfly", Description = "Member of 3 clubs", Type = AchievementType.ClubsJoined, Threshold = 3, PointsAwarded = 25 },
                new Achievement { Name = "Activist", Description = "Attended 5 events", Type = AchievementType.EventsAttended, Threshold = 5, PointsAwarded = 30 },
                new Achievement { Name = "Event Fan", Description = "Attended 10 events", Type = AchievementType.EventsAttended, Threshold = 10, PointsAwarded = 50 },
                new Achievement { Name = "Veteran", Description = "Member of a club for more than 90 days", Type = AchievementType.DaysAsMember, Threshold = 90, PointsAwarded = 40 },
                new Achievement { Name = "Voice of the People", Description = "Published 5 announcements", Type = AchievementType.AnnouncementsMade, Threshold = 5, PointsAwarded = 20 },
                new Achievement { Name = "Photographer", Description = "Uploaded 10 photos to the gallery", Type = AchievementType.PhotosUploaded, Threshold = 10, PointsAwarded = 35 },
            };
            context.Achievements.AddRange(achievements);
            await context.SaveChangesAsync();
        }
    }
}