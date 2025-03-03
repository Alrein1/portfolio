namespace GameBrain;

public record struct GameConfiguration()
{
    public int ConfigId { get; set; } = 0;
    public string Name { get; set; } = default!;
    
    public int BoardSizeWidth { get; set; } = 5;
    public int BoardSizeHeight { get; set; } = 5;
    public int WinCondition { get; set; } = 3;
    public int MovePieceAfterNMoves { get; set; } = 2;

    public override string ToString() =>
        $"Board {BoardSizeWidth}x{BoardSizeHeight}, " +
        "to win: {WinCondition}, " +
        "can move piece after {MovePieceAfterNMoves} moves";
}