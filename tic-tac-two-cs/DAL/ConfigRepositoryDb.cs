using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    private readonly DAL.AppDbContext _context;

    public ConfigRepositoryDb(AppDbContext context)
    {
        _context = context;
    }

    public List<string> GetConfigurationNames()
    {
        return _context.Configs
            .OrderBy(c => c.ConfigName)
            .Select(c => c.ConfigName)
            .ToList();
    }
    
    public GameConfiguration GetConfigurationByName(string name)
    {
        var data = _context.Configs.First(c => c.ConfigName == name);
        var res = new GameConfiguration()
        {
            Name = data.ConfigName,
            BoardSizeWidth = data.BoardSizeWidth,
            BoardSizeHeight = data.BoardSizeHeight,
            WinCondition = data.WinCondition,
            MovePieceAfterNMoves = data.MovePieceAfterNMoves
        };

        return res;
    }
    public GameConfiguration GetConfigurationById(int id)
    {
        var data = _context.Configs.First(c => c.Id == id);
        var res = new GameConfiguration()
        {
            Name = data.ConfigName,
            BoardSizeWidth = data.BoardSizeWidth,
            BoardSizeHeight = data.BoardSizeHeight,
            WinCondition = data.WinCondition,
            MovePieceAfterNMoves = data.MovePieceAfterNMoves
        };

        return res;
    }

    public void SaveConfiguration(GameConfiguration gameConfig)
    {
        Config conf = new Config
        {
            ConfigName = gameConfig.Name,
            WinCondition = gameConfig.WinCondition,
            BoardSizeHeight = gameConfig.BoardSizeHeight,
            BoardSizeWidth = gameConfig.BoardSizeWidth,
            MovePieceAfterNMoves = gameConfig.MovePieceAfterNMoves
        };

        _context.Configs.Add(conf);
        _context.SaveChanges();
    }
    public void DeleteConfiguration(string configName)
    {
        var config = _context.Configs.FirstOrDefault(c => c.ConfigName == configName);
        if (config == null)
        {
            throw new ArgumentException($"Configuration '{configName}' not found.");
        }

        // Check if there are any saved games using this configuration
        var hasGames = _context.Games.Any(g => g.ConfigId == config.Id);
        if (hasGames)
        {
            throw new InvalidOperationException("Cannot delete configuration that has saved games.");
        }

        _context.Configs.Remove(config);
        _context.SaveChanges();
    }

    public void UpdateConfiguration(GameConfiguration config)
    {
        var existingConfig = _context.Configs.FirstOrDefault(c => c.ConfigName == config.Name);
        if (existingConfig == null)
        {
            throw new ArgumentException($"Configuration '{config.Name}' not found.");
        }

        // Update the existing configuration
        existingConfig.BoardSizeWidth = config.BoardSizeWidth;
        existingConfig.BoardSizeHeight = config.BoardSizeHeight;
        existingConfig.WinCondition = config.WinCondition;
        existingConfig.MovePieceAfterNMoves = config.MovePieceAfterNMoves;

        _context.SaveChanges();
    }

    
}