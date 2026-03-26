# SchoolClubs

Уеб приложение за управление на училищни клубове, изградено с ASP.NET Core 8.0 MVC.

## Технологии

- **ASP.NET Core 8.0 MVC**
- **Entity Framework Core 8.0** с SQLite
- **ASP.NET Core Identity** за автентикация и авторизация
- **Bootstrap 5.3** за UI
- **xUnit + Moq** за тестване

## Функционалности

### Управление на клубове
- Разглеждане, търсене и филтриране по категория
- Създаване, редактиране на клубове (Admin/Teacher)
- Записване / напускане на клуб
- Лимит на членове

### Събития с присъствие
- Създаване на събития към клуб
- Записване за събитие (RSVP) с лимит на участници
- Отбелязване на присъствие с код (QR check-in)
- Преглед на предстоящи и минали събития

### Система за награди (Achievements)
- Автоматично присъждане на бадж-ове при достигане на условия
- Видове: брой клубове, посетени събития, дни членство, обяви, снимки
- Точкова система с натрупване

### Препоръчване на клубове
- Анализира категориите на текущите клубове на потребителя
- Предлага нови клубове от същите или сходни категории
- За нови потребители показва най-популярните

### Класация (Leaderboard)
- Топ ученици по натрупани точки
- Профилна страница с награди и статистики
- Ранг в общата класация

### Обяви
- Публикуване на обяви с приоритет (нисък, нормален, висок, спешен)
- Закачване на важни обяви в началото
- Автор и дата на публикуване

### Галерия
- Качване на снимки към клуб
- Валидация на формат и размер (макс. 5MB)
- Описание на снимките

### Табло (Dashboard)
- Общи статистики
- Моите клубове и предстоящи събития
- Последни обяви и награди
- Препоръчани клубове

## Роли

| Роля | Права |
|------|-------|
| Student | Разглеждане, записване, участие в събития |
| Teacher | Създаване и управление на клубове |
| Admin | Пълен достъп до всички функционалности |

## Структура на проекта

```
SchoolClubs/
├── SchoolClubs.sln
├── SchoolClubs.Web/
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── ClubsController.cs
│   │   ├── EventsController.cs
│   │   ├── AnnouncementsController.cs
│   │   ├── GalleryController.cs
│   │   └── ProfileController.cs
│   ├── Models/
│   │   ├── ApplicationUser.cs
│   │   ├── Club.cs
│   │   ├── ClubMembership.cs
│   │   ├── Event.cs
│   │   ├── EventAttendance.cs
│   │   ├── Announcement.cs
│   │   ├── Achievement.cs
│   │   ├── UserAchievement.cs
│   │   ├── GalleryPhoto.cs
│   │   └── ViewModels/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── SeedData.cs
│   ├── Services/
│   │   ├── ClubRecommendationService.cs
│   │   └── AchievementService.cs
│   ├── Views/
│   │   ├── Home/
│   │   ├── Clubs/
│   │   ├── Events/
│   │   ├── Announcements/
│   │   ├── Gallery/
│   │   ├── Profile/
│   │   └── Shared/
│   └── wwwroot/
│       ├── css/site.css
│       └── js/site.js
└── SchoolClubs.Tests/
    ├── ClubRecommendationServiceTests.cs
    ├── AchievementServiceTests.cs
    └── ClubsControllerTests.cs
```

## Стартиране

```bash
cd SchoolClubs.Web
dotnet restore
dotnet ef database update
dotnet run
```

## Тестови акаунти

| Email | Парола | Роля |
|-------|--------|------|
| admin@schoolclubs.bg | Admin123! | Admin |
| ivan.petrov@school.bg | Student123! | Student |
| maria.ivanova@school.bg | Student123! | Student |
