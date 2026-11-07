using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Models;
using TengriTrails.Services;

namespace TengriTrails.Pages.Tours;

public sealed class DetailModel(TourCatalog catalog, BookingService bookings, ITextCatalog text) : PageModel
{
    public string Language { get; private set; } = "en";
    public Tour Tour { get; private set; } = null!;
    public IReadOnlyList<TourDeparture> Departures { get; private set; } = [];
    public IReadOnlyList<Tour> RelatedTours { get; private set; } = [];
    public int Difficulty => Tour.Difficulty;

    public IActionResult OnGet(string lang, string slug)
    {
        if (lang is not ("ky" or "ru" or "en" or "es" or "fr" or "zh"))
            return NotFound();

        var selectedTour = catalog.Find(slug);
        if (selectedTour is null)
            return NotFound();

        Language = lang;
        Tour = selectedTour;
        Departures = catalog.DeparturesFor(slug);
        RelatedTours = catalog.All
            .Where(tour => tour.Slug != slug && tour.RegionKey == selectedTour.RegionKey)
            .Concat(catalog.All.Where(tour => tour.Slug != slug && tour.RegionKey != selectedTour.RegionKey))
            .Take(3)
            .ToArray();
        return Page();
    }

    public int RemainingSeats(string departureId) => bookings.RemainingSeats(departureId);

    public string DifficultyLabelKey(int difficulty) => $"difficulty.{difficulty}";

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

    public string FormatCompactDate(DateTime date)
    {
        var culture = CultureInfo.GetCultureInfo(Language switch
        {
            "ky" => "ky-KG", "ru" => "ru-KG", "en" => "en-US",
            "es" => "es-ES", "fr" => "fr-FR", _ => "zh-CN"
        });
        return date.ToString("d MMM yyyy", culture);
    }
}
