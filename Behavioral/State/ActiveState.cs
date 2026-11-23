// Active: can receive grades, can be suspended, can graduate with a passing average.
public class ActiveState : EnrollmentStateBase
{
    public override string Name => "Active";

    public override void AddGrade(Student student, int grade) => student.StoreGrade(grade);

    public override void Suspend(Student student) => student.TransitionTo(new SuspendedState());

    public override void Graduate(Student student)
    {
        if (student.Grades.Count == 0)
            throw new InvalidStateTransitionException("Cannot graduate: student has no grades.");

        double average = new GradeManager(new AverageCalculator()).GetResult(student);
        if (average < GradeManager.PassingAverage)
            throw new InvalidStateTransitionException(
                $"Cannot graduate: average {average:F2} is below {GradeManager.PassingAverage:F2}.");

        student.TransitionTo(new GraduatedState());
    }
}
