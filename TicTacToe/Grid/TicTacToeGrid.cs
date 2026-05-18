using System.Text.Json;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.TicTacToe.Pieces;

namespace BoardGameFramework.TicTacToe.Grid;

public class TicTacToeGrid : GameBoard
{
    public TicTacToeGrid() : base(3, 3) { }

    public override void PlacePiece(int row, int col, Piece piece)
        => Cells[row, col] = piece;

    public bool IsValidPosition(int position)
    {
        if (position < 1 || position > 9) return false;
        int row = (position - 1) / 3;
        int col = (position - 1) % 3;
        return IsEmpty(row, col);
    }

    public void PlaceAtPosition(int position, Piece piece)
    {
        int row = (position - 1) / 3;
        int col = (position - 1) % 3;
        PlacePiece(row, col, piece);
    }

    public override bool CheckWin(int playerNumber)
    {
        for (int row = 0; row < 3; row++)
            if (Cells[row, 0]?.Owner == playerNumber &&
                Cells[row, 1]?.Owner == playerNumber &&
                Cells[row, 2]?.Owner == playerNumber)
                return true;

        for (int col = 0; col < 3; col++)
            if (Cells[0, col]?.Owner == playerNumber &&
                Cells[1, col]?.Owner == playerNumber &&
                Cells[2, col]?.Owner == playerNumber)
                return true;

        if (Cells[0, 0]?.Owner == playerNumber &&
            Cells[1, 1]?.Owner == playerNumber &&
            Cells[2, 2]?.Owner == playerNumber)
            return true;

        if (Cells[0, 2]?.Owner == playerNumber &&
            Cells[1, 1]?.Owner == playerNumber &&
            Cells[2, 0]?.Owner == playerNumber)
            return true;

        return false;
    }

    public override string ExportState()
    {
        var flat = new List<int>();
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
                flat.Add(Cells[row, col]?.Owner ?? 0);

        return JsonSerializer.Serialize(flat);
    }

    public override void ImportState(string state)
    {
        var flat = JsonSerializer.Deserialize<List<int>>(state) ?? new List<int>();

        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                int index = row * Columns + col;
                int owner = index < flat.Count ? flat[index] : 0;

                Cells[row, col] = owner == 0
                    ? null
                    : new TicTacToePiece(owner, owner == 1 ? 'X' : 'O');
            }
    }

    public override void Display()
    {
        Console.WriteLine();

        for (int row = 0; row < 3; row++)
        {
            Console.Write(" ");
            for (int col = 0; col < 3; col++)
            {
                int position = row * 3 + col + 1;
                char symbol = Cells[row, col]?.Symbol ?? position.ToString()[0];
                Console.Write($" {symbol} ");

                if (col < 2) Console.Write("|");
            }

            Console.WriteLine();
            if (row < 2) Console.WriteLine(" ---+---+---");
        }

        Console.WriteLine();
    }
}