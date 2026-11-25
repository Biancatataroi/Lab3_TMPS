// FACTORY METHOD
public class AverageReport : IAcademicReport
{
    public string Generate(Student student)
    {
        var result = new GradeManager(new AverageCalculator()).GetResult(student);
        return $"Average grade: {result:F2}";
    }
}
