using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.Gomoku.Pieces;

public class GomokuPiece : Piece
{
    public GomokuPiece(int owner, char symbol) 
        : base(owner, symbol) { }
}