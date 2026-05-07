namespace Ustalar.Models;

public class Specialization
{
    public int Id { get; set; }
    public string NameAz { get; set; } = string.Empty;
    public string NameRu { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<MasterSpecialization> MasterSpecializations { get; set; } = [];
}
