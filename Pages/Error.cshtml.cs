using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TengriTrails.Pages;

public sealed class ErrorModel : PageModel
{
    public string Language { get; private set; } = "en";

    public void OnGet()
    {
        var remembered = Request.Cookies["tengri-lang"];
        if (remembered is "ky" or "ru" or "en" or "es" or "fr" or "zh")
            Language = remembered;
    }
}
