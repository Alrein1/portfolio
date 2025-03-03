using System.Text.Json;
using DAL;
using GameBrain;
using Microsoft.Extensions.Logging;

public class ConfigRepositoryJson : IConfigRepository
{
    private readonly string _configsDirectory;
    private readonly ILogger<ConfigRepositoryJson> _logger;
    private readonly Dictionary<string, GameConfiguration> _configCache;

    public ConfigRepositoryJson(ILogger<ConfigRepositoryJson> logger)
    {
        _configsDirectory = Path.Combine(".", "AppData", "Configs");
        _logger = logger;
        _configCache = new Dictionary<string, GameConfiguration>();
        
        // Ensure directory exists
        if (!Directory.Exists(_configsDirectory))
        {
            Directory.CreateDirectory(_configsDirectory);
            // Create default configuration if no configs exist
            CreateDefaultConfig();
        }
        
        // Load configs into cache
        LoadConfigs();
    }

    private void CreateDefaultConfig()
    {
        var defaultConfig = new GameConfiguration
        {
            ConfigId = 1, // First config gets ID 1
            Name = "default",
            BoardSizeWidth = 5,
            BoardSizeHeight = 5,
            WinCondition = 3,
            MovePieceAfterNMoves = 4
        };
        
        SaveConfiguration(defaultConfig);
    }

    private void LoadConfigs()
    {
        _configCache.Clear();
        foreach (var file in Directory.GetFiles(_configsDirectory, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var config = System.Text.Json.JsonSerializer.Deserialize<GameConfiguration>(json);
                if (config != null)
                {
                    _configCache[config.Name] = config;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading config: {file}");
            }
        }
    }

    public List<string> GetConfigurationNames()
    {
        return _configCache.Keys.OrderBy(name => name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        if (!_configCache.TryGetValue(name, out var config))
        {
            throw new ArgumentException($"Configuration not found: {name}");
        }
        return config;
    }

    public GameConfiguration GetConfigurationById(int id)
    {
        var config = _configCache.Values.FirstOrDefault(c => c.ConfigId == id);
        if (config == null)
        {
            throw new ArgumentException($"Configuration not found with ID: {id}");
        }
        return config;
    }

    public void DeleteConfiguration(string configName)
    {
        var filePath = Path.Combine(_configsDirectory, $"{configName}.json");
        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"Configuration file not found: {configName}");
        }

        try
        {
            File.Delete(filePath);
            _configCache.Remove(configName);
            _logger.LogInformation($"Configuration deleted: {configName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting configuration: {configName}");
            throw;
        }
    }

    public void UpdateConfiguration(GameConfiguration config)
    {
        if (!_configCache.ContainsKey(config.Name))
        {
            throw new ArgumentException($"Configuration not found: {config.Name}");
        }

        SaveConfiguration(config);
    }

    public void SaveConfiguration(GameConfiguration gameConfig)
    {
        // Assign new ID if it's a new config
        if (gameConfig.ConfigId == 0)
        {
            gameConfig.ConfigId = _configCache.Values
                .Select(c => c.ConfigId)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }

        var filePath = Path.Combine(_configsDirectory, $"{gameConfig.Name}.json");
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(gameConfig, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
            _configCache[gameConfig.Name] = gameConfig;
            _logger.LogInformation($"Configuration saved: {gameConfig.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving configuration: {gameConfig.Name}");
            throw;
        }
    }
}