var TRANSLATIONS = {
    bg: {
        'nav.brand': 'Училищни клубове', 'nav.clubs': 'Клубове', 'nav.events': 'Събития',
        'nav.leaderboard': 'Класация', 'nav.feedback': 'Обратна връзка', 'nav.admin': 'Админ',
        'nav.profile': 'Профил', 'nav.logout': 'Изход', 'nav.login': 'Вход', 'nav.register': 'Регистрация',
        'footer.desc': 'Платформа за управление на училищни клубове и събития.',
        'footer.nav': 'Навигация', 'footer.home': 'Начало', 'footer.contact': 'Контакти',
        'footer.copy': '© ' + new Date().getFullYear() + ' Училищни клубове — Дипломен проект',
        'clubs.title': 'Клубове', 'clubs.new': 'Нов клуб', 'clubs.allCats': 'Всички категории',
        'clubs.details': 'Детайли', 'clubs.inactive': 'Неактивен',
        'clubs.notFound': 'Няма намерени клубове по зададените критерии.',
        'clubs.search': 'Търси клуб...',
        'events.title': 'Събития', 'events.upcoming': 'Предстоящи', 'events.past': 'Минали',
        'events.view': 'Виж', 'events.none': 'Няма налични събития.',
        'dash.clubs': 'Клуба', 'dash.upcomingEvents': 'Предстоящи събития',
        'dash.myPoints': 'Моите точки', 'dash.myRank': 'Моя ранг',
        'dash.myClubs': 'Моите клубове', 'dash.open': 'Отвори',
        'dash.upcoming': 'Предстоящи събития', 'dash.announcements': 'Последни обяви',
        'dash.recentAwards': 'Последни награди', 'dash.recommended': 'Препоръчани за теб',
        'dash.top5': 'Топ 5 класация',
        'lb.title': 'Класация', 'lb.student': 'Ученик', 'lb.awards': 'Награди',
        'lb.points': 'Точки', 'lb.empty': 'Все още няма класирани ученици.',
        'profile.memberships': 'Членства в клубове', 'profile.awards': 'Награди',
        'profile.points': 'Точки', 'profile.rank': 'Ранг', 'profile.events': 'Събития',
        'profile.edit': 'Редактирай профила', 'profile.noClubs': 'Не е член на нито един клуб.',
        'profile.noAwards': 'Все още няма спечелени награди.',
        'feedback.title': 'Обратна връзка', 'feedback.new': 'Нов отзив',
        'feedback.noItems': 'Все още нямате подадени отзиви.',
        'landing.hero': 'Открий своя клуб',
        'landing.sub': 'Разгледай наличните клубове в училището и се запиши за участие',
        'landing.guestNote': 'Разглеждате сайта като гост. Влезте в профил или се регистрирайте, за да се запишете в клуб, да участвате в събития и да виждате своите постижения.',
        'landing.activeClubs': 'Активни клуба', 'landing.students': 'Регистрирани ученици',
        'landing.upcomingEvents': 'Предстоящи събития', 'landing.available': 'Налични клубове',
        'landing.loginToJoin': 'За да се запишеш, трябва да влезеш в профила си.',
        'landing.loginBtn': 'Влез, за да се запишеш'
    },
    en: {
        'nav.brand': 'School Clubs', 'nav.clubs': 'Clubs', 'nav.events': 'Events',
        'nav.leaderboard': 'Leaderboard', 'nav.feedback': 'Feedback', 'nav.admin': 'Admin',
        'nav.profile': 'Profile', 'nav.logout': 'Log out', 'nav.login': 'Log in', 'nav.register': 'Register',
        'footer.desc': 'A platform for managing school clubs and events.',
        'footer.nav': 'Navigation', 'footer.home': 'Home', 'footer.contact': 'Contact',
        'footer.copy': '© ' + new Date().getFullYear() + ' School Clubs — Diploma Project',
        'clubs.title': 'Clubs', 'clubs.new': 'New Club', 'clubs.allCats': 'All categories',
        'clubs.details': 'Details', 'clubs.inactive': 'Inactive',
        'clubs.notFound': 'No clubs found for the given criteria.',
        'clubs.search': 'Search club...',
        'events.title': 'Events', 'events.upcoming': 'Upcoming', 'events.past': 'Past',
        'events.view': 'View', 'events.none': 'No events available.',
        'dash.clubs': 'Clubs', 'dash.upcomingEvents': 'Upcoming events',
        'dash.myPoints': 'My points', 'dash.myRank': 'My rank',
        'dash.myClubs': 'My clubs', 'dash.open': 'Open',
        'dash.upcoming': 'Upcoming events', 'dash.announcements': 'Latest announcements',
        'dash.recentAwards': 'Recent awards', 'dash.recommended': 'Recommended for you',
        'dash.top5': 'Top 5 leaderboard',
        'lb.title': 'Leaderboard', 'lb.student': 'Student', 'lb.awards': 'Awards',
        'lb.points': 'Points', 'lb.empty': 'No ranked students yet.',
        'profile.memberships': 'Club memberships', 'profile.awards': 'Awards',
        'profile.points': 'Points', 'profile.rank': 'Rank', 'profile.events': 'Events',
        'profile.edit': 'Edit profile', 'profile.noClubs': 'Not a member of any club.',
        'profile.noAwards': 'No awards earned yet.',
        'feedback.title': 'Feedback', 'feedback.new': 'New feedback',
        'feedback.noItems': 'You have not submitted any feedback yet.',
        'landing.hero': 'Discover your club',
        'landing.sub': 'Browse the clubs available at school and sign up to participate',
        'landing.guestNote': 'You are browsing as a guest. Log in or register to join clubs, attend events and track your achievements.',
        'landing.activeClubs': 'Active clubs', 'landing.students': 'Registered students',
        'landing.upcomingEvents': 'Upcoming events', 'landing.available': 'Available clubs',
        'landing.loginToJoin': 'You need to log in to join a club.',
        'landing.loginBtn': 'Log in to join'
    }
};

