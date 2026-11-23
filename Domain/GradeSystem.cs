using System;
using System.Collections.Generic;
using System.Linq;

public interface IGradeCalculator
{
    double Calculate(IReadOnlyList<int> grades);
}

public class AverageCalculator : IGradeCalculator
{
    public double Calculate(IReadOnlyList<int> grades) => grades.Average();
}

public class HighestCalculator : IGradeCalculator
{
    public double Calculate(IReadOnlyList<int> grades) => grades.Max();
}

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
