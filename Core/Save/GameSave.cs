using System.Text.Json;
using BoardGameFramework.Core.Commands;
using BoardGameFramework.Core.Exceptions;
using BoardGameFramework.Core.Grid;
using BoardGameFramework.Core.Players;

namespace BoardGameFramework.Core.Save;

/// <summary>
/// Abstract base for all game save managers.
/// 
/// Two save formats are written on every save:
/// - JSON (.json): machine-readable, used to fully restore game state on load.
/// - TXT (.txt):  human-readable summary for reviewing saves without running the game.
/// 
/// Save files are organised into per-game subfolders under "saves/".
/// e.g. saves/ConnectFour/mysave.json
/// </summary>
public abstract class GameSave
{
    public string GameType { get; }

    // Shared fields — every subclass needs these, so they live in Core
    protected readonly GameBoard Grid;
    protected readonly MoveHistory History;
    protected readonly Player[] Players;
    protected int CurrentPlayerIndex { get; private set; }
    protected int TurnCount { get; private set; }

    protected GameSave(GameBoard grid, MoveHistory history,
        Player[] players, string gameType)
    {
        Grid = grid;
        History = history;
        Players = players;
        GameType = gameType;
    }

    public abstract void SaveToFile(string path);
    public abstract void LoadFromFile(string path);

    // Shared state management — identical across all games
    public void SetState(int currentPlayerIndex, int turnCount)
    {
        CurrentPlayerIndex = currentPlayerIndex;
        TurnCount = turnCount;
    }

    public (int playerIndex, int turnCount) GetLoadedState()
        => (CurrentPlayerIndex, TurnCount);

    // Read player names and types from JSON without fully loading the game.
    // Used by GameRunner to restore the correct player types before creating the game.
    public (string[] names, string[] types) ReadPlayerInfo(string path)
    {
        string jsonPath = EnsureJson(path);
        ValidateFileExists(jsonPath);

        using var doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
        var root = doc.RootElement;

        string loadedType = root.GetProperty("GameType").GetString() ?? "";
        ValidateGameType(loadedType);

        string[] names = root.GetProperty("PlayerNames")
            .EnumerateArray().Select(e => e.GetString() ?? "").ToArray();
        string[] types = root.GetProperty("PlayerTypes")
            .EnumerateArray().Select(e => e.GetString() ?? "").ToArray();

        return (names, types);
    }

    // Each game saves into its own subfolder for organisation.
    // e.g. "saves/ConnectFour/mysave.json"
    protected string EnsureJson(string name)
    {
        string folder = Path.Combine("saves", GameType);
        Directory.CreateDirectory(folder);
        string filename = name.EndsWith(".json") ? name : name + ".json";
        return Path.Combine(folder, Path.GetFileName(filename));
    }

    protected string EnsureTxt(string name)
    {
        string folder = Path.Combine("saves", GameType);
        Directory.CreateDirectory(folder);
        string filename = name.EndsWith(".txt") ? name : name + ".txt";
        return Path.Combine(folder, Path.GetFileName(filename));
    }

    // Shared TXT summary — all games write the same plain-text format
    protected void WriteTxtSummary(string path)
    {
        File.WriteAllLines(EnsureTxt(path),
        [
            $"GameType={GameType}",
            $"CurrentPlayerIndex={CurrentPlayerIndex}",
            $"TurnCount={TurnCount}",
            $"GridState={Grid.ExportState()}"
        ]);
    }

    // Shared player type string — avoids duplicated lambda in every subclass
    protected static string GetPlayerType(Player player)
        => player.IsHuman ? "Human" : "Computer";

    protected void ValidateGameType(string loadedType)
    {
        if (loadedType != GameType)
            throw new InvalidGameFileException(GameType, loadedType);
    }

    protected void ValidateFileExists(string path)
    {
        if (!File.Exists(path))
            throw new SaveFileNotFoundException(path);
    }
}
