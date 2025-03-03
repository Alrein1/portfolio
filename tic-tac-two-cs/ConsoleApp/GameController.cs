using DAL;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;

    public static string MainLoop(IConfigRepository configRepository, IGameRepository gameRepository, TicTacTwoBrain? instance = null)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
        TicTacTwoBrain gameInstance;
        if (instance != null)
        {
            gameInstance = instance;
        }
        else
        {
            var chosenConfigShortcut = ChooseConfiguration();

            if (!int.TryParse(chosenConfigShortcut, out var configNo))
            {
                return chosenConfigShortcut;
            }

            var chosenConfig = _configRepository.GetConfigurationByName(
                _configRepository.GetConfigurationNames()[configNo]
            );

            var chosenGameMode = ChooseGameMode();
            switch (chosenGameMode)
            {
                case("PlayerVsPlayer"):
                    gameInstance = new TicTacTwoBrain(chosenConfig, GameMode.PlayerVsPlayer);
                    break;
                case("PlayerVsAI"):
                    gameInstance = new TicTacTwoBrain(chosenConfig, GameMode.PlayerVsAI);
                    break;
                case("AIVsAI"):
                    gameInstance = new TicTacTwoBrain(chosenConfig, GameMode.AIVsAI);
                    break;
                default:
                    gameInstance = new TicTacTwoBrain(chosenConfig);
                    break;
            }
        }
        

        do
        {
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            GameStatus? gameStatus = null;

            if (!gameInstance.IsAITurn())
            {
                Console.Write(
                    $"Player {ConsoleUI.Visualizer.GamePieceToString(gameInstance.GetNextMoveBy())}, choose your action (Place, MovePiece, MoveGrid) or Save/Quit: ");
                var input = Console.ReadLine()!;

                if (input.StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                {

                    _gameRepository.SaveGame(
                        gameInstance.GetGameStateJson(),
                        gameInstance.GetGameConfigName());
                    Console.WriteLine("Game saved successfully!");
                    continue;
                }

                if (input.StartsWith("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                var action = input.Split(' ')[0].ToLower();
                var actionInput = input.Length > action.Length + 1 ? input.Substring(action.Length + 1) : "";
                switch (action)
                {
                case "place":
                    gameStatus = HandlePlaceAction(actionInput, gameInstance);
                    break;

                case "movepiece":
                    gameStatus = HandleMovePieceAction(actionInput, gameInstance);
                    break;

                case "movegrid":
                    gameStatus = HandleMoveGridAction(actionInput, gameInstance);
                    break;

                default:
                    Console.WriteLine("Invalid action. Use Place, MovePiece, or MoveGrid.");
                    break;
                }
            }
            else
            {
                Console.WriteLine($"AI is thinking...");
                Thread.Sleep(1000);
                gameStatus = gameInstance.MakeAIMove();
            }
            
            if (gameStatus == GameStatus.Ongoing) continue;
            if (gameStatus != null)
            {
                Console.WriteLine($"Game over! Status: {gameStatus}");
                break;
            }

        } while (true);

        return "";
    }

    private static GameStatus? HandleAIMove(TicTacTwoBrain gameInstance)
    {
        return gameInstance.MakeAIMove();
    }
    private static GameStatus? HandlePlaceAction(string input, TicTacTwoBrain gameInstance)
    {
        var coordinates = ParseCoordinates(input);
        if (coordinates == null) return null;

        if (!gameInstance.PlacePieceInGrid(coordinates.Value.x, coordinates.Value.y))
        {
            Console.WriteLine("Invalid move: position is already occupied or out of bounds.");
            return null;
        }

        return gameInstance.CheckGameStatus();
    }

    private static GameStatus? HandleMovePieceAction(string input, TicTacTwoBrain gameInstance)
    {
        var coordinates = input.Split(',');
        if (coordinates.Length != 4 ||
            !int.TryParse(coordinates[0], out var startX) ||
            !int.TryParse(coordinates[1], out var startY) ||
            !int.TryParse(coordinates[2], out var endX) ||
            !int.TryParse(coordinates[3], out var endY))
        {
            Console.WriteLine("Invalid input. Format: MovePiece x1,y1,x2,y2");
            return null;
        }

        if (!gameInstance.MovePieceInGrid(startX - 1, startY - 1, endX - 1, endY - 1))
        {
            Console.WriteLine("Invalid move: cannot move to the specified location.");
            return null;
        }

        return gameInstance.CheckGameStatus();
    }

    private static GameStatus? HandleMoveGridAction(string input, TicTacTwoBrain gameInstance)
    {
        if (!int.TryParse(input, out var direction) || direction < 1 || direction > 8)
        {
            Console.WriteLine("Invalid input. Direction must be a number between 1-8:");
            Console.WriteLine("1: Up, 2: Up-Right, 3: Right, 4: Down-Right");
            Console.WriteLine("5: Down, 6: Down-Left, 7: Left, 8: Up-Left");
            return null;
        }

        var (dx, dy) = DirectionToOffset(direction);
        var currentPos = gameInstance.GridPosition;
        
        if (!gameInstance.MoveGrid(currentPos.x + dx, currentPos.y + dy))
        {
            Console.WriteLine("Invalid move: cannot move the grid in that direction.");
            return null;
        }

        return gameInstance.CheckGameStatus();
    }

    private static (int dx, int dy) DirectionToOffset(int direction)
    {
        return direction switch
        {
            1 => (0, -1),  // Up
            2 => (1, -1),  // Up-Right
            3 => (1, 0),   // Right
            4 => (1, 1),   // Down-Right
            5 => (0, 1),   // Down
            6 => (-1, 1),  // Down-Left
            7 => (-1, 0),  // Left
            8 => (-1, -1), // Up-Left
            _ => (0, 0)
        };
    }

    private static (int x, int y)? ParseCoordinates(string input)
    {
        var inputSplit = input.Split(',');
        if (inputSplit.Length != 2 ||
            !int.TryParse(inputSplit[0], out var x) ||
            !int.TryParse(inputSplit[1], out var y))
        {
            Console.WriteLine("Invalid input. Format: x,y");
            return null;
        }

        return (x - 1, y - 1);
    }

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = _configRepository.GetConfigurationNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - Choose game config",
            configMenuItems,
            isCustomMenu: true
        );

        return configMenu.Run();
    }
    private static string ChooseGameMode()
    {
        var gameModeMenuItems = new List<MenuItem>();
        for (var i = 0; i < 3; i++)
        {
            var returnValue = Enum.GetName(typeof(GameMode), i);
            gameModeMenuItems.Add(new MenuItem()
            {
                Title = Enum.GetName(typeof(GameMode), i),
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var gameModeMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - Choose game mode",
            gameModeMenuItems,
            isCustomMenu: true
        );

        return gameModeMenu.Run();
    }
}
