// COMMAND: Concrete Commands
public class SuspendStudentCommand : ChangeStatusCommand
{
    public SuspendStudentCommand(Student student) : base(student) { }
    public override string Description => $"Suspend {Student.Name}";
    protected override void Apply() => Student.Suspend();
}
