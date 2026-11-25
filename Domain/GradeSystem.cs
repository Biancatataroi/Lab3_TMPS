using System;
using System.Collections.Generic;
using System.Linq;

public interface IGradeCalculator
{
    double Calculate(IReadOnlyList<int> grades);
}
