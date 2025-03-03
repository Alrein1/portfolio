using DAL;
using GameBrain;
using Web.Models;
using Web.Services;

public class GameService : IGameService
{
    private readonly IActiveGamesManager _activeGames;
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;

    private readonly ILogger<GameService> _logger;

    public GameService(
        IActiveGamesManager activeGames,
        IConfigRepository configRepository,
        IGameRepository gameRepository,
        ILogger<GameService> logger)
    {
        _configRepository = configRepository;
        _activeGames = activeGames;
        _gameRepository = gameRepository;
        _logger = logger;
    }

    public string CreateNewGame(GameConfiguration config, string password, GameMode gameMode)
    {
        var gameName = $"{config.Name}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        var game = new TicTacTwoBrain(config, gameMode);
        
        _activeGames.AddGame(gameName, game, password);

        // For AI vs AI, add both AI players immediately
        if (gameMode == GameMode.AIVsAI)
        {
            _activeGames.AddAIPlayer(gameName, "AI_Player1", EGamePiece.X);
            _activeGames.AddAIPlayer(gameName, "AI_Player2", EGamePiece.O);
        }
        // For Player vs AI, add AI player as O
        else if (gameMode == GameMode.PlayerVsAI)
        {
            _activeGames.AddAIPlayer(gameName, "AI_Player", EGamePiece.O);
        }

        return gameName;
    }

    public bool IsGameFull(string gameName)
    {
        var gameMode = GetGameMode(gameName);
        switch (gameMode)
        {
            case GameMode.AIVsAI:
                return true; // AI vs AI is always full
            case GameMode.PlayerVsAI:
                return true; // Player vs AI is always full (AI is pre-added)
            case GameMode.PlayerVsPlayer:
                return _activeGames.IsGameFull(gameName);
            default:
                return false;
        }
    }

    public GameMode GetGameMode(string gameName)
    {
        return _activeGames.GetGameMode(gameName);
    }

    public bool IsGameReady(string gameName)
    {
        return _activeGames.IsGameReady(gameName);
    }
    
    public TicTacTwoBrain? GetGame(string gameName)
    {
        return _activeGames.GetGame(gameName);
    }

    public (bool success, EGamePiece piece) JoinGame(string gameName, string password, string playerId)
    {
        if (!_activeGames.ValidatePassword(gameName, password))
        {
            return (false, EGamePiece.Empty);
        }

        var piece = _activeGames.AddPlayer(gameName, playerId);
        return (piece != EGamePiece.Empty, piece);
    }

    public GameMoveResult MakeMove(string gameName, int x, int y, string playerId)
    {
        var game = GetGame(gameName);
        if (game == null)
        {
            return new GameMoveResult { Success = false, Error = "Game not found" };
        }

        // Check if it's this player's turn
        if (!IsPlayersTurn(gameName, playerId))
        {
            return new GameMoveResult { Success = false, Error = "Not your turn" };
        }

        var success = game.PlacePieceInGrid(x, y);
        var result = CreateMoveResult(game, success);
        result.IsAITurn = IsAITurn(gameName);
        return result;
    }

    public GameMoveResult MovePiece(string gameName, int fromX, int fromY, int toX, int toY, string playerId)
    {
        var game = GetGame(gameName);
        if (game == null)
        {
            return new GameMoveResult { Success = false, Error = "Game not found" };
        }
        
        if (!IsPlayersTurn(gameName, playerId))
        {
            return new GameMoveResult { Success = false, Error = "Not your turn" };
        }

        var success = game.MovePieceInGrid(fromX, fromY, toX, toY);
        return CreateMoveResult(game, success);
    }

    private bool IsPlayersTurn(string gameName, string playerId)
    {
        return _activeGames.IsPlayersTurn(gameName, playerId);
    }
    
    public GameMoveResult MoveGrid(string gameName, int newX, int newY, string playerId)
    {
        var game = _activeGames.GetGame(gameName);
        if (game == null || !_activeGames.IsPlayersTurn(gameName, playerId))
        {
            return new GameMoveResult { Success = false, Error = "Not your turn" };
        }

        var success = game.MoveGrid(newX, newY);
        var result = CreateMoveResult(game, success);
        result.IsAITurn = IsAITurn(gameName);
        return result;
    }
    
    public void SaveGame(string gameName)
    {
        var game = GetGame(gameName);
        if (game == null)
        {
            throw new InvalidOperationException("Game not found");
        }

        try
        {
            var configName = game.GetGameConfigName();
            if (string.IsNullOrEmpty(configName))
            {
                configName = "default";
            }

            var saveTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var saveName = $"{configName}_{saveTime}";

            _logger.LogInformation($"Saving game with config: {configName}");
            _gameRepository.SaveGame(game.GetGameStateJson(), configName);
            _logger.LogInformation($"Game saved successfully as: {saveName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving game");
            throw new InvalidOperationException($"Failed to save game: {ex.Message}");
        }
    }
    

    public bool LoadGame(string gameName, bool playerIsX)
    {
        try
        {
            var savedGame = _gameRepository.LoadGame(gameName);
            if (savedGame == null)
            {
                _logger.LogWarning($"Saved game not found: {gameName}");
                return false;
            }
            
            var game = new TicTacTwoBrain(_configRepository.GetConfigurationById(savedGame.ConfigId), GameMode.PlayerVsAI);
            string password = "lelelelele";
            game.SetGameStateJson(savedGame.GameState);
            _activeGames.AddGame(gameName, game, password);

            _activeGames.AddAIPlayer(gameName, "AI_Player", playerIsX ? EGamePiece.O : EGamePiece.X);

            _logger.LogInformation($"Game {gameName} loaded successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading game: {gameName}");
            return false;
        }
    }
    
    public List<string> GetActiveGames()
    {
        return _activeGames.GetActiveGames();
    }
    public string? FindGameByPassword(string password)
    {
        return _activeGames.FindGameByPassword(password);
    }
    public bool IsAITurn(string gameName)
    {
        return _activeGames.IsAITurn(gameName);
    }
    private GameMoveResult CreateMoveResult(TicTacTwoBrain game, bool success)
    {
        return new GameMoveResult
        {
            Success = success,
            Board = game.GameBoard,
            NextPlayer = game.GetNextMoveBy().ToString(),
            GameStatus = game.CheckGameStatus(),
            GridPosition = new GridPosition(){ X= game.GridPosition.x,Y= game.GridPosition.y },
            IsAITurn = IsAITurn(game.GetGameConfigName())
        };
    }
}