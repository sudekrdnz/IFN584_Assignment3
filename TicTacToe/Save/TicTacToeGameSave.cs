using System.Text.Json;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Core.Save;
using BoardGameFramework.TicTacToe.Commands;
using BoardGameFramework.TicTacToe.Grid;

namespace BoardGameFramework.TicTacToe.Save;

public class TicTacToeGameSave : GameSave
{
    public TicTacToeGameSave(TicTacToeGrid grid, MoveHistory history, Player[] players)
        : base(grid, history, players, "TicTacToe") { }

    public override void SaveToFile(string path)
    {
        string jsonPath = EnsureJson(path);

        var data = new
        {
            GameType,
            GridState = Grid.ExportState(),
            CurrentPlayerIndex,
            TurnCount,
            PlayerNames = Players.Select(p => p.Name).ToArray(),
            PlayerTypes = Players.Select(GetPlayerType).ToArray(),
            DoneStack = History.GetDoneStack()
                .OfType<TicTacToeMoveCommand>()
                .Select(c => new
                {
                    c.Position,
                    c.Symbol,
                    PlayerNumber = c.Player.PlayerNumber,
                    GridSnapshot = c.GridSnapshot,
                    PlayerIndex = c.CurrentPlayerIndex,
                    c.TurnCount
                })
        };

        File.WriteAllText(jsonPath,
            JsonSerializer.Serialize(data, JsonOptions));

        WriteTxtSummary(path);
        Console.WriteLine($"Saved: {jsonPath} and {EnsureTxt(path)}");
    }

    public override void LoadFromFile(string path)
    {
        string jsonPath = EnsureJson(path);
        ValidateFileExists(jsonPath);

        using var doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
        var root = doc.RootElement;

        ValidateGameType(root.GetProperty("GameType").GetString() ?? "");

        Grid.ImportState(root.GetProperty("GridState").GetString() ?? "");

        SetState(
            root.GetProperty("CurrentPlayerIndex").GetInt32(),
            root.GetProperty("TurnCount").GetInt32());

        var commands = root.GetProperty("DoneStack").EnumerateArray()
            .Select(d => (MoveCommand)new TicTacToeMoveCommand(
                Players[d.GetProperty("PlayerNumber").GetInt32() - 1],
                d.GetProperty("GridSnapshot").GetString() ?? "",
                d.GetProperty("PlayerIndex").GetInt32(),
                d.GetProperty("TurnCount").GetInt32(),
                d.GetProperty("Position").GetInt32(),
                d.GetProperty("Symbol").GetString()?[0] ?? ' '
            ))
            .ToList();

        History.LoadHistory(commands);
        Console.WriteLine($"Loaded: {jsonPath}");
    }
}