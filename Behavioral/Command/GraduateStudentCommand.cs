// COMMAND: Concrete Commands
public class GraduateStudentCommand : ChangeStatusCommand
{
    public GraduateStudentCommand(Student student) : base(student) { }
    public override string Description => $"Graduate {Student.Name}";
    protected override void Apply() => Student.Graduate();
}
