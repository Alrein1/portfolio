using System.Text.Json;

namespace GameBrain;

public class TicTacTwoBrain
{
    
    private GameState _gameState;
    private readonly GameMode _gameMode;
    private readonly GameAI? _ai;
    public TicTacTwoBrain(GameConfiguration gameConfiguration, GameMode gameMode = GameMode.PlayerVsPlayer)
    {
        _gameMode = gameMode;
        if (gameMode != GameMode.PlayerVsPlayer) _ai = new GameAI();
        var gameBoard = new EGamePiece[gameConfiguration.BoardSizeWidth][]; 
        for (var x = 0; x < gameConfiguration.BoardSizeWidth; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSizeHeight];
        }

        _gameState = new GameState(
            gameBoard,
            gameConfiguration
        );
    }
    public bool IsAITurn()
    {
        return _gameMode switch
        {
            GameMode.PlayerVsAI => _gameState.NextMoveBy == EGamePiece.O,
            GameMode.AIVsAI => true,
            _ => false
        };
    }
    public GameStatus? MakeAIMove()
    {
        if (!IsAITurn()) return null;

        // If we can move pieces, randomly choose between placing, moving piece, or moving grid
        if (_gameState.MovesPlayed >= _gameState.GameConfiguration.MovePieceAfterNMoves)
        {
            var moveType = new Random().Next(3);
            switch (moveType)
            {
                case 0:
                    MakeAIPlacePiece();
                    break;
                case 1:
                    MakeAIMovePiece();
                    break;
                case 2:
                    MakeAIMoveGrid();
                    break;
            }
        }
        else
        {
            MakeAIPlacePiece();
        }

        return CheckGameStatus();
    }

    private void MakeAIPlacePiece()
    {
        var move = _ai.GetRandomMove(this);
        if (move.HasValue)
        {
            PlacePieceInGrid(move.Value.x, move.Value.y);
        }
    }

    private void MakeAIMovePiece()
    {
        var move = _ai.GetRandomPieceMove(this, _gameState.NextMoveBy);
        if (move.HasValue)
        {
            MovePieceInGrid(move.Value.fromX, move.Value.fromY, 
                move.Value.toX, move.Value.toY);
        }
    }

    private void MakeAIMoveGrid()
    {
        var move = _ai.GetRandomGridMove(this);
        if (move.HasValue)
        {
            MoveGrid(move.Value.x, move.Value.y);
        }
    }
    public GameStatus CheckGameStatus()
    {
        // Check for wins within the current grid
        for (int x = _gameState.GridPosition.x; x < _gameState.GridPosition.x + 3; x++)
        {
            for (int y = _gameState.GridPosition.y; y < _gameState.GridPosition.y + 3; y++)
            {
                if (_gameState.GameBoard[x][y] != EGamePiece.Empty)
                {
                    var status = CheckWinFromPosition(x, y);
                    if (status != GameStatus.Ongoing)
                    {
                        return status;
                    }
                }
            }
        }

        // Check for draw only if the entire board is full
        bool hasEmptySpace = false;
        for (int x = 0; x < _gameState.GameBoard.Length; x++)
        {
            for (int y = 0; y < _gameState.GameBoard[0].Length; y++)
            {
                if (_gameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    hasEmptySpace = true;
                    break;
                }
            }
            if (hasEmptySpace) break;
        }

        return hasEmptySpace ? GameStatus.Ongoing : GameStatus.Draw;
    }
    public GameMode GameMode => _gameMode;
    public EGamePiece GetNextMoveBy()
    {
        return _gameState.NextMoveBy;
    }

    public string GetGameStateJson()
    {
        return _gameState.ToString();
    }

    public string GetGameConfigName()
    {
        return _gameState.GameConfiguration.Name;
    }

    public EGamePiece[][] GameBoard
    {
        get => GetBoard();
        private set => _gameState.GameBoard = value;
    }

    public (int x, int y) GridPosition => _gameState.GridPosition;
    public int DimX => _gameState.GameBoard.Length;
    public int DimY => _gameState.GameBoard[0].Length;

    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameState.GameConfiguration.BoardSizeWidth][];
        for (var x = 0; x < _gameState.GameConfiguration.BoardSizeWidth; x++)
        {
            copyOfBoard[x] = new EGamePiece[_gameState.GameConfiguration.BoardSizeHeight];
            for (var y = 0; y < _gameState.GameConfiguration.BoardSizeHeight; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }

        return copyOfBoard;
    }
    
    
    public bool PlacePieceInGrid(int x, int y)
    {
        if (!IsInGrid(x, y) || _gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return false;
        }

        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        _gameState.MovesPlayed++;
        return true;
    }

    public bool MovePieceInGrid(int fromX, int fromY, int toX, int toY)
    {
        if (_gameState.MovesPlayed < _gameState.GameConfiguration.MovePieceAfterNMoves * 2)
        {
            return false;
        }
        if (!IsInGrid(toX, toY)) return false;
        if (_gameState.GameBoard[fromX][fromY] != _gameState.NextMoveBy || 
            _gameState.GameBoard[toX][toY] != EGamePiece.Empty) return false;

        _gameState.GameBoard[toX][toY] = _gameState.GameBoard[fromX][fromY];
        _gameState.GameBoard[fromX][fromY] = EGamePiece.Empty;
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool MoveGrid(int newX, int newY)
    {
        if (_gameState.MovesPlayed < _gameState.GameConfiguration.MovePieceAfterNMoves * 2)
        {
            return false;
        }
        if (newX < 0 || newY < 0 || 
            newX + 2 >= _gameState.GameBoard.Length || 
            newY + 2 >= _gameState.GameBoard[0].Length)
            return false;
        
        if (Math.Abs(newX - _gameState.GridPosition.x) > 1 || 
            Math.Abs(newY - _gameState.GridPosition.y) > 1)
            return false;

        _gameState.GridPosition = (newX, newY);
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool IsInGrid(int x, int y)
    {
        return x >= _gameState.GridPosition.x && 
               x < _gameState.GridPosition.x + 3 && 
               y >= _gameState.GridPosition.y && 
               y < _gameState.GridPosition.y + 3;
    }
    private GameStatus CheckWinFromPosition(int x, int y)
    {
        var piece = _gameState.GameBoard[x][y];
        if (piece == EGamePiece.Empty) return GameStatus.Ongoing;

        // Check all directions
        var directions = new[]
        {
            (1, 0),   // horizontal
            (0, 1),   // vertical
            (1, 1),   // diagonal down-right
            (1, -1)   // diagonal up-right
        };

        foreach (var (dx, dy) in directions)
        {
            int count = 1;

            // Check forward
            for (int i = 1; i < 3; i++)
            {
                int newX = x + (dx * i);
                int newY = y + (dy * i);
                if (!IsValidPosition(newX, newY) || _gameState.GameBoard[newX][newY] != piece)
                    break;
                count++;
            }

            // Check backward
            for (int i = 1; i < 3; i++)
            {
                int newX = x - (dx * i);
                int newY = y - (dy * i);
                if (!IsValidPosition(newX, newY) || _gameState.GameBoard[newX][newY] != piece)
                    break;
                count++;
            }

            if (count >= 3)
            {
                return piece == EGamePiece.X ? GameStatus.XWon : GameStatus.OWon;
            }
        }

        return GameStatus.Ongoing;
    }
    
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < _gameState.GameBoard.Length &&
               y >= 0 && y < _gameState.GameBoard[0].Length &&
               IsInGrid(x, y);
    }
    
    public void ResetGame()
    {
        var gameBoard = new EGamePiece[_gameState.GameConfiguration.BoardSizeWidth][];
        for (var x = 0; x < _gameState.GameConfiguration.BoardSizeWidth; x++)
        {
            gameBoard[x] = new EGamePiece[_gameState.GameConfiguration.BoardSizeHeight];
        }

        _gameState.GameBoard = gameBoard;
        _gameState.NextMoveBy = EGamePiece.X;
        _gameState.GridPosition = (1, 1);
    }

    public void SetGameStateJson(string dbGameGameState)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        _gameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(dbGameGameState, options);
    }
}
