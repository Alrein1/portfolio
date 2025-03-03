using System.Runtime.CompilerServices;
using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class SavedGamesController
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;
    
    public static string MainLoop(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;

        var menuItems = GetSavedGames();
        menuItems.Add(new MenuItem()
        {
            Title = $"Clear",
            Shortcut = "D",
            MenuItemAction = () => _gameRepository.DeleteAll()
        });
        if (menuItems.Count == 0)
        {
            Console.WriteLine("No saved games found!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return "";
        }

        var menu = new Menu(
            EMenuLevel.Secondary,
            "TIC-TAC-TWO - Saved Games",
            menuItems,
            true
        );

        return menu.Run();
    }

    private static List<MenuItem> GetSavedGames()
    {
        var res = new List<MenuItem>();
        var savedGames = _gameRepository.GetSavedGames();

        for (var i = 0; i < savedGames.Count; i++)
        {
            var gameName = savedGames[i];
            
            res.Add(new MenuItem()
            {
                Title = $"{savedGames[i]}",
                Shortcut = (i).ToString(),
                MenuItemAction = () => LoadGame(gameName)
            });
        }
        return res;
    }

    private static string LoadGame(string gameName)
    {
        var savedGame = _gameRepository.LoadGame(gameName);
        var config = _configRepository.GetConfigurationById(savedGame.ConfigId);
  

        // Create new game instance with the saved configuration
        var gameInstance = new TicTacTwoBrain(config);
        
        // Load the saved game state
        gameInstance.SetGameStateJson(savedGame.GameState);

        // Return to main game loop with loaded game
        return GameController.MainLoop(_configRepository, _gameRepository, gameInstance);
    }
}