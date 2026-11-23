using System.Collections.Generic;
using System.Linq;

// INVOKER: executes commands and keeps undo / redo stacks.
// It only knows the IStudentCommand interface, never the concrete commands.
public class CommandHistory
{
    private readonly Stack<IStudentCommand> _undo = new();
    private readonly Stack<IStudentCommand> _redo = new();

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    // Oldest first.
    public IReadOnlyList<string> ExecutedActions => _undo.Reverse().Select(c => c.Description).ToList();

    public void Execute(IStudentCommand command)
    {
        command.Execute();      // if this throws, nothing is recorded
        _undo.Push(command);
        _redo.Clear();          // a new action makes the old redo branch invalid
    }

    public string? Undo()
    {
        if (!CanUndo) return null;
        IStudentCommand command = _undo.Pop();
        command.Undo();
        _redo.Push(command);
        return command.Description;
    }

    public string? Redo()
    {
        if (!CanRedo) return null;
        IStudentCommand command = _redo.Pop();
        command.Execute();
        _undo.Push(command);
        return command.Description;
    }
}
