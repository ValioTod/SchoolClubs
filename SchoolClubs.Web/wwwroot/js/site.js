document.addEventListener('DOMContentLoaded', function () {

    // =============== Auto-dismiss alerts ===============
    var alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 5000);
    });

    // =============== Category filter auto-submit ===============
    var categorySelect = document.querySelector('select[name="category"]');
    if (categorySelect) {
        categorySelect.addEventListener('change', function () {
            this.closest('form').submit();
        });
    }

    // =============== Confirm on leave/delete ===============
    var joinForms = document.querySelectorAll('form[action*="Leave"]');
    joinForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            var msg = document.documentElement.lang === 'en'
                ? 'Are you sure you want to leave the club?'
                : 'Сигурни ли сте, че искате да напуснете клуба?';
            if (!confirm(msg)) e.preventDefault();
        });
    });

    var deleteForms = document.querySelectorAll('form[action*="Delete"]');
    deleteForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            var msg = document.documentElement.lang === 'en'
                ? 'Are you sure you want to delete?'
                : 'Сигурни ли сте, че искате да изтриете?';
            if (!confirm(msg)) e.preventDefault();
        });
    });

    // =============== Dark / Light theme ===============
    var themeSwitcher = document.getElementById('themeSwitcher');
    var html = document.documentElement;

    function applyTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem('sc-theme', theme);
        var icon = themeSwitcher ? themeSwitcher.querySelector('i') : null;
        if (icon) {
            icon.className = theme === 'dark'
                ? 'bi bi-sun-fill'
                : 'bi bi-moon-stars-fill';
        }
    }

    var savedTheme = localStorage.getItem('sc-theme') || 'light';
    applyTheme(savedTheme);

    if (themeSwitcher) {
        themeSwitcher.addEventListener('click', function () {
            var current = html.getAttribute('data-theme');
            applyTheme(current === 'dark' ? 'light' : 'dark');
        });
    }

    // =============== Language (BG / EN) ===============
    var translations = {
        // Navbar
        'nav.clubs': { bg: 'Клубове', en: 'Clubs' },
        'nav.events': { bg: 'Събития', en: 'Events' },
        'nav.leaderboard': { bg: 'Класация', en: 'Leaderboard' },
        'nav.profile': { bg: 'Профил', en: 'Profile' },
        'nav.logout': { bg: 'Изход', en: 'Logout' },
        'nav.login': { bg: 'Вход', en: 'Login' },
        'nav.register': { bg: 'Регистрация', en: 'Register' },
        'nav.brand': { bg: 'Училищни клубове', en: 'School Clubs' },
        // Footer
        'footer.copy': { bg: '© 2026 Училищни клубове. Дипломен проект.', en: '© 2026 School Clubs. Diploma project.' },
        // Landing
        'landing.title': { bg: 'Открий своя клуб', en: 'Discover Your Club' },
        'landing.subtitle': { bg: 'Разгледай училищните клубове, запиши се и участвай в събития', en: 'Browse school clubs, sign up and participate in events' },
        'landing.browse': { bg: 'Разгледай клубовете', en: 'Browse Clubs' },
        'landing.register': { bg: 'Регистрация', en: 'Register' },
        'landing.active': { bg: 'Активни клуба', en: 'Active Clubs' },
        'landing.students': { bg: 'Регистрирани ученици', en: 'Registered Students' },
        'landing.events': { bg: 'Събития', en: 'Events' },
        'landing.popular': { bg: 'Популярни клубове', en: 'Popular Clubs' },
        'landing.viewmore': { bg: 'Виж повече', en: 'View More' },
        // Dashboard
        'dash.clubs': { bg: 'Клуба', en: 'Clubs' },
        'dash.upcoming': { bg: 'Предстоящи събития', en: 'Upcoming Events' },
        'dash.upcomingevents': { bg: 'Предстоящи събития', en: 'Upcoming Events' },
        'dash.mypoints': { bg: 'Моите точки', en: 'My Points' },
        'dash.myrank': { bg: 'Моя ранг', en: 'My Rank' },
        'dash.myclubs': { bg: 'Моите клубове', en: 'My Clubs' },
        'dash.open': { bg: 'Отвори', en: 'Open' },
        'dash.noclubs': { bg: 'Все още не сте член на нито един клуб.', en: 'You are not a member of any club yet.' },
        'dash.browselink': { bg: 'Разгледайте наличните.', en: 'Browse available clubs.' },
        'dash.announcements': { bg: 'Последни обяви', en: 'Recent Announcements' },
        'dash.awards': { bg: 'Последни награди', en: 'Recent Awards' },
        'dash.recommended': { bg: 'Препоръчани за теб', en: 'Recommended for You' },
        'dash.top5': { bg: 'Топ 5 класация', en: 'Top 5 Leaderboard' },
        // Clubs
        'clubs.title': { bg: 'Клубове', en: 'Clubs' },
        'clubs.new': { bg: 'Нов клуб', en: 'New Club' },
        'clubs.search': { bg: 'Търси клуб...', en: 'Search club...' },
        'clubs.allcat': { bg: 'Всички категории', en: 'All Categories' },
        'clubs.allcats': { bg: 'Всички категории', en: 'All Categories' },
        'clubs.details': { bg: 'Детайли', en: 'Details' },
        'clubs.inactive': { bg: 'Неактивен', en: 'Inactive' },
        'clubs.noresult': { bg: 'Няма намерени клубове по зададените критерии.', en: 'No clubs found matching the criteria.' },
        'clubs.noresults': { bg: 'Няма намерени клубове по зададените критерии.', en: 'No clubs found matching the criteria.' },
        'clubs.edit': { bg: 'Редактирай', en: 'Edit' },
        'clubs.schedule': { bg: 'График:', en: 'Schedule:' },
        'clubs.location': { bg: 'Място:', en: 'Location:' },
        'clubs.leader': { bg: 'Ръководител:', en: 'Leader:' },
        'clubs.leave': { bg: 'Напусни клуба', en: 'Leave Club' },
        'clubs.join': { bg: 'Запиши се', en: 'Join' },
        'clubs.full': { bg: 'Клубът е пълен', en: 'Club is full' },
        'clubs.upcomingevents': { bg: 'Предстоящи събития', en: 'Upcoming Events' },
        'clubs.members': { bg: 'Членове', en: 'Members' },
        'clubs.president': { bg: 'Председател', en: 'President' },
        'clubs.moderator': { bg: 'Модератор', en: 'Moderator' },
        'clubs.photos': { bg: 'Снимки', en: 'Photos' },
        'clubs.viewall': { bg: 'Виж всички', en: 'View All' },
        'clubs.newevent': { bg: 'Ново събитие', en: 'New Event' },
        'clubs.newann': { bg: 'Нова обява', en: 'New Announcement' },
        'clubs.newannouncement': { bg: 'Нова обява', en: 'New Announcement' },
        'clubs.gallery': { bg: 'Галерия', en: 'Gallery' },
        'clubs.announcements': { bg: 'Последни обяви', en: 'Recent Announcements' },
        'clubs.recentann': { bg: 'Последни обяви', en: 'Recent Announcements' },
        // Club Create / Edit
        'clubs.createtitle': { bg: 'Създаване на нов клуб', en: 'Create New Club' },
        'clubs.edittitle': { bg: 'Редактиране на клуб', en: 'Edit Club' },
        'clubs.create': { bg: 'Създай', en: 'Create' },
        'clubs.createbtn': { bg: 'Създай', en: 'Create' },
        'clubs.save': { bg: 'Запази', en: 'Save' },
        'clubs.back': { bg: 'Назад', en: 'Back' },
        'clubs.cancel': { bg: 'Отказ', en: 'Cancel' },
        'clubs.status': { bg: 'Статус', en: 'Status' },
        'clubs.active': { bg: 'Активен', en: 'Active' },
        // Events
        'events.title': { bg: 'Събития', en: 'Events' },
        'events.upcoming': { bg: 'Предстоящи', en: 'Upcoming' },
        'events.past': { bg: 'Минали', en: 'Past' },
        'events.view': { bg: 'Виж', en: 'View' },
        'events.noevents': { bg: 'Няма налични събития.', en: 'No events available.' },
        'events.cancelled': { bg: 'Това събитие е отменено.', en: 'This event has been cancelled.' },
        'events.start': { bg: 'Начало:', en: 'Start:' },
        'events.end': { bg: 'Край:', en: 'End:' },
        'events.location': { bg: 'Място:', en: 'Location:' },
        'events.signedup': { bg: 'Записани:', en: 'Signed up:' },
        'events.signup': { bg: 'Запиши се', en: 'Sign Up' },
        'events.registered': { bg: 'Записан/а', en: 'Registered' },
        'events.cancelreg': { bg: 'Отказ', en: 'Cancel' },
        'events.attended': { bg: 'Присъствал/а', en: 'Attended' },
        'events.attendedshort': { bg: 'Присъствал', en: 'Attended' },
        'events.checkin': { bg: 'Отбележи присъствие с код', en: 'Check in with code' },
        'events.entercode': { bg: 'Въведи кода...', en: 'Enter code...' },
        'events.confirm': { bg: 'Потвърди', en: 'Confirm' },
        'events.attendees': { bg: 'Записани участници', en: 'Registered Attendees' },
        'events.noatt': { bg: 'Все още няма записани.', en: 'No registrations yet.' },
        'events.nosignups': { bg: 'Все още няма записани.', en: 'No registrations yet.' },
        'events.wasattended': { bg: 'Присъствал', en: 'Attended' },
        'events.createtitle': { bg: 'Създаване на събитие', en: 'Create Event' },
        'events.create': { bg: 'Създай', en: 'Create' },
        'events.createbtn': { bg: 'Създай', en: 'Create' },
        'events.back': { bg: 'Назад', en: 'Back' },
        // Announcements
        'ann.title': { bg: 'Обяви', en: 'Announcements' },
        'ann.new': { bg: 'Нова обява', en: 'New Announcement' },
        'ann.urgent': { bg: 'Спешно', en: 'Urgent' },
        'ann.important': { bg: 'Важно', en: 'Important' },
        'ann.from': { bg: 'от', en: 'by' },
        'ann.none': { bg: 'Няма обяви за този клуб.', en: 'No announcements for this club.' },
        'ann.createtitle': { bg: 'Публикуване на обява', en: 'Post Announcement' },
        'ann.publishtitle': { bg: 'Публикуване на обява', en: 'Post Announcement' },
        'ann.pin': { bg: 'Закачи в началото', en: 'Pin to top' },
        'ann.publish': { bg: 'Публикувай', en: 'Publish' },
        'ann.publishbtn': { bg: 'Публикувай', en: 'Publish' },
        'ann.back': { bg: 'Назад', en: 'Back' },
        // Gallery
        'gallery.title': { bg: 'Галерия', en: 'Gallery' },
        'gallery.photo': { bg: 'Снимка', en: 'Photo' },
        'gallery.desc': { bg: 'Описание', en: 'Caption' },
        'gallery.descinput': { bg: 'Кратко описание...', en: 'Short caption...' },
        'gallery.captionph': { bg: 'Кратко описание...', en: 'Short caption...' },
        'gallery.upload': { bg: 'Качи', en: 'Upload' },
        'gallery.empty': { bg: 'Все още няма снимки. Бъдете първият!', en: 'No photos yet. Be the first!' },
        'gallery.none': { bg: 'Все още няма снимки. Бъдете първият!', en: 'No photos yet. Be the first!' },
        // Profile
        'profile.class': { bg: 'клас', en: 'class' },
        'profile.points': { bg: 'Точки', en: 'Points' },
        'profile.rank': { bg: 'Ранг', en: 'Rank' },
        'profile.events': { bg: 'Събития', en: 'Events' },
        'profile.memberships': { bg: 'Членства в клубове', en: 'Club Memberships' },
        'profile.member': { bg: 'Член', en: 'Member' },
        'profile.nomember': { bg: 'Не е член на нито един клуб.', en: 'Not a member of any club.' },
        'profile.awards': { bg: 'Награди', en: 'Awards' },
        'profile.noawards': { bg: 'Все още няма спечелени награди.', en: 'No awards earned yet.' },
        // Leaderboard
        'lb.title': { bg: 'Класация', en: 'Leaderboard' },
        'lb.student': { bg: 'Ученик', en: 'Student' },
        'lb.awards': { bg: 'Награди', en: 'Awards' },
        'lb.points': { bg: 'Точки', en: 'Points' },
        'lb.noranked': { bg: 'Все още няма класирани ученици.', en: 'No ranked students yet.' },
        // Error
        'error.title': { bg: 'Нещо се обърка', en: 'Something went wrong' },
        'error.msg': { bg: 'Възникна грешка при обработката на заявката.', en: 'An error occurred while processing your request.' },
        'error.message': { bg: 'Възникна неочаквана грешка. Моля, опитайте отново.', en: 'An unexpected error occurred. Please try again.' },
        'error.home': { bg: 'Към началната страница', en: 'Go to Homepage' },
        // Feedback
        'fb.title': { bg: 'Моите отзиви', en: 'My Feedback' },
        'fb.new': { bg: 'Нов отзив', en: 'New Feedback' },
        'fb.createtitle': { bg: 'Изпращане на отзив', en: 'Submit Feedback' },
        'fb.submit': { bg: 'Изпрати', en: 'Submit' },
        'fb.none': { bg: 'Все още няма отзиви.', en: 'No feedback submitted yet.' },
        // Nav extras
        'nav.feedback': { bg: 'Обратна връзка', en: 'Feedback' },
        'nav.admin': { bg: 'Админ', en: 'Admin' },
        // Admin
        'admin.title': { bg: 'Админ панел', en: 'Admin Panel' },
        // Profile edit
        'profile.edit': { bg: 'Редактирай профила', en: 'Edit Profile' },
        'profile.edittitle': { bg: 'Редактиране на профил', en: 'Edit Profile' },
        'profile.picture': { bg: 'Профилна снимка', en: 'Profile Picture' },
        // Club stats
        'clubs.stats': { bg: 'Статистика', en: 'Statistics' },
        // Events extras
        'events.cancelevent': { bg: 'Отмени събитие', en: 'Cancel Event' },
        'events.restoreevent': { bg: 'Възстанови събитие', en: 'Restore Event' }
    };

    var langSwitcher = document.getElementById('langSwitcher');
    var langLabel = document.getElementById('langLabel');

    function applyLang(lang) {
        localStorage.setItem('sc-lang', lang);
        html.setAttribute('lang', lang === 'en' ? 'en' : 'bg');
        if (langLabel) langLabel.textContent = lang === 'en' ? 'BG' : 'EN';

        document.querySelectorAll('[data-i18n]').forEach(function (el) {
            var key = el.getAttribute('data-i18n');
            if (translations[key] && translations[key][lang]) {
                el.textContent = translations[key][lang];
            }
        });

        document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-placeholder');
            if (translations[key] && translations[key][lang]) {
                el.setAttribute('placeholder', translations[key][lang]);
            }
        });

        document.querySelectorAll('[data-i18n-title]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-title');
            if (translations[key] && translations[key][lang]) {
                el.setAttribute('title', translations[key][lang]);
            }
        });
    }

    var savedLang = localStorage.getItem('sc-lang') || 'bg';
    applyLang(savedLang);

    if (langSwitcher) {
        langSwitcher.addEventListener('click', function () {
            var current = localStorage.getItem('sc-lang') || 'bg';
            applyLang(current === 'bg' ? 'en' : 'bg');
        });
    }
});
