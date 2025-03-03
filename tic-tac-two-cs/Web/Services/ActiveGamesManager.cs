using GameBrain;

public class ActiveGamesManager : IActiveGamesManager
{
    private class GameSession
    {
        public TicTacTwoBrain Game { get; set; } = default!;
        public string Password { get; set; } = default!;
        public Dictionary<string, EGamePiece> Players { get; set; } = new();
        public HashSet<string> AIPlayers { get; set; } = new();
    }
    public List<string> GetActiveGames()
    {
        lock (_lock)
        {
            return _activeGames.Keys.ToList();
        }
    }
    private readonly Dictionary<string, GameSession> _activeGames = new();
    private readonly object _lock = new();
    private readonly ILogger<ActiveGamesManager> _logger;

    public ActiveGamesManager(ILogger<ActiveGamesManager> logger)
    {
        _logger = logger;
    }
    public void AddAIPlayer(string gameName, string aiId, EGamePiece piece)
    {
        lock (_lock)
        {
            if (_activeGames.TryGetValue(gameName, out var session))
            {
                session.Players[aiId] = piece;
                session.AIPlayers.Add(aiId);
                _logger.LogInformation($"Added AI player {aiId} to game {gameName} as {piece}");
            }
        }
    }

    public bool IsGameReady(string gameName)
    {
        lock (_lock)
        {
            if (_activeGames.TryGetValue(gameName, out var session))
            {
                return session.Players.Count == 2;
            }
            return false;
        }
    }

    public void AddGame(string gameName, TicTacTwoBrain game, string? password)
    {
        lock (_lock)
        {
            if (_activeGames.ContainsKey(gameName))
            {
                _activeGames.Remove(gameName);
                _logger.LogInformation($"Removed existing game: {gameName}");
            }

            _activeGames[gameName] = new GameSession
            {
                Game = game,
                Password = password,
                Players = new Dictionary<string, EGamePiece>()
            };
            _logger.LogInformation($"Game added: {gameName}");
        }
    }

    public TicTacTwoBrain? GetGame(string gameName)
    {
        lock (_lock)
        {
            if (_activeGames.TryGetValue(gameName, out var session))
            {
                return session.Game;
            }
            return null;
        }
    }
    public string? FindGameByPassword(string password)
    {
        lock (_lock)
        {
            var gameEntry = _activeGames.FirstOrDefault(g => g.Value.Password == password);
            return gameEntry.Key;
        }
    }
    public bool ValidatePassword(string gameName, string password)
    {
        lock (_lock)
        {
            return _activeGames.TryGetValue(gameName, out var session) && 
                   session.Password == password;
        }
    }

    public EGamePiece AddPlayer(string gameName, string playerId)
    {
        lock (_lock)
        {
            if (!_activeGames.TryGetValue(gameName, out var session))
            {
                return EGamePiece.Empty;
            }

            // If player already joined, return their piece
            if (session.Players.TryGetValue(playerId, out var existingPiece))
            {
                return existingPiece;
            }

            // For PlayerVsAI, human is always X
            if (session.Game.GameMode == GameMode.PlayerVsAI)
            {
                session.Players[playerId] = EGamePiece.X;
                return EGamePiece.X;
            }

            // For regular games
            if (session.Players.Count >= 2)
            {
                return EGamePiece.Empty;
            }

            var piece = session.Players.Count == 0 ? EGamePiece.X : EGamePiece.O;
            session.Players[playerId] = piece;
            return piece;
        }
    }

    public void RemovePlayer(string gameName, string playerId)
    {
        lock (_lock)
        {
            if (_activeGames.TryGetValue(gameName, out var session))
            {
                session.Players.Remove(playerId);
                _logger.LogInformation($"Removed player {playerId} from game {gameName}");
            }
        }
    }

    public bool IsPlayersTurn(string gameName, string playerId)
    {
        lock (_lock)
        {
            if (!_activeGames.TryGetValue(gameName, out var session))
                return false;

            // Check if player is in the game and it's their turn
            return session.Players.TryGetValue(playerId, out var playerPiece) &&
                   playerPiece == session.Game.GetNextMoveBy();
        }
    }
    
    public bool IsAITurn(string gameName)
    {
        lock (_lock)
        {
            if (!_activeGames.TryGetValue(gameName, out var session))
                return false;

            var currentPlayer = session.Game.GetNextMoveBy();
            return session.AIPlayers.Any(aiId => session.Players[aiId] == currentPlayer);
        }
    }

    public bool IsGameFull(string gameName)
    {
        lock (_lock)
        {
            if (!_activeGames.TryGetValue(gameName, out var session))
                return false;

            // For AI games, count AI players too
            return session.Players.Count >= 2;
        }
    }

    public GameMode GetGameMode(string gameName)
    {
        lock (_lock)
        {
            return _activeGames.TryGetValue(gameName, out var session) 
                ? session.Game.GameMode 
                : GameMode.PlayerVsPlayer;
        }
    }
    
    public EGamePiece? GetPlayerPiece(string gameName, string playerId)
    {
        lock (_lock)
        {
            if (_activeGames.TryGetValue(gameName, out var session) && 
                session.Players.TryGetValue(playerId, out var piece))
            {
                return piece;
            }
            return null;
        }
    }

    public void RemoveGame(string gameName)
    {
        lock (_lock)
        {
            _activeGames.Remove(gameName);
            _logger.LogInformation($"Game removed: {gameName}");
        }
    }
}