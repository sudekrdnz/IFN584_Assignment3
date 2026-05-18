using BoardGameFramework.Core.Grid;

namespace BoardGameFramework.Core.Pieces;

public abstract class Piece
{
    // Use C# properties directly — no need for Java-style getters
    public int Owner { get; set; }
    public char Symbol { get; set; }

    protected Piece(int owner, char symbol)
    {
        Owner = owner;
        Symbol = symbol;
    }

    // Virtual (not abstract) — most pieces have no special effect.
    // Only override when a piece type has a meaningful effect on the board.
    public virtual void ApplyEffect(GameBoard board) { }
}
