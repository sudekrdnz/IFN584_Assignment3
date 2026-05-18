using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.TicTacToe.Pieces;

public class TicTacToePiece : Piece
{
    public TicTacToePiece(int owner, char symbol)
        : base(owner, symbol) { }
}