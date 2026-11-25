// DECORATOR
public abstract class ReportDecorator : IAcademicReport
{
    protected readonly IAcademicReport InnerReport;

    protected ReportDecorator(IAcademicReport innerReport) => InnerReport = innerReport;

    public abstract string Generate(Student student);
}
