using System.ComponentModel.DataAnnotations;
using GameBrain;

public class ConfigurationDto
{
    public string Name { get; set; } = default!;

    [Range(3, 20, ErrorMessage = "Board width must be between 3 and 20")]
    public int BoardSizeWidth { get; set; }

    [Range(3, 20, ErrorMessage = "Board height must be between 3 and 20")]
    public int BoardSizeHeight { get; set; }

    [Range(3, int.MaxValue, ErrorMessage = "Win condition must be at least 3")]
    public int WinCondition { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Move piece after N moves must be non-negative")]
    public int MovePieceAfterNMoves { get; set; }

    public GameConfiguration ToGameConfiguration()
    {
        return new GameConfiguration
        {
            Name = Name,
            BoardSizeWidth = BoardSizeWidth,
            BoardSizeHeight = BoardSizeHeight,
            WinCondition = WinCondition,
            MovePieceAfterNMoves = MovePieceAfterNMoves
        };
    }

    public static ConfigurationDto FromGameConfiguration(GameConfiguration config)
    {
        return new ConfigurationDto
        {
            Name = config.Name,
            BoardSizeWidth = config.BoardSizeWidth,
            BoardSizeHeight = config.BoardSizeHeight,
            WinCondition = config.WinCondition,
            MovePieceAfterNMoves = config.MovePieceAfterNMoves
        };
    }
}