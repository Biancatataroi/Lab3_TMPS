using System.IO;
using static TestRunner;

public static class ObserverPatternTests
{
    public static void Run()
    {
        Check("Observer: all subscribed observers are notified", () =>
        {
            var student = MakeStudent(8);
            var first = new RecordingObserver();
            var second = new RecordingObserver();
            student.Subscribe(first);
            student.Subscribe(second);
            student.AddGrade(9);
            Assert(first.Received.Count == 1 && second.Received.Count == 1);
            Assert(first.Received[0].Kind == StudentEventKind.GradeAdded);
        });

        Check("Observer: unsubscribed observer is not notified", () =>
        {
            var student = MakeStudent(8);
            var stays = new RecordingObserver();
            var leaves = new RecordingObserver();
            student.Subscribe(stays);
            student.Subscribe(leaves);
            student.Unsubscribe(leaves);
            student.Suspend();
            Assert(stays.Received.Count == 1, "subscribed observer notified");
            Assert(leaves.Received.Count == 0, "unsubscribed observer not notified");
        });

        Check("Observer: academic risk alert reacts only when average < 5", () =>
        {
            var student = MakeStudent(6);
            var alert = new AcademicRiskAlert(TextWriter.Null);
            student.Subscribe(alert);
            student.AddGrade(5);
            Assert(alert.Alerts.Count == 0);
            student.AddGrade(1);
            Assert(alert.Alerts.Count == 1);
        });

        Check("Interaction: Command -> State -> Observer (execute and undo both notify)", () =>
        {
            var student = MakeStudent(8);
            var audit = new AuditLogObserver();
            student.Subscribe(audit);
            var history = new CommandHistory();

            history.Execute(new SuspendStudentCommand(student));
            history.Undo();

            Assert(audit.Entries.Count == 2);
            Assert(audit.Entries[0].Contains("Active -> Suspended"));
            Assert(audit.Entries[1].Contains("Suspended -> Active (undo)"));
        });
    }
}
