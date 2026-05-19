using System.Text.Json;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.Gomoku.Pieces;

namespace BoardGameFramework.Gomoku.Grid;

public class GomokuGrid : GameBoard
{
    private static readonly string AllColLabels = 
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string ColLabels => AllColLabels[..Columns];

    public GomokuGrid(int rows = 15, int cols = 15) 
        : base(rows, cols) { }

    public override void PlacePiece(int row, int col, Piece piece)
        => Cells[row, col] = piece;

    // Instance method — needs access to Rows/Columns for bounds checking
    public bool TryParseCoordinate(
        string input, out int row, out int col)
    {
        row = -1;
        col = -1;
        if (input.Length < 2) return false;

        char colChar = char.ToUpper(input[0]);
        col = ColLabels.IndexOf(colChar);
        if (col < 0) return false;

        if (!int.TryParse(input[1..], out int rowNum)) 
            return false;

        row = rowNum - 1; // convert to 0-based index
        return row >= 0 && row < Rows 
            && col >= 0 && col < Columns;
    }

    public override bool CheckWin(int playerNumber)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                if (Cells[r, c]?.Owner == playerNumber)
                    if (CheckFrom(r, c, playerNumber))
                        return true;
        return false;
    }

    // Check 4 directions from a starting cell
    private bool CheckFrom(int row, int col, int player)
    {
        int[][] directions =
        [
            [0, 1],   // horizontal
            [1, 0],   // vertical
            [1, 1],   // diagonal down-right
            [1, -1]   // diagonal down-left
        ];

        foreach (var dir in directions)
        {
            int count = 1;
            count += CountDir(row, col, dir[0], dir[1], player);
            count += CountDir(row, col, -dir[0], -dir[1], player);
            if (count >= 5) return true;
        }
        return false;
    }

    private int CountDir(
        int row, int col, int dr, int dc, int player)
    {
        int count = 0;
        int r = row + dr, c = col + dc;
        while (r >= 0 && r < Rows 
            && c >= 0 && c < Columns
            && Cells[r, c]?.Owner == player)
        {
            count++;
            r += dr;
            c += dc;
        }
        return count;
    }

    public override string ExportState()
    {
        // Include grid dimensions so Load can restore the correct board size
        var flat = new List<int>();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                flat.Add(Cells[r, c]?.Owner ?? 0);
        var data = new { Rows, Columns, Cells = flat };
        return JsonSerializer.Serialize(data);
    }

    public override void ImportState(string state)
    {
        var doc = JsonSerializer.Deserialize<JsonElement>(state);

        // Restore grid dimensions from save
        int rows = doc.GetProperty("Rows").GetInt32();
        int cols = doc.GetProperty("Columns").GetInt32();
        if (rows != Rows || cols != Columns)
        {
            Rows = rows;
            Columns = cols;
            Cells = new Core.Pieces.Piece?[rows, cols];
        }

        var flat = doc.GetProperty("Cells")
            .EnumerateArray().Select(e => e.GetInt32()).ToList();

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
            {
                int owner = flat[r * Columns + c];
                Cells[r, c] = owner == 0 ? null
                    : new GomokuPiece(
                        owner, owner == 1 ? 'X' : 'O');
            }
    }

    public override void Display()
    {
        Console.WriteLine();
        // Column header row
        Console.Write("    ");
        for (int c = 0; c < Columns; c++)
            Console.Write($" {ColLabels[c]} ");
        Console.WriteLine();

        for (int r = 0; r < Rows; r++)
        {
            // Row number right-aligned (2 chars)
            Console.Write($"{r + 1,2}  ");
            for (int c = 0; c < Columns; c++)
            {
                char symbol = Cells[r, c]?.Symbol ?? '.';
                Console.Write($" {symbol} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}