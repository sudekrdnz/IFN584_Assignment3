using BoardGameFramework.Core.Players;
using BoardGameFramework.TicTacToe.Players;

namespace BoardGameFramework.TicTacToe.Game;

public static class TicTacToeLauncher
{
    public static void Run()
    {
        GameRunner.ShowGameHeader("TIC-TAC-TOE",
            "3×3 board. First to 3 in a row wins.",
            "Enter position 1-9.");

        string? mode = GameRunner.ReadMode();
        if (mode == null) return;
        if (mode == "3") { Load(); return; }

        string p1Name = GameRunner.ReadName("Player 1");

        Player[] players = mode == "2"
            ?
            [
                new TicTacToeHumanPlayer(p1Name, 1),
                new TicTacToeComputerPlayer("Computer", 2)
            ]
            :
            [
                new TicTacToeHumanPlayer(p1Name, 1),
                new TicTacToeHumanPlayer(GameRunner.ReadName("Player 2"), 2)
            ];

        Console.Clear();
        new TicTacToeGame(players).Run();
        GameRunner.Pause();
    }

    private static void Load()
    {
        var temp = new TicTacToeGame(
        GameRunner.TempHumanPlayers((n, i) => new TicTacToeHumanPlayer(n, i)));

        string? name = GameRunner.ReadSaveName(temp.SaveManager.GameType);
        if (name == null) return;

        if (!GameRunner.TryReadInfo(temp.SaveManager.ReadPlayerInfo, name,
            out var names, out var types)) return;

        Player[] players = GameRunner.BuildPlayers(names, types,
            (n, i) => new TicTacToeHumanPlayer(n, i),
            (n, i) => new TicTacToeComputerPlayer(n, i));

        Console.Clear();

        var game = new TicTacToeGame(players);
        GameRunner.TryLoad(game.LoadGame, name);

        if (!game.IsGameOver)
            game.RunLoaded();

        GameRunner.Pause();
    }
}