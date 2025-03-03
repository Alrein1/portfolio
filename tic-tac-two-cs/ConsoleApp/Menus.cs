using DAL;
using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    //private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryJson();

    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;

    public static void Init(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    public static readonly Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = () => GameController.MainLoop(_configRepository, _gameRepository)
            },
            new MenuItem()
            {
                Shortcut = "S",
                Title = "Saved games",
                MenuItemAction = () => SavedGamesController.MainLoop(_configRepository, _gameRepository)
            },
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Configuration",
                MenuItemAction = () => ConfigsController.MainLoop(_configRepository)
            },
        ]);
}