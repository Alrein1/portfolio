using DAL;
using Microsoft.EntityFrameworkCore;
using Web.Hubs;
using Web.Services;


var builder = WebApplication.CreateBuilder(args);
const EStorageType storageType = EStorageType.Sqlite;
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
if (storageType == EStorageType.Sqlite)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = $"Data Source={FileHelper.BasePath}app.db";
        Console.WriteLine($"Using connection string: {connectionString}");
        options.UseSqlite(connectionString);
    });
    
    builder.Services.AddScoped<IConfigRepository, ConfigRepositoryDb>();
    builder.Services.AddScoped<IGameRepository, GameRepositoryDb>();
}
else // Json storage
{
    builder.Services.AddSingleton<IConfigRepository, ConfigRepositoryJson>();
    builder.Services.AddSingleton<IGameRepository, GameRepositoryJson>();
}

builder.Services.AddSingleton<IActiveGamesManager, ActiveGamesManager>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddAntiforgery(options => 
{
    options.HeaderName = "RequestVerificationToken";
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = $"Data Source={FileHelper.BasePath}app.db";
    Console.WriteLine($"Using connection string: {connectionString}"); // Debug info
    options.UseSqlite(connectionString);
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
Console.WriteLine($"{FileHelper.BasePath}");
Console.WriteLine($"Web Root Path: {builder.Environment.WebRootPath}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.MapHub<GameHub>("/gameHub");
app.MapRazorPages();

app.Run();