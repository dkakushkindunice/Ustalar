using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Masters;

public class CityModel : PageModel
{
    private readonly MastersCatalogService _catalog;

    public CityModel(MastersCatalogService catalog)
    {
        _catalog = catalog;
    }

    public CatalogViewModel Catalog { get; set; } = new();
    public City? CurrentCity { get; set; }

    public async Task<IActionResult> OnGetAsync(string citySlug, int page = 1)
    {
        var result = await _catalog.QueryAsync(citySlug, null, page);
        if (result == null) return NotFound();

        CurrentCity = result.CurrentCity;
        Catalog = new CatalogViewModel
        {
            Masters = result.Masters,
            Cities = result.Cities,
            Specializations = result.Specializations,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages,
            CurrentCitySlug = citySlug
        };
        return Page();
    }
}
