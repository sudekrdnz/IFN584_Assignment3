using BoardGameFramework.Core.Players;

namespace BoardGameFramework.TicTacToe.Players;

public class TicTacToeHumanPlayer : HumanPlayer
{
    public TicTacToeHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }
}