using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.Notakto.Grid;

/// <summary>
/// Container for three SubBoards that make up a Notakto game.
/// Delegates per-board logic to SubBoard and coordinates display,
/// serialisation, and game-over detection across all three boards.
/// </summary>
public class NotaktoGrid : GameBoard
{
    public const int BoardCount = 3;

    private readonly SubBoard[] _subBoards;

    public NotaktoGrid()
        : base(SubBoard.Size, SubBoard.Size)
    {
        _subBoards = Enumerable.Range(0, BoardCount)
            .Select(_ => new SubBoard())
            .ToArray();
    }

    // Required abstract method from GameBoard — not used in Notakto
    // Notakto uses the 4-parameter overload (boardIndex, row, col, piece) instead
    public override void PlacePiece(int row, int col, Piece piece)
        => throw new NotSupportedException(
            "Use PlacePiece(boardIndex, row, col, piece) for Notakto.");

    // Notakto-specific placement — delegates to SubBoard
    public void PlacePiece(int boardIndex, int row, int col, Piece piece)
        => _subBoards[boardIndex].Place(row, col, piece);

    public bool IsEmpty(int boardIndex, int row, int col)
        => _subBoards[boardIndex].IsEmpty(row, col);

    public bool IsBoardDead(int boardIndex)
        => _subBoards[boardIndex].IsDead;

    public bool AreAllBoardsDead()
        => _subBoards.All(b => b.IsDead);

    // CheckWin not used — AreAllBoardsDead() drives game-over instead
    public override bool CheckWin(int playerNumber) => false;

    public override string ExportState()
    {
        var cells = _subBoards.SelectMany(b => b.ExportCells()).ToList();
        var dead  = _subBoards.Select(b => b.IsDead).ToArray();
        var data  = new { Cells = cells, DeadBoards = dead };
        return System.Text.Json.JsonSerializer.Serialize(data);
    }

    public override void ImportState(string state)
    {
        var doc = System.Text.Json.JsonSerializer
            .Deserialize<System.Text.Json.JsonElement>(state);

        var flat = doc.GetProperty("Cells")
            .EnumerateArray()
            .Select(e => e.GetInt32())
            .ToList();

        var dead = doc.GetProperty("DeadBoards")
            .EnumerateArray()
            .Select(e => e.GetBoolean())
            .ToArray();

        int cellsPerBoard = SubBoard.Size * SubBoard.Size;
        for (int b = 0; b < BoardCount; b++)
        {
            _subBoards[b].ImportCells(flat, b * cellsPerBoard);
            _subBoards[b].SetDead(dead[b]);
        }
    }

    public override void Display()
    {
        Console.WriteLine();

        // Header row: Board 1   Board 2   Board 3
        Console.Write("  ");
        for (int b = 0; b < BoardCount; b++)
        {
            string label = _subBoards[b].IsDead
                ? $"Board {b + 1} [DEAD]"
                : $"Board {b + 1}";
            Console.Write($"{label,-18}");
        }
        Console.WriteLine();
        Console.WriteLine();

        // All 3 boards side by side, row by row
        for (int r = 0; r < SubBoard.Size; r++)
        {
            Console.Write("  ");
            for (int b = 0; b < BoardCount; b++)
            {
                for (int c = 0; c < SubBoard.Size; c++)
                {
                    Console.Write($" {_subBoards[b].SymbolAt(r, c)} ");
                    if (c < SubBoard.Size - 1) Console.Write("|");
                }
                Console.Write("   ");
            }
            Console.WriteLine();

            if (r < SubBoard.Size - 1)
            {
                Console.Write("  ");
                for (int b = 0; b < BoardCount; b++)
                {
                    Console.Write("---+---+---");
                    Console.Write("   ");
                }
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
}