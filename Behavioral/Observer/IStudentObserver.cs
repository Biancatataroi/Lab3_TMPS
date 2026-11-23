// OBSERVER: anything that wants to react when a student changes.
public interface IStudentObserver
{
    void Update(StudentEvent studentEvent);
}

// SUBJECT: observers can subscribe and unsubscribe at runtime.
// Student implements it; it depends only on IStudentObserver, never on concrete observers.
public interface IStudentSubject
{
    void Subscribe(IStudentObserver observer);
    void Unsubscribe(IStudentObserver observer);
}
