using GameBrain;

public interface IActiveGamesManager
{
    void AddGame(string gameName, TicTacTwoBrain game, string password);
    TicTacTwoBrain? GetGame(string gameName);
    bool ValidatePassword(string gameName, string password);
    EGamePiece AddPlayer(string gameName, string playerId);
    bool IsPlayersTurn(string gameName, string playerId);
    EGamePiece? GetPlayerPiece(string gameName, string playerId);
    void RemoveGame(string gameName);
    bool IsGameFull(string gameName);
    GameMode GetGameMode(string gameName);
    bool IsAITurn(string gameName);

    List<string> GetActiveGames();
    string? FindGameByPassword(string password);
    void AddAIPlayer(string gameName, string aiId, EGamePiece aiPiece);
    bool IsGameReady(string gameName);
}