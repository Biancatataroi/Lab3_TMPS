using System;
using System.Collections.Generic;

// OBSERVER: Concrete Observer
public class AuditLogObserver : IStudentObserver
{
    private readonly List<string> _entries = new();
    public IReadOnlyList<string> Entries => _entries;

    public void Update(StudentEvent e) =>
        _entries.Add($"{DateTime.Now:HH:mm:ss} | #{e.Student.Id} {e.Student.Name} | {e.Kind} | {e.Description}");
}
