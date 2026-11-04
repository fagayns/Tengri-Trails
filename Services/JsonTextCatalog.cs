using System.Text.Json;

namespace TengriTrails.Services;

public sealed class JsonTextCatalog : ITextCatalog
{
    private static readonly HashSet<string> Languages = ["ky", "ru", "en", "es", "fr", "zh"];
    private readonly Dictionary<string, Dictionary<string, string>> _translations;

    public JsonTextCatalog(IWebHostEnvironment environment)
    {
        _translations = [];
        foreach (var language in Languages)
        {
            var path = Path.Combine(environment.ContentRootPath, "Localization", $"{language}.json");
            var json = File.ReadAllText(path);
            _translations[language] = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? throw new InvalidDataException($"Translation file is empty: {path}");
        }
    }

    public string Get(string key, string language)
    {
        var selectedLanguage = Languages.Contains(language) ? language : "en";
        if (_translations[selectedLanguage].TryGetValue(key, out var value))
            return value;

        throw new KeyNotFoundException($"Translation key '{key}' is missing for '{selectedLanguage}'.");
    }

    public string Get(string key, string language, params object?[] arguments) =>
        string.Format(System.Globalization.CultureInfo.GetCultureInfo(CultureName(language)), Get(key, language), arguments);

    private static string CultureName(string language) => language switch
    {
        "ky" => "ky-KG",
        "ru" => "ru-KG",
        "en" => "en-US",
        "es" => "es-ES",
        "fr" => "fr-FR",
        "zh" => "zh-CN",
        _ => "en-US"
    };
}
