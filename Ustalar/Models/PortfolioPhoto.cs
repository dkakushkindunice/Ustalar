namespace Ustalar.Models;

public class PortfolioPhoto
{
    public int Id { get; set; }
    public int MasterId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }

    public Master Master { get; set; } = null!;
}
