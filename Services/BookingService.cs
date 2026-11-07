using TengriTrails.Models;

namespace TengriTrails.Services;

public sealed class BookingService(TourCatalog catalog)
{
    private readonly object _sync = new();
    private readonly Dictionary<string, int> _remainingSeats =
        catalog.Departures.ToDictionary(departure => departure.Id, departure => departure.Capacity);
    private readonly HashSet<string> _bookingNumbers = [];
    private int _lastSequence = 122;

    public int RemainingSeats(string departureId)
    {
        lock (_sync)
            return _remainingSeats.GetValueOrDefault(departureId);
    }

    public bool TryBook(string departureId, int guests, out string bookingNumber)
    {
        lock (_sync)
        {
            if (!_remainingSeats.TryGetValue(departureId, out var remaining) || guests > remaining)
            {
                bookingNumber = "";
                return false;
            }

            _remainingSeats[departureId] = remaining - guests;
            bookingNumber = $"KG-{DateTime.UtcNow.Year}-{++_lastSequence:D6}";
            _bookingNumbers.Add(bookingNumber);
            return true;
        }
    }

    public bool Exists(string bookingNumber)
    {
        lock (_sync)
            return _bookingNumbers.Contains(bookingNumber);
    }
}
