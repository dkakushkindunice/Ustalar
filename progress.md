# Progress Log

Лог прогресса агентов по задачам проекта Ustalar.az.

## Формат записи

```
### [ДАТА] TASK-XXX — [Название задачи]
**Агент:** [имя/модель]
**Статус:** done | blocked | in_progress
**Summary:** Что было сделано, какие файлы изменены, какие команды выполнены.
**Проблемы:** Что пошло не так (если были).
```

---

<!-- Агенты добавляют записи ниже этой строки -->

### [2026-05-07] TASK-013 — Cloudflare R2 + ImageSharp (file storage)
**Статус:** done
**Summary:** IFileStorageService.cs + R2FileStorageService.cs + ImageProcessingService.cs созданы. AmazonS3Client зарегистрирован в DI с BasicAWSCredentials + ServiceURL (ForcePathStyle=true для R2). IFileStorageService и ImageProcessingService зарегистрированы как Singleton. R2 секция добавлена в appsettings.json. SixLabors.ImageSharp обновлён до 3.1.12 (CVE закрыты). dotnet build: 0 ошибок, 0 предупреждений.
**Проблемы:** 3.1.6 имел high+medium CVE → обновлён до 3.1.12, где оба закрыты.

### [2026-05-07] TASK-020 — Загрузка фото в портфолио
**Статус:** done
**Summary:** Pages/Cabinet/Photos/Upload.cshtml (@page "/cabinet/photos/upload") — форма загрузки с [Authorize(MasterCookie)]. OnPostAsync: ImageProcessingService.ProcessUploadAsync → upload original в "photos" + thumb в "thumbs" → PortfolioPhoto в БД (IsApproved=false). Лимит 10 фото на мастера.
**Проблемы:** Нет.

### [2026-05-07] TASK-021 — Удаление фото из портфолио
**Статус:** done
**Summary:** OnPostDeletePhotoAsync добавлен в Cabinet/Index.cshtml.cs — проверяет принадлежность фото мастеру, удаляет из R2 (original + thumb), удаляет из БД. Кнопка "✕" (hover) добавлена в Index.cshtml с confirm диалогом.
**Проблемы:** Нет.

### [2026-05-07] TASK-031 — Загрузка аватара
**Статус:** done
**Summary:** Cabinet/Edit.cshtml дополнен отдельной формой с enctype=multipart/form-data для аватара. OnPostUploadAvatarAsync: ProcessAvatarAsync (300x300 WebP) → DeleteAsync старого → UploadAsync в "avatars" → Master.AvatarUrl обновлён. Edit.cshtml.cs получил IFileStorageService + ImageProcessingService в DI.
**Проблемы:** Нет.

### [2026-05-07] TASK-026 — SEO: sitemap.xml + robots.txt
**Статус:** done
**Summary:** Pages/Sitemap.cshtml (@page "/sitemap.xml") — генерирует XML sitemap со всеми городами, городами+специализациями, активными мастерами. wwwroot/robots.txt: Disallow /admin/ /cabinet/ /register/, Sitemap: https://ustalar.az/sitemap.xml.
**Проблемы:** Нет.

### [2026-05-07] TASK-030 — BackgroundService: очистка SMS кодов
**Статус:** done
**Summary:** Services/SmsCleanupService.cs : BackgroundService — каждые 30 мин ExecuteDeleteAsync для записей где ExpiresAt < UtcNow. Использует IServiceScopeFactory для получения DbContext в hosted service. Зарегистрирован через AddHostedService<SmsCleanupService>().
**Проблемы:** Нет.

### [2026-05-07] TASK-032 — IMemoryCache для городов и специализаций
**Статус:** done
**Summary:** MastersCatalogService обновлён: _cache.GetOrCreateAsync("catalog:cities") и ("catalog:specs") с AbsoluteExpiration 10 мин. AddMemoryCache() добавлен в Program.cs. DI конструктор обновлён.
**Проблемы:** Нет.

### [2026-05-07] TASK-033 — Response compression gzip/brotli
**Статус:** done
**Summary:** AddResponseCompression с BrotliCompressionProvider + GzipCompressionProvider (уровень Fastest), EnableForHttps=true. MimeTypes расширены text/html, application/json, image/svg+xml, application/xml. app.UseResponseCompression() перед UseStaticFiles.
**Проблемы:** Нет.

