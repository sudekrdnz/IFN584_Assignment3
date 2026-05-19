using BoardGameFramework.Core;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.NumericalTicTacToe.Commands;
using BoardGameFramework.NumericalTicTacToe.Grid;
using BoardGameFramework.NumericalTicTacToe.Pieces;
using BoardGameFramework.NumericalTicTacToe.Players;
using BoardGameFramework.NumericalTicTacToe.Save;
using System.IO;

namespace BoardGameFramework.NumericalTicTacToe.Game;

public class NumericalTicTacToeGame : BoardGame
{
    private readonly NumericalTicTacToeGrid _nttGrid;
    private readonly NumericalTicTacToeSave _nttSave;
    private int _winner = 0;

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
        InitialiseHelpMenu();
        Console.WriteLine($"=== Numerical Tic Tac Toe ({_nttGrid.Rows}x{_nttGrid.Columns}) ===");
        Console.WriteLine($"Sum required to win: {_nttGrid.SumToWin}");
    }

    protected override void InitialiseHelpMenu()
    {
        HelpMenu.AddCommand("1,9", "Use format: row,col=value");
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
            return $"{computer.LastRow},{computer.LastColumn}={computer.LastValue}";
        }
        Console.Write($"{player.Name}, enter your move (row,col=value): ");
        Console.WriteLine($"{player.Name} your numbers are: {string.Join(", ", nums)}");
        return Console.ReadLine()?.Trim() ?? "";
    }

    protected override void HandleMove(string input)
    {
        var player = Players[CurrentPlayerIndex];
        int row;
        int col;
        int num;

        try
        {
            string[] inputArray = input.Split("=");

            num = int.Parse(inputArray[1]);

            string[] indices = inputArray[0].Split(",");

            row = int.Parse(indices[0]);
            col = int.Parse(indices[1]);
        }
        catch (FormatException)
        {
            Console.WriteLine("Incorrect format. Use row,col=num");
            return;
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Incorrect format. Use row,col=num");
            return;
        }
        //Check if within board's bounds
        if (row < 0 || row >= _nttGrid.Rows || col < 0 || col >= _nttGrid.Columns)
        {
            Console.WriteLine("Out of bounds. Try again.");
            return;
        }

        //Check if cells are full
        if (_nttGrid.GetCell(row, col) != null)
        {
            Console.WriteLine("The cell is full. Try again.");
            return;
        }

        //Check if player numbers are within board's range
        int max = _nttGrid.Rows * _nttGrid.Columns;
        if (num < 1 || num > max)
        {
            Console.WriteLine($"You must input a number between 1 and {max}.");
            return;
        }

        //Check if numbers belong to player
        var playableNumber = _nttGrid.GeneratePlayableNumbers(player.PlayerNumber);

        if (!playableNumber.Contains(num))
        {
            Console.WriteLine("That number is not available. Try again.");
            return;
        }

        //Save current snapshot of the grid
        string snapshot = _nttGrid.ExportState();

        //Move number onto the grid
        _nttGrid.PlacePiece(row, col,
        new NumericalTicTacToePiece(player.PlayerNumber, num));

        //To be used by MoveCommand
        _lastRow = row;
        _lastCol = col;
        _lastNumber = num;

        var cmd = CreateMoveCommand(player, snapshot, CurrentPlayerIndex, TurnCount);

        OnAfterMove();
        History.Record(cmd);
        TurnCount++;

        //Check if game has ended
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
            _winner = 0;
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
            var winner = Players[_winner - 1];
            Console.WriteLine($"\n Player {winner.Name} is the winner!");
        }
    }

    public void LoadGame(string path)
    {
        try
        {
            _nttSave.LoadFromFile(path);

            var (playerIndex, turnCount) = _nttSave.GetLoadedState();
            CurrentPlayerIndex = playerIndex;
            TurnCount = turnCount;
            Console.WriteLine("Game loaded successfully!");
        }
        catch (InvalidOperationException _exception)
        {
            Console.WriteLine($"Error: {_exception.Message}");
            throw;
        }
    }
}