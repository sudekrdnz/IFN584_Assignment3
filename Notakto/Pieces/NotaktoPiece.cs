using BoardGameFramework.Core.Pieces;

namespace BoardGameFramework.Notakto.Pieces;

public class NotaktoPiece : Piece
{
    public NotaktoPiece(int owner)
        : base(owner, 'X') { }
}