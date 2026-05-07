using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Masters;

public class CitySpecModel : PageModel
{
    private readonly MastersCatalogService _catalog;

    public CitySpecModel(MastersCatalogService catalog)
    {
        _catalog = catalog;
    }

    public CatalogViewModel Catalog { get; set; } = new();
    public City? CurrentCity { get; set; }
    public Specialization? CurrentSpec { get; set; }

    public async Task<IActionResult> OnGetAsync(string citySlug, string specSlug, int page = 1)
    {
        var result = await _catalog.QueryAsync(citySlug, specSlug, page);
        if (result == null) return NotFound();

        CurrentCity = result.CurrentCity;
        CurrentSpec = result.CurrentSpec;
        Catalog = new CatalogViewModel
        {
            Masters = result.Masters,
            Cities = result.Cities,
            Specializations = result.Specializations,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages,
            CurrentCitySlug = citySlug,
            CurrentSpecSlug = specSlug
        };
        return Page();
    }
}
