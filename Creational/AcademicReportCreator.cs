// FACTORY METHOD
public abstract class AcademicReportCreator
{
    public abstract IAcademicReport CreateReport();

    public string GenerateReport(Student student)
    {
        IAcademicReport report = CreateReport();
        return report.Generate(student);
    }
}
