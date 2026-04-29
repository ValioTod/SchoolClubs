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
- Обща табло със статистики (потребители, клубове, събития, обяви, обратна връзка)
- Управление на клубове (активиране/деактивиране, изтриване) - отделна страница
- Управление на потребителски роли - отделна страница
- Управление и разрешаване на обратна връзка - отделна страница
- Преглед на категории на обратна връзка

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

## Развръщане (Deployment)

Приложението е готово за развръщане в производство. Съществуват няколко опции:

### Docker (препоръчано)
```bash
docker-compose up -d
```
Вижте подробното [DEPLOYMENT.md](./DEPLOYMENT.md) за конфигурация и опции.

### IIS (Windows Server)
Вижте [DEPLOYMENT.md](./DEPLOYMENT.md) за пълни инструкции.

### Linux (Nginx)
Вижте [DEPLOYMENT.md](./DEPLOYMENT.md) за пълна конфигурация със systemd услуга.

## Нови функции в този выпуск

✅ Одобрение на членство на клуб (Membership Approval)
✅ Валидация на дати на события (Prevent Past Events)
✅ Управление на роли на администратор със свежаване на сесия
✅ Отделни страници за управление (Users, Clubs, Feedbacks)
✅ Категории в обратна връзка (Feedback Categories)
✅ Docker и Docker Compose поддръжка
✅ GitHub Actions CI/CD pipeline
✅ Документация за развръщане

## Тестови акаунти

| Роля | Email | Парола |
|------|-------|--------|
| Admin | admin@schoolclubs.bg | Admin123! |
| Student | ivan.petrov@school.bg | Student123! |
| Student | maria.ivanova@school.bg | Student123! |
| Student | georgi.dimitrov@school.bg | Student123! |
| Student | elena.stoyanova@school.bg | Student123! |
| Student | nikolay.kolev@school.bg | Student123! |

## Тестове
```bash
dotnet test
```
43 теста (5 Achievement + 5 Recommendation + 4 Controller + 29 Integration) - всички преминават успешно ✅

## Тестови акаунти

| Имейл | Парола | Роля |
|-------|--------|------|
| admin@schoolclubs.bg | Admin123! | Admin |
| teacher@schoolclubs.bg | Teacher123! | Teacher |
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

## Документация

- **🚀 [AZURE_QUICKSTART.md](./AZURE_QUICKSTART.md)** - Deploy в 5 минути (препоръчано!)
- **[AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)** - Детално ръководство за Azure App Service
- **[AZURE_DEPLOYMENT_CHECKLIST.md](./AZURE_DEPLOYMENT_CHECKLIST.md)** - Контролен списък за развръщане
- [DEPLOYMENT.md](./DEPLOYMENT.md) - Пълно ръководство за всички опции (Docker, IIS, Linux)
- [IMPLEMENTATION_SUMMARY.md](./IMPLEMENTATION_SUMMARY.md) - Резюме на всички решени проблеми
- [Leaderboard_Criteria_BG.md](./Leaderboard_Criteria_BG.md) - Система за класация и точки

## Развръщане (Deployment)

**Препоръчано**: Развържете в **Azure** за 5 минути! 

```powershell
# 1. Автоматична настройка на Azure ресурси
.\scripts\azure-setup.ps1

# 2. Публикуване на приложението
dotnet publish SchoolClubs.Web -c Release -o ./publish

# 3. Развръщане в Azure
az webapp up --resource-group schoolclubs-rg --name schoolclubs-app --runtime DOTNETCORE:8.0

# 4. Отворете в браузър
# https://schoolclubs-app.azurewebsites.net
```

👉 **[Пълна инструкция →](./AZURE_QUICKSTART.md)**

Други опции: Docker, IIS, Linux - вижте [DEPLOYMENT.md](./DEPLOYMENT.md)
