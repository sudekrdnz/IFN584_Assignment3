using BoardGameFramework.Core.Grid;

namespace BoardGameFramework.Core.Pieces;

public abstract class Piece
{
    // Use C# properties directly — no need for Java-style getters
    public int Owner { get; private set; }
    public char Symbol { get; private set; }

    protected Piece(int owner, char symbol)
    {
        Owner = owner;
        Symbol = symbol;
    }

}