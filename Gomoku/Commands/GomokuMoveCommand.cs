using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Gomoku.Grid;
using BoardGameFramework.Gomoku.Pieces;

namespace BoardGameFramework.Gomoku.Commands;

public class GomokuMoveCommand : MoveCommand
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public char Symbol { get; private set; }

    public GomokuMoveCommand(
        Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount,
        int row, int col, char symbol)
        : base(player, gridSnapshot, 
               currentPlayerIndex, turnCount)
    {
        Row = row;
        Col = col;
        Symbol = symbol;
    }

    // Replay the move for Redo
    public override void Execute(GameBoard board)
    {
        var grid = (GomokuGrid)board;
        grid.PlacePiece(Row, Col,
            new GomokuPiece(Player.PlayerNumber, Symbol));
    }

    // Undo the move
    public override void Reverse(GameBoard board)
    {
        board.ImportState(GridSnapshot);
    }
}