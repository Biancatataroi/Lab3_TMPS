using System;
using static ConsoleInput;

public class ConsoleMenu
{
    private readonly StudentRegistry _registry;
    private readonly AcademicReportFacade _reportFacade;
    private readonly CommandHistory _history = new();
    private readonly NotificationSettings _notifications;
    private readonly AuditLogObserver _auditLog = new();

    public ConsoleMenu(StudentRegistry registry)
    {
        _registry = registry;
        _reportFacade = new AcademicReportFacade(registry);
        _notifications = new NotificationSettings(registry);
    }

    public void Run()
    {
        SeedStudents();
        _notifications.Register("Console notifications", new ConsoleNotifier(Console.Out), enabled: true);
        _notifications.Register("Audit log", _auditLog, enabled: true);
        _notifications.Register("Academic risk alerts", new AcademicRiskAlert(Console.Out), enabled: false);

        while (true)
        {
            PrintMenu();
            string? choice = Console.ReadLine();
            if (choice == "0") return;

            try
            {
                switch (choice)
                {
                    case "1": ListStudents(); break;
                    case "2": AddStudent(); break;
                    case "3": AddGrade(); break;
                    case "4": ViewProfile(); break;
                    case "5": UpdateStudent(); break;
                    case "6": DeleteStudent(); break;
                    case "7": GenerateReport(); break;
                    case "8": ChangeStatus(); break;
                    case "9": Undo(); break;
                    case "10": Redo(); break;
                    case "11": ShowHistory(); break;
                    case "12": ManageNotifications(); break;
                    case "13": ShowAuditLog(); break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
            catch (InvalidOperationException ex) { Console.WriteLine("Rejected: " + ex.Message); }
            catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n=== STUDENT GRADE MANAGEMENT SYSTEM - LAB 3 ===");
        Console.WriteLine("1. View all students");
        Console.WriteLine("2. Add new student");
        Console.WriteLine("3. Add grade");
        Console.WriteLine("4. View student profile");
        Console.WriteLine("5. Update student information");
        Console.WriteLine("6. Delete student");
        Console.WriteLine("7. Generate academic report");
        Console.WriteLine("8. Change enrollment status (suspend / reinstate / graduate)");
        Console.WriteLine("9. Undo last action");
        Console.WriteLine("10. Redo");
        Console.WriteLine("11. View action history");
        Console.WriteLine("12. Manage notifications (subscribe / unsubscribe observers)");
        Console.WriteLine("13. View audit log");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
    }

    private void SeedStudents()
    {
        if (_registry.Students.Count > 0) return;
        _registry.Add(new StudentBuilder().SetId(1).SetName("Ana Popescu")
            .SetFaculty("FCIM").SetGroup("FAF-242").SetStudyYear(2)
            .SetEmail("ana@example.com").AddGrade(8).AddGrade(9).Build());
        _registry.Add(new StudentBuilder().SetId(2).SetName("Alex Rusu")
            .SetFaculty("FCIM").SetGroup("FAF-242").SetStudyYear(2)
            .SetEmail("alex@example.com").AddGrade(7).AddGrade(10).Build());
    }

    private Student? ChooseStudent()
    {
        int id = ReadInt("Student ID: ", 1, int.MaxValue);
        Student? student = _registry.FindById(id);
        if (student is null) Console.WriteLine("Student not found.");
        return student;
    }

    private static void PrintProfile(Student student)
    {
        Console.WriteLine($"ID: {student.Id} | Name: {student.Name} | Status: {student.State.Name}");
        Console.WriteLine($"Faculty: {student.Faculty} | Group: {student.Group} | Year: {student.StudyYear}");
        Console.WriteLine($"Email: {student.Email}");
        Console.WriteLine("Grades: " + (student.Grades.Count == 0 ? "none" : string.Join(", ", student.Grades)));
    }

    private void ListStudents()
    {
        if (_registry.Students.Count == 0) Console.WriteLine("No students yet.");
        foreach (Student s in _registry.Students)
            Console.WriteLine($"{s.Id}: {s.Name} ({s.Group}) - {s.State.Name}");
    }

    private void AddStudent()
    {
        Student created = new StudentBuilder()
            .SetId(_registry.NextId())
            .SetName(ReadRequired("Name: "))
            .SetFaculty(ReadRequired("Faculty: "))
            .SetGroup(ReadRequired("Group: "))
            .SetStudyYear(ReadInt("Study year (1-6): ", 1, 6))
            .SetEmail(ReadRequired("Email: "))
            .Build();
        _registry.Add(created);
        _notifications.AttachEnabledObservers(created);
        Console.WriteLine($"Student added with ID {created.Id}.");
    }

    private void AddGrade()
    {
        Student? student = ChooseStudent();
        if (student is null) return;
        int grade = ReadInt("Grade (1-10): ", 1, 10);
        _history.Execute(new AddGradeCommand(student, grade));
    }

    private void ViewProfile()
    {
        Student? student = ChooseStudent();
        if (student is not null) PrintProfile(student);
    }

    private void UpdateStudent()
    {
        Student? student = ChooseStudent();
        if (student is null) return;
        Console.WriteLine("Enter new data (leave empty to keep current value).");
        Console.Write($"Name [{student.Name}]: ");
        string? name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name)) student.Name = name.Trim();
        Console.Write($"Faculty [{student.Faculty}]: ");
        string? faculty = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(faculty)) student.Faculty = faculty.Trim();
        Console.Write($"Group [{student.Group}]: ");
        string? group = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(group)) student.Group = group.Trim();
        Console.Write($"Email [{student.Email}]: ");
        string? email = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(email))
        {
            if (System.Net.Mail.MailAddress.TryCreate(email.Trim(), out var parsed) && parsed.Address == email.Trim())
                student.Email = email.Trim();
            else Console.WriteLine("Invalid email: previous email kept.");
        }
        Console.WriteLine("Student information updated.");
    }

    private void DeleteStudent()
    {
        Student? student = ChooseStudent();
        if (student is null) return;
        if (ReadYes($"Delete {student.Name}? (yes/no): "))
            Console.WriteLine(_registry.Remove(student.Id) ? "Student deleted." : "Could not delete student.");
        else Console.WriteLine("Deletion cancelled.");
    }

    private void GenerateReport()
    {
        Student? student = ChooseStudent();
        if (student is null) return;
        Console.WriteLine("1. Average Grade Report");
        Console.WriteLine("2. Highest Grade Report");
        Console.WriteLine("3. Pass/Fail Report");
        int type = ReadInt("Choose report: ", 1, 3);
        AcademicReportCreator creator = type switch
        {
            1 => new AverageReportCreator(),
            2 => new HighestGradeReportCreator(),
            _ => new PassFailReportCreator()
        };
        bool withHeader = ReadYes("Add report header? (yes/no): ");
        bool withHonors = ReadYes("Add honors classification? (yes/no): ");
        Console.WriteLine("\n=== ACADEMIC REPORT ===");
        PrintProfile(student);
        Console.WriteLine(_reportFacade.Generate(student.Id, creator, withHeader, withHonors));
    }

    private void ChangeStatus()
    {
        Student? student = ChooseStudent();
        if (student is null) return;
        Console.WriteLine($"Current status: {student.State.Name}");
        Console.WriteLine("1. Suspend");
        Console.WriteLine("2. Reinstate");
        Console.WriteLine("3. Graduate");
        int option = ReadInt("Choose action: ", 1, 3);
        IStudentCommand command = option switch
        {
            1 => new SuspendStudentCommand(student),
            2 => new ReinstateStudentCommand(student),
            _ => new GraduateStudentCommand(student)
        };
        _history.Execute(command);
    }

    private void Undo()
    {
        string? undone = _history.Undo();
        Console.WriteLine(undone is null ? "Nothing to undo." : $"Undone: {undone}");
    }

    private void Redo()
    {
        string? redone = _history.Redo();
        Console.WriteLine(redone is null ? "Nothing to redo." : $"Redone: {redone}");
    }

    private void ShowHistory()
    {
        if (_history.ExecutedActions.Count == 0) { Console.WriteLine("No actions yet."); return; }
        int i = 1;
        foreach (string action in _history.ExecutedActions) Console.WriteLine($"{i++}. {action}");
    }

    private void ManageNotifications()
    {
        var names = _notifications.Names;
        for (int i = 0; i < names.Count; i++)
            Console.WriteLine($"{i + 1}. [{(_notifications.IsEnabled(names[i]) ? "ON " : "OFF")}] {names[i]}");
        int option = ReadInt("Toggle which observer? (0 = back): ", 0, names.Count);
        if (option == 0) return;
        _notifications.Toggle(names[option - 1]);
        Console.WriteLine($"{names[option - 1]} is now {(_notifications.IsEnabled(names[option - 1]) ? "subscribed" : "unsubscribed")}.");
    }

    private void ShowAuditLog()
    {
        if (_auditLog.Entries.Count == 0) Console.WriteLine("Audit log is empty (or the observer is unsubscribed).");
        foreach (string entry in _auditLog.Entries) Console.WriteLine(entry);
    }
}
