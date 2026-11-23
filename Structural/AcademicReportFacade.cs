// FACADE
public class AcademicReportFacade
{
    private readonly StudentRegistry _registry;

    public AcademicReportFacade(StudentRegistry registry) => _registry = registry;

    public string Generate(int studentId, AcademicReportCreator creator,
        bool withHeader = false, bool withHonors = false)
    {
        Student student = _registry.FindById(studentId)
            ?? throw new InvalidOperationException("Student not found.");

        IAcademicReport report = creator.CreateReport();

        if (withHeader) report = new HeaderDecorator(report);
        if (withHonors) report = new HonorsDecorator(report);

        report = new ReportProxy(report);
        return report.Generate(student);
    }
}
