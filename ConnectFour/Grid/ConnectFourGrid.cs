using System.Text.Json;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.ConnectFour.Pieces;

namespace BoardGameFramework.ConnectFour.Grid;

public class ConnectFourGrid : GameBoard
{
    private const int WinLength = 4;
    public ConnectFourGrid(int rows = 6, int cols = 7) : base(rows, cols) { }

    public override void PlacePiece(int row, int col, Piece piece)
        => Cells[row, col] = piece;

    // Drops a piece into the given column, returns the landing row or -1 if full
    public int DropDisc(int col, Piece piece)
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (Cells[row, col] == null)
            {
                Cells[row, col] = piece;
                return row;
            }
        }
        return -1;
    }

    public bool IsColumnFull(int col) => Cells[0, col] != null;

    public override bool CheckWin(int playerNumber)
        => CheckHorizontal(playerNumber)
        || CheckVertical(playerNumber)
        || CheckDiagonalDown(playerNumber)
        || CheckDiagonalUp(playerNumber);

    private bool CheckHorizontal(int player)
    {
        for (int row = 0; row < Rows; row++)
        {
            int count = 0;
            for (int col = 0; col < Columns; col++)
            {
                count = Cells[row, col]?.Owner == player ? count + 1 : 0;
                if (count >= WinLength) return true;
            }
        }
        return false;
    }

    private bool CheckVertical(int player)
    {
        for (int col = 0; col < Columns; col++)
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
            {
                count = Cells[row, col]?.Owner == player ? count + 1 : 0;
                if (count >= WinLength) return true;
            }
        }
        return false;
    }

    private bool CheckDiagonalDown(int player)
    {
        for (int row = 0; row <= Rows - WinLength; row++)
            for (int col = 0; col <= Columns - WinLength; col++)
            {
                int count = 0;
                for (int i = 0; i < WinLength; i++)
                    if (Cells[row + i, col + i]?.Owner == player) count++;
                if (count >= WinLength) return true;
            }
        return false;
    }

    private bool CheckDiagonalUp(int player)
    {
        for (int row = WinLength - 1; row < Rows; row++)
            for (int col = 0; col <= Columns - WinLength; col++)
            {
                int count = 0;
                for (int i = 0; i < WinLength; i++)
                    if (Cells[row - i, col + i]?.Owner == player) count++;
                if (count >= WinLength) return true;
            }
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
        var flat = JsonSerializer.Deserialize<List<int>>(state)!;
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                int owner = flat[row * Columns + col];
                Cells[row, col] = owner == 0 ? null
                    : new ConnectFourPiece(owner, owner == 1 ? 'X' : 'O');
            }
    }

    public override void Display()
    {
        Console.WriteLine();
        Console.Write(" ");
        for (int col = 0; col < Columns; col++)
            Console.Write($" {col + 1}  ");
        Console.WriteLine();

        for (int row = 0; row < Rows; row++)
        {
            Console.Write("|");
            for (int col = 0; col < Columns; col++)
            {
                char symbol = Cells[row, col]?.Symbol ?? ' ';
                Console.Write($" {symbol} |");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}