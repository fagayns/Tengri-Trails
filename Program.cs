using Microsoft.EntityFrameworkCore;
using TengriTrails.Database;
using TengriTrails.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ITextCatalog, JsonTextCatalog>();
builder.Services.AddSingleton<TourSeedCatalog>();
builder.Services.AddScoped<TourCatalog>();
builder.Services.AddScoped<BookingService>();

var databaseProvider = builder.Configuration["Database:Provider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString(databaseProvider)
    ?? throw new InvalidOperationException($"Connection string for database provider '{databaseProvider}' is missing.");

if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
{
    var sqlite = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
    if (!Path.IsPathRooted(sqlite.DataSource))
        sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);
    builder.Services.AddDbContext<TengriDbContext>(options => options.UseSqlite(sqlite.ToString()));
}
else if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<TengriDbContext>(options => options.UseNpgsql(connectionString));
}
else
{
    throw new InvalidOperationException($"Unsupported database provider '{databaseProvider}'. Use Sqlite or Postgres.");
}

builder.Services.AddHostedService<DatabaseInitializer>();
builder.Services.AddHostedService<BookingExpirationService>();

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
