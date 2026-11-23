// COMMAND: a user action turned into an object so it can be stored in a history and undone.
public interface IStudentCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
