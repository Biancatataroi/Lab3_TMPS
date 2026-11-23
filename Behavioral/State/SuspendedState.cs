// Suspended: grades and graduation are blocked; the only way out is Reinstate.
public class SuspendedState : EnrollmentStateBase
{
    public override string Name => "Suspended";

    public override void Reinstate(Student student) => student.TransitionTo(new ActiveState());
}
