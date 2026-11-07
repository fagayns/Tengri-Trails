using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Services;

namespace TengriTrails.Pages.Tours;

public sealed class BookingConfirmedModel(TourCatalog catalog, BookingService bookings) : PageModel
{
    public string Language { get; private set; } = "en";
    public string BookingNumber { get; private set; } = "";
    public string TourSlug { get; private set; } = "";
    public string TourNameKey { get; private set; } = "";

    public IActionResult OnGet(string lang, string slug, string bookingNumber)
    {
        if (lang is not ("ky" or "ru" or "en" or "es" or "fr" or "zh")
            || catalog.Find(slug) is not { } tour
            || !bookings.Exists(bookingNumber))
            return NotFound();

        Language = lang;
        BookingNumber = bookingNumber;
        TourSlug = tour.Slug;
        TourNameKey = tour.NameKey;
        return Page();
    }
}
