using ConsoleApp;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

const EStorageType storageType = EStorageType.Sqlite;

IConfigRepository configRepository;
IGameRepository gameRepository;

if (storageType == EStorageType.Sqlite)
{
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    var connectionString = $"Data Source={FileHelper.BasePath}app.db";
    connectionString = connectionString.Replace("<%location%>", FileHelper.BasePath);
    optionsBuilder.UseSqlite(connectionString);

    var db = new AppDbContext(optionsBuilder.Options);
    db.Database.EnsureCreated();

    configRepository = new ConfigRepositoryDb(db);
    gameRepository = new GameRepositoryDb(db);
}
else // Json storage
{
    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    configRepository = new ConfigRepositoryJson(loggerFactory.CreateLogger<ConfigRepositoryJson>());
    gameRepository = new GameRepositoryJson(loggerFactory.CreateLogger<GameRepositoryJson>());
}

// menu configuration is in Menus.cs
Menus.Init(configRepository, gameRepository);
Menus.MainMenu.Run();

// here 'using var db' calls db.Dispose()