### [2026-05-07] TASK-006 — GitLab CI/CD
**Статус:** done
**Summary:** .gitlab-ci.yml создан: stages build/test/deploy. build: docker:27-dind, docker build + push к CI_REGISTRY. test: dotnet restore + build + test (Release). deploy: SSH к DEPLOY_HOST, docker compose pull + up -d --no-deps. Переменные: SSH_PRIVATE_KEY, DEPLOY_HOST, DEPLOY_USER через GitLab CI variables.
**Проблемы:** Нет.

### [2026-05-07] TASK-007 — Nginx SSL + Let's Encrypt
**Статус:** done
**Summary:** nginx/nginx.conf обновлён: HTTP→HTTPS redirect (кроме /.well-known/acme-challenge/), HTTPS server с ssl_certificate из /etc/letsencrypt, TLS 1.2+1.3, HSTS, X-Frame-Options DENY, www→non-www редирект, client_max_body_size 12m, Cache-Control для статики.
**Проблемы:** Нет.

### [2026-05-07] TASK-009 — Локализация AZ/RU (.resx)
**Статус:** done
**Summary:** Resources/SharedResource.az.resx + Resources/SharedResource.ru.resx созданы (Masters, Register, Cabinet, Search, City, Specialization, Experience, Price, Reviews, Phone, Save, Cancel, Delete, Upload, VerifiedMaster, YearsExp). Resources/SharedResource.cs — marker класс. AddLocalization(ResourcesPath="Resources") + RequestLocalizationOptions (default=az, supported=az+ru) зарегистрированы. UseRequestLocalization() добавлен в pipeline.
**Проблемы:** Нет.

### [2026-05-07] TASK-034 — Финальная полировка: 404/500 страницы
**Статус:** done
**Summary:** Pages/Shared/Error404.cshtml + Error500.cshtml созданы (Tailwind, ссылка на главную). UseStatusCodePagesWithReExecute("/Shared/Error{0}") добавлен в pipeline. UseExceptionHandler заменён на /Shared/Error500.
**Проблемы:** Нет.

### [2026-05-07] TASK-001 — Создать ASP.NET Core 10 Razor Pages проект
**Статус:** done
**Summary:** Создан проект Ustalar в /Users/albina/Desktop/work/Ustalar/ через `dotnet new webapp --framework net10.0`. Добавлены папки Models/, Services/, Data/, Resources/Pages/, Resources/Shared/. Program.cs настроен с AddRazorPages(), AddAntiforgery(), UseStaticFiles(), UseRouting(), UseAuthorization(). dotnet build: 0 ошибок, 0 предупреждений.
**Проблемы:** .NET SDK не был установлен — потребовалась установка dotnet 10.0.203 через dotnet-install.sh.

### [2026-05-07] TASK-002 — Docker Compose с postgres:17.9 и nginx
**Статус:** done
**Summary:** Созданы docker-compose.yml (сервисы app/postgres/nginx с healthcheck и сетью ustalar-net), Ustalar/Dockerfile (многоэтапный build sdk:10.0→aspnet:10.0, curl для healthcheck), nginx/nginx.conf (reverse proxy на app:8080), .env.example, .gitignore.
**Проблемы:** Docker CLI не в PATH (Docker Desktop установлен, но не запущен) — валидация `docker compose config` не выполнена, будет проверена при первом запуске.

### [2026-05-07] TASK-003 — EF Core 10 + Npgsql + ApplicationDbContext
**Статус:** done
**Summary:** Добавлены пакеты Npgsql.EntityFrameworkCore.PostgreSQL 10.0.1 и Microsoft.EntityFrameworkCore.Design. Создан Data/ApplicationDbContext.cs. Program.cs обновлён с UseNpgsql(). appsettings.json содержит ConnectionStrings:Default. dotnet build: 0 ошибок. dotnet ef dbcontext info: Provider=Npgsql, Database=ustalar.
**Проблемы:** Нет.

### [2026-05-07] TASK-012 — AntiForgery + Rate Limiting middleware
**Статус:** done
**Summary:** Program.cs обновлён: UseAntiforgery() добавлен в pipeline. AddRateLimiter() с двумя политиками: "sms" (3 req / 10 мин по IP) и "reviews" (2 req / час по IP), RejectionStatusCode=429. Создан Services/RateLimitPolicies.cs с константами. dotnet build: 0 ошибок.
**Проблемы:** Была взята раньше TASK-005 — ошибка приоритизации (TASK-005 разблокирует цепочку TASK-010/011/016).

