using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.Core.Players;
using BoardGameFramework.NumericalTicTacToe.Grid;

namespace BoardGameFramework.NumericalTicTacToe.Players;

public class NumericalTicTacToeHumanPlayer : HumanPlayer
{
    public int LastRow { get; private set; } = -1;
    public int LastColumn { get; private set; } = 1;
    public int LastValue { get; private set; } = 1;

    private readonly Random random = new Random();
    public NumericalTicTacToeHumanPlayer(string name, int playerNumber)
        : base(name, playerNumber)
    {

    }
    public IEnumerable<int> GetPlayableNumbers(NumericalTicTacToeGrid grid)
    {
        return grid.GeneratePlayableNumbers(PlayerNumber);
    }
}
