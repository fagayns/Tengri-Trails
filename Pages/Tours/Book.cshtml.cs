using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Models;
using TengriTrails.Services;

namespace TengriTrails.Pages.Tours;

public sealed class BookModel(TourCatalog catalog, BookingService bookings, ITextCatalog text) : PageModel
{
    private static readonly string[] MessengerOptions = ["whatsapp", "telegram", "viber", "wechat"];

    [BindProperty]
    public string? DepartureId { get; set; }

    [BindProperty]
    public int Adults { get; set; } = 1;

    [BindProperty]
    public int Children { get; set; }

    [BindProperty]
    public string? FirstName { get; set; }

    [BindProperty]
    public string? LastName { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? Phone { get; set; }

    [BindProperty]
    public string? Citizenship { get; set; }

    [BindProperty]
    public string? Messenger { get; set; }

    [BindProperty]
    public string? Comment { get; set; }

    [BindProperty]
    public bool Consent { get; set; }

    public string Language { get; private set; } = "en";
    public CatalogTour Tour { get; private set; } = null!;
    public IReadOnlyList<TourDeparture> Departures { get; private set; } = [];
    public IReadOnlyList<string> MessengerChoices => MessengerOptions;

    public IActionResult OnGet(string lang, string slug, string? departureId)
    {
        if (!SetPageData(lang, slug))
            return NotFound();

        DepartureId = Departures.Any(departure => departure.Id == departureId && bookings.RemainingSeats(departure.Id) > 0)
            ? departureId
            : null;
        if (DepartureId is null)
            ModelState.Remove(nameof(DepartureId));
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string lang, string slug, CancellationToken cancellationToken)
    {
        if (!SetPageData(lang, slug))
            return NotFound();

        ValidateForm();
        var departure = Departures.FirstOrDefault(item => item.Id == DepartureId);
        if (departure is null)
            AddError(nameof(DepartureId), "booking.error.departure");
        else if (bookings.RemainingSeats(departure.Id) == 0)
            AddError(nameof(DepartureId), "booking.error.sold_out");
        else if (Adults + Children > Tour.MaximumGroupSize)
            AddError(nameof(Children), "booking.error.group_limit", Tour.MaximumGroupSize);
        else if (Adults + Children > bookings.RemainingSeats(departure.Id))
            AddError(nameof(Children), "booking.error.seats_limit", bookings.RemainingSeats(departure.Id));

        if (!ModelState.IsValid)
            return Page();

        var bookingNumber = await bookings.CreateBookingAsync(
            departure!.Id,
            Adults,
            Children,
            FirstName!,
            LastName!,
            Email!,
            Phone!,
            Citizenship!,
            Messenger!,
            Language,
            Consent,
            Comment,
            cancellationToken);
        if (bookingNumber is null)
        {
            AddError(nameof(DepartureId), "booking.error.sold_out");
            return Page();
        }

        return RedirectToPage("/Tours/BookingConfirmed", new { lang = Language, slug = Tour.Slug, bookingNumber });
    }

    public int RemainingSeats(string departureId) => bookings.RemainingSeats(departureId);

    public string FormatPrice(decimal amount)
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

    private bool SetPageData(string? language, string slug)
    {
        if (language is not ("ky" or "ru" or "en" or "es" or "fr" or "zh"))
            return false;

        var tour = catalog.Find(slug, language);
        if (tour is null)
            return false;

        Language = language;
        Tour = tour;
        Departures = catalog.DeparturesFor(slug, language);
        return true;
    }

    private void ValidateForm()
    {
        ValidateIntegerBinding(nameof(Adults), "booking.error.adults");
        ValidateIntegerBinding(nameof(Children), "booking.error.children");
        if (Adults is < 1 or > 20)
            AddError(nameof(Adults), "booking.error.adults");
        if (Children is < 0 or > 20)
            AddError(nameof(Children), "booking.error.children");
        if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Trim().Length > 80)
            AddError(nameof(FirstName), "booking.error.first_name");
        if (string.IsNullOrWhiteSpace(LastName) || LastName.Trim().Length > 80)
            AddError(nameof(LastName), "booking.error.last_name");
        var trimmedEmail = Email?.Trim();
        if (trimmedEmail is null || trimmedEmail.Length > 254
            || !MailAddress.TryCreate(trimmedEmail, out var address) || address.Address != trimmedEmail)
            AddError(nameof(Email), "booking.error.email");

        var phoneDigits = new string((Phone ?? "").Where(char.IsDigit).ToArray());
        if (Phone is null || Phone.Length > 24
            || !Regex.IsMatch(Phone, @"^\+[0-9\s().-]{7,20}$", RegexOptions.CultureInvariant)
            || phoneDigits.Length is < 8 or > 15)
            AddError(nameof(Phone), "booking.error.phone");
        if (string.IsNullOrWhiteSpace(Citizenship) || Citizenship.Trim().Length > 80)
            AddError(nameof(Citizenship), "booking.error.citizenship");
        if (!MessengerOptions.Contains(Messenger))
            AddError(nameof(Messenger), "booking.error.messenger");
        if (Comment?.Length > 1000)
            AddError(nameof(Comment), "booking.error.comment");
        if (!Consent)
            AddError(nameof(Consent), "booking.error.consent");
    }

    private void AddError(string field, string key, params object?[] arguments) =>
        ModelState.AddModelError(field, text.Get(key, Language, arguments));

    private void ValidateIntegerBinding(string field, string errorKey)
    {
        if (ModelState.TryGetValue(field, out var value) && value.Errors.Count > 0)
        {
            ModelState.Remove(field);
            AddError(field, errorKey);
        }
    }
}
