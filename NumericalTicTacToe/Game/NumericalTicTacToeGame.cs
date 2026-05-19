using BoardGameFramework.Core;
using BoardGameFramework.Core.Exceptions;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.NumericalTicTacToe.Commands;
using BoardGameFramework.NumericalTicTacToe.Grid;
using BoardGameFramework.NumericalTicTacToe.Pieces;
using BoardGameFramework.NumericalTicTacToe.Players;
using BoardGameFramework.NumericalTicTacToe.Save;

namespace BoardGameFramework.NumericalTicTacToe.Game;

public class NumericalTicTacToeGame : BoardGame
{
    private readonly NumericalTicTacToeGrid _nttGrid;
    private readonly NumericalTicTacToeSave _nttSave;
    private int? _winner = null;

    private int _lastRow;
    private int _lastCol;
    private int _lastNumber;


    public NumericalTicTacToeGame(Player[] players) : base(players)
    {
        _nttGrid = new NumericalTicTacToeGrid(3);
        Board = _nttGrid;

        _nttSave = new NumericalTicTacToeSave(_nttGrid, History, players);
        SaveManager = _nttSave;
    }

    protected override void Initialise()
    {
        BaseInitialise();
        _winner = null;
        InitialiseHelpMenu();
        Console.WriteLine($"=== Numerical Tic Tac Toe ({_nttGrid.Rows}x{_nttGrid.Columns}) ===");
        Console.WriteLine($"Sum required to win: {_nttGrid.SumToWin}");
    }

    protected override void OnBeforeSave(int currentPlayerIndex, int turnCount)
        => _nttSave.SetState(currentPlayerIndex, turnCount);

    protected override void InitialiseHelpMenu()
    {
        HelpMenu.Clear();
        HelpMenu.AddCommand("pos=value", "Place number at position 1-9, e.g. 5=7");
        HelpMenu.AddCommand("U", "Undo last move");
        HelpMenu.AddCommand("R", "Redo last undone move");
        HelpMenu.AddCommand("S", "Save game");
        HelpMenu.AddCommand("Q", "Quit to main menu");
        HelpMenu.AddCommand("H", "Show help");
    }

    protected override string PromptAction()
    {
        var player = Players[CurrentPlayerIndex];
        var nums = _nttGrid.GeneratePlayableNumbers(player.PlayerNumber);

        if (player is NumericalTicTacToeComputerPlayer computer)
        {
            computer.ChooseMove(_nttGrid);
            // Convert row/col to 1-based position
            int pos = computer.LastRow * _nttGrid.Columns + computer.LastColumn + 1;
            return $"{pos}={computer.LastValue}";
        }
        Console.WriteLine($"{player.Name}, your numbers: {string.Join(", ", nums)}");
        Console.Write(
            $"{player.Name}'s turn " +
            $"[pos=value / H=help / S=save / U=undo / R=redo / Q=quit]: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    protected override void HandleMove(string input)
    {
        var player = Players[CurrentPlayerIndex];
        int row;
        int col;
        int num;

        // Input format: position=value  e.g. "5=7"
        // position: 1-9 (top-left to bottom-right), value: player's available number
        try
        {
            string[] parts = input.Split("=");
            int position = int.Parse(parts[0]) - 1; // 0-based
            num = int.Parse(parts[1]);
            row = position / _nttGrid.Columns;
            col = position % _nttGrid.Columns;
        }
        catch (FormatException)
        {
            throw new InvalidMoveException("Incorrect format. Use position=value  e.g. 5=7");
        }
        catch (IndexOutOfRangeException)
        {
            throw new InvalidMoveException("Incorrect format. Use position=value  e.g. 5=7");
        }
        if (row < 0 || row >= _nttGrid.Rows || col < 0 || col >= _nttGrid.Columns)
            throw new InvalidMoveException("Out of bounds. Use position 1-9.");

        if (_nttGrid.GetCell(row, col) != null)
            throw new InvalidMoveException("The cell is full. Try again.");

        int max = _nttGrid.Rows * _nttGrid.Columns;
        if (num < 1 || num > max)
            throw new InvalidMoveException($"You must input a number between 1 and {max}.");

        var playableNumber = _nttGrid.GeneratePlayableNumbers(player.PlayerNumber);

        if (!playableNumber.Contains(num))
            throw new InvalidMoveException("That number is not available. Try again.");

        string snapshot = _nttGrid.ExportState();

        _nttGrid.PlacePiece(row, col,
        new NumericalTicTacToePiece(player.PlayerNumber, num));

        _lastRow = row;
        _lastCol = col;
        _lastNumber = num;

        var cmd = CreateMoveCommand(player, snapshot, CurrentPlayerIndex, TurnCount);

        OnAfterMove();
        History.Record(cmd);
        TurnCount++;

        if (CheckGameOver())
        {
            IsGameOver = true;
            return;
        }

        SwitchTurn();
    }

    protected override MoveCommand CreateMoveCommand(Player player, string gridSnapshot, int currentPlayerIndex, int turnCount)
    => new NumericalTicTacToeMoveCommand(player, gridSnapshot, currentPlayerIndex, turnCount, _lastRow, _lastCol, _lastNumber);

    protected override void OnAfterMove()
    {

    }

    protected override bool CheckGameOver()
    {
        foreach (var player in Players)
        {
            if (_nttGrid.CheckWin(player.PlayerNumber))
            {
                _winner = player.PlayerNumber;
                return true;
            }
        }
        if (_nttGrid.IsFull())
        {
            _winner = 0; // 0 = draw
            return true;
        }
        return false;
    }

    protected override void AnnounceResult()
    {
        if (_winner == 0)
        {
            Console.WriteLine("It's a draw!");
        }
        else
        {
            var winner = Players[_winner!.Value - 1];
            Console.WriteLine($"\n Player {winner.Name} is the winner!");
        }
    }

}