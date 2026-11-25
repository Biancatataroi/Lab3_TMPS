// FACTORY METHOD
public class AverageReportCreator : AcademicReportCreator
{
    public override IAcademicReport CreateReport() => new AverageReport();
}
