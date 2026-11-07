using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TengriTrails.Models;

namespace TengriTrails.Pages.Regions;

public sealed class IndexModel : PageModel
{
    public string Language { get; private set; } = "en";

    public IReadOnlyList<RegionCard> Regions { get; } =
    [
        Region("bishkek", "photo-1519501025264-65ba15a82390"),
        Region("chuy", "photo-1464822759023-fed622ff2c3b"),
        Region("issyk_kul", "photo-1500530855697-b586d89ba3ee"),
        Region("naryn", "photo-1470770841072-f978cf4d019e"),
        Region("talas", "photo-1464278533981-50106e6176b1"),
        Region("jalal_abad", "photo-1500534623283-312aade485b7"),
        Region("osh", "photo-1519681393784-d120267933ba"),
        Region("batken", "photo-1464822759023-fed622ff2c3b")
    ];

    public IActionResult OnGet(string lang)
    {
        if (lang is not ("ky" or "ru" or "en" or "es" or "fr" or "zh"))
            return NotFound();

        Language = lang;
        return Page();
    }

    private static RegionCard Region(string slug, string photoId) =>
        new(slug.Replace('_', '-'), $"regions.card.{slug}", $"region.{slug}.copy",
            $"https://images.unsplash.com/{photoId}?auto=format&fit=crop&w=1200&q=85");
}
