using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Core.Save;

namespace BoardGameFramework.Core;

public abstract class BoardGame
{
    protected Player[] Players;
    protected GameBoard Board = null!;
    protected MoveHistory History = new();
    public GameSave SaveManager { get; protected set; } = null!;
    protected HelpMenu HelpMenu = new();
    protected int CurrentPlayerIndex = 0;
    protected int TurnCount = 0;
    public bool IsGameOver { get; protected set; } = false;
    private bool _hasUnsavedMoves = false;

    protected BoardGame(Player[] players)
    {
        Players = players;
    }

    public void Run()
    {
        Initialise();
        RunGameLoop();
    }

    public void RunLoaded()
    {
        // Board state is already restored from the save file — skip Initialise()
        InitialiseHelpMenu();
        RunGameLoop();
    }

    private void RunGameLoop()
    {
        while (!IsGameOver)
        {
            Board.Display();
            string action = PromptAction();

            if (IsSystemCommand(action))
            {
                switch (action.ToUpper())
                {
                    case "H": HelpMenu.Show(); break;
                    case "S": HandleSave(); break;
                    case "U": HandleUndo(); break;
                    case "R": HandleRedo(); break;
                    case "Q": HandleQuit(); return;
                }
            }
            else
            {
                HandleMove(action);
                _hasUnsavedMoves = true; // mark unsaved after every move
            }
        }
        Board.Display();
        AnnounceResult();
    }

    // Override to setup HelpMenu without clearing board
    protected virtual void InitialiseHelpMenu() { }

    // Override in games where single letters can be valid moves (e.g. Gomoku H8)
    protected virtual bool IsSystemCommand(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;
        return input.Length == 1 &&
               "HSURQ".Contains(input.ToUpper());
    }

    protected virtual string PromptAction()
    {
        Console.Write($"\n{Players[CurrentPlayerIndex].Name}'s turn " +
            $"[move / H=help / S=save / U=undo / R=redo / Q=quit]: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    // Each subclass implements HandleMove — no shared logic possible here
    protected abstract void HandleMove(string input);

    // Calls OnBeforeSave() so each game can update its save state before writing
    protected virtual void HandleSave()
    {
        Console.Write("Enter save file name: ");
        string? name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name)) return;
        OnBeforeSave(CurrentPlayerIndex, TurnCount);
        SaveManager.SaveToFile(name);
        _hasUnsavedMoves = false; // game state is now saved
    }

    // Each game calls SetState on its own GameSave via this hook
    protected virtual void OnBeforeSave(int currentPlayerIndex, int turnCount) { }

    protected virtual void HandleUndo()
    {
        if (!History.CanUndo())
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        bool isHvC = Players.Any(p => !p.IsHuman);

        if (isHvC)
        {
            bool undidAtLeastOne = false;
            while (History.CanUndo())
            {
                MoveCommand? cmd = History.Undo();
                if (cmd == null) break;
                cmd.Reverse(Board);
                CurrentPlayerIndex = cmd.CurrentPlayerIndex;
                TurnCount = cmd.TurnCount;
                undidAtLeastOne = true;
                if (IsHumanPlayer(Players[CurrentPlayerIndex])) break;
            }
            if (undidAtLeastOne) Console.WriteLine("Undo successful.");
            else Console.WriteLine("Nothing to undo.");
        }
        else
        {
            MoveCommand? cmd = History.Undo();
            if (cmd == null) { Console.WriteLine("Nothing to undo."); return; }
            cmd.Reverse(Board);
            CurrentPlayerIndex = cmd.CurrentPlayerIndex;
            TurnCount = cmd.TurnCount;
            Console.WriteLine("Undo successful.");
        }
    }

    protected virtual bool IsHumanPlayer(Player player) => player.IsHuman;

    protected virtual void HandleRedo()
    {
        if (!History.CanRedo())
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        bool isHvC = Players.Any(p => !p.IsHuman);
        bool redidAtLeastOne = false;

        while (History.CanRedo())
        {
            MoveCommand? cmd = History.Redo();
            if (cmd == null) break;
            cmd.Execute(Board);
            OnAfterMove();
            TurnCount++;
            redidAtLeastOne = true;

            // Check game over after each redo, same as HandleMove does
            if (CheckGameOver())
            {
                IsGameOver = true;
                break;
            }

            SwitchTurn();

            // Stop when it's a human turn (or game over)
            if (!isHvC || IsHumanPlayer(Players[CurrentPlayerIndex])) break;
        }

        if (redidAtLeastOne)
        {
            Console.WriteLine("Redo successful.");
            if (IsGameOver)
            {
                Board.Display();
                AnnounceResult();
            }
        }
        else
            Console.WriteLine("Nothing to redo.");
    }

    protected virtual void HandleQuit()
    {
        if (_hasUnsavedMoves)
        {
            Console.Write("Save before quitting? [Y/N]: ");
            string? ans = Console.ReadLine()?.Trim().ToUpper();
            if (ans == "Y") HandleSave();
        }
        Console.WriteLine("Returning to main menu...");
    }

    protected void SwitchTurn()
        => CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Length;

    // Resets all shared state. Subclasses call this at the start of Initialise().
    protected void BaseInitialise()
    {
        Board.Clear();
        IsGameOver = false;
        CurrentPlayerIndex = 0;
        TurnCount = 0;
        // Clear in-place — GameSave holds a reference to this History object,
        // so creating a new instance would break the save manager's reference.
        History.Clear();
    }

    // Shared LoadGame logic — all subclasses restore the same state fields.
    // Each game's SaveManager already knows the correct types; no override needed.
    public void LoadGame(string path)
    {
        try
        {
            SaveManager.LoadFromFile(path);
            var (playerIndex, turnCount) = SaveManager.GetLoadedState();
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

    protected abstract void Initialise();
    protected abstract void OnAfterMove();
    protected abstract bool CheckGameOver();
    protected abstract void AnnounceResult();

    // Factory Method: each subclass decides which MoveCommand type to instantiate.
    // BoardGame depends only on the abstract MoveCommand — never on concrete types.
    protected abstract MoveCommand CreateMoveCommand(
        Player player, string gridSnapshot,
        int currentPlayerIndex, int turnCount);
}