using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Masters;

public class IndexModel : PageModel
{
    private readonly MastersCatalogService _catalog;

    public IndexModel(MastersCatalogService catalog)
    {
        _catalog = catalog;
    }

    public CatalogViewModel Catalog { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? citySlug, string? specSlug, int page = 1)
    {
        var result = await _catalog.QueryAsync(citySlug, specSlug, page);

        Catalog = new CatalogViewModel
        {
            Masters = result?.Masters ?? [],
            Cities = result?.Cities ?? [],
            Specializations = result?.Specializations ?? [],
            CurrentPage = result?.CurrentPage ?? 1,
            TotalPages = result?.TotalPages ?? 0,
            CurrentCitySlug = citySlug,
            CurrentSpecSlug = specSlug
        };

        if (Request.IsHtmx())
            return Partial("_MastersList", Catalog);

        return Page();
    }
}
