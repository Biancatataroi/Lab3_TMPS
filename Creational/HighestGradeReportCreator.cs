// FACTORY METHOD
public class HighestGradeReportCreator : AcademicReportCreator
{
    public override IAcademicReport CreateReport() => new HighestGradeReport();
}
