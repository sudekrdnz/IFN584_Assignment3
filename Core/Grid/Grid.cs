using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.Core.Grid;

public abstract class GameBoard
{
    public int Rows { get; protected set; }
    public int Columns { get; protected set; }
    protected Piece?[,] Cells;

    protected GameBoard(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new Piece?[rows, columns];
    }

    public abstract void PlacePiece(int row, int col, Piece piece);
    public abstract bool CheckWin(int playerNumber);
    public abstract string ExportState();
    public abstract void ImportState(string state);
    public abstract void Display();

    public bool IsFull()
    {
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
                if (Cells[row, col] == null) return false;
        return true;
    }

    public bool IsEmpty(int row, int col)
        => Cells[row, col] == null;

    public Piece? GetCell(int row, int col)
        => Cells[row, col];

    public void Clear()
    {
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
                Cells[row, col] = null;
    }
}
