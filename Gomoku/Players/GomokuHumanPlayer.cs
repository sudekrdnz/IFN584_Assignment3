using BoardGameFramework.Core.Players;

namespace BoardGameFramework.Gomoku.Players;

public class GomokuHumanPlayer : HumanPlayer
{
    public int LastRow { get; private set; } = -1;
    public int LastCol { get; private set; } = -1;

    public GomokuHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }

    public void SetMove(int row, int col)
    {
        LastRow = row;
        LastCol = col;
    }
}