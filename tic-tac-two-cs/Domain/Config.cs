using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Config : BaseEntity
{
    [MaxLength(128)]
    public string ConfigName { get; set; } = default!;
    
    public int BoardSizeWidth { get; set; }
    public int BoardSizeHeight { get; set; }

    // how many pieces in straight to win the game
    public int WinCondition { get; set; }

    // 0 disabled
    public int MovePieceAfterNMoves { get; set; }

    public ICollection<Game>? Games { get; set; }
}