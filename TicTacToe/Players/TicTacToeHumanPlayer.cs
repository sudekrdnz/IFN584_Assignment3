using BoardGameFramework.Core.Players;

namespace BoardGameFramework.TicTacToe.Players;

public class TicTacToeHumanPlayer : HumanPlayer
{
    public char Symbol { get; }

    public TicTacToeHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber)
    {
        Symbol = playerNumber == 1 ? 'X' : 'O';
    }
}