using System.Text.Json;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Core.Save;
using BoardGameFramework.Notakto.Commands;
using BoardGameFramework.Notakto.Grid;

namespace BoardGameFramework.Notakto.Save;

public class NotaktoGameSave : GameSave
{
    public NotaktoGameSave(
        NotaktoGrid grid,
        MoveHistory history,
        Player[] players)
        : base(grid, history, players, "Notakto") { }

    public override void SaveToFile(string path)
    {
        string jsonPath = EnsureJson(path);

        var data = new
        {
            GameType,
            GridState = Grid.ExportState(),
            CurrentPlayerIndex,
            TurnCount,

            PlayerNames = Players
                .Select(p => p.Name)
                .ToArray(),

            PlayerTypes = Players
                .Select(GetPlayerType)
                .ToArray(),

            DoneStack = History.GetDoneStack()
                .OfType<NotaktoMoveCommand>()
                .Select(c => new
                {
                    c.BoardIndex,
                    c.Row,
                    c.Col,

                    PlayerNumber =
                        c.Player.PlayerNumber,

                    GridSnapshot =
                        c.GridSnapshot,

                    PlayerIndex =
                        c.CurrentPlayerIndex,

                    c.TurnCount
                })
        };

        File.WriteAllText(
            jsonPath,
            JsonSerializer.Serialize(
                data,
                JsonOptions));

        WriteTxtSummary(path);

        Console.WriteLine(
            $"Saved: {jsonPath} and {EnsureTxt(path)}");
    }

    public override void LoadFromFile(string path)
    {
        string jsonPath = EnsureJson(path);

        ValidateFileExists(jsonPath);

        using var doc = JsonDocument.Parse(
            File.ReadAllText(jsonPath));

        var root = doc.RootElement;

        ValidateGameType(
            root.GetProperty("GameType")
                .GetString() ?? "");

        Grid.ImportState(
            root.GetProperty("GridState")
                .GetString() ?? "");

        SetState(
            root.GetProperty("CurrentPlayerIndex")
                .GetInt32(),

            root.GetProperty("TurnCount")
                .GetInt32());

        var commands = root
            .GetProperty("DoneStack")
            .EnumerateArray()
            .Select(d => (MoveCommand)
                new NotaktoMoveCommand(
                    Players[
                        d.GetProperty("PlayerNumber")
                            .GetInt32() - 1
                    ],

                    d.GetProperty("GridSnapshot")
                        .GetString() ?? "",

                    d.GetProperty("PlayerIndex")
                        .GetInt32(),

                    d.GetProperty("TurnCount")
                        .GetInt32(),

                    d.GetProperty("BoardIndex")
                        .GetInt32(),

                    d.GetProperty("Row")
                        .GetInt32(),

                    d.GetProperty("Col")
                        .GetInt32()
                ))
            .ToList();

        History.LoadHistory(commands);

        Console.WriteLine($"Loaded: {jsonPath}");
    }
}