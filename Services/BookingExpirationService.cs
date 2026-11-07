namespace TengriTrails.Services;

public sealed class BookingExpirationService(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingExpirationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var bookings = scope.ServiceProvider.GetRequiredService<BookingService>();
            var cancelled = await bookings.CancelExpiredBookingsAsync(stoppingToken);
            if (cancelled > 0)
                logger.LogInformation("Cancelled {BookingCount} expired booking reservations.", cancelled);
        }
    }
}
