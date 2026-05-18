using BoardGameFramework.Core.Players;
using BoardGameFramework.Gomoku.Grid;
using BoardGameFramework.Gomoku.Pieces;

namespace BoardGameFramework.Gomoku.Players;

public class GomokuComputerPlayer : ComputerPlayer
{
    public int LastRow { get; private set; } = -1;
    public int LastCol { get; private set; } = -1;
    private readonly Random _rng = new();

    public GomokuComputerPlayer(string name, int playerNumber)
        : base(name, playerNumber) { }

    public void ChooseMove(GomokuGrid grid)
    {
        Console.WriteLine($"{Name} (Computer) is thinking...");
        Thread.Sleep(300);

        string saved = grid.ExportState();

        // 1. Win immediately
        var win = FindWinningMove(grid, PlayerNumber, saved);
        if (win.HasValue)
        {
            LastRow = win.Value.r;
            LastCol = win.Value.c;
            Console.WriteLine(
                $"{Name} played " +
                $"{(char)('A' + LastCol)}{LastRow + 1}");
            return;
        }

        // 2. Block opponent
        int opponent = PlayerNumber == 1 ? 2 : 1;
        var block = FindWinningMove(grid, opponent, saved);
        if (block.HasValue)
        {
            LastRow = block.Value.r;
            LastCol = block.Value.c;
            Console.WriteLine(
                $"{Name} played " +
                $"{(char)('A' + LastCol)}{LastRow + 1}");
            return;
        }

        // 3. Play near existing pieces
        var near = FindNearMove(grid);
        if (near.HasValue)
        {
            LastRow = near.Value.r;
            LastCol = near.Value.c;
            Console.WriteLine(
                $"{Name} played " +
                $"{(char)('A' + LastCol)}{LastRow + 1}");
            return;
        }

        // 4. Random fallback
        var rand = FindRandomMove(grid);
        LastRow = rand.r;
        LastCol = rand.c;
        Console.WriteLine(
            $"{Name} played " +
            $"{(char)('A' + LastCol)}{LastRow + 1}");
    }

    private (int r, int c)? FindWinningMove(
        GomokuGrid grid, int player, string saved)
    {
        for (int r = 0; r < grid.Rows; r++)
            for (int c = 0; c < grid.Columns; c++)
                if (grid.IsEmpty(r, c))
                {
                    grid.PlacePiece(r, c,
                        new GomokuPiece(
                            player, 
                            player == 1 ? 'X' : 'O'));
                    bool wins = grid.CheckWin(player);
                    grid.ImportState(saved);
                    if (wins) return (r, c);
                }
        return null;
    }

    private (int r, int c)? FindNearMove(GomokuGrid grid)
    {
        var candidates = new List<(int r, int c)>();
        int[] d = [-1, 0, 1];

        for (int r = 0; r < grid.Rows; r++)
            for (int c = 0; c < grid.Columns; c++)
                if (!grid.IsEmpty(r, c))
                    foreach (int dr in d)
                        foreach (int dc in d)
                        {
                            int nr = r + dr, nc = c + dc;
                            if (nr >= 0 && nr < grid.Rows
                                && nc >= 0 
                                && nc < grid.Columns
                                && grid.IsEmpty(nr, nc))
                                candidates.Add((nr, nc));
                        }

        if (candidates.Count == 0) return null;
        return candidates[_rng.Next(candidates.Count)];
    }

    private (int r, int c) FindRandomMove(GomokuGrid grid)
    {
        var empty = new List<(int r, int c)>();
        for (int r = 0; r < grid.Rows; r++)
            for (int c = 0; c < grid.Columns; c++)
                if (grid.IsEmpty(r, c))
                    empty.Add((r, c));
        return empty[_rng.Next(empty.Count)];
    }
}