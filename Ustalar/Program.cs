using System.IO.Compression;
using System.Threading.RateLimiting;
using Amazon.Runtime;
using Amazon.S3;
using Ustalar.Data;
using Ustalar.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Razor Pages
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")!);

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

// R2 / S3 file storage
var r2Config = builder.Configuration.GetSection("R2");
var r2Credentials = new BasicAWSCredentials(
    r2Config["AccessKey"] ?? throw new InvalidOperationException("R2:AccessKey not configured"),
    r2Config["SecretKey"] ?? throw new InvalidOperationException("R2:SecretKey not configured"));
var r2S3Config = new AmazonS3Config
{
    ServiceURL = r2Config["ServiceUrl"] ?? throw new InvalidOperationException("R2:ServiceUrl not configured"),
    ForcePathStyle = true
};
builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(r2Credentials, r2S3Config));
builder.Services.AddSingleton<IFileStorageService, R2FileStorageService>();
builder.Services.AddSingleton<ImageProcessingService>();

// In-memory cache for catalog dropdowns
builder.Services.AddMemoryCache();

// Response compression (gzip + brotli)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["text/html", "application/json", "image/svg+xml", "application/xml"]);
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

// Background services
builder.Services.AddHostedService<SmsCleanupService>();

// Localization AZ (default) + RU
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supported = new[] { "az", "ru" };
    options.SetDefaultCulture("az")
           .AddSupportedCultures(supported)
           .AddSupportedUICultures(supported);
    options.ApplyCurrentCultureToResponseHeaders = true;
});

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
    app.UseExceptionHandler("/Shared/Error500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Shared/Error{0}");
app.UseRequestLocalization();

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseRateLimiter();
app.UseAntiforgery();
app.UseAuthorization();
app.MapRazorPages();
app.MapHealthChecks("/health");

app.Run();
