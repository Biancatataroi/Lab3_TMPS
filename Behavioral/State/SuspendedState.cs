// STATE: Concrete State
public class SuspendedState : EnrollmentStateBase
{
    public override string Name => "Suspended";

    public override void Reinstate(Student student) => student.TransitionTo(new ActiveState());
}
