using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.NumericalTicTacToe.Pieces;

public class NumericalTicTacToePiece : Piece
{
    public int Value { get; }

    public NumericalTicTacToePiece (int owner, int value)
        : base(owner, value.ToString()[0])
    {
        Value = value;
    }
}
