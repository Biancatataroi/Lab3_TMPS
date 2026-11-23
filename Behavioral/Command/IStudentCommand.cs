// COMMAND: Command interface
public interface IStudentCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
