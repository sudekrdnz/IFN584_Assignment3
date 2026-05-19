using BoardGameFramework.Core.Players;

namespace BoardGameFramework.NumericalTicTacToe.Players;

public class NumericalTicTacToeHumanPlayer : HumanPlayer
{
    public NumericalTicTacToeHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }
}