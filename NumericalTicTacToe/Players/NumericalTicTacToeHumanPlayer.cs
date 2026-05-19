using BoardGameFramework.Core.Players;
using BoardGameFramework.NumericalTicTacToe.Grid;

namespace BoardGameFramework.NumericalTicTacToe.Players;

public class NumericalTicTacToeHumanPlayer : HumanPlayer
{
    public NumericalTicTacToeHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }
    public IEnumerable<int> GetPlayableNumbers(NumericalTicTacToeGrid grid)
    {
        return grid.GeneratePlayableNumbers(PlayerNumber);
    }
}