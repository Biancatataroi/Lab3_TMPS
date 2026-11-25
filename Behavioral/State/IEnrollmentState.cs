using System;

// STATE: State interface
public interface IEnrollmentState
{
    string Name { get; }

    void AddGrade(Student student, int grade);
    void Suspend(Student student);
    void Reinstate(Student student);
    void Graduate(Student student);
}
