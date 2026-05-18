using BoardGameFramework.Core.Players;
using BoardGameFramework.Core.Save;
using BoardGameFramework.NumericalTicTacToe.Players;
using BoardGameFramework.NumericalTicTacToe.Save;

namespace BoardGameFramework.NumericalTicTacToe.Game;

public static class NumericalTicTacToeLauncher
{
    public static void Run()
    {
        GameRunner.ShowGameHeader("NUMERICAL TIC TAC TOE",
            "3×3 board. First to score 15 wins.",
            "Enter format: row,col=num");
        string? mode = GameRunner.ReadMode();
        if (mode == null) return;
        if (mode == "3") 
        { 
            Load(); 
            return; 
        }

        string p1Name = GameRunner.ReadName("Player 1");
        Player[] players = mode == "2"
            ?
            [
                new NumericalTicTacToeHumanPlayer(p1Name, 1),
                new NumericalTicTacToeComputerPlayer("Computer", 2)
            ]
            :
            [
                new NumericalTicTacToeHumanPlayer(p1Name, 1),
                new NumericalTicTacToeComputerPlayer(GameRunner.ReadName("Player 2"), 2)
            ];

        Console.Clear();
        new NumericalTicTacToeGame(players).Run();
        GameRunner.Pause();
    }

    private static void Load()
    {
        string? name = GameRunner.ReadSaveName();
        if (name == null) 
            return;

        var temp = new NumericalTicTacToeGame(
            GameRunner.TempHumanPlayers((n, i) => new NumericalTicTacToeHumanPlayer(n, i)));

        if (!GameRunner.TryReadInfo(temp.SaveManager.ReadPlayerInfo, name,
            out var names, out var types)) return;

        Player[] players = GameRunner.BuildPlayers(names, types,
            (n, i) => new NumericalTicTacToeHumanPlayer(n, i),
            (n, i) => new NumericalTicTacToeComputerPlayer(n, i));

        Console.Clear();

        var game = new NumericalTicTacToeGame(players);

        GameRunner.TryLoad(game.LoadGame, name);

        if (!game.IsGameOver) 
            game.RunLoaded();

        GameRunner.Pause();

    }
}
