using System.IO;

// OBSERVER: Concrete Observer
public class ConsoleNotifier : IStudentObserver
{
    private readonly TextWriter _output;

    public ConsoleNotifier(TextWriter output) => _output = output;

    public void Update(StudentEvent e) =>
        _output.WriteLine($"  [Notification] {e.Student.Name}: {e.Description}");
}
