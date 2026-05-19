namespace BoardGameFramework.Core;

public class HelpMenu
{
    private readonly Dictionary<string, string> _commands = new();
    private readonly List<string> _examples = new();

    // Call before re-populating — prevents duplicate entries if InitialiseHelpMenu runs twice
    public void Clear()
    {
        _commands.Clear();
        _examples.Clear();
    }

    public void AddCommand(string key, string description)
        => _commands[key] = description;

    public void AddExample(string example)
        => _examples.Add(example);

    public void ShowCommands()
    {
        Console.WriteLine("\n=== Available Commands ===");
        foreach (var (key, desc) in _commands)
            Console.WriteLine($"  {key,-12} {desc}");
        Console.WriteLine();
    }

    public void ShowExamples()
    {
        Console.WriteLine("\n=== Examples ===");
        foreach (var example in _examples)
            Console.WriteLine($"  {example}");
        Console.WriteLine();
    }

    public void Show()
    {
        ShowCommands();
        ShowExamples();
    }
}