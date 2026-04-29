document.addEventListener('DOMContentLoaded', function () {
    var html = document.documentElement;

    // --- Page loader ---
    var pageLoader = document.getElementById('pageLoader');
    if (pageLoader) {
        setTimeout(function () { pageLoader.classList.add('hidden'); }, 300);
        setTimeout(function () { if (pageLoader.parentNode) pageLoader.parentNode.removeChild(pageLoader); }, 700);
    }

    // --- Scroll Reveal (IntersectionObserver) ---
    var revealElements = document.querySelectorAll('.reveal, .reveal-left, .reveal-right, .reveal-scale');
    if ('IntersectionObserver' in window && revealElements.length > 0) {
        var revealObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    revealObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });
        revealElements.forEach(function (el) { revealObserver.observe(el); });
    } else {
        // Fallback: just show everything
        revealElements.forEach(function (el) { el.classList.add('visible'); });
    }

    // --- Animated Counters ---
    var counters = document.querySelectorAll('[data-counter]');
    if ('IntersectionObserver' in window && counters.length > 0) {
        var counterObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    animateCounter(entry.target);
                    counterObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.3 });
        counters.forEach(function (el) { counterObserver.observe(el); });
    }

    function animateCounter(el) {
        var target = parseInt(el.getAttribute('data-counter'), 10);
        var prefix = el.getAttribute('data-counter-prefix') || '';
        var suffix = el.getAttribute('data-counter-suffix') || '';
        if (isNaN(target)) { return; }
        var duration = 1500;
        var start = 0;
        var startTime = null;

        function easeOutCubic(t) { return 1 - Math.pow(1 - t, 3); }

        function step(timestamp) {
            if (!startTime) startTime = timestamp;
            var progress = Math.min((timestamp - startTime) / duration, 1);
            var current = Math.floor(easeOutCubic(progress) * target);
            el.textContent = prefix + current + suffix;
            if (progress < 1) {
                requestAnimationFrame(step);
            } else {
                el.textContent = prefix + target + suffix;
            }
        }
        requestAnimationFrame(step);
    }

    // --- Navbar scroll effect ---
    var navbar = document.querySelector('.navbar');
    if (navbar) {
        var lastScroll = 0;
        window.addEventListener('scroll', function () {
            var scrollY = window.scrollY || window.pageYOffset;
            if (scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
            lastScroll = scrollY;
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

    // --- Toast Notifications ---
    // Convert standard alerts to toast notifications
    var alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        // Get message text
        var msg = alert.textContent.trim();
        var isSuccess = alert.classList.contains('alert-success');
        var isDanger = alert.classList.contains('alert-danger');
        var isInfo = alert.classList.contains('alert-info');

        // Remove the original alert
        alert.style.display = 'none';

        // Create toast
        showToast(msg, isSuccess ? 'success' : isDanger ? 'error' : 'info');
    });

    function showToast(message, type) {
        var container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        var toast = document.createElement('div');
        toast.className = 'toast toast-' + (type || 'info');

        var icons = { success: 'bi-check-circle-fill', error: 'bi-exclamation-triangle-fill', info: 'bi-info-circle-fill' };
        var colors = { success: '#10b981', error: '#ef4444', info: '#06b6d4' };

        toast.innerHTML =
            '<i class="bi ' + (icons[type] || icons.info) + '" style="color:' + (colors[type] || colors.info) + ';font-size:1.25rem"></i>' +
            '<span style="flex:1;font-weight:500">' + escapeHtml(message) + '</span>' +
            '<button onclick="this.parentElement.remove()" style="background:none;border:none;color:var(--text-muted);cursor:pointer;padding:0 0 0 0.5rem;font-size:1.1rem"><i class="bi bi-x"></i></button>';

        container.appendChild(toast);

        // Auto dismiss
        setTimeout(function () {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(100%)';
            toast.style.transition = 'all 0.3s ease-out';
            setTimeout(function () { if (toast.parentNode) toast.parentNode.removeChild(toast); }, 300);
        }, 5000);
    }

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    // --- Ripple effect on buttons ---
    document.querySelectorAll('.btn-primary, .btn-success, .btn-outline-primary').forEach(function (btn) {
        btn.classList.add('ripple');
        btn.addEventListener('click', function (e) {
            var circle = document.createElement('span');
            circle.className = 'ripple-circle';
            var rect = this.getBoundingClientRect();
            circle.style.left = (e.clientX - rect.left) + 'px';
            circle.style.top = (e.clientY - rect.top) + 'px';
            this.appendChild(circle);
            setTimeout(function () { if (circle.parentNode) circle.parentNode.removeChild(circle); }, 600);
        });
    });

    // --- Better confirm dialogs ---
    function showConfirmModal(message, onConfirm) {
        // Create backdrop
        var backdrop = document.createElement('div');
        backdrop.className = 'confirm-modal-backdrop';

        // Create modal
        var modal = document.createElement('div');
        modal.className = 'confirm-modal';
        modal.innerHTML =
            '<div style="text-align:center;margin-bottom:1.5rem">' +
            '<i class="bi bi-exclamation-triangle" style="font-size:3rem;color:var(--warning)"></i>' +
            '</div>' +
            '<p style="text-align:center;font-weight:600;font-size:1.05rem;margin-bottom:1.5rem">' + escapeHtml(message) + '</p>' +
            '<div style="display:flex;gap:0.75rem;justify-content:center">' +
            '<button class="btn btn-secondary confirm-no" style="min-width:100px">' +
            (html.lang === 'en' ? 'Cancel' : 'Отказ') + '</button>' +
            '<button class="btn btn-danger confirm-yes" style="min-width:100px">' +
            (html.lang === 'en' ? 'Confirm' : 'Потвърди') + '</button>' +
            '</div>';

        document.body.appendChild(backdrop);
        document.body.appendChild(modal);

        // Animate in
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

    // Confirm on leave/delete - replace browser alerts
    var leaveForms = document.querySelectorAll('form[action*="Leave"]');
    leaveForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var msg = html.lang === 'en'
                ? 'Are you sure you want to leave the club?'
                : 'Сигурни ли сте, че искате да напуснете клуба?';
            var f = form;
            showConfirmModal(msg, function () { f.submit(); });
        });
    });

    var deleteForms = document.querySelectorAll('form[action*="Delete"]');
    deleteForms.forEach(function (form) {
        // Remove any inline onsubmit handler to avoid double confirms
        form.removeAttribute('onsubmit');
        var submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) submitBtn.removeAttribute('onclick');

        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var msg = html.lang === 'en'
                ? 'Are you sure you want to delete?'
                : 'Сигурни ли сте, че искате да изтриете?';
            var f = form;
            showConfirmModal(msg, function () { f.submit(); });
        });
    });

    // --- Category filter auto-submit ---
    var categorySelect = document.querySelector('select[name="category"]');
    if (categorySelect) {
        categorySelect.addEventListener('change', function () {
            this.closest('form').submit();
        });
    }

    // --- Dark / Light theme ---
    var themeSwitcher = document.getElementById('themeSwitcher');

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

    // --- Language (BG / EN) ---
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
        'events.restoreevent': { bg: 'Възстанови събитие', en: 'Restore Event' },
        // Footer
        'footer.desc': { bg: 'Платформа за управление на училищни клубове, събития и постижения. Дипломен проект.', en: 'Platform for managing school clubs, events and achievements. Diploma project.' },
        'footer.links': { bg: 'Навигация', en: 'Navigation' },
        'footer.home': { bg: 'Начало', en: 'Home' },
        'footer.tech': { bg: 'Технологии', en: 'Technologies' },
        'footer.contact': { bg: 'Контакти', en: 'Contact' },
        'footer.email': { bg: 'Email: support@schoolclubs.edu', en: 'Email: support@schoolclubs.edu' },
        'footer.phone': { bg: 'Телефон: +359 (0) 2 XXX XXXX', en: 'Phone: +359 (0) 2 XXX XXXX' },
        'footer.location': { bg: 'Място: България', en: 'Location: Bulgaria' },
        // Landing extras
        'landing.popularsub': { bg: 'Най-активните клубове в училището', en: 'The most active clubs in the school' },
        // Auth pages
        'auth.login': { bg: 'Вход в профил', en: 'Sign In' },
        'auth.loginDesc': { bg: 'Влезте в своя профил на SchoolClubs', en: 'Sign in to your SchoolClubs account' },
        'auth.register': { bg: 'Регистрация', en: 'Register' },
        'auth.registerDesc': { bg: 'Присъедини се към SchoolClubs комюнитета', en: 'Join the SchoolClubs community' },
        'auth.loginBtn': { bg: 'Вход', en: 'Sign In' },
        'auth.registerBtn': { bg: 'Регистрирай се', en: 'Register' },
        'auth.rememberMe': { bg: 'Запомни ме', en: 'Remember Me' },
        'auth.noAccount': { bg: 'Нямаш акаунт?', en: 'Don\'t have an account?' },
        'auth.registerLink': { bg: 'Регистрирай се', en: 'Register' },
        'auth.haveAccount': { bg: 'Вече имаш акаунт?', en: 'Already have an account?' },
        'auth.loginLink': { bg: 'Влез', en: 'Sign In' },
        'auth.guestInfo': { bg: 'Можеш да разглеждаш клубовете без регистрация.', en: 'You can browse clubs without registration.' },
        'auth.browseAsGuest': { bg: 'Разглеждай като гост', en: 'Browse as Guest' },
        'auth.termsAgree': { bg: 'Регистрирайки се, приемаш нашите условия за ползване.', en: 'By registering, you agree to our terms of use.' },
        // Form
        'form.email': { bg: 'Email', en: 'Email' },
        'form.password': { bg: 'Парола', en: 'Password' },
        'form.confirmPassword': { bg: 'Потвърди парола', en: 'Confirm Password' },
        'form.fullName': { bg: 'Пълно име', en: 'Full Name' },
        'form.passwordHint': { bg: 'Поне 6 символа', en: 'At least 6 characters' },
        // Club approval
        'clubs.pendingRequests': { bg: 'Очаквани заявки', en: 'Pending Requests' },
        'clubs.approve': { bg: 'Одобри', en: 'Approve' },
        'clubs.reject': { bg: 'Отхвърли', en: 'Reject' },
        'clubs.rejectRequest': { bg: 'Отхвърли заявка', en: 'Reject Request' },
        'clubs.rejectReason': { bg: 'Причина (опционална):', en: 'Reason (optional):' },
        'clubs.rejectBtn': { bg: 'Отхвърли', en: 'Reject' },
        // Gallery
        'gallery.title': { bg: 'Галерия', en: 'Gallery' },
        'gallery.photo': { bg: 'Снимка', en: 'Photo' },
        'gallery.desc': { bg: 'Описание', en: 'Description' },
        'gallery.captionph': { bg: 'Кратко описание...', en: 'Brief description...' },
        'gallery.upload': { bg: 'Качи', en: 'Upload' },
        'gallery.none': { bg: 'Все още няма снимки. Бъдете първият!', en: 'No photos yet. Be the first!' }
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
