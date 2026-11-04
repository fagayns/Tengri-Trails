namespace TengriTrails.Services;

public interface ITextCatalog
{
    string Get(string key, string language);
    string Get(string key, string language, params object?[] arguments);
}
