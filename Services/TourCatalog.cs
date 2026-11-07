using TengriTrails.Models;

namespace TengriTrails.Services;

public sealed class TourCatalog
{
    public IReadOnlyList<Tour> All { get; } =
    [
        new("ala-kul", "tour.ala_kul", "region.issyk_kul", "tour.ala_kul_desc",
            "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1400&q=85",
            48900, 5, new DateTime(2027, 6, 12), "trekking", 3,
            Images("photo-1500530855697-b586d89ba3ee", "photo-1470770841072-f978cf4d019e", "photo-1464822759023-fed622ff2c3b"),
            Days("ala_kul", 5), Included("guide", "lodging", "meals", "transfers"), Excluded("flights", "insurance", "personal"), Packing("boots", "layers", "raincoat", "sunscreen"), 4, 8),
        new("song-kol", "tour.song_kol", "region.naryn", "tour.song_kol_desc",
            "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?auto=format&fit=crop&w=1400&q=85",
            36900, 4, new DateTime(2027, 6, 20), "equestrian", 2,
            Images("photo-1464822759023-fed622ff2c3b", "photo-1500534623283-312aade485b7", "photo-1500530855697-b586d89ba3ee"),
            Days("song_kol", 4), Included("guide", "lodging", "meals", "horse"), Excluded("flights", "insurance", "personal"), Packing("layers", "sunhat", "sunglasses", "personal_medication"), 2, 8),
        new("alpine-lakes", "tour.alpine_lakes", "region.issyk_kul", "tour.alpine_lakes_desc",
            "https://images.unsplash.com/photo-1470770841072-f978cf4d019e?auto=format&fit=crop&w=1400&q=85",
            62900, 6, new DateTime(2027, 7, 3), "lakes", 3,
            Images("photo-1470770841072-f978cf4d019e", "photo-1500530855697-b586d89ba3ee", "photo-1464278533981-50106e6176b1"),
            Days("alpine_lakes", 6), Included("guide", "lodging", "meals", "transfers"), Excluded("flights", "insurance", "personal"), Packing("boots", "layers", "raincoat", "water_bottle"), 4, 8),
        new("jyrgalan", "tour.jyrgalan", "region.issyk_kul", "tour.jyrgalan_desc",
            "https://images.unsplash.com/photo-1464278533981-50106e6176b1?auto=format&fit=crop&w=1400&q=85",
            32900, 3, new DateTime(2027, 6, 27), "cultural", 1,
            Images("photo-1464278533981-50106e6176b1", "photo-1500530855697-b586d89ba3ee", "photo-1470770841072-f978cf4d019e"),
            Days("jyrgalan", 3), Included("guide", "lodging", "meals"), Excluded("flights", "insurance", "personal"), Packing("walking_shoes", "layers", "sunhat"), 2, 10),
        new("alaysky", "tour.alaysky", "region.osh", "tour.alaysky_desc",
            "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?auto=format&fit=crop&w=1400&q=85",
            74900, 7, new DateTime(2027, 7, 11), "road", 4,
            Images("photo-1464822759023-fed622ff2c3b", "photo-1470770841072-f978cf4d019e", "photo-1500534623283-312aade485b7"),
            Days("alaysky", 7), Included("guide", "lodging", "meals", "vehicle"), Excluded("flights", "insurance", "personal"), Packing("layers", "sunscreen", "sunglasses", "personal_medication"), 4, 8),
        new("kel-suu", "tour.kel_suu", "region.naryn", "tour.kel_suu_desc",
            "https://images.unsplash.com/photo-1500534623283-312aade485b7?auto=format&fit=crop&w=1400&q=85",
            55900, 5, new DateTime(2027, 7, 18), "lakes", 2,
            Images("photo-1500534623283-312aade485b7", "photo-1500530855697-b586d89ba3ee", "photo-1470770841072-f978cf4d019e"),
            Days("kel_suu", 5), Included("guide", "lodging", "meals", "transfers"), Excluded("flights", "insurance", "personal"), Packing("boots", "layers", "raincoat", "personal_medication"), 2, 8)
    ];

    public IReadOnlyList<TourDeparture> Departures { get; } =
    [
        new("ala-kul-2027-06-12", "ala-kul", new DateTime(2027, 6, 12), "08:00", "meeting.ala_kul", 8),
        new("ala-kul-2027-07-10", "ala-kul", new DateTime(2027, 7, 10), "08:00", "meeting.ala_kul", 3),
        new("song-kol-2027-06-20", "song-kol", new DateTime(2027, 6, 20), "07:30", "meeting.song_kol", 8),
        new("song-kol-2027-07-15", "song-kol", new DateTime(2027, 7, 15), "07:30", "meeting.song_kol", 4),
        new("alpine-lakes-2027-07-03", "alpine-lakes", new DateTime(2027, 7, 3), "08:00", "meeting.alpine_lakes", 8),
        new("alpine-lakes-2027-08-02", "alpine-lakes", new DateTime(2027, 8, 2), "08:00", "meeting.alpine_lakes", 5),
        new("jyrgalan-2027-06-27", "jyrgalan", new DateTime(2027, 6, 27), "09:00", "meeting.jyrgalan", 8),
        new("jyrgalan-2027-07-25", "jyrgalan", new DateTime(2027, 7, 25), "09:00", "meeting.jyrgalan", 4),
        new("alaysky-2027-07-11", "alaysky", new DateTime(2027, 7, 11), "07:00", "meeting.alaysky", 6),
        new("alaysky-2027-08-12", "alaysky", new DateTime(2027, 8, 12), "07:00", "meeting.alaysky", 0),
        new("kel-suu-2027-07-18", "kel-suu", new DateTime(2027, 7, 18), "06:30", "meeting.kel_suu", 0),
        new("kel-suu-2027-08-18", "kel-suu", new DateTime(2027, 8, 18), "06:30", "meeting.kel_suu", 5)
    ];

    public Tour? Find(string slug) => All.FirstOrDefault(tour => tour.Slug == slug);

    public IReadOnlyList<TourDeparture> DeparturesFor(string slug) =>
        Departures.Where(departure => departure.TourSlug == slug).OrderBy(departure => departure.Date).ToArray();

    private static IReadOnlyList<string> Images(params string[] ids) =>
        ids.Select(id => $"https://images.unsplash.com/{id}?auto=format&fit=crop&w=1800&q=90").ToArray();

    private static IReadOnlyList<TourDay> Days(string slug, int count) =>
        Enumerable.Range(1, count)
            .Select(day => new TourDay(day, day <= 3
                ? $"itinerary.{slug}.stage{day}"
                : $"itinerary.{slug}.day{day}"))
            .ToArray();

    private static IReadOnlyList<string> Included(params string[] items) =>
        items.Select(item => $"included.{item}").ToArray();

    private static IReadOnlyList<string> Excluded(params string[] items) =>
        items.Select(item => $"excluded.{item}").ToArray();

    private static IReadOnlyList<string> Packing(params string[] items) =>
        items.Select(item => $"packing.{item}").ToArray();
}
