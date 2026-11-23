# TMPS Lab 3 — Behavioral Design Patterns

Student Grade Management System (C# console app), extended from Lab 2
(Singleton, Builder, Factory Method, Decorator, Proxy, Facade).

## Behavioral patterns added
- **State:** a `Student` is `Active`, `Suspended` or `Graduated`. The student delegates `AddGrade`, `Suspend`, `Reinstate` and `Graduate` to its current `IEnrollmentState` object, which allows or rejects the action.
- **Command:** every grade or status change is an `IStudentCommand` (`AddGradeCommand`, `SuspendStudentCommand`, `ReinstateStudentCommand`, `GraduateStudentCommand`) run by `CommandHistory`, which supports undo, redo and an action history.
- **Observer:** `Student` is the subject. `ConsoleNotifier`, `AuditLogObserver` and `AcademicRiskAlert` can subscribe and unsubscribe at runtime (menu option 12).

Pattern interaction: `CommandHistory.Execute(command)` → `Student` (context) → current state accepts or rejects → `Student.TransitionTo` / `StoreGrade` → observers are notified. Undo goes back through the same path.

## Requirements
.NET 10 SDK. No NuGet packages.

## Build and run
```bash
dotnet build
```
```bash
dotnet run
```
```bash
dotnet run -- --demo
```
```bash
dotnet run -- --test
```

- `dotnet run` — interactive menu. New options: 8 change status, 9 undo, 10 redo, 11 action history, 12 subscribe/unsubscribe observers, 13 audit log.
- `--demo` — scripted walkthrough of all three patterns working together.
- `--test` — 18 tests (6 from Lab 2, 4 for each behavioral pattern). Exit code is 1 if any test fails.

## Structure
```
Domain/            Student (State context + Observer subject), StudentEvent, grade calculators
Creational/        Lab 1/2: Singleton registry, Builder, Factory Method reports
Structural/        Lab 2: Decorator, Proxy, Facade
Behavioral/State/     IEnrollmentState, EnrollmentStateBase, ActiveState, SuspendedState, GraduatedState
Behavioral/Command/   IStudentCommand, AddGradeCommand, ChangeStatusCommand (+3 subclasses), CommandHistory
Behavioral/Observer/  IStudentObserver, IStudentSubject, ConsoleNotifier, AuditLogObserver, AcademicRiskAlert
Client/            ConsoleMenu, DemoScenario, NotificationSettings, ConsoleInput
Tests/             TestRunner and tests per pattern
Program.cs         entry point
REPORT.md          lab report
```

Data resets when the application restarts. The passing threshold (average 5) is a demo rule, not an official university rule.
