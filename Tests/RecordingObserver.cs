public class RecordingObserver : IStudentObserver
{
    public System.Collections.Generic.List<StudentEvent> Received { get; } = new();
    public void Update(StudentEvent studentEvent) => Received.Add(studentEvent);
}
