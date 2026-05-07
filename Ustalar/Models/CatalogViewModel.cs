using Ustalar.Models;

namespace Ustalar.Models;

public class CatalogViewModel
{
    public List<Master> Masters { get; set; } = [];
    public List<City> Cities { get; set; } = [];
    public List<Specialization> Specializations { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? CurrentCitySlug { get; set; }
    public string? CurrentSpecSlug { get; set; }
}
