using TengriTrails.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ITextCatalog, JsonTextCatalog>();
builder.Services.AddSingleton<TourCatalog>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", (HttpContext context) =>
{
    var supported = new[] { "ky", "ru", "en", "es", "fr", "zh" };
    var remembered = context.Request.Cookies["tengri-lang"];
    var browser = context.Request.Headers["Accept-Language"].ToString()
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(value => value.Split(';')[0].Split('-')[0].ToLowerInvariant())
        .FirstOrDefault(supported.Contains);
    var language = supported.Contains(remembered) ? remembered : browser ?? "en";
    return Results.Redirect($"/{language}");
});

app.MapRazorPages();
app.Run();
