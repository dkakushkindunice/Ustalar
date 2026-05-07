namespace Ustalar.Models;

public class Review
{
    public int Id { get; set; }
    public int MasterId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerPhone { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Text { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }

    public Master Master { get; set; } = null!;
}
