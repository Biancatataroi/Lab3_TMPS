// FACTORY METHOD
public class PassFailReport : IAcademicReport
{
    public string Generate(Student student)
    {
        var average = new GradeManager(new AverageCalculator()).GetResult(student);
        return $"Average: {average:F2} | Status: {(average >= 5 ? "PASS" : "FAIL")}";
    }
}
