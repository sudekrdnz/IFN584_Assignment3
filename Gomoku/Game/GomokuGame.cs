using BoardGameFramework.Core;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Gomoku.Commands;
using BoardGameFramework.Gomoku.Grid;
using BoardGameFramework.Gomoku.Pieces;
using BoardGameFramework.Gomoku.Players;
using BoardGameFramework.Gomoku.Save;

namespace BoardGameFramework.Gomoku.Game;

public class GomokuGame : BoardGame
{
    private readonly GomokuGrid _gomokuGrid;
    private readonly GomokuGameSave _gomokuSave;
    private int? _winner = null;
    private int _lastRow = -1;
    private int _lastCol = -1;
    private char _lastSymbol = ' ';

    public GomokuGame(Player[] players, int size = 15)
        : base(players)
    {
        _gomokuGrid = new GomokuGrid(size, size);
        Board = _gomokuGrid;
        _gomokuSave = new GomokuGameSave(
            _gomokuGrid, History, players);
        SaveManager = _gomokuSave;
    }

    // CRITICAL — override so H8 is not caught as Help
    protected override bool IsSystemCommand(string input)
    {
        return input.Length == 1
            && "HSURQ".Contains(input.ToUpper());
    }

    protected override void InitialiseHelpMenu()
    {
        HelpMenu.AddCommand("A1-O15", 
            "Place piece using coordinate (e.g. H8)");
        HelpMenu.AddCommand("U", "Undo last move");
        HelpMenu.AddCommand("R", "Redo last undone move");
        HelpMenu.AddCommand("S", "Save game");
        HelpMenu.AddCommand("Q", "Quit to main menu");
        HelpMenu.AddCommand("H", "Show help");
        HelpMenu.AddExample("H8  → place piece at column H, row 8");
        HelpMenu.AddExample("U   → undo your last move");
        Console.WriteLine(
            $"=== Gomoku " +
            $"({_gomokuGrid.Rows}x{_gomokuGrid.Columns})" +
            $" — Resumed ===");
        Console.WriteLine(
            $"{Players[0].Name} = X    " +
            $"{Players[1].Name} = O");
    }

    protected override void Initialise()
    {
        BaseInitialise();
        InitialiseHelpMenu();
        Console.WriteLine(
            $"=== Gomoku " +
            $"({_gomokuGrid.Rows}x{_gomokuGrid.Columns}) ===");
        Console.WriteLine(
            $"{Players[0].Name} = X    " +
            $"{Players[1].Name} = O");
    }

    protected override void OnBeforeSave(
        int currentPlayerIndex, int turnCount)
        => _gomokuSave.SetState(currentPlayerIndex, turnCount);

    protected override string PromptAction()
    {
        var player = Players[CurrentPlayerIndex];
        if (player is GomokuComputerPlayer computer)
        {
            computer.ChooseMove(_gomokuGrid);
            // Convert row/col back to coordinate string
            string coord =
                $"{(char)('A' + computer.LastCol)}" +
                $"{computer.LastRow + 1}";
            return coord;
        }
        Console.Write(
            $"\n{player.Name}'s turn " +
            $"[coordinate e.g. H8 / " +
            $"H=help / S=save / U=undo / " +
            $"R=redo / Q=quit]: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    protected override MoveCommand CreateMoveCommand(
        Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount)
        => new GomokuMoveCommand(
            player, gridSnapshot,
            currentPlayerIndex, turnCount,
            _lastRow, _lastCol, _lastSymbol);

    protected override void HandleMove(string input)
    {
        var player = Players[CurrentPlayerIndex];

        if (!_gomokuGrid.TryParseCoordinate(
            input, out int row, out int col))
        {
            Console.WriteLine(
                "Invalid coordinate. " +
                "Use format like H8 " +
                $"(A-{_gomokuGrid.ColLabels[^1]}, " +
                $"1-{_gomokuGrid.Rows})");
            return;
        }

        if (!_gomokuGrid.IsEmpty(row, col))
        {
            Console.WriteLine(
                "That cell is already occupied. " +
                "Try again.");
            return;
        }

        string snapshot = _gomokuGrid.ExportState();
        char symbol = player.PlayerNumber == 1 
            ? 'X' : 'O';

        _gomokuGrid.PlacePiece(row, col,
            new GomokuPiece(player.PlayerNumber, symbol));

        if (player is GomokuHumanPlayer human)
            human.SetMove(row, col);

        _lastRow = row;
        _lastCol = col;
        _lastSymbol = symbol;

        var cmd = CreateMoveCommand(
            player, snapshot, 
            CurrentPlayerIndex, TurnCount);

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
            if (_gomokuGrid.CheckWin(player.PlayerNumber))
            {
                _winner = player.PlayerNumber;
                return true;
            }
        }
        if (_gomokuGrid.IsFull()) { _winner = 0; return true; }
        return false;
    }

    protected override void AnnounceResult()
    {
        if (_winner == 0)
            Console.WriteLine("It's a draw!");
        else
        {
            var winner = Players.First(
                p => p.PlayerNumber == _winner);
            Console.WriteLine(
                $"\n{winner.Name} wins! Congratulations!");
        }
    }

}