function applyLang(lang) {
    var t = TRANSLATIONS[lang] || TRANSLATIONS['bg'];
    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        var key = el.getAttribute('data-i18n');
        if (t[key] !== undefined) el.textContent = t[key];
    });
    document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
        var key = el.getAttribute('data-i18n-placeholder');
        if (t[key] !== undefined) el.placeholder = t[key];
    });
    // Dynamic content (club names, descriptions stored server-side in both languages)
    document.querySelectorAll('[data-lang-en]').forEach(function (el) {
        var en = el.getAttribute('data-lang-en');
        var bg = el.getAttribute('data-lang-bg');
        if (lang === 'en' && en) el.textContent = en;
        else if (bg) el.textContent = bg;
    });
    var label = document.getElementById('langLabel');
    if (label) label.textContent = lang === 'en' ? 'BG' : 'EN';
    localStorage.setItem('sc-lang', lang);
    document.documentElement.lang = lang === 'en' ? 'en' : 'bg';
}

document.addEventListener('DOMContentLoaded', function () {
    var html = document.documentElement;

    // --- Navbar scroll effect ---
    var navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            if ((window.scrollY || window.pageYOffset) > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        }, { passive: true });
    }

    // --- Back to Top Button ---
    var backToTopBtn = document.getElementById('backToTop');
    if (backToTopBtn) {
        window.addEventListener('scroll', function () {
            if ((window.scrollY || window.pageYOffset) > 400) {
                backToTopBtn.classList.add('show');
            } else {
                backToTopBtn.classList.remove('show');
            }
        }, { passive: true });
        backToTopBtn.addEventListener('click', function () {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // --- Dark / Light theme ---
    var themeSwitcher = document.getElementById('themeSwitcher');

    function applyTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem('sc-theme', theme);
        var icon = themeSwitcher ? themeSwitcher.querySelector('i') : null;
        if (icon) {
            icon.className = theme === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
        }
    }

    applyTheme(localStorage.getItem('sc-theme') || 'light');

    if (themeSwitcher) {
        themeSwitcher.addEventListener('click', function () {
            applyTheme(html.getAttribute('data-theme') === 'dark' ? 'light' : 'dark');
        });
    }

    // --- Confirm dialogs for leave/delete ---
    function escapeHtml(text) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    function showConfirm(message, onConfirm) {
        var backdrop = document.createElement('div');
        backdrop.className = 'confirm-modal-backdrop';

        var modal = document.createElement('div');
        modal.className = 'confirm-modal';
        modal.innerHTML =
            '<div style="text-align:center;margin-bottom:1.5rem">' +
            '<i class="bi bi-exclamation-triangle" style="font-size:3rem;color:var(--warning)"></i>' +
            '</div>' +
            '<p style="text-align:center;font-weight:600;font-size:1.05rem;margin-bottom:1.5rem">' + escapeHtml(message) + '</p>' +
            '<div style="display:flex;gap:0.75rem;justify-content:center">' +
            '<button class="btn btn-secondary confirm-no" style="min-width:100px">Отказ</button>' +
            '<button class="btn btn-danger confirm-yes" style="min-width:100px">Потвърди</button>' +
            '</div>';

        document.body.appendChild(backdrop);
        document.body.appendChild(modal);

        requestAnimationFrame(function () {
            backdrop.classList.add('show');
            modal.classList.add('show');
        });

        function close() {
            backdrop.classList.remove('show');
            modal.classList.remove('show');
            setTimeout(function () {
                if (backdrop.parentNode) backdrop.parentNode.removeChild(backdrop);
                if (modal.parentNode) modal.parentNode.removeChild(modal);
            }, 300);
        }

        modal.querySelector('.confirm-no').addEventListener('click', close);
        backdrop.addEventListener('click', close);
        modal.querySelector('.confirm-yes').addEventListener('click', function () {
            close();
            onConfirm();
        });
    }

    document.querySelectorAll('form[action*="Leave"]').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var f = form;
            showConfirm('Сигурни ли сте, че искате да напуснете клуба?', function () { f.submit(); });
        });
    });

    document.querySelectorAll('form[action*="Delete"]').forEach(function (form) {
        form.removeAttribute('onsubmit');
        var btn = form.querySelector('button[type="submit"]');
        if (btn) btn.removeAttribute('onclick');
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var f = form;
            showConfirm('Сигурни ли сте, че искате да изтриете?', function () { f.submit(); });
        });
    });

    // --- Language switcher ---
    var langSwitcher = document.getElementById('langSwitcher');
    var savedLang = localStorage.getItem('sc-lang') || 'bg';
    applyLang(savedLang);

    if (langSwitcher) {
        langSwitcher.addEventListener('click', function () {
            var current = localStorage.getItem('sc-lang') || 'bg';
            applyLang(current === 'bg' ? 'en' : 'bg');
        });
    }

    // --- Category filter auto-submit ---
    var categorySelect = document.querySelector('select[name="category"]');
    if (categorySelect) {
        categorySelect.addEventListener('change', function () {
            this.closest('form').submit();
        });
    }
});
