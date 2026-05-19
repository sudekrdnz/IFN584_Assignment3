using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.NumericalTicTacToe.Grid;
using BoardGameFramework.NumericalTicTacToe.Pieces;

namespace BoardGameFramework.NumericalTicTacToe.Commands;

public class NumericalTicTacToeMoveCommand : MoveCommand
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Value { get; private set; }

    public NumericalTicTacToeMoveCommand(Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount,
        int row, int col, int value)
        : base(player, gridSnapshot, currentPlayerIndex, turnCount)
    {
        Row = row; 
        Column = col; 
        Value = value;
    }

    public override void Execute(GameBoard board)
    {
        var nttGrid = (NumericalTicTacToeGrid)board;
        var piece = new NumericalTicTacToePiece(Player.PlayerNumber, Value);
        nttGrid.PlacePiece(Row, Column, piece);
    }

    public override void Reverse(GameBoard board)
    {
        board.ImportState(GridSnapshot);
    }
}