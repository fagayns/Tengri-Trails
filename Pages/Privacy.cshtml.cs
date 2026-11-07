using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TengriTrails.Pages;

public sealed class PrivacyModel : PageModel
{
    public string Language { get; private set; } = "en";

    public IActionResult OnGet(string lang)
    {
        if (lang is not ("ky" or "ru" or "en" or "es" or "fr" or "zh"))
            return NotFound();

        Language = lang;
        return Page();
    }
}
