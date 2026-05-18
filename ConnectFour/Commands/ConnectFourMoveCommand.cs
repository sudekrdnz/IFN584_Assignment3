using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.ConnectFour.Grid;
using BoardGameFramework.ConnectFour.Pieces;

namespace BoardGameFramework.ConnectFour.Commands;

public class ConnectFourMoveCommand : MoveCommand
{
    public int Column { get; private set; }
    public int LandingRow { get; private set; }
    public char Symbol { get; private set; }

    public ConnectFourMoveCommand(Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount,
        int column, int landingRow, char symbol)
        : base(player, gridSnapshot, currentPlayerIndex, turnCount)
    {
        Column = column;
        LandingRow = landingRow;
        Symbol = symbol;
    }

    public override void Execute(GameBoard board)
    {
        var cfGrid = (ConnectFourGrid)board;
        var piece = new ConnectFourPiece(Player.PlayerNumber, Symbol);
        cfGrid.DropDisc(Column, piece);
    }

    public override void Reverse(GameBoard board)
    {
        board.ImportState(GridSnapshot);
    }
}
