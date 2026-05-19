using BoardGameFramework.Core;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.TicTacToe.Commands;
using BoardGameFramework.TicTacToe.Grid;
using BoardGameFramework.TicTacToe.Players;
using BoardGameFramework.TicTacToe.Save;

namespace BoardGameFramework.TicTacToe.Game;

public class TicTacToeGame : BoardGame
{
    private int _winner = 0;
    private bool _draw = false;
    private int _pendingPosition = -1;

    public TicTacToeGame(Player[] players) : base(players)
    {
        Board = new TicTacToeGrid();
        SaveManager = new TicTacToeGameSave((TicTacToeGrid)Board, History, Players);
    }

    protected override void Initialise()
    {
        BaseInitialise();
        InitialiseHelpMenu();
        _winner = 0;
        _draw = false;
        _pendingPosition = -1;
    }

    protected override void InitialiseHelpMenu()
    {
        HelpMenu.AddCommand("1-9", "Place your piece in a numbered square.");
        HelpMenu.AddCommand("S", "Save the current game.");
        HelpMenu.AddCommand("U", "Undo move.");
        HelpMenu.AddCommand("R", "Redo move.");
        HelpMenu.AddCommand("Q", "Quit to main menu.");
        HelpMenu.AddExample("Example move: 5");
        HelpMenu.AddExample("Example winning line: 1, 2, 3");
    }

    protected override string PromptAction()
    {
        if (!Players[CurrentPlayerIndex].IsHuman)
            return "AUTO";

        return base.PromptAction();
    }

    protected override void HandleMove(string input)
    {
        var grid = (TicTacToeGrid)Board;
        Player player = Players[CurrentPlayerIndex];

        int position;

        if (player is TicTacToeComputerPlayer computer)
        {
            computer.ChooseMove(grid);
            position = computer.LastPosition;
        }
        else if (!int.TryParse(input, out position))
        {
            Console.WriteLine("Invalid input. Enter a number from 1 to 9.");
            return;
        }

        if (!grid.IsValidPosition(position))
        {
            Console.WriteLine("Invalid move. Choose an empty position from 1 to 9.");
            return;
        }

        _pendingPosition = position;

        string snapshot = Board.ExportState();
        MoveCommand command = CreateMoveCommand(player, snapshot, CurrentPlayerIndex, TurnCount);
        command.Execute(Board);
        History.Record(command);

        OnAfterMove();
        TurnCount++;

        if (CheckGameOver())
        {
            IsGameOver = true;
            return;
        }

        SwitchTurn();
    }

    protected override MoveCommand CreateMoveCommand(
        Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount)
    {
        char symbol = player.PlayerNumber == 1 ? 'X' : 'O';

        return new TicTacToeMoveCommand(
            player,
            gridSnapshot,
            currentPlayerIndex,
            turnCount,
            _pendingPosition,
            symbol);
    }

    protected override void OnAfterMove()
    {
        _pendingPosition = -1;
    }

    protected override bool CheckGameOver()
    {
        Player current = Players[CurrentPlayerIndex];

        if (Board.CheckWin(current.PlayerNumber))
        {
            _winner = current.PlayerNumber;
            return true;
        }

        if (Board.IsFull())
        {
            _draw = true;
            return true;
        }

        return false;
    }

    protected override void AnnounceResult()
    {
        if (_draw)
        {
            Console.WriteLine("Game over: Draw.");
            return;
        }

        Player winner = Players.First(p => p.PlayerNumber == _winner);
        Console.WriteLine($"Game over: {winner.Name} wins!");
    }

    protected override void OnBeforeSave(int currentPlayerIndex, int turnCount)
    {
        SaveManager.SetState(currentPlayerIndex, turnCount);
    }

    public void LoadGame(string name)
    {
        SaveManager.LoadFromFile(name);
        var state = SaveManager.GetLoadedState();
        CurrentPlayerIndex = state.playerIndex;
        TurnCount = state.turnCount;
    }
}