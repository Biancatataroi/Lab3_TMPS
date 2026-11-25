// DECORATOR
public class HeaderDecorator : ReportDecorator
{
    public HeaderDecorator(IAcademicReport innerReport) : base(innerReport) { }

    public override string Generate(Student student)
    {
        return $"ACADEMIC REPORT - {student.Name}\n" + InnerReport.Generate(student);
    }
}
