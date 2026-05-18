using BoardGameFramework.Core;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.ConnectFour.Commands;
using BoardGameFramework.ConnectFour.Grid;
using BoardGameFramework.ConnectFour.Pieces;
using BoardGameFramework.ConnectFour.Players;
using BoardGameFramework.ConnectFour.Save;

namespace BoardGameFramework.ConnectFour.Game;

public class ConnectFourGame : BoardGame
{
    private readonly ConnectFourGrid _cfGrid;
    private readonly ConnectFourGameSave _cfSave;
    private int? _winner = null;
    private int _lastCol = -1;
    private int _lastRow = -1;
    private char _lastSymbol = ' ';

    public ConnectFourGame(Player[] players, int rows = 6, int cols = 7) : base(players)
    {
        _cfGrid = new ConnectFourGrid(rows, cols);
        Board = _cfGrid;
        _cfSave = new ConnectFourGameSave(_cfGrid, History, players);
        SaveManager = _cfSave;
    }

    protected override void InitialiseHelpMenu()
    {
        HelpMenu.AddCommand($"1-{_cfGrid.Columns}", $"Drop disc into column 1-{_cfGrid.Columns}");
        HelpMenu.AddCommand("U", "Undo last move");
        HelpMenu.AddCommand("R", "Redo last undone move");
        HelpMenu.AddCommand("S", "Save game");
        HelpMenu.AddCommand("Q", "Quit to main menu");
        HelpMenu.AddCommand("H", "Show help");
        HelpMenu.AddExample("4  → drop into column 4");
        HelpMenu.AddExample("U  → undo your last move");
        Console.WriteLine($"=== Connect Four ({_cfGrid.Rows}x{_cfGrid.Columns}) — Resumed ===");
        Console.WriteLine($"{Players[0].Name} = X    {Players[1].Name} = O");
    }

    protected override void Initialise()
    {
        BaseInitialise();
        InitialiseHelpMenu();
        Console.WriteLine($"=== Connect Four ({_cfGrid.Rows}x{_cfGrid.Columns}) ===");
        Console.WriteLine($"{Players[0].Name} = X    {Players[1].Name} = O");
    }

    protected override void OnBeforeSave(int currentPlayerIndex, int turnCount)
        => _cfSave.SetState(currentPlayerIndex, turnCount);

    protected override string PromptAction()
    {
        var player = Players[CurrentPlayerIndex];
        if (player is ConnectFourComputerPlayer computer)
        {
            computer.ChooseMove(_cfGrid);
            return computer.LastColumn.ToString();
        }
        Console.Write($"\n{player.Name}'s turn " +
            $"[1-{_cfGrid.Columns} / H=help / S=save / U=undo / R=redo / Q=quit]: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    // ── Factory Method ────────────────────────────────────────────────────
    protected override MoveCommand CreateMoveCommand(
        Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount)
        => new ConnectFourMoveCommand(player, gridSnapshot,
            currentPlayerIndex, turnCount, _lastCol, _lastRow, _lastSymbol);

    protected override void HandleMove(string input)
    {
        var player = Players[CurrentPlayerIndex];

        if (player is ConnectFourHumanPlayer human)
        {
            if (!human.ValidateInput(input, _cfGrid))
            {
                Console.WriteLine($"Invalid. Enter column 1-{_cfGrid.Columns} (must not be full).");
                return;
            }
            human.SetColumn(int.Parse(input) - 1);
        }

        string snapshot = _cfGrid.ExportState();
        char symbol = player.PlayerNumber == 1 ? 'X' : 'O';
        int col = player switch
        {
            ConnectFourHumanPlayer h => h.LastColumn,
            ConnectFourComputerPlayer c => c.LastColumn,
            _ => throw new InvalidOperationException("Unknown player type")
        };

        var piece = new ConnectFourPiece(player.PlayerNumber, symbol);
        int row = _cfGrid.DropDisc(col, piece);

        if (row == -1) { Console.WriteLine("Column is full. Try again."); return; }

        // Store for Factory Method
        _lastCol = col;
        _lastRow = row;
        _lastSymbol = symbol;

        // Use Factory Method to create command
        var cmd = CreateMoveCommand(player, snapshot, CurrentPlayerIndex, TurnCount);

        OnAfterMove();
        History.Record(cmd);
        TurnCount++;

        if (CheckGameOver()) { IsGameOver = true; return; }
        SwitchTurn();
    }

    protected override void OnAfterMove() { }

    protected override bool CheckGameOver()
    {
        foreach (var player in Players)
        {
            if (_cfGrid.CheckWin(player.PlayerNumber))
            { _winner = player.PlayerNumber; return true; }
        }
        if (_cfGrid.IsFull()) { _winner = 0; return true; }
        return false;
    }

    protected override void AnnounceResult()
    {
        if (_winner == 0) Console.WriteLine("It's a draw!");
        else
        {
            var winner = Players.First(p => p.PlayerNumber == _winner);
            Console.WriteLine($"\n{winner.Name} wins! Congratulations!");
        }
    }

    public void LoadGame(string path)
    {
        try
        {
            _cfSave.LoadFromFile(path);
            var (playerIndex, turnCount) = _cfSave.GetLoadedState();
            CurrentPlayerIndex = playerIndex;
            TurnCount = turnCount;
            Console.WriteLine("Game loaded successfully!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
