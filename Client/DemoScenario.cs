using System;

// Scripted walkthrough: dotnet run -- --demo
// One execution path that crosses all three behavioral patterns:
// Command (history) -> State (Student delegates to its state) -> Observer (notifications).
public static class DemoScenario
{
    public static void Run()
    {
        // Lab 2 Builder creates the objects the behavioral patterns work with.
        Student maria = new StudentBuilder().SetId(1).SetName("Maria Ciobanu")
            .SetFaculty("FCIM").SetGroup("FAF-242").SetStudyYear(3)
            .SetEmail("maria@example.com").AddGrade(4).Build();

        IStudentObserver console = new ConsoleNotifier(Console.Out);
        var audit = new AuditLogObserver();
        IStudentObserver risk = new AcademicRiskAlert(Console.Out);
        maria.Subscribe(console);
        maria.Subscribe(audit);
        maria.Subscribe(risk);

        var history = new CommandHistory();

        Section("1. COMMAND + OBSERVER: add grades through the command history");
        Run(history, new AddGradeCommand(maria, 9));
        Run(history, new AddGradeCommand(maria, 1));   // pulls the average down -> risk alert
        Status(maria);

        Section("2. COMMAND: undo the bad grade");
        Console.WriteLine($"Undone: {history.Undo()}");
        Status(maria);

        Section("3. STATE: suspend, then try actions the Suspended state forbids");
        Run(history, new SuspendStudentCommand(maria));
        Run(history, new AddGradeCommand(maria, 10));    // rejected by SuspendedState
        Run(history, new GraduateStudentCommand(maria)); // rejected by SuspendedState
        Status(maria);

        Section("4. COMMAND + STATE: undo the suspension restores the Active state");
        Console.WriteLine($"Undone: {history.Undo()}");
        Status(maria);

        Section("5. OBSERVER: unsubscribe the console notifier at runtime");
        maria.Unsubscribe(console);
        Run(history, new GraduateStudentCommand(maria));
        Console.WriteLine("(no [Notification] line above: the console notifier is unsubscribed)");
        Run(history, new SuspendStudentCommand(maria));  // Graduated is final
        Status(maria);

        Section("6. Lab 2 Facade still works with the student");
        Console.WriteLine(new AcademicReportFacade(RegistryWith(maria))
            .Generate(maria.Id, new PassFailReportCreator(), withHeader: true, withHonors: true));

        Section("Command history");
        foreach (string action in history.ExecutedActions) Console.WriteLine($"  - {action}");

        Section("Audit log (observer that stayed subscribed)");
        foreach (string entry in audit.Entries) Console.WriteLine($"  {entry}");
    }

    private static void Run(CommandHistory history, IStudentCommand command)
    {
        Console.WriteLine($"> {command.Description}");
        try { history.Execute(command); }
        catch (InvalidOperationException ex) { Console.WriteLine($"  Rejected: {ex.Message}"); }
    }

    private static void Status(Student s) =>
        Console.WriteLine($"  Status: {s.State.Name} | Grades: {string.Join(", ", s.Grades)}");

    private static void Section(string title) => Console.WriteLine($"\n=== {title} ===");

    private static StudentRegistry RegistryWith(Student student)
    {
        StudentRegistry registry = StudentRegistry.Instance;
        if (registry.FindById(student.Id) is null) registry.Add(student);
        return registry;
    }
}
