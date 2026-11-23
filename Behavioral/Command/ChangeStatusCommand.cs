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

public class SuspendStudentCommand : ChangeStatusCommand
{
    public SuspendStudentCommand(Student student) : base(student) { }
    public override string Description => $"Suspend {Student.Name}";
    protected override void Apply() => Student.Suspend();
}

public class ReinstateStudentCommand : ChangeStatusCommand
{
    public ReinstateStudentCommand(Student student) : base(student) { }
    public override string Description => $"Reinstate {Student.Name}";
    protected override void Apply() => Student.Reinstate();
}

public class GraduateStudentCommand : ChangeStatusCommand
{
    public GraduateStudentCommand(Student student) : base(student) { }
    public override string Description => $"Graduate {Student.Name}";
    protected override void Apply() => Student.Graduate();
}
