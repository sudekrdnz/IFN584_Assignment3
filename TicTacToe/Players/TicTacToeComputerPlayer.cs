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

        int move = FindWinningMove(grid);
        if (move == -1) move = ChooseRandomMove(grid);

        LastPosition = move;
        Console.WriteLine($"{Name} chose position {move}");
    }

    private int FindWinningMove(TicTacToeGrid grid)
    {
        string saved = grid.ExportState();

        for (int pos = 1; pos <= 9; pos++)
        {
            if (!grid.IsValidPosition(pos)) continue;

            grid.PlaceAtPosition(pos, new TicTacToePiece(PlayerNumber, Symbol));
            bool wins = grid.CheckWin(PlayerNumber);
            grid.ImportState(saved);

            if (wins) return pos;
        }

        return -1;
    }

    private int ChooseRandomMove(TicTacToeGrid grid)
    {
        var available = Enumerable.Range(1, 9)
            .Where(grid.IsValidPosition)
            .ToList();

        return available.Count > 0 ? available[_rng.Next(available.Count)] : -1;
    }
}