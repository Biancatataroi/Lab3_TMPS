public enum StudentEventKind
{
    GradeAdded,
    GradeRemoved,
    StateChanged
}

public record StudentEvent(Student Student, StudentEventKind Kind, string Description);
