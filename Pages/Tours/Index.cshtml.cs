using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Models;
using TengriTrails.Services;

namespace TengriTrails.Pages.Tours;

public sealed class IndexModel(TourCatalog catalog, ITextCatalog text) : PageModel
{
    private static readonly (string Key, string Value)[] RegionOptions =
    [
        ("region.bishkek", "bishkek"), ("region.chuy", "chuy"),
        ("region.issyk_kul", "issyk-kul"), ("region.naryn", "naryn"),
        ("region.talas", "talas"), ("region.jalal_abad", "jalal-abad"),
        ("region.osh", "osh"), ("region.batken", "batken")
    ];

    private static readonly (string Key, string Value)[] TypeOptions =
    [
        ("tour.type.trekking", "trekking"), ("tour.type.equestrian", "equestrian"),
        ("tour.type.cultural", "cultural"), ("tour.type.road", "road"), ("tour.type.lakes", "lakes")
    ];

    private static readonly (string Key, int Value)[] DifficultyOptions =
    [
        ("difficulty.1", 1), ("difficulty.2", 2), ("difficulty.3", 3), ("difficulty.4", 4)
    ];

    public string Language { get; private set; } = "en";
    public string? Query { get; private set; }
    public string? Region { get; private set; }
    public string? Type { get; private set; }
    public string? DateFrom { get; private set; }
    public string? DateTo { get; private set; }
    public int? MinDays { get; private set; }
    public int? MaxDays { get; private set; }
    public int? MinPrice { get; private set; }
    public int? MaxPrice { get; private set; }
    public int? Difficulty { get; private set; }
    public string Sort { get; private set; } = "date";
    public IReadOnlyList<(string Key, string Value)> Regions => RegionOptions;
    public IReadOnlyList<(string Key, string Value)> Types => TypeOptions;
    public IReadOnlyList<(string Key, int Value)> Difficulties => DifficultyOptions;
    public IReadOnlyList<CatalogTour> Results { get; private set; } = [];

    public IActionResult OnGet(
        string lang,
        string? query,
        string? region,
        string? type,
        string? dateFrom,
        string? dateTo,
        int? minDays,
        int? maxDays,
        int? minPrice,
        int? maxPrice,
        int? difficulty,
        string? sort)
    {
        if (!IsSupported(lang))
            return NotFound();

        Language = lang;
        Query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        Region = RegionOptions.Any(option => option.Value == region) ? region : null;
        Type = TypeOptions.Any(option => option.Value == type) ? type : null;
        DateFrom = ParseDate(dateFrom);
        DateTo = ParseDate(dateTo);
        MinDays = PositiveOrNull(minDays);
        MaxDays = PositiveOrNull(maxDays);
        MinPrice = PositiveOrNull(minPrice);
        MaxPrice = PositiveOrNull(maxPrice);
        Difficulty = DifficultyOptions.Any(option => option.Value == difficulty) ? difficulty : null;
        Sort = sort == "price" ? "price" : "date";

        var fromDate = ParseDateOnly(DateFrom);
        var toDate = ParseDateOnly(DateTo);
        var filtered = catalog.All(Language).Where(tour =>
            (Region is null || tour.RegionCode == Region)
            && (Type is null || tour.Type == Type)
            && (fromDate is null || DateOnly.FromDateTime(tour.Departure) >= fromDate)
            && (toDate is null || DateOnly.FromDateTime(tour.Departure) <= toDate)
            && (MinDays is null || tour.Days >= MinDays)
            && (MaxDays is null || tour.Days <= MaxDays)
            && (MinPrice is null || tour.Price >= MinPrice)
            && (MaxPrice is null || tour.Price <= MaxPrice)
            && (Difficulty is null || tour.Difficulty == Difficulty)
            && (Query is null
                || tour.Name.Contains(Query, StringComparison.CurrentCultureIgnoreCase)
                || tour.RegionName.Contains(Query, StringComparison.CurrentCultureIgnoreCase)));

        Results = (Sort == "price"
                ? filtered.OrderBy(tour => tour.Price).ThenBy(tour => tour.Departure)
                : filtered.OrderBy(tour => tour.Departure).ThenBy(tour => tour.Price))
            .ToArray();
        return Page();
    }

    public string FormatPrice(decimal amount)
    {
        var cultureName = Language switch
        {
            "ky" => "ky-KG", "ru" => "ru-KG", "en" => "en-US",
            "es" => "es-ES", "fr" => "fr-FR", _ => "zh-CN"
        };
        return $"{amount.ToString("N0", CultureInfo.GetCultureInfo(cultureName))} {text.Get("currency", Language)}";
    }

    public string FormatReferencePrices(decimal amount) =>
        text.Get("tour.reference_prices", Language, FormatUsd(amount), FormatEur(amount));

    public string FormatDate(DateTime date)
    {
        var cultureName = Language switch
        {
            "ky" => "ky-KG", "ru" => "ru-KG", "en" => "en-US",
            "es" => "es-ES", "fr" => "fr-FR", _ => "zh-CN"
        };
        return date.ToString("d MMM yyyy", CultureInfo.GetCultureInfo(cultureName));
    }

    private static int? PositiveOrNull(int? value) => value is > 0 ? value : null;

    private static string? ParseDate(string? value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
            ? value
            : null;

    private static DateOnly? ParseDateOnly(string? value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;

    private static string FormatUsd(decimal amount) => $"${amount / 87.5m:N0}";

    private static string FormatEur(decimal amount) =>
        $"€{(amount / 95m).ToString("N0", CultureInfo.GetCultureInfo("en-US"))}";

    private static bool IsSupported(string? language) =>
        language is "ky" or "ru" or "en" or "es" or "fr" or "zh";
}
