namespace Ustalar.Models;

public class AdminActionLog
{
    public int Id { get; set; }
    public int AdminId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public DateTime CreatedAt { get; set; }

    public AdminUser Admin { get; set; } = null!;
}
