namespace BoardGameFramework.Core.Exceptions;

/// <summary>Base exception for all board game framework errors.</summary>
public class GameException : Exception
{
    public GameException(string message) : base(message) { }
    public GameException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Thrown when a save file belongs to a different game type.</summary>
public class InvalidGameFileException : GameException
{
    public string ExpectedType { get; }
    public string ActualType { get; }

    public InvalidGameFileException(string expected, string actual)
        : base($"Wrong save file. Expected '{expected}' but got '{actual}'.")
    {
        ExpectedType = expected;
        ActualType = actual;
    }
}

/// <summary>Thrown when a save file cannot be found.</summary>
public class SaveFileNotFoundException : GameException
{
    public string FilePath { get; }

    public SaveFileNotFoundException(string path)
        : base($"Save file not found: {path}")
    {
        FilePath = path;
    }
}

/// <summary>Thrown when a player attempts an invalid move.</summary>
public class InvalidMoveException : GameException
{
    public InvalidMoveException(string message) : base(message) { }
}