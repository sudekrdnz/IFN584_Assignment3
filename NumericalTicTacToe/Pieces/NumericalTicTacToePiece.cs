using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.NumericalTicTacToe.Pieces;

public class NumericalTicTacToePiece : Piece
{
    public int Value { get; }

    public NumericalTicTacToePiece (int owner, int value)
        // Symbol shows last digit for display; Value holds the actual number
        : base(owner, value < 10 ? (char)('0' + value) : '#')
    {
        Value = value;
    }
}