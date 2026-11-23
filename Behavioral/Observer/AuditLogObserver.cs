using System;
using System.Collections.Generic;

// Keeps a timestamped history of every change, viewable from the menu.
public class AuditLogObserver : IStudentObserver
{
    private readonly List<string> _entries = new();
    public IReadOnlyList<string> Entries => _entries;

    public void Update(StudentEvent e) =>
        _entries.Add($"{DateTime.Now:HH:mm:ss} | #{e.Student.Id} {e.Student.Name} | {e.Kind} | {e.Description}");
}
