using System;

// STATE: the interface every enrollment state implements.
// The Student (context) calls these methods; the state decides what happens.
public interface IEnrollmentState
{
    string Name { get; }

    void AddGrade(Student student, int grade);
    void Suspend(Student student);
    void Reinstate(Student student);
    void Graduate(Student student);
}

// Thrown when the current state does not allow an action.
public class InvalidStateTransitionException : InvalidOperationException
{
    public InvalidStateTransitionException(string message) : base(message) { }
}

// Default behaviour: every action is forbidden.
// A concrete state overrides only the actions it allows.
public abstract class EnrollmentStateBase : IEnrollmentState
{
    public abstract string Name { get; }

    public virtual void AddGrade(Student student, int grade) => Reject("add a grade");
    public virtual void Suspend(Student student) => Reject("suspend");
    public virtual void Reinstate(Student student) => Reject("reinstate");
    public virtual void Graduate(Student student) => Reject("graduate");

    protected void Reject(string action) =>
        throw new InvalidStateTransitionException($"Cannot {action}: student is {Name}.");
}
