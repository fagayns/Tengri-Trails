using TengriTrails.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TengriTrails.Database;

namespace TengriTrails.Services;

public sealed class TourCatalog(TengriDbContext db)
{
    public IReadOnlyList<CatalogTour> All(string language)
    {
        var nowUtc = DateTime.UtcNow;
        return db.Tours.AsNoTracking()
            .Where(tour => tour.IsPublished)
            .Include(tour => tour.Region)
            .Include(tour => tour.Translations)
            .Include(tour => tour.Photos)
            .Include(tour => tour.Departures.Where(departure => departure.StartAtUtc >= nowUtc))
            .AsEnumerable()
            .Select(tour => Map(tour, language))
            .Where(tour => tour is not null)
            .Cast<CatalogTour>()
            .ToArray();
    }

    public CatalogTour? Find(string slug, string language) =>
        All(language).FirstOrDefault(tour => tour.Slug == slug);

    public IReadOnlyList<TourDeparture> DeparturesFor(string slug, string language)
    {
        var nowUtc = DateTime.UtcNow;
        var departures = db.Departures.AsNoTracking()
            .Include(departure => departure.Tour)
            .ThenInclude(tour => tour.Translations)
            .Where(departure => departure.Tour.IsPublished
                && departure.StartAtUtc >= nowUtc
                && departure.Tour.Translations.Any(translation =>
                    translation.Language == language && translation.Slug == slug))
            .OrderBy(departure => departure.StartAtUtc)
            .ToList();

        return departures.Select(departure =>
        {
            var localTime = DateTime.SpecifyKind(departure.StartAtUtc, DateTimeKind.Utc).AddHours(6);
            return new TourDeparture(
                departure.Id,
                slug,
                localTime.Date,
                localTime.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                departure.MeetingPlace,
                departure.SeatsTotal - departure.SeatsTaken,
                departure.PricePerPersonKgs);
        }).ToArray();
    }

    private static CatalogTour? Map(TourEntity entity, string language)
    {
        var translation = entity.Translations.FirstOrDefault(item => item.Language == language);
        if (translation is null)
            return null;

        var cover = entity.Photos.OrderBy(photo => photo.SortOrder).FirstOrDefault(photo => photo.IsCover)
            ?? entity.Photos.OrderBy(photo => photo.SortOrder).FirstOrDefault();
        if (cover is null)
            throw new InvalidDataException($"Published tour {translation.Slug} has no cover photo.");

        var firstDeparture = entity.Departures.OrderBy(departure => departure.StartAtUtc).FirstOrDefault();
        var departureDate = firstDeparture is null
            ? DateTime.MaxValue
            : DateTime.SpecifyKind(firstDeparture.StartAtUtc, DateTimeKind.Utc).AddHours(6).Date;
        var regionCode = entity.Region.Code;
        return new CatalogTour(
            translation.Slug,
            translation.Name,
            regionCode,
            RegionName(entity.Region, language),
            $"region.{regionCode.Replace('-', '_')}",
            translation.FullDescription,
            cover.FilePath,
            entity.PriceFromKgs,
            entity.DurationDays,
            departureDate,
            entity.Type,
            entity.Difficulty,
            entity.Photos.OrderBy(photo => photo.SortOrder).Select(photo => photo.FilePath).ToArray(),
            JsonSerializer.Deserialize<string[]>(translation.ItineraryJson)!.Select((text, index) => new TourDay(index + 1, text)).ToArray(),
            JsonSerializer.Deserialize<string[]>(translation.IncludedJson)!,
            JsonSerializer.Deserialize<string[]>(translation.ExcludedJson)!,
            JsonSerializer.Deserialize<string[]>(translation.PackingJson)!,
            entity.MinimumGroupSize,
            entity.MaximumGroupSize);
    }

    private static string RegionName(RegionEntity region, string language) => language switch
    {
        "ky" => region.NameKy,
        "ru" => region.NameRu,
        "en" => region.NameEn,
        "es" => region.NameEs,
        "fr" => region.NameFr,
        "zh" => region.NameZh,
        _ => throw new ArgumentOutOfRangeException(nameof(language))
    };
}
