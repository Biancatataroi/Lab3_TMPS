using System;

public class GradeManager
{
    public const double PassingAverage = 5.0;

    private readonly IGradeCalculator _calculator;
    public GradeManager(IGradeCalculator calculator) => _calculator = calculator;

    public double GetResult(Student student)
    {
        if (student.Grades.Count == 0)
            throw new InvalidOperationException("Student has no grades.");
        return _calculator.Calculate(student.Grades);
    }
}
