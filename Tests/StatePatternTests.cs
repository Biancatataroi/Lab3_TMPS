using static TestRunner;

public static class StatePatternTests
{
    public static void Run()
    {
        Check("State: valid transitions Active -> Suspended -> Active -> Graduated", () =>
        {
            var student = MakeStudent(8, 9);
            Assert(student.State is ActiveState, "new student should be Active");
            student.Suspend();
            Assert(student.State is SuspendedState, "should be Suspended");
            student.Reinstate();
            Assert(student.State is ActiveState, "should be Active again");
            student.Graduate();
            Assert(student.State is GraduatedState, "should be Graduated");
        });

        Check("State: forbidden transitions are rejected and state is unchanged", () =>
        {
            var active = MakeStudent(8);
            AssertThrows<InvalidStateTransitionException>(() => active.Reinstate());
            Assert(active.State is ActiveState);

            var graduated = MakeStudent(9);
            graduated.Graduate();
            AssertThrows<InvalidStateTransitionException>(() => graduated.Suspend());
            Assert(graduated.State is GraduatedState, "Graduated is final");
        });

        Check("State: same action behaves differently per state (AddGrade)", () =>
        {
            var student = MakeStudent(7);
            student.AddGrade(10);
            student.Suspend();
            AssertThrows<InvalidStateTransitionException>(() => student.AddGrade(5));
            Assert(student.Grades.Count == 2, "rejected grade must not be stored");
        });

        Check("State: graduation requires a passing average", () =>
        {
            var failing = MakeStudent(3, 4);
            AssertThrows<InvalidStateTransitionException>(() => failing.Graduate());
            Assert(failing.State is ActiveState);
        });
    }
}
