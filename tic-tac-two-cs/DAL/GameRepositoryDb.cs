using Domain;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    private readonly DAL.AppDbContext _context;

    public GameRepositoryDb(AppDbContext context)
    {
        _context = context;
    }

    public int SaveGame(string jsonStateString, string gameConfigName)
    {
        var config = _context.Configs.First(c => c.ConfigName == gameConfigName);
        var game = new Game()
        {
            GameName = gameConfigName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
            GameState = jsonStateString,
            ConfigId = config.Id,
            Config = config
        };
        _context.Games.Add(game);
        _context.SaveChanges();
        return game.Id;
    }

    public string DeleteAll()
    {
        var allGames = _context.Games.ToList();
        _context.Games.RemoveRange(allGames);
        _context.SaveChanges();
        return "";
    }
    
    public List<string> GetSavedGames()
    {
        return _context.Games
            .OrderBy(g => g.GameName)
            .Select(g => g.GameName)
            .ToList();
    }
    public Game LoadGame(string gameName)
    {
        return _context.Games.First(g => g.GameName == gameName);
    }
}