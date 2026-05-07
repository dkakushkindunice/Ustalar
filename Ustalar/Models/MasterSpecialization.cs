namespace Ustalar.Models;

public class MasterSpecialization
{
    public int MasterId { get; set; }
    public int SpecializationId { get; set; }

    public Master Master { get; set; } = null!;
    public Specialization Specialization { get; set; } = null!;
}