### [2026-05-07] TASK-024 — Страница профиля мастера
**Статус:** done
**Summary:** Pages/Masters/Profile.cshtml (@page "/masters/{citySlug}/{specSlug}/{masterId:int}") — профиль с аватаром, специализациями, опытом, ценой, WhatsApp/tel кнопками. Галерея фото через GLightbox. Список одобренных отзывов с рейтингом и средним баллом. Форма отзыва подгружается через hx-get="/reviews/{masterId}" hx-trigger="load". 404 при status!=Active или неверном citySlug. OG-теги и canonical.

### [2026-05-07] TASK-025 — Главная страница
**Статус:** done
**Summary:** Pages/Index.cshtml — hero-блок с поиском (select специализации → /masters?specSlug=), секция популярных специализаций (10 штук), секция проверенных мастеров (6 random is_verified=true). hreflang az/ru. SSR, без JS-зависимостей для контента.

### [2026-05-07] TASK-015 — Health check endpoint /health
**Статус:** done
**Summary:** AddHealthChecks().AddNpgSql(connectionString) + MapHealthChecks("/health"). Пакет AspNetCore.HealthChecks.NpgSql 9.0.0 добавлен. При недоступной БД → 503, при доступной → 200 Healthy.

### [2026-05-07] TASK-029 — Admin: модерация отзывов и фото
**Статус:** done
**Summary:** Pages/Admin/Reviews.cshtml — список неодобренных отзывов с OnPostApproveAsync/OnPostRejectAsync. Pages/Admin/Photos.cshtml — сетка фото на модерации с теми же хендлерами. Оба [Authorize(AdminCookie)]. dotnet build: 0 ошибок.

### [2026-05-07] TASK-023 — URL-роутинг каталога по городу и специализации
**Статус:** done
**Summary:** Создан MastersCatalogService с общей логикой запроса (фильтр, пагинация, 404 при несуществующем slug). CatalogViewModel выделен в отдельный класс. Созданы Pages/Masters/City.cshtml (@page "/masters/{citySlug}") и CitySpec.cshtml (@page "/masters/{citySlug}/{specSlug}") с хлебными крошками и canonical URL. Index.cshtml переработан под CatalogViewModel. dotnet build: 0 ошибок.

### [2026-05-07] TASK-027 — Система отзывов
**Статус:** done
**Summary:** Pages/Reviews/Create.cshtml (@page "/reviews/{masterId:int}") — форма с reviewer_name, reviewer_phone, rating (Alpine.js звёзды), text. POST создаёт Review с is_approved=false. [EnableRateLimiting(Reviews)]. AntiForgery. Успех показывает inline сообщение без перезагрузки (Layout=null, встраивается в профиль через HTMX).

### [2026-05-07] TASK-028 — Административная панель: список мастеров
**Статус:** done
**Summary:** Pages/Admin/Index.cshtml — таблица мастеров с фильтром по статусу (Pending/Active/Blocked), бейдж с количеством на модерации, кнопки Approve/Block прямо в строке, пагинация 20/стр. [Authorize(AdminCookie)]. OnPostApproveAsync/OnPostBlockAsync меняют статус. dotnet build: 0 ошибок.

### [2026-05-07] TASK-019 — Личный кабинет мастера
**Статус:** done
**Summary:** Pages/Cabinet/Index.cshtml — показывает профиль, статус (pending/blocked баннер), специализации, портфолио. Pages/Cabinet/Edit.cshtml — редактирование About, Whatsapp, ExperienceYears, PriceFrom, PriceTo с сохранением UpdatedAt. Оба [Authorize(MasterCookie)], logout через OnPostLogoutAsync. dotnet build: 0 ошибок.

### [2026-05-07] TASK-022 — Каталог мастеров с HTMX-фильтрацией
**Статус:** done
**Summary:** Pages/Masters/Index.cshtml — фильтры по городу и специализации через hx-get. Pages/Masters/_MastersList.cshtml — partial с карточками (аватар, имя, город, специализации, цена) и пагинацией. IndexModel: AsNoTracking(), Skip/Take, фильтр WHERE status=Active, HTMX-детект через Request.IsHtmx() → return Partial. HtmxExtensions.cs создан. dotnet build: 0 ошибок.

### [2026-05-07] TASK-016 — Регистрация Шаг 1: телефон + SMS
**Статус:** done
**Summary:** Pages/Register/Index.cshtml — форма с телефоном (+994 валидация regex). POST /register: если мастер уже есть — SignInAsync → /cabinet, иначе SmsVerificationService.SendCodeAsync → TempData["RegisterPhone"] → /register/step2. [EnableRateLimiting(Sms)].

