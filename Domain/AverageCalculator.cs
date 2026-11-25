using System.Collections.Generic;
using System.Linq;

public class AverageCalculator : IGradeCalculator
{
    public double Calculate(IReadOnlyList<int> grades) => grades.Average();
}
