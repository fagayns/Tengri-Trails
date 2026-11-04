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
    int Difficulty);
