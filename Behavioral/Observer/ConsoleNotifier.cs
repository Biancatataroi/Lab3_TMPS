using System.IO;

// Prints every change. The output is injected so tests can capture it.
public class ConsoleNotifier : IStudentObserver
{
    private readonly TextWriter _output;

    public ConsoleNotifier(TextWriter output) => _output = output;

    public void Update(StudentEvent e) =>
        _output.WriteLine($"  [Notification] {e.Student.Name}: {e.Description}");
}
