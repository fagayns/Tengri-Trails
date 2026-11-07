using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TengriTrails.Services;

namespace TengriTrails.Database;

public sealed class DatabaseInitializer(
    IServiceScopeFactory scopeFactory,
    IWebHostEnvironment environment,
    ILogger<DatabaseInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TengriDbContext>();
        await db.Database.EnsureCreatedAsync(cancellationToken);

        if (await db.Regions.AnyAsync(cancellationToken))
            return;

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var text = scope.ServiceProvider.GetRequiredService<ITextCatalog>();
        var regionCodes = new[]
        {
            "bishkek", "chuy", "issyk-kul", "naryn", "talas", "jalal-abad", "osh", "batken"
        };
        var regions = regionCodes.ToDictionary(
            code => code,
            code => CreateRegion(code, text),
            StringComparer.Ordinal);
        db.Regions.AddRange(regions.Values);
        await db.SaveChangesAsync(cancellationToken);

        var seedCatalog = scope.ServiceProvider.GetRequiredService<TourSeedCatalog>();
        var toursBySlug = new Dictionary<string, TourEntity>(StringComparer.Ordinal);
        foreach (var source in seedCatalog.All)
        {
            var regionCode = source.RegionKey["region.".Length..].Replace('_', '-');
            var tour = new TourEntity
            {
                RegionId = regions[regionCode].Id,
                Type = source.Type,
                DurationDays = source.Days,
                Difficulty = source.Difficulty,
                PriceFromKgs = source.Price,
                MinimumGroupSize = source.MinimumGroupSize,
                MaximumGroupSize = source.MaximumGroupSize,
                IsPublished = true,
                Latitude = null,
                Longitude = null
            };
            foreach (var language in Languages)
            {
                var name = text.Get(source.NameKey, language);
                var fullDescription = text.Get(source.DescriptionKey, language);
                tour.Translations.Add(new TourTranslationEntity
                {
                    Language = language,
                    Name = name,
                    Slug = source.Slug,
                    ShortDescription = Shorten(fullDescription),
                    FullDescription = fullDescription,
                    ItineraryJson = JsonSerializer.Serialize(source.Itinerary.Select(day =>
                        text.Get(day.DescriptionKey, language))),
                    IncludedJson = JsonSerializer.Serialize(source.IncludedKeys.Select(key => text.Get(key, language))),
                    ExcludedJson = JsonSerializer.Serialize(source.ExcludedKeys.Select(key => text.Get(key, language))),
                    PackingJson = JsonSerializer.Serialize(source.PackingKeys.Select(key => text.Get(key, language)))
                });
            }

            var imagePath = source.ImageUrl;
            tour.Photos.Add(new TourPhotoEntity
            {
                FilePath = imagePath,
                ThumbnailPath = imagePath,
                SortOrder = 0,
                IsCover = true,
                Caption = text.Get(source.NameKey, "en")
            });
            toursBySlug.Add(source.Slug, tour);
        }

        db.Tours.AddRange(toursBySlug.Values);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var source in seedCatalog.Departures)
        {
            var tour = toursBySlug[source.TourSlug];
            var localTime = TimeSpan.Parse(source.Time, System.Globalization.CultureInfo.InvariantCulture);
            var bishkekStart = new DateTimeOffset(
                source.Date.Year, source.Date.Month, source.Date.Day,
                localTime.Hours, localTime.Minutes, 0, TimeSpan.FromHours(6));
            db.Departures.Add(new DepartureEntity
            {
                Id = source.Id,
                TourId = tour.Id,
                StartAtUtc = bishkekStart.UtcDateTime,
                MeetingPlace = source.MeetingPlaceKey,
                PricePerPersonKgs = source.PricePerPersonKgs,
                SeatsTotal = source.Capacity,
                SeatsTaken = 0,
                Guide = null
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation(
            "Seeded {RegionCount} regions and {TourCount} tours into {DatabasePath}.",
            regions.Count, toursBySlug.Count, GetDatabasePath());
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private string GetDatabasePath() =>
        Path.GetFullPath(Path.Combine(environment.ContentRootPath, "tengri.db"));

    private static RegionEntity CreateRegion(string code, ITextCatalog text)
    {
        string LocalizedName(string language) => text.Get($"region.{code.Replace('-', '_')}", language);
        return new RegionEntity
        {
            Code = code,
            NameKy = LocalizedName("ky"),
            NameRu = LocalizedName("ru"),
            NameEn = LocalizedName("en"),
            NameEs = LocalizedName("es"),
            NameFr = LocalizedName("fr"),
            NameZh = LocalizedName("zh")
        };
    }

    private static string Shorten(string value) =>
        value.Length <= 180 ? value : string.Concat(value.AsSpan(0, 177), "...");

    private static readonly string[] Languages = ["ky", "ru", "en", "es", "fr", "zh"];
}
