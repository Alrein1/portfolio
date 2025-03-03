namespace GameBrain;

public class GameAI
{
    private readonly Random _random = new Random();

    public (int x, int y)? GetRandomMove(TicTacTwoBrain game)
    {
        var validMoves = new List<(int x, int y)>();
        var gridPos = game.GridPosition;

        for (int x = gridPos.x; x < gridPos.x + 3; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + 3; y++)
            {
                if (game.GameBoard[x][y] == EGamePiece.Empty)
                {
                    validMoves.Add((x, y));
                }
            }
        }

        if (!validMoves.Any()) return null;
        return validMoves[_random.Next(validMoves.Count)];
    }

    public (int fromX, int fromY, int toX, int toY)? GetRandomPieceMove(TicTacTwoBrain game, EGamePiece currentPlayer)
    {
        var gridPos = game.GridPosition;
        var playerPieces = new List<(int x, int y)>();
        var emptySpaces = new List<(int x, int y)>();

        for (int x = gridPos.x; x < gridPos.x + 3; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + 3; y++)
            {
                if (game.GameBoard[x][y] == currentPlayer)
                {
                    playerPieces.Add((x, y));
                }
                else if (game.GameBoard[x][y] == EGamePiece.Empty)
                {
                    emptySpaces.Add((x, y));
                }
            }
        }

        if (!playerPieces.Any() || !emptySpaces.Any()) return null;

        var fromPos = playerPieces[_random.Next(playerPieces.Count)];
        var toPos = emptySpaces[_random.Next(emptySpaces.Count)];

        return (fromPos.x, fromPos.y, toPos.x, toPos.y);
    }

    public (int x, int y)? GetRandomGridMove(TicTacTwoBrain game)
    {
        var currentPos = game.GridPosition;
        var validMoves = new List<(int x, int y)>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int newX = currentPos.x + dx;
                int newY = currentPos.y + dy;

                if (newX >= 0 && newY >= 0 && 
                    newX + 2 < game.DimX && 
                    newY + 2 < game.DimY)
                {
                    validMoves.Add((newX, newY));
                }
            }
        }

        if (!validMoves.Any()) return null;
        return validMoves[_random.Next(validMoves.Count)];
    }
}