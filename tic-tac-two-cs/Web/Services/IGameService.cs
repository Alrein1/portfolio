using GameBrain;
using Web.Models;

namespace Web.Services;

public interface IGameService
{
    string CreateNewGame(GameConfiguration config, string password, GameMode gameMode);
    (bool success, EGamePiece piece) JoinGame(string gameName, string password, string playerId);    
    GameMoveResult MakeMove(string gameName, int x, int y, string playerId);
    GameMoveResult MovePiece(string gameName, int fromX, int fromY, int toX, int toY, string playerId);
    GameMoveResult MoveGrid(string gameName, int newX, int newY, string playerId);
    void SaveGame(string gameName);
    bool IsGameReady(string gameName);
    bool LoadGame(string gameName, bool playerIsX);
    bool IsAITurn(string gameName);
    TicTacTwoBrain? GetGame(string gameName); // Added this method
    List<string> GetActiveGames();
    string? FindGameByPassword(string password);
    bool IsGameFull(string gameName);
    GameMode GetGameMode(string gameName);
}