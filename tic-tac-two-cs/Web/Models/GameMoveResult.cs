using GameBrain;

namespace Web.Models;

public class GameMoveResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public EGamePiece[][] Board { get; set; } = default!;
    public string NextPlayer { get; set; } = default!;
    public GameStatus GameStatus { get; set; }
    public GridPosition GridPosition { get; set; } = default!;
    public bool IsAITurn { get; set; }
}

public class GridPosition
{
    public int X { get; set; }
    public int Y { get; set; }
}