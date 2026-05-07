namespace Ustalar.Models;

public class Master
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Whatsapp { get; set; }
    public string? About { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public string? AvatarUrl { get; set; }
    public MasterStatus Status { get; set; } = MasterStatus.Pending;
    public bool IsVerified { get; set; }
    public int CityId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public City City { get; set; } = null!;
    public ICollection<MasterSpecialization> MasterSpecializations { get; set; } = [];
    public ICollection<PortfolioPhoto> PortfolioPhotos { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}

public enum MasterStatus
{
    Pending,
    Active,
    Blocked
}
