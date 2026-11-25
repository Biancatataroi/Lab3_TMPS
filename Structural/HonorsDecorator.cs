// DECORATOR
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
