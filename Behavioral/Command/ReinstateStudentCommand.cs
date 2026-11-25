// COMMAND: Concrete Commands
public class ReinstateStudentCommand : ChangeStatusCommand
{
    public ReinstateStudentCommand(Student student) : base(student) { }
    public override string Description => $"Reinstate {Student.Name}";
    protected override void Apply() => Student.Reinstate();
}
