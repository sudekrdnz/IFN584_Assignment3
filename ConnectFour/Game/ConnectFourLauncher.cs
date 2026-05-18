using BoardGameFramework.ConnectFour.Players;
using BoardGameFramework.Core.Players;

namespace BoardGameFramework.ConnectFour.Game;

public static class ConnectFourLauncher
{
    public static void Run()
    {
        GameRunner.ShowGameHeader("CONNECT FOUR",
            "6×7 board. First to 4 in a row wins.",
            "Enter column 1-7.");
        string? mode = GameRunner.ReadMode();
        if (mode == null) return;
        if (mode == "3") { Load(); return; }

        string p1Name = GameRunner.ReadName("Player 1");
        Player[] players = mode == "2"
            ?
            [
                new ConnectFourHumanPlayer(p1Name, 1),
                new ConnectFourComputerPlayer("Computer", 2, 'O')
            ]
            :
            [
                new ConnectFourHumanPlayer(p1Name, 1),
                new ConnectFourHumanPlayer(GameRunner.ReadName("Player 2"), 2)
            ];

        Console.Clear();
        new ConnectFourGame(players).Run();
        GameRunner.Pause();
    }

    private static void Load()
    {
        var temp = new ConnectFourGame(
            GameRunner.TempHumanPlayers((n, i) => new ConnectFourHumanPlayer(n, i)));
        string? name = GameRunner.ReadSaveName(temp.SaveManager.GameType); if (name == null) return;
        if (!GameRunner.TryReadInfo(temp.SaveManager.ReadPlayerInfo, name,
            out var names, out var types)) return;

        Player[] players = GameRunner.BuildPlayers(names, types,
            (n, i) => new ConnectFourHumanPlayer(n, i),
            (n, i) => new ConnectFourComputerPlayer(n, i, i == 1 ? 'X' : 'O'));

        Console.Clear();
        var game = new ConnectFourGame(players);
        GameRunner.TryLoad(game.LoadGame, name);
        if (!game.IsGameOver) game.RunLoaded();
        GameRunner.Pause();
    }
}
