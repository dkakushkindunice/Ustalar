using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ustalar.Data;
using Ustalar.Models;

namespace Ustalar.Services;

public record MastersCatalogResult(
    List<Master> Masters,
    List<City> Cities,
    List<Specialization> Specializations,
    int TotalPages,
    int CurrentPage,
    City? CurrentCity,
    Specialization? CurrentSpec
);

public class MastersCatalogService
{
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    private const int PageSize = 12;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public MastersCatalogService(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<MastersCatalogResult?> QueryAsync(
        string? citySlug, string? specSlug, int page = 1)
    {
        page = page < 1 ? 1 : page;

        var cities = await _cache.GetOrCreateAsync("catalog:cities", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _db.Cities.AsNoTracking().OrderBy(c => c.NameAz).ToListAsync();
        }) ?? [];

        var specs = await _cache.GetOrCreateAsync("catalog:specs", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _db.Specializations.AsNoTracking().OrderBy(s => s.NameAz).ToListAsync();
        }) ?? [];

        City? currentCity = null;
        Specialization? currentSpec = null;

        if (!string.IsNullOrEmpty(citySlug))
        {
            currentCity = cities.FirstOrDefault(c => c.Slug == citySlug);
            if (currentCity == null) return null; // → 404
        }

        if (!string.IsNullOrEmpty(specSlug))
        {
            currentSpec = specs.FirstOrDefault(s => s.Slug == specSlug);
            if (currentSpec == null) return null; // → 404
        }

        var query = _db.Masters
            .Where(m => m.Status == MasterStatus.Active)
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations)
                .ThenInclude(ms => ms.Specialization)
            .AsNoTracking();

        if (currentCity != null)
            query = query.Where(m => m.CityId == currentCity.Id);

        if (currentSpec != null)
            query = query.Where(m => m.MasterSpecializations
                .Any(ms => ms.SpecializationId == currentSpec.Id));

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)PageSize);

        var masters = await query
            .OrderByDescending(m => m.IsVerified)
            .ThenByDescending(m => m.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return new MastersCatalogResult(
            masters, cities, specs, totalPages, page, currentCity, currentSpec);
    }
}
