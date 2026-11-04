using TengriTrails.Models;

namespace TengriTrails.Services;

public sealed class TourCatalog
{
    public IReadOnlyList<Tour> All { get; } =
    [
        new("ala-kul", "tour.ala_kul", "region.issyk_kul", "tour.ala_kul_desc",
            "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1400&q=85",
            48900, 5, new DateTime(2027, 6, 12), "trekking", 3),
        new("song-kol", "tour.song_kol", "region.naryn", "tour.song_kol_desc",
            "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?auto=format&fit=crop&w=1400&q=85",
            36900, 4, new DateTime(2027, 6, 20), "equestrian", 2),
        new("alpine-lakes", "tour.alpine_lakes", "region.issyk_kul", "tour.alpine_lakes_desc",
            "https://images.unsplash.com/photo-1470770841072-f978cf4d019e?auto=format&fit=crop&w=1400&q=85",
            62900, 6, new DateTime(2027, 7, 3), "lakes", 3),
        new("jyrgalan", "tour.jyrgalan", "region.issyk_kul", "tour.jyrgalan_desc",
            "https://images.unsplash.com/photo-1464278533981-50106e6176b1?auto=format&fit=crop&w=1400&q=85",
            32900, 3, new DateTime(2027, 6, 27), "cultural", 1),
        new("alaysky", "tour.alaysky", "region.osh", "tour.alaysky_desc",
            "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?auto=format&fit=crop&w=1400&q=85",
            74900, 7, new DateTime(2027, 7, 11), "road", 4),
        new("kel-suu", "tour.kel_suu", "region.naryn", "tour.kel_suu_desc",
            "https://images.unsplash.com/photo-1500534623283-312aade485b7?auto=format&fit=crop&w=1400&q=85",
            55900, 5, new DateTime(2027, 7, 18), "lakes", 2)
    ];

    public Tour? Find(string slug) => All.FirstOrDefault(tour => tour.Slug == slug);
}
