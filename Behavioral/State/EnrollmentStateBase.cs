// STATE: State base class
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
