using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;

namespace BoardGameFramework.Core.Commands;

public abstract class MoveCommand
{
    public Player Player { get; protected set; }
    public string GridSnapshot { get; protected set; }
    public int CurrentPlayerIndex { get; protected set; }
    public int TurnCount { get; protected set; }

    protected MoveCommand(Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount)
    {
        Player = player;
        GridSnapshot = gridSnapshot;
        CurrentPlayerIndex = currentPlayerIndex;
        TurnCount = turnCount;
    }

    public abstract void Execute(GameBoard board);
    public abstract void Reverse(GameBoard board);
}
