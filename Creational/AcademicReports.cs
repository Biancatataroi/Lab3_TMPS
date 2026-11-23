using System;

// FACTORY METHOD
public interface IAcademicReport
{
    string Generate(Student student);
}

public class AverageReport : IAcademicReport
{
    public string Generate(Student student)
    {
        var result = new GradeManager(new AverageCalculator()).GetResult(student);
        return $"Average grade: {result:F2}";
    }
}

public class HighestGradeReport : IAcademicReport
{
    public string Generate(Student student)
    {
        var result = new GradeManager(new HighestCalculator()).GetResult(student);
        return $"Highest grade: {result:F2}";
    }
}

public class PassFailReport : IAcademicReport
{
    public string Generate(Student student)
    {
        var average = new GradeManager(new AverageCalculator()).GetResult(student);
        return $"Average: {average:F2} | Status: {(average >= 5 ? "PASS" : "FAIL")}";
    }
}

public abstract class AcademicReportCreator
{
    public abstract IAcademicReport CreateReport();

    public string GenerateReport(Student student)
    {
        IAcademicReport report = CreateReport();
        return report.Generate(student);
    }
}

public class AverageReportCreator : AcademicReportCreator
{
    public override IAcademicReport CreateReport() => new AverageReport();
}

public class HighestGradeReportCreator : AcademicReportCreator
{
    public override IAcademicReport CreateReport() => new HighestGradeReport();
}

public class PassFailReportCreator : AcademicReportCreator
{
    public override IAcademicReport CreateReport() => new PassFailReport();
}
