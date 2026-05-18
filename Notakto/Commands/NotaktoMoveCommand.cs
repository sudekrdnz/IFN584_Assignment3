using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Notakto.Grid;
using BoardGameFramework.Notakto.Pieces;

namespace BoardGameFramework.Notakto.Commands;

public class NotaktoMoveCommand : MoveCommand
{
    public int BoardIndex { get; }
    public int Row { get; }
    public int Col { get; }

    public NotaktoMoveCommand(
        Player player,
        string gridSnapshot,
        int currentPlayerIndex,
        int turnCount,
        int boardIndex,
        int row,
        int col)
        : base(player, gridSnapshot,
               currentPlayerIndex, turnCount)
    {
        BoardIndex = boardIndex;
        Row = row;
        Col = col;
    }

    public override void Execute(GameBoard board)
    {
        var grid = (NotaktoGrid)board;

        grid.PlacePiece(
            BoardIndex,
            Row,
            Col,
            new NotaktoPiece(Player.PlayerNumber));
    }

    public override void Reverse(GameBoard board)
    {
        board.ImportState(GridSnapshot);
    }
}