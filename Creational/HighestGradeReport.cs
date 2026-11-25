// FACTORY METHOD
public class HighestGradeReport : IAcademicReport
{
    public string Generate(Student student)
    {
        var result = new GradeManager(new HighestCalculator()).GetResult(student);
        return $"Highest grade: {result:F2}";
    }
}
