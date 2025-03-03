using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.X;
    public GameMode GameMode { get; set; } 

    public GameConfiguration GameConfiguration { get; set; }
    public int MovesPlayed { get; set; } = 0;
    public (int x, int y) GridPosition { get; set; } = (1, 1);
    public GameState(EGamePiece[][] gameBoard, GameConfiguration gameConfiguration)
    {
        GameBoard = gameBoard;
        GameConfiguration = gameConfiguration;
    }

    public override string ToString()
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }
}