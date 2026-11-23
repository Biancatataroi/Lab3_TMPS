using System.Collections.Generic;
using System.IO;

// Reacts only to grade changes: warns the dean's office when the average
// falls below the passing threshold. It only reads the student, never changes it,
// so it cannot trigger an endless notification loop.
public class AcademicRiskAlert : IStudentObserver
{
    private readonly TextWriter _output;
    private readonly List<string> _alerts = new();
    public IReadOnlyList<string> Alerts => _alerts;

    public AcademicRiskAlert(TextWriter output) => _output = output;

    public void Update(StudentEvent e)
    {
        if (e.Kind == StudentEventKind.StateChanged || e.Student.Grades.Count == 0) return;

        double average = new GradeManager(new AverageCalculator()).GetResult(e.Student);
        if (average >= GradeManager.PassingAverage) return;

        string alert = $"{e.Student.Name} is at academic risk (average {average:F2})";
        _alerts.Add(alert);
        _output.WriteLine($"  [Dean's office] {alert}");
    }
}
