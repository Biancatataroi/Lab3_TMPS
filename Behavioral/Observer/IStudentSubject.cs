// OBSERVER: Subject interface
public interface IStudentSubject
{
    void Subscribe(IStudentObserver observer);
    void Unsubscribe(IStudentObserver observer);
}
