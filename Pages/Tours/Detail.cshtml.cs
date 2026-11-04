using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Models;
using TengriTrails.Services;

namespace TengriTrails.Pages.Tours;

public sealed class DetailModel(TourCatalog catalog, ITextCatalog text) : PageModel
{
    public string Language { get; private set; } = "en";
    public Tour Tour { get; private set; } = null!;

    public IActionResult OnGet(string lang, string slug)
    {
        if (lang is not ("ky" or "ru" or "en" or "es" or "fr" or "zh"))
            return NotFound();

        var selectedTour = catalog.Find(slug);
        if (selectedTour is null)
            return NotFound();

        Language = lang;
        Tour = selectedTour;
        return Page();
    }

    public string FormatPrice(int amount)
    {
        var culture = CultureInfo.GetCultureInfo(Language switch
        {
            "ky" => "ky-KG", "ru" => "ru-KG", "en" => "en-US",
            "es" => "es-ES", "fr" => "fr-FR", _ => "zh-CN"
        });
        return $"{amount.ToString("N0", culture)} {text.Get("currency", Language)}";
    }

    public string FormatDate(DateTime date)
    {
        var culture = CultureInfo.GetCultureInfo(Language switch
        {
            "ky" => "ky-KG", "ru" => "ru-KG", "en" => "en-US",
            "es" => "es-ES", "fr" => "fr-FR", _ => "zh-CN"
        });
        return date.ToString("d MMMM yyyy", culture);
    }
}
