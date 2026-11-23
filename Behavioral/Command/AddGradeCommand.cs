// Concrete command. The receiver (Student) and its state hold the business rules;
// the command only remembers what is needed to undo the action.
public class AddGradeCommand : IStudentCommand
{
    private readonly Student _student;
    private readonly int _grade;
    private int _position = -1;

    public AddGradeCommand(Student student, int grade)
    {
        _student = student;
        _grade = grade;
    }

    public string Description => $"Add grade {_grade} to {_student.Name}";

    public void Execute()
    {
        int position = _student.Grades.Count;
        _student.AddGrade(_grade);      // may throw if the state forbids it
        _position = position;           // remembered only after success
    }

    public void Undo()
    {
        if (_position < 0) return;      // never executed successfully
        _student.RemoveGradeAt(_position);
        _position = -1;
    }
}
