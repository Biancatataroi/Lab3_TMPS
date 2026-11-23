// OBSERVER: Observer interface
public interface IStudentObserver
{
    void Update(StudentEvent studentEvent);
}

// OBSERVER: Subject interface
public interface IStudentSubject
{
    void Subscribe(IStudentObserver observer);
    void Unsubscribe(IStudentObserver observer);
}
