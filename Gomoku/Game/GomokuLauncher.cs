using BoardGameFramework.Core.Players;
using BoardGameFramework.Gomoku.Players;

namespace BoardGameFramework.Gomoku.Game;

public static class GomokuLauncher
{
    public static void Run()
    {
        GameRunner.ShowGameHeader("GOMOKU",
            "15×15 board. First to 5 in a row wins.",
            "Enter coordinate e.g. H8.");
        string? mode = GameRunner.ReadMode();
        if (mode == null) return;
        if (mode == "3") { Load(); return; }

        int size = GameRunner.ReadBoardSize(9, 19, 15, "board size (9-19)");
        string p1Name = GameRunner.ReadName("Player 1");
        Player[] players = mode == "2"
            ?
            [
                new GomokuHumanPlayer(p1Name, 1),
                new GomokuComputerPlayer("Computer", 2)
            ]
            :
            [
                new GomokuHumanPlayer(p1Name, 1),
                new GomokuHumanPlayer(
                    GameRunner.ReadName("Player 2"), 2)
            ];

        Console.Clear();
        new GomokuGame(players, size).Run();
        GameRunner.Pause();
    }

    private static void Load()
    {
        var temp = new GomokuGame(
            GameRunner.TempHumanPlayers(
                (n, i) => new GomokuHumanPlayer(n, i)));

        string? name = GameRunner.ReadSaveName(temp.SaveManager.GameType);
        if (name == null) return;

        if (!GameRunner.TryReadInfo(
            temp.SaveManager.ReadPlayerInfo, name,
            out var names, out var types)) return;

        Player[] players = GameRunner.BuildPlayers(
            names, types,
            (n, i) => new GomokuHumanPlayer(n, i),
            (n, i) => new GomokuComputerPlayer(n, i));

        Console.Clear();
        var game = new GomokuGame(players);
        GameRunner.TryLoad(game.LoadGame, name);
        if (!game.IsGameOver) game.RunLoaded();
        GameRunner.Pause();
    }
}