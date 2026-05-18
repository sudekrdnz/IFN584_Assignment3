using BoardGameFramework.Core.Exceptions;
using BoardGameFramework.Core.Players;
using BoardGameFramework.Core.Save;

namespace BoardGameFramework;

/// <summary>
/// Shared helpers for all game launchers.
/// Each game's launcher (e.g. ConnectFourLauncher) calls these directly.
/// </summary>
public static class GameRunner
{
    public static void ShowGameHeader(string title, params string[] info)
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine($"║ {title,-38}║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.WriteLine();
        foreach (var line in info)
            Console.WriteLine($"  {line}");
        Console.WriteLine();
        Console.WriteLine("  1. New game — Human vs Human");
        Console.WriteLine("  2. New game — Human vs Computer");
        Console.WriteLine("  3. Load saved game");
        Console.WriteLine("  4. Back to main menu");
        Console.WriteLine();
        Console.Write("  Enter choice: ");
    }

    public static string? ReadMode()
    {
        string? m = Console.ReadLine()?.Trim();
        return (m == "4" || string.IsNullOrEmpty(m)) ? null : m;
    }

    public static string ReadName(string label)
    {
        Console.Write($"\n  {label} name: ");
        return Console.ReadLine()?.Trim() ?? label;
    }

    public static int ReadBoardSize(int min, int max, int def,
        string label = "board size")
    {
        Console.Write($"  Select {label} ({min}-{max}, default {def}): ");
        string? input = Console.ReadLine()?.Trim();
        return int.TryParse(input, out int s) && s >= min && s <= max ? s : def;
    }

    public static string? ReadSaveName(string gameType)
    {
        string folder = Path.Combine("saves", gameType);

        if (Directory.Exists(folder))
        {
            var files = Directory.GetFiles(folder, "*.json")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .OrderBy(f => f)
                .ToArray();

            if (files.Length > 0)
            {
                Console.WriteLine("\n  Available save files:");
                foreach (var file in files)
                    Console.WriteLine($"    • {file}");
            }
            else
            {
                Console.WriteLine("\n  No save files found.");
            }
        }
        else
        {
            Console.WriteLine("\n  No save files found.");
        }

        Console.Write("\n  Enter save file name (without extension): ");
        string? name = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(name) ? null : name;
    }

    public static void Pause()
    {
        Console.WriteLine("\n  Press Enter to return to menu...");
        Console.ReadLine();
    }

    public static Player[] TempHumanPlayers(Func<string, int, Player> factory)
        => [factory("Player 1", 1), factory("Player 2", 2)];

    public static bool TryReadInfo(
        Func<string, (string[] names, string[] types)> reader,
        string name,
        out string[] names, out string[] types)
    {
        try
        {
            (names, types) = reader(name);
            return true;
        }
        catch (InvalidGameFileException ex)
        {
            Console.WriteLine($"\n  Wrong save file: {ex.Message}");
            Console.WriteLine("  Press Enter to continue...");
            Console.ReadLine();
            names = types = [];
            return false;
        }
        catch (SaveFileNotFoundException ex)
        {
            Console.WriteLine($"\n  {ex.Message}");
            Console.WriteLine("  Press Enter to continue...");
            Console.ReadLine();
            names = types = [];
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  Failed to read save file: {ex.Message}");
            Console.WriteLine("  Press Enter to continue...");
            Console.ReadLine();
            names = types = [];
            return false;
        }
    }

    public static void TryLoad(Action<string> loader, string name)
    {
        try { loader(name); }
        catch (Exception ex) { Console.WriteLine($"\n  Failed to load: {ex.Message}"); }
    }

    public static Player[] BuildPlayers(
        string[] names, string[] types,
        Func<string, int, Player> makeHuman,
        Func<string, int, Player> makeComputer)
    {
        var players = new Player[2];
        for (int i = 0; i < 2; i++)
            players[i] = types[i] == "Computer"
                ? makeComputer(names[i], i + 1)
                : makeHuman(names[i], i + 1);
        return players;
    }
}