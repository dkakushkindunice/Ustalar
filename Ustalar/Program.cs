using System.Threading.RateLimiting;
using Ustalar.Data;
using Ustalar.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

// AntiForgery protection for CSRF
builder.Services.AddAntiforgery();

// Authentication — две независимые схемы
builder.Services.AddAuthentication("MasterCookie")
    .AddCookie("MasterCookie", options =>
    {
        options.LoginPath = "/register";
        options.AccessDeniedPath = "/register";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "ustalar_master";
    })
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "ustalar_admin";
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MasterAuthService>();
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<SmsVerificationService>();
builder.Services.AddScoped<MastersCatalogService>();
builder.Services.AddSingleton<ISmsService, TwilioSmsService>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Лимит для SMS: 3 запроса за 10 минут с одного IP
    options.AddPolicy(RateLimitPolicies.Sms, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Лимит для отзывов: 2 запроса за час с одного IP
    options.AddPolicy(RateLimitPolicies.Reviews, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2,
                Window = TimeSpan.FromHours(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseRateLimiter();
app.UseAntiforgery();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
