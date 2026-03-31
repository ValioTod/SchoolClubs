# SchoolClubs

Уеб приложение за управление на училищни клубове, изградено с ASP.NET Core 8.0 MVC.

## Технологии

- **ASP.NET Core 8.0 MVC**
- **Entity Framework Core 8.0** с SQL Server (LocalDB)
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
- Отмяна и възстановяване на събития

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

### Администраторски панел
- Общи статистики (потребители, клубове, събития, обяви, обратна връзка)
- Управление на клубове (активиране/деактивиране, изтриване)
- Управление на потребителски роли
- Преглед и разрешаване на обратна връзка

### Профил
- Преглед на профил с клубове, награди и точки
- Редактиране на профил (име, клас, биография, снимка)
- Експорт на членове като CSV файл
- Статистики на клуб (членове, събития, посещаемост)

### Обратна връзка (Feedback)
- Подаване на обратна връзка с категория (Общо, Бъг, Нова функция, Клуб, Събитие)
- Преглед на лична история с обратна връзка
- Администраторски отговор и разрешаване

### Двуезичен интерфейс
- Български / Английски с бутон за превключване
- 100+ преведени елемента
- Запазване на избора в localStorage

### Тъмна / Светла тема
- Превключване с бутон в навигацията
- Запазване на избора в localStorage

## Роли

| Роля | Права |
|------|-------|
| Student | Разглеждане, записване, участие в събития, обратна връзка |
| Teacher | Създаване и управление на клубове |
| Admin | Пълен достъп, админ панел, управление на роли и обратна връзка |

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
│   │   ├── ProfileController.cs
│   │   ├── AdminController.cs
│   │   └── FeedbackController.cs
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
│   │   ├── Feedback.cs
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
│   │   ├── Admin/
│   │   ├── Feedback/
│   │   └── Shared/
│   └── wwwroot/
│       ├── css/site.css
│       └── js/site.js
└── SchoolClubs.Tests/
    ├── AchievementServiceTests.cs
    ├── ClubRecommendationServiceTests.cs
    └── ClubsControllerTests.cs
```

## Стартиране

### Изисквания
- .NET 8.0 SDK
- SQL Server LocalDB (включен с Visual Studio)

### Инсталация
```bash
git clone https://github.com/ValioTod/SchoolClubs.git
cd SchoolClubs
git checkout working-version
dotnet run --project SchoolClubs.Web
```

Приложението е достъпно на `http://localhost:5000`. Базата данни се създава и засява автоматично при първо стартиране — не са нужни миграции.

## Тестови акаунти

| Роля | Email | Парола |
|------|-------|--------|
| Admin | admin@schoolclubs.bg | Admin123! |
| Student | ivan.petrov@school.bg | Student123! |
| Student | maria.ivanova@school.bg | Student123! |
| Student | georgi.dimitrov@school.bg | Student123! |
| Student | elena.stoyanova@school.bg | Student123! |
| Student | nikolay.kolev@school.bg | Student123! |

### Тестове
```bash
dotnet test
```
14 теста (5 Achievement + 5 Recommendation + 4 Controller).

## Тестови акаунти

| Имейл | Парола | Роля |
|-------|--------|------|
| admin@schoolclubs.bg | Admin123! | Admin |
| ivan.petrov@school.bg | Student123! | Student |
| maria.ivanova@school.bg | Student123! | Student |
| georgi.dimitrov@school.bg | Student123! | Student |
| elena.stoyanova@school.bg | Student123! | Student |
| nikolay.kolev@school.bg | Student123! | Student |

## Преносимост

Проектът е напълно преносим между различни компютри:
1. Копирайте или клонирайте проекта
2. Уверете се, че .NET 8.0 SDK и SQL Server LocalDB са инсталирани
3. Стартирайте с `dotnet run --project SchoolClubs.Web`
4. Базата данни се създава и засява автоматично
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
