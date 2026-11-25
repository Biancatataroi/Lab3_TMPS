// FACTORY METHOD
public class PassFailReportCreator : AcademicReportCreator
{
    public override IAcademicReport CreateReport() => new PassFailReport();
}
