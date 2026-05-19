using System.Text.Json;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.NumericalTicTacToe.Pieces;

namespace BoardGameFramework.NumericalTicTacToe.Grid;

public class NumericalTicTacToeGrid : GameBoard
{
    public int SumToWin { get; }

    public NumericalTicTacToeGrid(int size) : base(size, size)
    {
        SumToWin = size * (size * size + 1) / 2;
    }

    public override void PlacePiece(int row, int column, Piece piece)
        => Cells[row, column] = piece;

    public override bool CheckWin(int playerNumber)
    {
        return CheckHorizontal(playerNumber, SumToWin)
            || CheckVertical(playerNumber, SumToWin)
            || CheckDiagonalDown(playerNumber, SumToWin)
            || CheckDiagonalUp(playerNumber, SumToWin);
    }

    public IEnumerable<int> GeneratePlayableNumbers(int playerNumber)
    {
        int maxNumber = Rows * Columns; //The maximum numbers we can have, based on grid rows and columns

        var allNumbers = Enumerable.Range(1, maxNumber); //List numbers from 1 to maxNumber

        IEnumerable<int> allocateNumbers;

        if (playerNumber == 1)
        {
            allocateNumbers = allNumbers.Where(n => n % 2 == 1); //Allocate odd numbers for P1
        }
        else
        {
            allocateNumbers = allNumbers.Where(n => n % 2 == 0); //Allocate even numbers for p2
        }

        var removedNumbers = Cells
            .OfType<NumericalTicTacToePiece>()
            .Select(p => p.Value);

        return allocateNumbers.Except(removedNumbers);

    }

    public override string ExportState()
    {
        var cellData = new List<int[]>();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var piece = Cells[row, col] as NumericalTicTacToePiece;

                cellData.Add(new int[]
                {
                    piece?.Owner ?? 0,
                    piece?.Value ?? 0
                });
            }
        }
        var save = new
        {
            Size = Rows,
            Cells = cellData
        };

        return JsonSerializer.Serialize(save);
    }

    public override void ImportState(string state)
    {
        var load = JsonSerializer.Deserialize<JsonElement>(state);

        int size = load.GetProperty("Size").GetInt32();
        ImportSize(size, size);

        var cells = load.GetProperty("Cells");

        int index = 0;

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var cell = cells[index];
                int owner = cell[0].GetInt32();
                int number = cell[1].GetInt32();
                index++;

                Cells[row, col] = owner == 0
                    ? null
                    : new NumericalTicTacToePiece(owner, number);
            }
        }
    }

    //To be used when loading a game; set grid size and reinitialise to match the save data
    public void ImportSize(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new Piece[rows, columns];
    }

    public override void Display()
    {
        Console.WriteLine();
        for (int row = 0; row < Rows; row++)
        {
            // Cell row — show number value (up to 2 digits) or blank
            Console.Write(" ");
            for (int col = 0; col < Columns; col++)
            {
                var piece = Cells[row, col] as NumericalTicTacToePiece;
                string cell = piece != null ? $"{piece.Value,2}" : "  ";
                Console.Write($" {cell} ");
                if (col < Columns - 1) Console.Write("|");
            }
            Console.WriteLine();

            // Separator row (not after last row)
            if (row < Rows - 1)
            {
                Console.Write(" ");
                for (int col = 0; col < Columns; col++)
                {
                    Console.Write("----");
                    if (col < Columns - 1) Console.Write("+");
                }
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }

    private bool CheckHorizontal(int playerNumber, int SumToWin)
    {
        for (int row = 0; row < Rows; row++)
        {
            int sum = 0;
            bool full = true;

            for (int col = 0; col < Columns; col++)
            {
                var piece = Cells[row, col] as NumericalTicTacToePiece;

                if (piece == null)
                {
                    full = false;
                    break;
                }

                // Check if this number belongs to the player
                if (playerNumber == 1 && piece.Value % 2 == 0) full = false;
                if (playerNumber == 2 && piece.Value % 2 == 1) full = false;

                sum += piece.Value;
            }

            if (full && sum == SumToWin)
                return true;
        }

        return false;
    }

    private bool CheckVertical(int playerNumber, int SumToWin)
    {
        for (int col = 0; col < Columns; col++)
        {
            int sum = 0;
            bool full = true;

            for (int row = 0; row < Rows; row++)
            {
                var piece = Cells[row, col] as NumericalTicTacToePiece;

                if (piece == null)
                {
                    full = false;
                    break;
                }

                if (playerNumber == 1 && piece.Value % 2 == 0) full = false;
                if (playerNumber == 2 && piece.Value % 2 == 1) full = false;

                sum += piece.Value;
            }

            if (full && sum == SumToWin)
                return true;
        }

        return false;
    }


    private bool CheckDiagonalUp(int playerNumber, int SumToWin)
    {
        for (int row = 2; row < Rows; row++)
        {
            for (int col = 0; col <= Columns - 3; col++)
            {
                int sum = 0;
                bool full = true;

                for (int i = 0; i < 3; i++)
                {
                    var piece = Cells[row - i, col + i] as NumericalTicTacToePiece;

                    if (piece == null)
                    {
                        full = false;
                        break;
                    }

                    if (playerNumber == 1 && piece.Value % 2 == 0) full = false;
                    if (playerNumber == 2 && piece.Value % 2 == 1) full = false;

                    sum += piece.Value;
                }

                if (full && sum == SumToWin)
                    return true;
            }
        }

        return false;
    }


    private bool CheckDiagonalDown(int playerNumber, int SumToWin)
    {
        for (int row = 0; row <= Rows - 3; row++)
        {
            for (int col = 0; col <= Columns - 3; col++)
            {
                int sum = 0;
                bool full = true;

                for (int i = 0; i < 3; i++)
                {
                    var piece = Cells[row + i, col + i] as NumericalTicTacToePiece;

                    if (piece == null)
                    {
                        full = false;
                        break;
                    }

                    if (playerNumber == 1 && piece.Value % 2 == 0) full = false;
                    if (playerNumber == 2 && piece.Value % 2 == 1) full = false;

                    sum += piece.Value;
                }

                if (full && sum == SumToWin)
                    return true;
            }
        }

        return false;
    }

}