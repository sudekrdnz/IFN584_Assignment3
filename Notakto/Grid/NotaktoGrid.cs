using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.Notakto.Pieces;

namespace BoardGameFramework.Notakto.Grid;

public class NotaktoGrid : GameBoard
{
    private readonly Piece?[,,] _boards;
    private readonly bool[] _deadBoards;

    public NotaktoGrid()
        : base(3, 3)
    {
        _boards = new Piece?[3, 3, 3];
        _deadBoards = new bool[3];
    }

    // Required abstract method from GameBoard — not used in Notakto
    // Notakto uses the 4-parameter overload (board, row, col, piece) instead
    public override void PlacePiece(
        int row,
        int col,
        Piece piece)
    {
        throw new NotSupportedException(
            "Use PlacePiece(board, row, col, piece) for Notakto.");
    }

    // Actual Notakto placement
    public void PlacePiece(
        int board,
        int row,
        int col,
        Piece piece)
    {
        _boards[board, row, col] = piece;

        if (CheckBoardDead(board))
            _deadBoards[board] = true;
    }

    public bool IsEmpty(
        int board,
        int row,
        int col)
    {
        return _boards[board, row, col] == null;
    }

    public bool IsBoardDead(int board)
    {
        return _deadBoards[board];
    }

    public bool AreAllBoardsDead()
    {
        return _deadBoards.All(x => x);
    }

    // Required abstract method
    public override bool CheckWin(int playerNumber)
    {
        return false;
    }

    private bool CheckBoardDead(int board)
    {
        // Rows
        for (int r = 0; r < 3; r++)
        {
            if (Same(board, r, 0, r, 1, r, 2))
                return true;
        }

        // Columns
        for (int c = 0; c < 3; c++)
        {
            if (Same(board, 0, c, 1, c, 2, c))
                return true;
        }

        // Diagonals
        if (Same(board, 0, 0, 1, 1, 2, 2))
            return true;

        if (Same(board, 0, 2, 1, 1, 2, 0))
            return true;

        return false;
    }

    private bool Same(
        int b,
        int r1, int c1,
        int r2, int c2,
        int r3, int c3)
    {
        return _boards[b, r1, c1] != null
            && _boards[b, r2, c2] != null
            && _boards[b, r3, c3] != null;
    }

    public override string ExportState()
    {
        // Serialize each cell as 0 (empty) or 1 (occupied),
        // plus the dead-board flags
        var cells = new List<int>();
        for (int b = 0; b < 3; b++)
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    cells.Add(_boards[b, r, c] == null ? 0 : 1);

        var data = new
        {
            Cells = cells,
            DeadBoards = _deadBoards.ToArray()
        };
        return System.Text.Json.JsonSerializer.Serialize(data);
    }

    public override void ImportState(string state)
    {
        var data = System.Text.Json.JsonSerializer
            .Deserialize<System.Text.Json.JsonElement>(state);

        var cells = data.GetProperty("Cells");
        int index = 0;
        for (int b = 0; b < 3; b++)
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    int val = cells[index++].GetInt32();
                    _boards[b, r, c] = val == 0
                        ? null
                        : new NotaktoPiece(1);
                }

        var dead = data.GetProperty("DeadBoards");
        for (int b = 0; b < 3; b++)
            _deadBoards[b] = dead[b].GetBoolean();
    }

    public override void Display()
    {
        Console.WriteLine();

        for (int b = 0; b < 3; b++)
        {
            Console.WriteLine(
                $"Board {b + 1} " +
                (_deadBoards[b]
                    ? "[DEAD]"
                    : ""));

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    char symbol =
                        _boards[b, r, c]?.Symbol
                        ?? '.';

                    Console.Write($" {symbol} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}