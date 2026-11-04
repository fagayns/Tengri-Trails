using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Models;
using TengriTrails.Services;

namespace TengriTrails.Pages;

public sealed class IndexModel(TourCatalog catalog, ITextCatalog text) : PageModel
{
    private static readonly (string Key, string Value)[] RegionOptions =
    [
        ("region.issyk_kul", "issyk-kul"),
        ("region.naryn", "naryn"),
        ("region.osh", "osh"),
        ("region.chuy", "chuy"),
        ("region.jalal_abad", "jalal-abad")
    ];

    public string Language { get; private set; } = "en";
    public IReadOnlyList<Tour> PopularTours => catalog.All.Take(3).ToArray();
    public IReadOnlyList<(string Key, string Value)> Regions => RegionOptions;

    public IActionResult OnGet(string lang)
    {
        if (!IsSupported(lang))
            return NotFound();

        Language = lang;
        return Page();
    }

    public string FormatPrice(int amount)
    {
        var culture = CultureInfo.GetCultureInfo(Language switch
        {
            "ky" => "ky-KG",
            "ru" => "ru-KG",
            "en" => "en-US",
            "es" => "es-ES",
            "fr" => "fr-FR",
            _ => "zh-CN"
        });
        return $"{amount.ToString("N0", culture)} {text.Get("currency", Language)}";
    }

    private static bool IsSupported(string? language) =>
        language is "ky" or "ru" or "en" or "es" or "fr" or "zh";
}
