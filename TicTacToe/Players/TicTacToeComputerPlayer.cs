using BoardGameFramework.Core.Players;
using BoardGameFramework.TicTacToe.Grid;
using BoardGameFramework.TicTacToe.Pieces;

namespace BoardGameFramework.TicTacToe.Players;

public class TicTacToeComputerPlayer : ComputerPlayer
{
    public char Symbol { get; }
    public int LastPosition { get; private set; } = -1;
    private readonly Random _rng = new();

    public TicTacToeComputerPlayer(string name, int playerNumber)
        : base(name, playerNumber)
    {
        Symbol = playerNumber == 1 ? 'X' : 'O';
    }

    public void ChooseMove(TicTacToeGrid grid)
    {
        Console.WriteLine($"{Name} (Computer) is thinking...");
        Thread.Sleep(300);

        // 1. Win immediately
        int move = FindWinningMove(grid, PlayerNumber, Symbol);
        if (move != -1) { LastPosition = move; Console.WriteLine($"{Name} chose position {move}"); return; }

        // 2. Block opponent from winning
        int opponent = PlayerNumber == 1 ? 2 : 1;
        char opponentSymbol = opponent == 1 ? 'X' : 'O';
        move = FindWinningMove(grid, opponent, opponentSymbol);
        if (move != -1) { LastPosition = move; Console.WriteLine($"{Name} chose position {move}"); return; }

        // 3. Random fallback
        move = ChooseRandomMove(grid);
        LastPosition = move;
        Console.WriteLine($"{Name} chose position {move}");
    }

    private int FindWinningMove(TicTacToeGrid grid, int playerNum, char sym)
    {
        string saved = grid.ExportState();

        for (int pos = 1; pos <= grid.Rows * grid.Columns; pos++)
        {
            if (!grid.IsValidPosition(pos)) continue;

            grid.PlaceAtPosition(pos, new TicTacToePiece(playerNum, sym));
            bool wins = grid.CheckWin(playerNum);
            grid.ImportState(saved);

            if (wins) return pos;
        }

        return -1;
    }

    private int ChooseRandomMove(TicTacToeGrid grid)
    {
        var available = Enumerable.Range(1, grid.Rows * grid.Columns)
            .Where(grid.IsValidPosition)
            .ToList();

        return available.Count > 0 ? available[_rng.Next(available.Count)] : -1;
    }
}