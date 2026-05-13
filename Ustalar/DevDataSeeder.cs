// УДАЛИТЬ ПЕРЕД ПРОДАКШНОМ: моковые данные для разработки
// Удалить этот файл + строку "await DevDataSeeder.SeedAsync(...)" в Program.cs

using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;

public static class DevDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await db.Masters.AnyAsync()) return; // уже засеяно

        var now = DateTime.UtcNow;

        var masters = new List<Master>
        {
            new() {
                FullName = "Rəşad Məmmədov",
                Phone = "+994501110001",
                Whatsapp = "+994501110001",
                About = "15 illik təcrübəm var. Mənzil və ofis elektrik işlərini yerinə yetirirəm. Keyfiyyəti zəmanət verirəm.",
                ExperienceYears = 15,
                PriceFrom = 80,
                PriceTo = 300,
                AvatarUrl = "https://i.pravatar.cc/300?img=11",
                Status = MasterStatus.Active,
                IsVerified = true,
                CityId = 1, // Bakı
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 1 }] // Elektrik
            },
            new() {
                FullName = "Fuad Hüseynov",
                Phone = "+994502220002",
                Whatsapp = "+994502220002",
                About = "Santexnik kimi 10 ildir işləyirəm. Su kəməri, kanalizasiya, qaynar su sistemi qurulması.",
                ExperienceYears = 10,
                PriceFrom = 60,
                PriceTo = 250,
                AvatarUrl = "https://i.pravatar.cc/300?img=15",
                Status = MasterStatus.Active,
                IsVerified = true,
                CityId = 1, // Bakı
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 2 }] // Santexnik
            },
            new() {
                FullName = "Elnur Quliyev",
                Phone = "+994503330003",
                Whatsapp = "+994503330003",
                About = "Boyaq işləri üzrə mütəxəssisəm. Divar, tavan, fasad boyaqlanması.",
                ExperienceYears = 8,
                PriceFrom = 50,
                PriceTo = 150,
                AvatarUrl = "https://i.pravatar.cc/300?img=20",
                Status = MasterStatus.Active,
                IsVerified = false,
                CityId = 1, // Bakı
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 3 }] // Boyaqçı
            },
            new() {
                FullName = "Mübariz Əliyev",
                Phone = "+994504440004",
                Whatsapp = "+994504440004",
                About = "Kafel döşənməsi, vanna otağı və mətbəx təmiri sahəsində 12 illik təcrübəm var.",
                ExperienceYears = 12,
                PriceFrom = 90,
                PriceTo = 400,
                AvatarUrl = "https://i.pravatar.cc/300?img=33",
                Status = MasterStatus.Active,
                IsVerified = true,
                CityId = 1, // Bakı
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 6 }] // Kafel
            },
            new() {
                FullName = "Tural İsmayılov",
                Phone = "+994505550005",
                Whatsapp = "+994505550005",
                About = "Gipsokarton konstruksiyaları, asma tavanlar, bölmə divarları qururam.",
                ExperienceYears = 6,
                PriceFrom = 70,
                PriceTo = 200,
                AvatarUrl = "https://i.pravatar.cc/300?img=41",
                Status = MasterStatus.Active,
                IsVerified = false,
                CityId = 2, // Gəncə
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 7 }] // Gipsokarton
            },
            new() {
                FullName = "Nicat Babayev",
                Phone = "+994506660006",
                Whatsapp = "+994506660006",
                About = "Kondisioner quraşdırılması və texniki xidmət. Bütün brendlər üzrə sertifikatım var.",
                ExperienceYears = 5,
                PriceFrom = 100,
                PriceTo = 350,
                AvatarUrl = "https://i.pravatar.cc/300?img=52",
                Status = MasterStatus.Active,
                IsVerified = true,
                CityId = 1, // Bakı
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 8 }] // Kondisioner
            },
            new() {
                FullName = "Samir Rzayev",
                Phone = "+994507770007",
                Whatsapp = "+994507770007",
                About = "Parket, laminat döşənməsi. Sürətli və keyfiyyətli iş görürəm.",
                ExperienceYears = 9,
                PriceFrom = 80,
                PriceTo = 220,
                AvatarUrl = "https://i.pravatar.cc/300?img=60",
                Status = MasterStatus.Active,
                IsVerified = true,
                CityId = 3, // Sumqayıt
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations = [new() { SpecializationId = 9 }] // Parket
            },
            new() {
                FullName = "Orxan Nəsirov",
                Phone = "+994508880008",
                Whatsapp = "+994508880008",
                About = "Elektrik və santexnik işlərini bir yerdə görürəm. Tam mənzil təmiri üçün ideal.",
                ExperienceYears = 20,
                PriceFrom = 120,
                PriceTo = 500,
                AvatarUrl = "https://i.pravatar.cc/300?img=68",
                Status = MasterStatus.Active,
                IsVerified = true,
                CityId = 1, // Bakı
                CreatedAt = now,
                UpdatedAt = now,
                MasterSpecializations =
                [
                    new() { SpecializationId = 1 }, // Elektrik
                    new() { SpecializationId = 2 }  // Santexnik
                ]
            },
        };

        db.Masters.AddRange(masters);
        await db.SaveChangesAsync();

        // Отзывы для первых трёх мастеров
        var reviews = new List<Review>
        {
            new() { MasterId = masters[0].Id, ReviewerName = "Aytən M.", ReviewerPhone = "+994501000001", Rating = 5, Text = "Əla usta! İşi vaxtında bitirdi, qiymət razılaşdığımız kimi oldu. Tövsiyyə edirəm!", IsApproved = true, CreatedAt = now },
            new() { MasterId = masters[0].Id, ReviewerName = "Kamran B.", ReviewerPhone = "+994502000002", Rating = 4, Text = "Yaxşı usta, işdən razıyam. Bir az gec gəldi amma nəticə qənaətbəxşdir.", IsApproved = true, CreatedAt = now },
            new() { MasterId = masters[1].Id, ReviewerName = "Leyla H.", ReviewerPhone = "+994503000003", Rating = 5, Text = "Çox peşəkar usta. Problemi tez həll etdi. Qiymət ədalətlidir.", IsApproved = true, CreatedAt = now },
            new() { MasterId = masters[2].Id, ReviewerName = "Vüsal R.", ReviewerPhone = "+994504000004", Rating = 4, Text = "İşi gözəl gördü. Otaq çox gözəl göründü boyandıqdan sonra.", IsApproved = true, CreatedAt = now },
            new() { MasterId = masters[3].Id, ReviewerName = "Günel Ə.", ReviewerPhone = "+994505000005", Rating = 5, Text = "Vanna otağı mükəmməl oldu! Kafel işi çox dəqiqdir. Hamıya tövsiyyə edirəm!", IsApproved = true, CreatedAt = now },
        };

        db.Reviews.AddRange(reviews);
        await db.SaveChangesAsync();

        // Admin user (только если ещё нет)
        if (!await db.AdminUsers.AnyAsync())
        {
            db.AdminUsers.Add(new AdminUser
            {
                Email = "admin@ustalar.az",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 12),
                CreatedAt = now
            });
            await db.SaveChangesAsync();
        }
    }
}
