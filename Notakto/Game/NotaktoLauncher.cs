using BoardGameFramework.Core.Players;
using BoardGameFramework.Notakto.Players;

namespace BoardGameFramework.Notakto.Game;

public static class NotaktoLauncher
{
    public static void Run()
    {
        GameRunner.ShowGameHeader(
            "NOTAKTO",
            "Three 3x3 boards. " +
            "Player who kills final board loses.",
            "Input example: 1 A1");

        string? mode = GameRunner.ReadMode();

        if (mode == null)
            return;

        if (mode == "3")
        {
            Load();
            return;
        }

        string p1Name =
            GameRunner.ReadName("Player 1");

        Player[] players =
            mode == "2"
            ?
            [
                new NotaktoHumanPlayer(
                    p1Name, 1),

                new NotaktoComputerPlayer(
                    "Computer", 2)
            ]
            :
            [
                new NotaktoHumanPlayer(
                    p1Name, 1),

                new NotaktoHumanPlayer(
                    GameRunner.ReadName("Player 2"),
                    2)
            ];

        Console.Clear();

        new NotaktoGame(players).Run();

        GameRunner.Pause();
    }

    private static void Load()
    {
        var temp = new NotaktoGame(
            GameRunner.TempHumanPlayers(
                (n, i) =>
                    new NotaktoHumanPlayer(n, i)));
        string? name = GameRunner.ReadSaveName(temp.SaveManager.GameType);
        if (name == null)
            return;

        if (!GameRunner.TryReadInfo(
            temp.SaveManager.ReadPlayerInfo,
            name,
            out var names,
            out var types))
            return;

        Player[] players =
            GameRunner.BuildPlayers(
                names,
                types,

                (n, i) =>
                    new NotaktoHumanPlayer(n, i),

                (n, i) =>
                    new NotaktoComputerPlayer(n, i));

        Console.Clear();

        var game =
            new NotaktoGame(players);

        GameRunner.TryLoad(
            game.LoadGame,
            name);

        if (!game.IsGameOver)
            game.RunLoaded();

        GameRunner.Pause();
    }
}