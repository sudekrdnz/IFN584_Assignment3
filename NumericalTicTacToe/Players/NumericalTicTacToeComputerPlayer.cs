using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Pieces;
using BoardGameFramework.Core.Players;
using BoardGameFramework.NumericalTicTacToe.Grid;
using BoardGameFramework.NumericalTicTacToe.Pieces;

namespace BoardGameFramework.NumericalTicTacToe.Players;

public class NumericalTicTacToeComputerPlayer : ComputerPlayer
{
    private readonly Random _rng = new();

    public int LastRow { get; private set; } = -1;
    public int LastColumn { get; private set; } = 1;
    public int LastValue { get; private set; } = 1;

    public NumericalTicTacToeComputerPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }

    public IEnumerable<int> GetPlayableNumbers(NumericalTicTacToeGrid grid)
    {
        return grid.GeneratePlayableNumbers(PlayerNumber);
    }

    public void ChooseMove(NumericalTicTacToeGrid grid)
    {
        Console.WriteLine($"{Name} (Computer) is thinking...");
        Thread.Sleep(300);

        string saved = grid.ExportState();

        // 1. Try winning move
        var win = FindWinningMove(grid, PlayerNumber, saved);
        if (win.HasValue)
        {
            (LastRow, LastColumn, LastValue) = win.Value;
            Console.WriteLine($"{Name} played {LastValue} at {LastRow},{LastColumn}");
            return;
        }

        // 2. Block opponent from winning
        int opponent = PlayerNumber == 1 ? 2 : 1;
        var block = FindWinningMove(grid, opponent, saved);
        if (block.HasValue)
        {
            // Place our own number in the blocking cell
            var ownNums = GetPlayableNumbers(grid).ToList();
            if (ownNums.Count > 0)
            {
                LastRow = block.Value.r;
                LastColumn = block.Value.c;
                LastValue = ownNums[_rng.Next(ownNums.Count)];
                Console.WriteLine($"{Name} played {LastValue} at {LastRow},{LastColumn}");
                return;
            }
        }

        // 3. Random fallback
        var rand = ChooseRandomMove(grid);
        (LastRow, LastColumn, LastValue) = rand;
        Console.WriteLine($"{Name} played {LastValue} at {LastRow},{LastColumn}");
    }

    private (int r, int c, int num)? FindWinningMove(NumericalTicTacToeGrid grid, int playerNum, string saved)
    {
        var playable = grid.GeneratePlayableNumbers(playerNum).ToList();

        for (int i = 0; i < grid.Rows; i++)
        {
            for (int k = 0; k < grid.Columns; k++)
            {
                if (grid.GetCell(i, k) != null)
                    continue;

                foreach (var num in playable)
                {
                    grid.PlacePiece(i, k, new NumericalTicTacToePiece(playerNum, num));
                    bool winningMove = grid.CheckWin(playerNum);
                    grid.ImportState(saved);

                    if (winningMove)
                        return (i, k, num);
                }
            }
        }
        return null;
    }

    private (int r, int c, int num) ChooseRandomMove(NumericalTicTacToeGrid grid)
    {
        var playable = GetPlayableNumbers(grid).ToList();
        var free = new List<(int r, int c)>();

        for (int r = 0; r < grid.Rows; r++)
            for (int c = 0; c < grid.Columns; c++)
                if (grid.GetCell(r, c) == null)
                    free.Add((r, c));

        var (row, col) = free[_rng.Next(free.Count)];
        int num = playable[_rng.Next(playable.Count)];

        return (row, col, num);
    }
}