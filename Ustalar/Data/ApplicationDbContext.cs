using Microsoft.EntityFrameworkCore;
using Ustalar.Models;

namespace Ustalar.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Master> Masters => Set<Master>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<MasterSpecialization> MasterSpecializations => Set<MasterSpecialization>();
    public DbSet<PortfolioPhoto> PortfolioPhotos => Set<PortfolioPhoto>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<SmsVerification> SmsVerifications => Set<SmsVerification>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<AdminActionLog> AdminActionLogs => Set<AdminActionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MasterSpecialization — составной PK
        modelBuilder.Entity<MasterSpecialization>()
            .HasKey(ms => new { ms.MasterId, ms.SpecializationId });

        // Master конфигурация
        modelBuilder.Entity<Master>(entity =>
        {
            entity.Property(m => m.Status)
                .HasConversion<string>();

            entity.Property(m => m.Phone)
                .HasMaxLength(20);

            entity.Property(m => m.PriceFrom)
                .HasColumnType("decimal(10,2)");

            entity.Property(m => m.PriceTo)
                .HasColumnType("decimal(10,2)");

            // Обязательные индексы для производительности каталога
            entity.HasIndex(m => new { m.CityId, m.Status });
            entity.HasIndex(m => m.Status);
            entity.HasIndex(m => m.Phone).IsUnique();
        });

        // City — уникальный slug
        modelBuilder.Entity<City>()
            .HasIndex(c => c.Slug).IsUnique();

        // Specialization — уникальный slug
        modelBuilder.Entity<Specialization>()
            .HasIndex(s => s.Slug).IsUnique();

        // PortfolioPhoto индекс
        modelBuilder.Entity<PortfolioPhoto>()
            .HasIndex(p => new { p.MasterId, p.IsApproved });

        // Review индекс
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasIndex(r => new { r.MasterId, r.IsApproved });
            entity.Property(r => r.Rating)
                .HasAnnotation("Range", new[] { 1, 5 });
        });

        // Автоматические timestamps через UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var createdAt = entityType.FindProperty("CreatedAt");
            if (createdAt != null && createdAt.ClrType == typeof(DateTime))
                createdAt.SetDefaultValueSql("NOW()");

            var updatedAt = entityType.FindProperty("UpdatedAt");
            if (updatedAt != null && updatedAt.ClrType == typeof(DateTime))
                updatedAt.SetDefaultValueSql("NOW()");
        }

        // Seed: Города Азербайджана
        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, NameAz = "Bakı", NameRu = "Баку", Slug = "baku" },
            new City { Id = 2, NameAz = "Gəncə", NameRu = "Гянджа", Slug = "ganja" },
            new City { Id = 3, NameAz = "Sumqayıt", NameRu = "Сумгайыт", Slug = "sumgayit" },
            new City { Id = 4, NameAz = "Mingəçevir", NameRu = "Мингечевир", Slug = "mingachevir" },
            new City { Id = 5, NameAz = "Lənkəran", NameRu = "Ленкорань", Slug = "lankaran" },
            new City { Id = 6, NameAz = "Şirvan", NameRu = "Ширван", Slug = "shirvan" },
            new City { Id = 7, NameAz = "Naxçıvan", NameRu = "Нахчыван", Slug = "nakhchivan" },
            new City { Id = 8, NameAz = "Quba", NameRu = "Куба", Slug = "quba" }
        );

        // Seed: Строительные специализации
        modelBuilder.Entity<Specialization>().HasData(
            new Specialization { Id = 1, NameAz = "Elektrik", NameRu = "Электрик", Slug = "elektrik" },
            new Specialization { Id = 2, NameAz = "Santexnik", NameRu = "Сантехник", Slug = "santexnik" },
            new Specialization { Id = 3, NameAz = "Rəssam-Boyaqçı", NameRu = "Маляр", Slug = "boyaqci" },
            new Specialization { Id = 4, NameAz = "Dülgər", NameRu = "Плотник", Slug = "dulger" },
            new Specialization { Id = 5, NameAz = "Qaynaqçı", NameRu = "Сварщик", Slug = "qaynaqci" },
            new Specialization { Id = 6, NameAz = "Kafel ustası", NameRu = "Плиточник", Slug = "kafel" },
            new Specialization { Id = 7, NameAz = "Gipsokarton ustası", NameRu = "Гипсокартон", Slug = "gipsokarton" },
            new Specialization { Id = 8, NameAz = "Kondisioner montajı", NameRu = "Кондиционеры", Slug = "kondisioner" },
            new Specialization { Id = 9, NameAz = "Parket ustası", NameRu = "Паркетчик", Slug = "parket" },
            new Specialization { Id = 10, NameAz = "Suvaqçı", NameRu = "Штукатур", Slug = "suvaqci" }
        );
    }
}
