using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.TicTacToe.Grid;
using BoardGameFramework.TicTacToe.Pieces;

namespace BoardGameFramework.TicTacToe.Commands;

public class TicTacToeMoveCommand : MoveCommand
{
    public int Position { get; private set; }
    public char Symbol { get; private set; }

    public TicTacToeMoveCommand(Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount,
        int position, char symbol)
        : base(player, gridSnapshot, currentPlayerIndex, turnCount)
    {
        Position = position;
        Symbol = symbol;
    }

    public override void Execute(GameBoard board)
    {
        var grid = (TicTacToeGrid)board;
        grid.PlaceAtPosition(Position, new TicTacToePiece(Player.PlayerNumber, Symbol));
    }

    public override void Reverse(GameBoard board)
    {
        board.ImportState(GridSnapshot);
    }
}