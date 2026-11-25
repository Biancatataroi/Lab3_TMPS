// COMMAND: Concrete Commands
public abstract class ChangeStatusCommand : IStudentCommand
{
    protected readonly Student Student;
    private IEnrollmentState? _previousState;

    protected ChangeStatusCommand(Student student) => Student = student;

    public abstract string Description { get; }

    public void Execute()
    {
        IEnrollmentState before = Student.State;
        Apply();
        _previousState = before;
    }

    public void Undo()
    {
        if (_previousState is null) return;
        Student.RestoreState(_previousState);
        _previousState = null;
    }

    protected abstract void Apply();
}
