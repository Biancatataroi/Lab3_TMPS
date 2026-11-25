using System.Collections.Generic;
using System.Linq;

public class HighestCalculator : IGradeCalculator
{
    public double Calculate(IReadOnlyList<int> grades) => grades.Max();
}
