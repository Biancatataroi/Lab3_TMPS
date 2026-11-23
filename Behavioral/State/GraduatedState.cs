// Graduated: final state. Every action is rejected by EnrollmentStateBase.
// Reports can still be generated, because they only read grades.
public class GraduatedState : EnrollmentStateBase
{
    public override string Name => "Graduated";
}
