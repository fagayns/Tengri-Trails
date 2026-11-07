namespace TengriTrails.Models;

public sealed record Tour(
    string Slug,
    string NameKey,
    string RegionKey,
    string DescriptionKey,
    string ImageUrl,
    int Price,
    int Days,
    DateTime Departure,
    string Type,
    int Difficulty,
    IReadOnlyList<string> GalleryImageUrls,
    IReadOnlyList<TourDay> Itinerary,
    IReadOnlyList<string> IncludedKeys,
    IReadOnlyList<string> ExcludedKeys,
    IReadOnlyList<string> PackingKeys,
    int MinimumGroupSize,
    int MaximumGroupSize);

public sealed record TourDay(int Day, string DescriptionKey);

public sealed record TourDeparture(
    string Id,
    string TourSlug,
    DateTime Date,
    string Time,
    string MeetingPlaceKey,
    int Capacity);
