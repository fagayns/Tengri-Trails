using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Services;

namespace TengriTrails.Pages.Tours;

public sealed class BookingConfirmedModel(TourCatalog catalog, BookingService bookings) : PageModel
{
    public string Language { get; private set; } = "en";
    public string BookingNumber { get; private set; } = "";
    public string TourSlug { get; private set; } = "";
    public string TourName { get; private set; } = "";

    public async Task<IActionResult> OnGetAsync(string lang, string slug, string bookingNumber, CancellationToken cancellationToken)
    {
        if (lang is not ("ky" or "ru" or "en" or "es" or "fr" or "zh")
            || catalog.Find(slug, lang) is not { } tour
            || !await bookings.ExistsAsync(bookingNumber, cancellationToken))
            return NotFound();

        Language = lang;
        BookingNumber = bookingNumber;
        TourSlug = tour.Slug;
        TourName = tour.Name;
        return Page();
    }
}
