using DAL;
using GameBrain;
using MenuSystem;

public static class ConfigsController
{
    private static IConfigRepository _configRepository = default!;
    
    public static string MainLoop(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
        
        var menuItems = GetConfigurations();
        
        menuItems.Add(new MenuItem()
        {
            Title = "Create new configuration",
            Shortcut = "C",
            MenuItemAction = CreateConfiguration
        });
        
        var menu = new Menu(
            EMenuLevel.Secondary,
            "TIC-TAC-TWO - Configuration Options", 
            menuItems,
            true
        );
        
        return menu.Run();
    }

    private static List<MenuItem> GetConfigurations()
    {
        var res = new List<MenuItem>();
        
        for (var i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var configName = _configRepository.GetConfigurationNames()[i];
            
            res.Add(new MenuItem()
            {
                Title = configName,
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => EditConfig(configName)
            });
        }

        return res;
    }

    private static string EditConfig(string configName)
    {
        var config = _configRepository.GetConfigurationByName(configName);
        if (config == null)
        {
            Console.WriteLine($"Configuration '{configName}' not found.");
            return "";
        }

        var menuItems = new List<MenuItem>
        {
            new()
            {
                Title = "Edit configuration",
                Shortcut = "E",
                MenuItemAction = () => EditConfigValues(config)
            },
            new()
            {
                Title = "Delete configuration",
                Shortcut = "D",
                MenuItemAction = () => DeleteConfig(config.Name)
            }
        };

        var menu = new Menu(
            EMenuLevel.Deep,
            $"Editing configuration: {configName}",
            menuItems,
            true
        );

        return menu.Run();
    }

    private static string EditConfigValues(GameConfiguration config)
    {
        Console.WriteLine($"Editing configuration: {config.Name}");
        Console.WriteLine("Press Enter to keep current value, or input new value.");

        do
        {
            Console.WriteLine($"Current Board Width: {config.BoardSizeWidth}");
            var dimX = ConsoleUI.Helpers.InputInt("New Board Width (3-20)", config.BoardSizeWidth, 3, 20);
            if (dimX == null) return "";

            Console.WriteLine($"Current Board Height: {config.BoardSizeHeight}");
            var dimY = ConsoleUI.Helpers.InputInt("New Board Height (3-20)", config.BoardSizeHeight, 3, 20);
            if (dimY == null) return "";

            var maxWin = Math.Min(dimX.Value, dimY.Value);

            Console.WriteLine($"Current Win Condition: {config.WinCondition}");
            var winCondition = ConsoleUI.Helpers.InputInt("New Win Condition", config.WinCondition, 3, maxWin);
            if (winCondition == null) return "";

            Console.WriteLine($"Current Move Piece After Moves: {config.MovePieceAfterNMoves}");
            var movePieceAfterNMoves = ConsoleUI.Helpers.InputInt("New Move Piece After X Moves", 
                config.MovePieceAfterNMoves, 0);
            if (movePieceAfterNMoves == null) return "";

            try
            {
                var updatedConfig = new GameConfiguration
                {
                    Name = config.Name,
                    BoardSizeWidth = dimX.Value,
                    BoardSizeHeight = dimY.Value,
                    WinCondition = winCondition.Value,
                    MovePieceAfterNMoves = movePieceAfterNMoves.Value
                };

                _configRepository.UpdateConfiguration(updatedConfig);
                Console.WriteLine("Configuration updated successfully.");
                return "R";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating configuration: {ex.Message}");
                return "";
            }
        } while (true);
    }

    private static string DeleteConfig(string configName)
    {
        Console.WriteLine($"Are you sure you want to delete configuration '{configName}'? (y/N)");
        var response = Console.ReadLine()?.ToLower();
        
        if (response != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            return "";
        }

        try
        {
            _configRepository.DeleteConfiguration(configName);
            Console.WriteLine("Configuration deleted successfully.");
            return "R";
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Cannot delete configuration: {ex.Message}");
            return "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting configuration: {ex.Message}");
            return "";
        }
    }

    private static string CreateConfiguration()
    {
        do
        {
            var dimX = ConsoleUI.Helpers.InputInt("Board Width", null, 3, 20);
            if (dimX == null) return "";
            
            var dimY = ConsoleUI.Helpers.InputInt("Board Height", null, 3, 20);
            if (dimY == null) return "";

            var maxWin = Math.Min(dimX.Value, dimY.Value);

            var winCondition = ConsoleUI.Helpers.InputInt("Win Condition", null, 3, maxWin);
            if (winCondition == null) return "";
            
            var movePieceAfterNMoves = ConsoleUI.Helpers.InputInt("Move Piece After X Moves", 0, 0);
            if (movePieceAfterNMoves == null) return "";

            var confName = ConsoleUI.Helpers.InputString("Configuration Name", 1, 32);
            if (confName == null) return "";

            if (_configRepository.GetConfigurationNames().Contains(confName))
            {
                Console.WriteLine("A configuration with this name already exists.");
                continue;
            }
            
            var config = new GameConfiguration()
            {
                Name = confName,
                BoardSizeWidth = dimX.Value,
                BoardSizeHeight = dimY.Value,
                WinCondition = winCondition.Value,
                MovePieceAfterNMoves = movePieceAfterNMoves.Value
            };
            
            _configRepository.SaveConfiguration(config);
            Console.WriteLine("Configuration created successfully.");
            
            return "R";
        } while (true);
    }
}