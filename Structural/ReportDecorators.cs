// DECORATOR: adds optional text to a report without editing existing reports.
public abstract class ReportDecorator : IAcademicReport
{
    protected readonly IAcademicReport InnerReport;

    protected ReportDecorator(IAcademicReport innerReport) => InnerReport = innerReport;

    public abstract string Generate(Student student);
}

public class HeaderDecorator : ReportDecorator
{
    public HeaderDecorator(IAcademicReport innerReport) : base(innerReport) { }

    public override string Generate(Student student)
    {
        return $"ACADEMIC REPORT - {student.Name}\n" + InnerReport.Generate(student);
    }
}

public class HonorsDecorator : ReportDecorator
{
    public HonorsDecorator(IAcademicReport innerReport) : base(innerReport) { }

    public override string Generate(Student student)
    {
        double average = new GradeManager(new AverageCalculator()).GetResult(student);
        string classification = average >= 9 ? "Excellent" : average >= 7 ? "Good" : "Regular";
        return InnerReport.Generate(student) + $"\nClassification: {classification}";
    }
}
