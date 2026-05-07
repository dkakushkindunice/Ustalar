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
