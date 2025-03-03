using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        WriteHeader(gameInstance.DimX);
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            WriteGamePieceLine(gameInstance, y);
            if (y == gameInstance.DimY - 1) continue;
            WriteGameDividerLine(gameInstance, y);
        }
    }

    private static void WriteGameDividerLine(TicTacTwoBrain gameInstance, int y)
    {
        Console.Write("   ");
        var gridPos = gameInstance.GridPosition;

        for (var x = 0; x < gameInstance.DimX; x++)
        {
            // Check if this line is part of the movable grid
            bool isGridHorizontal = y >= gridPos.y && y < gridPos.y + 2;
            bool isInGridX = x >= gridPos.x && x < gridPos.x + 3;

            if (isGridHorizontal && isInGridX)
            {
                Console.Write("==="); // Double line for grid boundaries
            }
            else
            {
                Console.Write("---");
            }

            if (x != gameInstance.DimX - 1)
            {
                if (isGridHorizontal && x >= gridPos.x - 1 && x < gridPos.x + 2)
                {
                    Console.Write("#"); // Special character for grid intersections
                }
                else
                {
                    Console.Write("+");
                }
            }
        }

        Console.WriteLine();
    }

    private static void WriteGamePieceLine(TicTacTwoBrain gameInstance, int y)
    {
        Console.Write(PadNoToCenter(y + 1));
        var gridPos = gameInstance.GridPosition;

        for (var x = 0; x < gameInstance.DimX; x++)
        {
            // Check if this position is part of the movable grid
            bool isInGrid = x >= gridPos.x && x < gridPos.x + 3 && 
                           y >= gridPos.y && y < gridPos.y + 3;

            Console.Write(" " + GamePieceToString(gameInstance.GameBoard[x][y]) + " ");
            
            if (x == gameInstance.DimX - 1) continue;
            
            if (x >= gridPos.x - 1 && x < gridPos.x + 2 && 
                y >= gridPos.y && y < gridPos.y + 3)
            {
                Console.Write("║"); // Vertical line for grid boundaries
            }
            else
            {
                Console.Write("|");
            }
        }

        Console.WriteLine();
    }

    private static void WriteHeader(int dimX)
    {
        Console.Write("   ");
        for (var x = 0; x < dimX; x++)
        {
            Console.Write(PadNoToCenter(x + 1));
            if (x == dimX - 1) continue;
            Console.Write(" ");
        }

        Console.WriteLine();
    }

    private static string PadNoToCenter(int no, int len = 3)
    {
        var noStr = no.ToString();
        var spacesLeft = (len - noStr.Length) / 2;
        return noStr.PadLeft(spacesLeft + noStr.Length).PadRight(len);
    }

    public static string GamePieceToString(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };
}