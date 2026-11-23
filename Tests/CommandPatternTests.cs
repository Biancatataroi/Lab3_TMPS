using System.Linq;
using static TestRunner;

public static class CommandPatternTests
{
    public static void Run()
    {
        Check("Command: execute then undo AddGrade restores original grades", () =>
        {
            var student = MakeStudent(6, 7);
            var history = new CommandHistory();
            history.Execute(new AddGradeCommand(student, 10));
            Assert(student.Grades.SequenceEqual(new[] { 6, 7, 10 }));
            history.Undo();
            Assert(student.Grades.SequenceEqual(new[] { 6, 7 }), "undo must restore the grades");
        });

        Check("Command: undo of a status change restores the previous state", () =>
        {
            var student = MakeStudent(8);
            var history = new CommandHistory();
            history.Execute(new SuspendStudentCommand(student));
            Assert(student.State is SuspendedState);
            history.Undo();
            Assert(student.State is ActiveState, "undo must restore Active");
        });

        Check("Command: redo re-applies the undone command", () =>
        {
            var student = MakeStudent(9);
            var history = new CommandHistory();
            history.Execute(new GraduateStudentCommand(student));
            history.Undo();
            history.Redo();
            Assert(student.State is GraduatedState);
            Assert(!history.CanRedo);
        });

        Check("Command: a rejected command is not recorded in history", () =>
        {
            var student = MakeStudent(8);
            var history = new CommandHistory();
            history.Execute(new SuspendStudentCommand(student));
            AssertThrows<InvalidStateTransitionException>(
                () => history.Execute(new AddGradeCommand(student, 10)));
            Assert(history.ExecutedActions.Count == 1, "only the suspension is recorded");
        });
    }
}
