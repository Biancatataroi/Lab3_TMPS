// Data sent by the subject (Student) to its observers.
public enum StudentEventKind
{
    GradeAdded,
    GradeRemoved,
    StateChanged
}

public record StudentEvent(Student Student, StudentEventKind Kind, string Description);
