namespace BoardGameFramework.Core.Commands;

public class MoveHistory
{
    private readonly Stack<MoveCommand> _doneStack = new();
    private readonly Stack<MoveCommand> _redoStack = new();

    public void Record(MoveCommand cmd)
    {
        _doneStack.Push(cmd);
        _redoStack.Clear();
    }

    public MoveCommand? Undo()
    {
        if (!CanUndo()) return null;
        MoveCommand cmd = _doneStack.Pop();
        _redoStack.Push(cmd);
        return cmd;
    }

    public MoveCommand? Redo()
    {
        if (!CanRedo()) return null;
        MoveCommand cmd = _redoStack.Pop();
        _doneStack.Push(cmd);
        return cmd;
    }

    public void ClearRedo() => _redoStack.Clear();

    public void Clear()
    {
        _doneStack.Clear();
        _redoStack.Clear();
    }

    public bool CanUndo() => _doneStack.Count > 0;
    public bool CanRedo() => _redoStack.Count > 0;

    public IEnumerable<MoveCommand> GetDoneStack() => _doneStack;
    public IEnumerable<MoveCommand> GetRedoStack() => _redoStack;

    public void LoadHistory(IEnumerable<MoveCommand> done)
    {
        _doneStack.Clear();
        _redoStack.Clear();
        foreach (var cmd in done.Reverse())
            _doneStack.Push(cmd);
    }
}