### [2026-05-07] TASK-017 — Регистрация Шаг 2: верификация SMS-кода
**Статус:** done
**Summary:** Pages/Register/Step2.cshtml — форма ввода 6-значного кода. POST /register/step2: SmsVerificationService.VerifyCodeAsync (проверяет IsUsed, ExpiresAt < UtcNow) → TempData["PhoneVerified"]=true → /register/step3.

### [2026-05-07] TASK-018 — Регистрация Шаг 3: создание профиля
**Статус:** done
**Summary:** Pages/Register/Step3.cshtml — форма: FullName, CityId (SelectList), SpecializationIds (checkbox), About, ExperienceYears, Whatsapp, PriceFrom, PriceTo. POST создаёт Master со status=Pending, MasterSpecialization записи, MasterAuthService.SignInAsync → /cabinet. TempData очищается. AddSession добавлен в Program.cs. dotnet build: 0 ошибок.
**Проблемы:** Нет.

### [2026-05-07] TASK-008 — Tailwind CSS v4 + HTMX 2.0 + Alpine.js + GLightbox
**Статус:** done
**Summary:** _Layout.cshtml переписан: Bootstrap убран, подключены HTMX 2.0.4 (unpkg), Alpine.js 3.14.9 (CDN), GLightbox 3.3.0 (CDN). Tailwind v4 настроен через wwwroot/css/app.css с @import "tailwindcss" и @theme (--color-primary: orange). package.json создан со скриптами css:build и css:watch через @tailwindcss/cli@next.
**Проблемы:** Нет.

### [2026-05-07] TASK-014 — Twilio SMS сервис
**Статус:** done
**Summary:** Добавлен пакет Twilio 7.4.0. Созданы ISmsService (интерфейс), TwilioSmsService (реализация через MessageResource.CreateAsync), SmsVerificationService (генерация 6-значного кода через RandomNumberGenerator, сохранение в БД с TTL 10 мин, инвалидация предыдущих кодов). Зарегистрированы в DI. Twilio:AccountSid/AuthToken/FromNumber добавлены в appsettings.json. dotnet build: 0 ошибок.
**Проблемы:** Нет.

### [2026-05-07] TASK-010 + TASK-011 — Cookie Auth (мастера + администраторы)
**Статус:** done
**Summary:** Program.cs обновлён: AddAuthentication("MasterCookie").AddCookie("AdminCookie") с раздельными куками (ustalar_master / ustalar_admin), оба HttpOnly/Secure/SameSite=Strict. Создан MasterAuthService (вход по телефону), AdminAuthService (вход по email + BCrypt.Verify, workFactor=12). Страницы: Cabinet/Index [Authorize MasterCookie], Register/Index (заглушка), Admin/Login, Admin/Index [Authorize AdminCookie] с logout. Пакет BCrypt.Net-Next 4.0.3 добавлен. dotnet build: 0 ошибок.
**Проблемы:** Субагенты были заблокированы на разрешениях Write/Bash — задачи реализованы напрямую.

### [2026-05-07] TASK-005 — Seed данных: города и специализации
**Статус:** done
**Summary:** ApplicationDbContext.OnModelCreating дополнен HasData() для City (8 городов: Baku, Ganja, Sumgayit, Mingachevir, Lankaran, Shirvan, Nakhchivan, Quba) и Specialization (10 специальностей: elektrik, santexnik, boyaqci, dulger, qaynaqci, kafel, gipsokarton, kondisioner, parket, suvaqci). Создана миграция SeedInitialData (20260507115013). dotnet build: 0 ошибок.
**Проблемы:** Нет.

### [2026-05-07] TASK-004 — Entity-модели EF Core + индексы + миграция
**Статус:** done
**Summary:** Созданы 9 моделей в Models/: Master, City, Specialization, MasterSpecialization, PortfolioPhoto, Review, SmsVerification, AdminUser, AdminActionLog. ApplicationDbContext обновлён с DbSet'ами и Fluent API конфигурацией: составной PK для MasterSpecializations, индексы {CityId,Status}, {Status}, Phone(unique), {MasterId,IsApproved} для фото и отзывов, уникальные slug для Cities/Specializations. Миграция InitialCreate создана (20260507114242). dotnet build: 0 ошибок.
**Проблемы:** Нет.
