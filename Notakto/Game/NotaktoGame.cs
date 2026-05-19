using BoardGameFramework.Core;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Notakto.Commands;
using BoardGameFramework.Notakto.Grid;
using BoardGameFramework.Notakto.Pieces;
using BoardGameFramework.Notakto.Players;
using BoardGameFramework.Notakto.Save;

namespace BoardGameFramework.Notakto.Game;

public class NotaktoGame : BoardGame
{
    private readonly NotaktoGrid _notaktoGrid;
    private readonly NotaktoGameSave _notaktoSave;

    private int? _winner = null;

    private int _lastBoard = -1;
    private int _lastRow = -1;
    private int _lastCol = -1;

    public NotaktoGame(Player[] players)
        : base(players)
    {
        _notaktoGrid = new NotaktoGrid();

        Board = _notaktoGrid;

        _notaktoSave = new NotaktoGameSave(
            _notaktoGrid,
            History,
            players);

        SaveManager = _notaktoSave;
    }

    protected override void InitialiseHelpMenu()
    {
        HelpMenu.AddCommand(
            "1 A1",
            "Place X on Board 1 at A1");

        HelpMenu.AddCommand(
            "U",
            "Undo last move");

        HelpMenu.AddCommand(
            "R",
            "Redo last undone move");

        HelpMenu.AddCommand(
            "S",
            "Save game");

        HelpMenu.AddCommand(
            "Q",
            "Quit to main menu");

        HelpMenu.AddCommand(
            "H",
            "Show help");

        Console.WriteLine(
            "=== NOTAKTO ===");

        Console.WriteLine(
            "Three 3x3 boards.");

        Console.WriteLine(
            "Both players place X.");

        Console.WriteLine(
            "Making 3-in-a-row kills a board.");

        Console.WriteLine(
            "Player who kills the LAST board loses.");
    }

    protected override void Initialise()
    {
        BaseInitialise();
        _winner = null;
        InitialiseHelpMenu();
    }

    protected override void OnBeforeSave(
        int currentPlayerIndex,
        int turnCount)
    {
        _notaktoSave.SetState(
            currentPlayerIndex,
            turnCount);
    }

    protected override string PromptAction()
    {
        var player = Players[CurrentPlayerIndex];

        if (player is NotaktoComputerPlayer computer)
        {
            computer.ChooseMove(_notaktoGrid);

            // Convert board/row/col back to "B P" format (position = row*3+col+1)
            int pos = computer.LastRow * 3 + computer.LastCol + 1;
            string coord = $"{computer.LastBoard + 1} {pos}";

            Console.WriteLine(
                $"{player.Name} played Board {computer.LastBoard + 1} position {pos}");

            return coord;
        }

        Console.Write(
            $"\n{player.Name}'s turn " +
            $"['B P' / H=help / S=save / U=undo / R=redo / Q=quit]: ");

        return Console.ReadLine()?.Trim() ?? "";
    }

    protected override MoveCommand CreateMoveCommand(
        Player player,
        string gridSnapshot,
        int currentPlayerIndex,
        int turnCount)
    {
        return new NotaktoMoveCommand(
            player,
            gridSnapshot,
            currentPlayerIndex,
            turnCount,
            _lastBoard,
            _lastRow,
            _lastCol);
    }

    protected override void HandleMove(string input)
    {
        var parts = input.Split(' ');

        if (parts.Length != 2)
        {
            Console.WriteLine(
                "Invalid input. Use format: [board 1-3] [position 1-9]");
            return;
        }

        if (!int.TryParse(parts[0], out int board))
        {
            Console.WriteLine(
                "Invalid board number.");
            return;
        }

        board--;

        if (board < 0 || board > 2)
        {
            Console.WriteLine(
                "Board must be 1, 2 or 3.");
            return;
        }

        if (_notaktoGrid.IsBoardDead(board))
        {
            Console.WriteLine(
                "That board is already dead.");
            return;
        }

        if (!int.TryParse(parts[1], out int position))
        {
            Console.WriteLine(
                "Invalid input. Use format: [board 1-3] [position 1-9]");
            return;
        }

        position--;
        if (position < 0 || position > 8)
        {
            Console.WriteLine(
                "Position must be 1-9.");
            return;
        }

        int row = position / 3;
        int col = position % 3;

        if (!_notaktoGrid.IsEmpty(
            board,
            row,
            col))
        {
            Console.WriteLine(
                "Cell already occupied.");
            return;
        }

        string snapshot =
            _notaktoGrid.ExportState();

        _notaktoGrid.PlacePiece(
            board,
            row,
            col,
            new NotaktoPiece(
                Players[CurrentPlayerIndex]
                    .PlayerNumber));

        _lastBoard = board;
        _lastRow = row;
        _lastCol = col;

        var cmd = CreateMoveCommand(
            Players[CurrentPlayerIndex],
            snapshot,
            CurrentPlayerIndex,
            TurnCount);

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

    protected override void OnAfterMove()
    {
    }

    protected override bool CheckGameOver()
    {
        if (_notaktoGrid.AreAllBoardsDead())
        {
            _winner =
                (CurrentPlayerIndex + 1) % 2;

            return true;
        }

        return false;
    }

    protected override void AnnounceResult()
    {
        if (_winner == null)
            return;

        var winner =
            Players[_winner.Value];

        Console.WriteLine(
            $"\n{winner.Name} wins!");

        Console.WriteLine(
            "Opponent killed the final board.");
    }

}