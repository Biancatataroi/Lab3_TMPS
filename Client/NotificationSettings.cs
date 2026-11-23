using System.Collections.Generic;
using System.Linq;

// Client-side helper: lets the user switch observers on and off at runtime.
// An enabled observer is subscribed to every student, including students added later.
public class NotificationSettings
{
    private readonly StudentRegistry _registry;
    private readonly Dictionary<string, IStudentObserver> _available = new();
    private readonly HashSet<string> _enabled = new();

    public NotificationSettings(StudentRegistry registry) => _registry = registry;

    public IReadOnlyList<string> Names => _available.Keys.ToList();
    public bool IsEnabled(string name) => _enabled.Contains(name);

    public void Register(string name, IStudentObserver observer, bool enabled)
    {
        _available[name] = observer;
        if (enabled) Enable(name);
    }

    public void Toggle(string name)
    {
        if (IsEnabled(name)) Disable(name);
        else Enable(name);
    }

    // Called when a new student joins the registry.
    public void AttachEnabledObservers(IStudentSubject student)
    {
        foreach (string name in _enabled) student.Subscribe(_available[name]);
    }

    private void Enable(string name)
    {
        _enabled.Add(name);
        foreach (Student student in _registry.Students) student.Subscribe(_available[name]);
    }

    private void Disable(string name)
    {
        _enabled.Remove(name);
        foreach (Student student in _registry.Students) student.Unsubscribe(_available[name]);
    }
}
