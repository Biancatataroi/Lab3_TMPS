// PROXY
public class ReportProxy : IAcademicReport
{
    private readonly IAcademicReport _realReport;

    public ReportProxy(IAcademicReport realReport) => _realReport = realReport;

    public string Generate(Student student)
    {
        if (student.Grades.Count == 0)
            throw new InvalidOperationException("Add at least one grade before generating a report.");

        return _realReport.Generate(student);
    }
}
