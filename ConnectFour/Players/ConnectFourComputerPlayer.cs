using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.ConnectFour.Grid;
using BoardGameFramework.ConnectFour.Pieces;

namespace BoardGameFramework.ConnectFour.Players;

public class ConnectFourComputerPlayer : ComputerPlayer
{
    public int LastColumn { get; private set; } = -1;
    private readonly char _symbol;
    private readonly Random _rng = new();

    public ConnectFourComputerPlayer(string name, int playerNumber, char symbol)
        : base(name, playerNumber)
    {
        _symbol = symbol;
    }

    public void ChooseMove(ConnectFourGrid grid)
    {
        Console.WriteLine($"{Name} (Computer) is thinking...");
        Thread.Sleep(300);

        // 1. Win immediately
        int col = FindWinningMove(grid, PlayerNumber);
        if (col != -1) { LastColumn = col; Console.WriteLine($"{Name} chose column {col + 1}"); return; }

        // 2. Block opponent from winning
        int opponent = PlayerNumber == 1 ? 2 : 1;
        char opponentSymbol = opponent == 1 ? 'X' : 'O';
        col = FindWinningMove(grid, opponent, opponentSymbol);
        if (col != -1) { LastColumn = col; Console.WriteLine($"{Name} chose column {col + 1}"); return; }

        // 3. Random fallback
        col = ChooseRandomMove(grid);
        LastColumn = col;
        Console.WriteLine($"{Name} chose column {col + 1}");
    }

    private int FindWinningMove(ConnectFourGrid grid, int playerNum, char? sym = null)
    {
        char symbol = sym ?? _symbol;
        string saved = grid.ExportState();
        for (int c = 0; c < grid.Columns; c++)
        {
            if (grid.IsColumnFull(c)) continue;
            var piece = new ConnectFourPiece(playerNum, symbol);
            grid.DropDisc(c, piece);
            bool wins = grid.CheckWin(playerNum);
            grid.ImportState(saved);
            if (wins) return c;
        }
        return -1;
    }

    private int ChooseRandomMove(ConnectFourGrid grid)
    {
        var available = Enumerable.Range(0, grid.Columns)
            .Where(c => !grid.IsColumnFull(c)).ToList();
        return available.Count > 0 ? available[_rng.Next(available.Count)] : -1;
    }
}