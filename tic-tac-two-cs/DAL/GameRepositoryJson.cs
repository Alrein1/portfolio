using System.Text.Json;
using DAL;
using Domain;
using Microsoft.Extensions.Logging;

public class GameRepositoryJson : IGameRepository
{
    private readonly string _gamesDirectory;
    private readonly ILogger<GameRepositoryJson> _logger;

    public GameRepositoryJson(ILogger<GameRepositoryJson> logger)
    {
        _gamesDirectory = Path.Combine(".", "AppData", "SavedGames");
        _logger = logger;
        
        // Ensure directory exists
        if (!Directory.Exists(_gamesDirectory))
        {
            Directory.CreateDirectory(_gamesDirectory);
        }
    }

    public List<string> GetSavedGames()
    {
        try
        {
            return Directory.GetFiles(_gamesDirectory, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList()!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting saved games");
            return new List<string>();
        }
    }

    public Game LoadGame(string gameName)
    {
        var filePath = Path.Combine(_gamesDirectory, $"{gameName}.json");
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"Save file not found: {gameName}");
            return null;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var savedGame = System.Text.Json.JsonSerializer.Deserialize<Game>(json);
            return savedGame;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading game: {gameName}");
            return null;
        }
    }

    public int SaveGame(string gameState, string gameName)
    {
        var filePath = Path.Combine(_gamesDirectory, $"{gameName}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.json");
        try
        {
            var savedGame = new Game
            {
                GameName = gameName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                GameState = gameState,
            };

            var json = System.Text.Json.JsonSerializer.Serialize(savedGame, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
            _logger.LogInformation($"Game saved: {gameName}");
            return savedGame.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving game: {gameName}");
            throw;
        }
    }

    public string DeleteAll()
    {
        foreach (var gameName in GetSavedGames())
        {
            DeleteSavedGame(gameName);
        }

        return "";
    }
    public void DeleteSavedGame(string gameName)
    {
        var filePath = Path.Combine(_gamesDirectory, $"{gameName}.json");
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"Save file not found: {gameName}");
            return;
        }

        try
        {
            File.Delete(filePath);
            _logger.LogInformation($"Game deleted: {gameName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting game: {gameName}");
            throw;
        }
    }
}