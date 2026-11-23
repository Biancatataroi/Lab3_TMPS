using System;
using System.Collections.Generic;
using System.Linq;

// Student is the domain model and plays two behavioral roles:
//  - STATE context:      it holds an IEnrollmentState and delegates actions to it.
//  - OBSERVER subject:   it notifies subscribed IStudentObserver objects about changes.
// It is also the RECEIVER of the commands in Behavioral/Command.
public class Student : IStudentSubject
{
    private readonly List<int> _grades = new();
    private readonly List<IStudentObserver> _observers = new();

    public int Id { get; }
    public string Name { get; set; }
    public string Faculty { get; set; }
    public string Group { get; set; }
    public int StudyYear { get; set; }
    public string Email { get; set; }

    // Read-only view: grades can only change through the current state.
    public IReadOnlyList<int> Grades => _grades;

    // Every new student starts as Active.
    public IEnrollmentState State { get; private set; } = new ActiveState();

    public Student(int id, string name, string faculty, string group, int studyYear, string email)
    {
        Id = id;
        Name = name;
        Faculty = faculty;
        Group = group;
        StudyYear = studyYear;
        Email = email;
    }

    // ---------- STATE: the context delegates to its current state ----------

    public void AddGrade(int grade)
    {
        if (grade < 1 || grade > 10)
            throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be between 1 and 10.");
        State.AddGrade(this, grade);
    }

    public void Suspend() => State.Suspend(this);
    public void Reinstate() => State.Reinstate(this);
    public void Graduate() => State.Graduate(this);

    // Called by a state object once it has allowed the action.
    internal void StoreGrade(int grade)
    {
        _grades.Add(grade);
        Notify(new StudentEvent(this, StudentEventKind.GradeAdded, $"grade {grade} added"));
    }

    // Called by a state object to move the student to the next state.
    internal void TransitionTo(IEnrollmentState next)
    {
        string previous = State.Name;
        State = next;
        Notify(new StudentEvent(this, StudentEventKind.StateChanged, $"status {previous} -> {next.Name}"));
    }

    // ---------- Used only by Command.Undo() to reverse a previous action ----------

    internal void RemoveGradeAt(int index)
    {
        int grade = _grades[index];
        _grades.RemoveAt(index);
        Notify(new StudentEvent(this, StudentEventKind.GradeRemoved, $"grade {grade} removed (undo)"));
    }

    internal void RestoreState(IEnrollmentState previous)
    {
        string current = State.Name;
        State = previous;
        Notify(new StudentEvent(this, StudentEventKind.StateChanged, $"status {current} -> {previous.Name} (undo)"));
    }

    // ---------- OBSERVER: subject side ----------

    public void Subscribe(IStudentObserver observer)
    {
        if (!_observers.Contains(observer)) _observers.Add(observer);
    }

    public void Unsubscribe(IStudentObserver observer) => _observers.Remove(observer);

    public bool IsSubscribed(IStudentObserver observer) => _observers.Contains(observer);

    private void Notify(StudentEvent studentEvent)
    {
        // Iterate over a copy so an observer may unsubscribe while being notified.
        foreach (IStudentObserver observer in _observers.ToList())
            observer.Update(studentEvent);
    }
}
