using TengriTrails.Models;

namespace TengriTrails.Services;

public sealed class TourSeedCatalog
{
    public IReadOnlyList<Tour> All { get; } =
    [
        new("song-kol-horseback", "tour.test_song_kol", "region.naryn", "tour.test_song_kol_desc",
            Image("song-kol-horseback.svg"), 26000, 3, new DateTime(2026, 11, 14), "equestrian", 2,
            Gallery("song-kol-horseback.svg"), Days(3, "song_kol"), Included("guide", "lodging", "meals", "horse"),
            Excluded("flights", "insurance", "personal"), Packing("layers", "sunhat", "sunglasses", "personal_medication"), 2, 8),
        new("ala-archa-day-hike", "tour.test_ala_archa", "region.chuy", "tour.test_ala_archa_desc",
            Image("ala-archa-gorge.svg"), 4500, 1, new DateTime(2026, 11, 15), "trekking", 1,
            Gallery("ala-archa-gorge.svg"), Days(1, "ala_archa"), Included("guide", "transfers"),
            Excluded("flights", "insurance", "personal"), Packing("walking_shoes", "layers", "water_bottle"), 2, 10),
        new("issyk-kul-road-trip", "tour.test_issyk_kul_road", "region.issyk_kul", "tour.test_issyk_kul_road_desc",
            Image("issyk-kul-shore.svg"), 14000, 2, new DateTime(2026, 11, 13), "road", 1,
            Gallery("issyk-kul-shore.svg"), Days(2, "issyk_kul_road"), Included("guide", "lodging", "transfers"),
            Excluded("flights", "insurance", "personal"), Packing("layers", "sunscreen", "sunglasses"), 2, 8),
        new("karakol-jeti-oguz", "tour.test_karakol_jeti_oguz", "region.issyk_kul", "tour.test_karakol_jeti_oguz_desc",
            Image("jeti-oguz-red-rocks.svg"), 21000, 3, new DateTime(2026, 11, 14), "trekking", 2,
            Gallery("jeti-oguz-red-rocks.svg"), Days(3, "karakol_jeti_oguz"), Included("guide", "lodging", "meals"),
            Excluded("flights", "insurance", "personal"), Packing("boots", "layers", "raincoat"), 2, 8),
        new("sary-chelek", "tour.test_sary_chelek", "region.jalal_abad", "tour.test_sary_chelek_desc",
            Image("sary-chelek-lake.svg"), 24000, 3, new DateTime(2026, 11, 15), "lakes", 1,
            Gallery("sary-chelek-lake.svg"), Days(3, "sary_chelek"), Included("guide", "lodging", "meals", "transfers"),
            Excluded("flights", "insurance", "personal"), Packing("walking_shoes", "layers", "water_bottle"), 2, 8),
        new("osh-city-day", "tour.test_osh_city", "region.osh", "tour.test_osh_city_desc",
            Image("osh-sulaiman-too.svg"), 5000, 1, new DateTime(2026, 11, 13), "cultural", 1,
            Gallery("osh-sulaiman-too.svg"), Days(1, "osh_city"), Included("guide"),
            Excluded("flights", "insurance", "personal"), Packing("walking_shoes", "sunhat", "water_bottle"), 2, 12)
    ];

    public IReadOnlyList<TourDeparture> Departures { get; } =
    [
        new("song-kol-horseback-2026-11-14", "song-kol-horseback", new DateTime(2026, 11, 14), "07:30", "meeting.song_kol", 8, 26000),
        new("song-kol-horseback-2026-11-21", "song-kol-horseback", new DateTime(2026, 11, 21), "07:30", "meeting.song_kol", 0, 26000),
        new("song-kol-horseback-2026-11-28", "song-kol-horseback", new DateTime(2026, 11, 28), "07:30", "meeting.song_kol", 4, 26000),
        new("ala-archa-day-hike-2026-11-15", "ala-archa-day-hike", new DateTime(2026, 11, 15), "08:00", "meeting.ala_archa", 8, 4500),
        new("ala-archa-day-hike-2026-11-22", "ala-archa-day-hike", new DateTime(2026, 11, 22), "08:00", "meeting.ala_archa", 0, 4500),
        new("ala-archa-day-hike-2026-11-29", "ala-archa-day-hike", new DateTime(2026, 11, 29), "08:00", "meeting.ala_archa", 6, 4500),
        new("issyk-kul-road-trip-2026-11-13", "issyk-kul-road-trip", new DateTime(2026, 11, 13), "08:00", "meeting.issyk_kul", 8, 14000),
        new("issyk-kul-road-trip-2026-11-20", "issyk-kul-road-trip", new DateTime(2026, 11, 20), "08:00", "meeting.issyk_kul", 0, 14000),
        new("issyk-kul-road-trip-2026-11-27", "issyk-kul-road-trip", new DateTime(2026, 11, 27), "08:00", "meeting.issyk_kul", 5, 14000),
        new("karakol-jeti-oguz-2026-11-14", "karakol-jeti-oguz", new DateTime(2026, 11, 14), "08:00", "meeting.karakol", 8, 21000),
        new("karakol-jeti-oguz-2026-11-21", "karakol-jeti-oguz", new DateTime(2026, 11, 21), "08:00", "meeting.karakol", 0, 21000),
        new("karakol-jeti-oguz-2026-11-28", "karakol-jeti-oguz", new DateTime(2026, 11, 28), "08:00", "meeting.karakol", 5, 21000),
        new("sary-chelek-2026-11-15", "sary-chelek", new DateTime(2026, 11, 15), "08:00", "meeting.sary_chelek", 8, 24000),
        new("sary-chelek-2026-11-22", "sary-chelek", new DateTime(2026, 11, 22), "08:00", "meeting.sary_chelek", 0, 24000),
        new("sary-chelek-2026-11-29", "sary-chelek", new DateTime(2026, 11, 29), "08:00", "meeting.sary_chelek", 4, 24000),
        new("osh-city-day-2026-11-13", "osh-city-day", new DateTime(2026, 11, 13), "09:00", "meeting.osh", 8, 5000),
        new("osh-city-day-2026-11-20", "osh-city-day", new DateTime(2026, 11, 20), "09:00", "meeting.osh", 0, 5000),
        new("osh-city-day-2026-11-27", "osh-city-day", new DateTime(2026, 11, 27), "09:00", "meeting.osh", 6, 5000)
    ];

    public Tour? Find(string slug) => All.FirstOrDefault(tour => tour.Slug == slug);

    private static string Image(string fileName) => $"/img/places/{fileName}";
    private static IReadOnlyList<string> Gallery(string fileName) => [Image(fileName)];
    private static IReadOnlyList<TourDay> Days(int count, string key) =>
        Enumerable.Range(1, count).Select(day => new TourDay(day, $"itinerary.test_{key}.day{day}")).ToArray();
    private static IReadOnlyList<string> Included(params string[] items) => items.Select(item => $"included.{item}").ToArray();
    private static IReadOnlyList<string> Excluded(params string[] items) => items.Select(item => $"excluded.{item}").ToArray();
    private static IReadOnlyList<string> Packing(params string[] items) => items.Select(item => $"packing.{item}").ToArray();
}
