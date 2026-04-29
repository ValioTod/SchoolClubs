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
                    Name = "Клуб по програмиране",
                    NameEn = "Programming Club",
                    Description = "Изучаваме C#, Python и уеб технологии. Подготовка за олимпиади по информатика и разработка на собствени проекти.",
                    DescriptionEn = "We study C#, Python and web technologies. Preparation for informatics olympiads and developing our own projects.",
                    Category = "Технологии",
                    MeetingSchedule = "Вторник и Четвъртък 15:00-17:00",
                    MeetingLocation = "Кабинет 301",
                    MaxMembers = 25,
                    LeaderId = leader?.Id,
                    IsActive = true
                },
                new Club
                {
                    Name = "Дебатен клуб",
                    NameEn = "Debate Club",
                    Description = "Учим се да защитаваме позицията си, да говорим публично и да мислим критично. Участваме в междуучилищни турнири.",
                    DescriptionEn = "We learn to defend our position, speak publicly and think critically. We participate in inter-school tournaments.",
                    Category = "Хуманитарни",
                    MeetingSchedule = "Сряда 14:30-16:00",
                    MeetingLocation = "Зала 102",
                    MaxMembers = 20,
                    LeaderId = allStudents.Skip(1).FirstOrDefault()?.Id
                },
                new Club
                {
                    Name = "Фотография и видео",
                    NameEn = "Photography & Video",
                    Description = "Снимаме, монтираме и създаваме визуално съдържание. От основите на композицията до работа с дрон.",
                    DescriptionEn = "We shoot, edit and create visual content. From the basics of composition to working with a drone.",
                    Category = "Изкуство",
                    MeetingSchedule = "Петък 14:00-16:30",
                    MeetingLocation = "Медийна зала",
                    MaxMembers = 15,
                    LeaderId = allStudents.Skip(2).FirstOrDefault()?.Id
                },
                new Club
                {
                    Name = "Еко клуб",
                    NameEn = "Eco Club",
                    Description = "Грижим се за училищната градина, организираме почиствания и кампании за рециклиране.",
                    DescriptionEn = "We take care of the school garden, organise clean-ups and recycling campaigns.",
                    Category = "Природа",
                    MeetingSchedule = "Понеделник 15:00-16:30",
                    MeetingLocation = "Двор / Кабинет 205",
                    MaxMembers = 30,
                    LeaderId = allStudents.Skip(3).FirstOrDefault()?.Id
                },
                new Club
                {
                    Name = "Роботика",
                    NameEn = "Robotics",
                    Description = "Строим роботи с Arduino и Raspberry Pi. Подготовка за First Lego League и други състезания.",
                    DescriptionEn = "We build robots with Arduino and Raspberry Pi. Preparation for First Lego League and other competitions.",
                    Category = "Технологии",
                    MeetingSchedule = "Понеделник и Сряда 15:30-17:00",
                    MeetingLocation = "Лаборатория 401",
                    MaxMembers = 18
                },
                new Club
                {
                    Name = "Театрална трупа",
                    NameEn = "Theatre Troupe",
                    Description = "Подготвяме представления за училищни тържества. Импровизации, актьорски умения, сценична реч.",
                    DescriptionEn = "We prepare performances for school celebrations. Improvisation, acting skills, stage speech.",
                    Category = "Изкуство",
                    MeetingSchedule = "Вторник и Петък 15:00-17:00",
                    MeetingLocation = "Актова зала",
                    MaxMembers = 25
                },
                new Club
                {
                    Name = "Шахматен клуб",
                    NameEn = "Chess Club",
                    Description = "За начинаещи и напреднали. Седмични турнири, подготовка за регионални и национални състезания.",
                    DescriptionEn = "For beginners and advanced players. Weekly tournaments, preparation for regional and national competitions.",
                    Category = "Спорт",
                    MeetingSchedule = "Четвъртък 14:00-16:00",
                    MeetingLocation = "Кабинет 108",
                    MaxMembers = 20
                },
                new Club
                {
                    Name = "Клуб за творческо писане",
                    NameEn = "Creative Writing Club",
                    Description = "Пишем разкази, поезия, есета. Имаме собствен литературен вестник и правим четения на живо.",
                    DescriptionEn = "We write stories, poetry and essays. We have our own literary newspaper and do live readings.",
                    Category = "Хуманитарни",
                    MeetingSchedule = "Сряда 14:30-16:00",
                    MeetingLocation = "Библиотека",
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
                    Title = "Хакатон 2026",
                    Description = "24-часов хакатон на тема 'Зелени технологии'. Отбори от по 3 човека. Осигурени са храна и напитки.",
                    StartDate = DateTime.UtcNow.AddDays(14),
                    EndDate = DateTime.UtcNow.AddDays(15),
                    Location = "Физкултурен салон",
                    MaxAttendees = 60,
                    ClubId = programmingClub.Id,
                    AttendanceCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
                },
                new Event
                {
                    Title = "Дебат: ИИ в образованието",
                    Description = "Публичен дебат за ролята на изкуствения интелект в образованието. Открито за всички.",
                    StartDate = DateTime.UtcNow.AddDays(7),
                    Location = "Зала 102",
                    MaxAttendees = 40,
                    ClubId = debateClub.Id,
                    AttendanceCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
                },
                new Event
                {
                    Title = "Фото разходка в центъра",
                    Description = "Срещаме се пред училището в 10:00 в събота. Вземете камери или телефони. Ще изследваме архитектурата.",
                    StartDate = DateTime.UtcNow.AddDays(10),
                    Location = "Пред училището",
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
                        Title = "Добре дошли в новата учебна година!",
                        Content = "Записването в клубове е отворено. Разгледайте наличните клубове и се запишете до 15 октомври.",
                        Priority = AnnouncementPriority.High,
                        IsPinned = true,
                        ClubId = programmingClub.Id,
                        AuthorId = leader.Id
                    },
                    new Announcement
                    {
                        Title = "Промяна в графика тази седмица",
                        Content = "Поради ремонт в кабинет 301, четвъртъшната среща ще се проведе в кабинет 205.",
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