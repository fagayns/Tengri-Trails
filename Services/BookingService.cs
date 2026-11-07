using Microsoft.EntityFrameworkCore;
using TengriTrails.Database;

namespace TengriTrails.Services;

public sealed class BookingService(TengriDbContext db)
{
    public int RemainingSeats(string departureId) =>
        db.Departures.AsNoTracking()
            .Where(departure => departure.Id == departureId)
            .Select(departure => departure.SeatsTotal - departure.SeatsTaken)
            .SingleOrDefault();

    public async Task<string?> CreateBookingAsync(
        string departureId,
        int adults,
        int children,
        string firstName,
        string lastName,
        string email,
        string phone,
        string citizenship,
        string messenger,
        string language,
        bool consent,
        string? comment,
        CancellationToken cancellationToken = default)
    {
        var guests = adults + children;
        var now = DateTime.UtcNow;
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var departure = await db.Departures.AsNoTracking()
            .Where(item => item.Id == departureId && item.StartAtUtc > now)
            .Select(item => new { item.Id, item.PricePerPersonKgs, item.Tour.MaximumGroupSize })
            .SingleOrDefaultAsync(cancellationToken);
        if (departure is null || guests > departure.MaximumGroupSize)
            return null;

        var reserved = await db.Departures
            .Where(item => item.Id == departureId && item.StartAtUtc > now
                && item.SeatsTaken + guests <= item.SeatsTotal)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(item => item.SeatsTaken, item => item.SeatsTaken + guests),
                cancellationToken);
        if (reserved == 0)
            return null;

        var booking = new BookingEntity
        {
            BookingNumber = $"PENDING-{Guid.NewGuid():N}",
            DepartureId = departureId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            Citizenship = citizenship.Trim(),
            Messenger = messenger,
            Adults = adults,
            Children = children,
            TotalAmountKgs = guests * departure.PricePerPersonKgs,
            RequestLanguage = language,
            Status = BookingStatus.New,
            ReservedUntilUtc = now.AddMinutes(30),
            PersonalDataConsent = consent,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        db.Bookings.Add(booking);
        await db.SaveChangesAsync(cancellationToken);

        booking.BookingNumber = $"KG-{now:yyyy}-{booking.Id:D6}";
        db.BookingStatusLogs.Add(new BookingStatusLogEntity
        {
            BookingId = booking.Id,
            OldStatus = null,
            NewStatus = BookingStatus.New,
            ChangedBy = "customer",
            ChangedAtUtc = now
        });
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return booking.BookingNumber;
    }

    public async Task<bool> ExistsAsync(string bookingNumber, CancellationToken cancellationToken = default) =>
        await db.Bookings.AsNoTracking()
            .AnyAsync(booking => booking.BookingNumber == bookingNumber, cancellationToken);

    public async Task<int> CancelExpiredBookingsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredBookings = await db.Bookings.AsNoTracking()
            .Where(booking => booking.Status == BookingStatus.New && booking.ReservedUntilUtc <= now)
            .Select(booking => new { booking.Id, booking.DepartureId, booking.Adults, booking.Children })
            .ToListAsync(cancellationToken);
        var cancelledCount = 0;

        foreach (var expired in expiredBookings)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var cancelled = await db.Bookings
                .Where(booking => booking.Id == expired.Id
                    && booking.Status == BookingStatus.New
                    && booking.ReservedUntilUtc <= now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(booking => booking.Status, BookingStatus.Cancelled)
                    .SetProperty(booking => booking.UpdatedAtUtc, now), cancellationToken);
            if (cancelled == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                continue;
            }

            var guests = expired.Adults + expired.Children;
            var released = await db.Departures
                .Where(departure => departure.Id == expired.DepartureId && departure.SeatsTaken >= guests)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(departure => departure.SeatsTaken, departure => departure.SeatsTaken - guests),
                    cancellationToken);
            if (released == 0)
                throw new InvalidOperationException($"Could not release reserved seats for expired booking {expired.Id}.");

            db.BookingStatusLogs.Add(new BookingStatusLogEntity
            {
                BookingId = expired.Id,
                OldStatus = BookingStatus.New,
                NewStatus = BookingStatus.Cancelled,
                ChangedBy = "reservation-expiration",
                ChangedAtUtc = now
            });
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            cancelledCount++;
        }

        return cancelledCount;
    }
}
