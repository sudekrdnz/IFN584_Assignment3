using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.ConnectFour.Grid;

namespace BoardGameFramework.ConnectFour.Players;

public class ConnectFourHumanPlayer : HumanPlayer
{
    public int LastColumn { get; private set; } = -1;

    public ConnectFourHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }

    public void SetColumn(int col) => LastColumn = col;

    public bool ValidateInput(string input, GameBoard board)
    {
        var cfGrid = (ConnectFourGrid)board;
        if (!int.TryParse(input, out int col)) return false;
        col -= 1;
        if (col < 0 || col >= cfGrid.Columns) return false;
        return !cfGrid.IsColumnFull(col);
    }
}
