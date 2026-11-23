# Laboratory Work No. 3 — Behavioral Design Patterns

## 1. Domain description

The application is a console **Student Grade Management System** built in C# for Lab 1 and Lab 2.
A registry (Singleton) stores students created with a Builder; academic reports are produced by a
Factory Method, optionally decorated (Decorator), checked by a Proxy and coordinated by a Facade.

Lab 3 adds the life cycle of a student. A student can be **Active**, **Suspended** or **Graduated**, and
what can be done with the student depends on that status (State). Every grade or status change is a
command that can be **undone and redone** (Command). Whenever a student changes, interested
components — a console notifier, an audit log and an academic-risk alert for the dean's office —
are **notified automatically**, and the user can switch them on or off while the program runs (Observer).

### How the patterns interact

```mermaid
sequenceDiagram
    actor User
    participant Menu as ConsoleMenu (client)
    participant H as CommandHistory (invoker)
    participant C as IStudentCommand
    participant S as Student (context + subject)
    participant St as IEnrollmentState
    participant O as IStudentObserver(s)
    User->>Menu: 8 → Suspend student 1
    Menu->>H: Execute(new SuspendStudentCommand(student))
    H->>C: Execute()
    C->>S: Suspend()
    S->>St: Suspend(student)
    St->>S: TransitionTo(new SuspendedState())
    S->>O: Update(StudentEvent "Active -> Suspended")
    User->>Menu: 9 → Undo
    Menu->>H: Undo()
    H->>C: Undo()
    C->>S: RestoreState(previous)
    S->>O: Update(StudentEvent "Suspended -> Active (undo)")
```

One user action passes through all three patterns: Command → State → Observer.
The client only works with `CommandHistory`, `IStudentCommand`, `IStudentObserver` and `IEnrollmentState`.
Lab 2 patterns are reused: students are created with `StudentBuilder`, stored in the `StudentRegistry`
Singleton, and reports are still generated through `AcademicReportFacade` for students in any state.

**Behavior changes at runtime (advanced A):** the user moves students between states (menu 8),
undoes/redoes actions (9, 10) and subscribes/unsubscribes observers (12) while the app runs.

---

## 2.1 State

**Problem it solves here.** Without State, every operation would need `if (status == Suspended) ... else if (status == Graduated) ...`
checks spread across the menu and `GradeManager`. With State, each status is a class that decides which actions are allowed and
which status comes next. Adding a new status (for example `OnAcademicLeave`) means adding one new class.

### Class diagram

```mermaid
classDiagram
    class Student {
        <<Context>>
        -IEnrollmentState State
        +AddGrade(int grade)
        +Suspend()
        +Reinstate()
        +Graduate()
        ~StoreGrade(int grade)
        ~TransitionTo(IEnrollmentState next)
    }
    class IEnrollmentState {
        <<interface>>
        +Name string
        +AddGrade(Student, int)
        +Suspend(Student)
        +Reinstate(Student)
        +Graduate(Student)
    }
    class EnrollmentStateBase {
        <<abstract>>
        +AddGrade() rejects
        +Suspend() rejects
        +Reinstate() rejects
        +Graduate() rejects
        #Reject(string action)
    }
    class ActiveState {
        <<ConcreteState>>
        +AddGrade() stores grade
        +Suspend() to Suspended
        +Graduate() to Graduated if average >= 5
    }
    class SuspendedState {
        <<ConcreteState>>
        +Reinstate() to Active
    }
    class GraduatedState {
        <<ConcreteState>>
        final state
    }
    class InvalidStateTransitionException
    Student o--> IEnrollmentState : current state
    IEnrollmentState <|.. EnrollmentStateBase
    EnrollmentStateBase <|-- ActiveState
    EnrollmentStateBase <|-- SuspendedState
    EnrollmentStateBase <|-- GraduatedState
    EnrollmentStateBase ..> InvalidStateTransitionException : throws
    ActiveState ..> SuspendedState : creates
    ActiveState ..> GraduatedState : creates
    SuspendedState ..> ActiveState : creates
```

Transitions: `Active → Suspended` (suspend), `Suspended → Active` (reinstate), `Active → Graduated` (graduate, passing average required).
Everything else is rejected; `Graduated` is final.

### Code snippet

[Domain/Student.cs](Domain/Student.cs) and [Behavioral/State/ActiveState.cs](Behavioral/State/ActiveState.cs)

```csharp
// Student (context): delegates to the current state
public IEnrollmentState State { get; private set; } = new ActiveState();

public void AddGrade(int grade)
{
    if (grade < 1 || grade > 10)
        throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be between 1 and 10.");
    State.AddGrade(this, grade);
}
public void Suspend() => State.Suspend(this);
public void Reinstate() => State.Reinstate(this);
public void Graduate() => State.Graduate(this);

// ActiveState (concrete state): decides what happens and which state is next
public override void AddGrade(Student student, int grade) => student.StoreGrade(grade);
public override void Suspend(Student student) => student.TransitionTo(new SuspendedState());
public override void Graduate(Student student)
{
    double average = new GradeManager(new AverageCalculator()).GetResult(student);
    if (average < GradeManager.PassingAverage)
        throw new InvalidStateTransitionException($"Cannot graduate: average {average:F2} is below 5.00.");
    student.TransitionTo(new GraduatedState());
}
```

`Student` contains no `switch` on a status: it forwards each call to the current state object. The transitions live inside the
state classes (`ActiveState` creates `SuspendedState`, etc.), and `EnrollmentStateBase` rejects every action that a state does not override.

### Unit tests ([Tests/StatePatternTests.cs](Tests/StatePatternTests.cs))

| Test | What it checks |
|---|---|
| `State: valid transitions Active -> Suspended -> Active -> Graduated` | Allowed transitions change the state object. |
| `State: forbidden transitions are rejected and state is unchanged` | `Reinstate` on Active and `Suspend` on Graduated throw, and the state stays the same. |
| `State: same action behaves differently per state (AddGrade)` | Active stores the grade; Suspended rejects it and nothing is stored. |
| `State: graduation requires a passing average` | Graduating with average < 5 is rejected. |

---

## 2.2 Command

**Problem it solves here.** Grade entry and status changes are mistakes waiting to happen (wrong grade, wrong student suspended).
Turning each action into an object lets `CommandHistory` keep a history and undo/redo it. Business rules stay in the receiver
(`Student` and its state); commands only remember what is needed to reverse the action.

### Class diagram

```mermaid
classDiagram
    class IStudentCommand {
        <<interface>>
        +Description string
        +Execute()
        +Undo()
    }
    class AddGradeCommand {
        <<ConcreteCommand>>
        -Student _student
        -int _grade
        -int _position
        +Execute()
        +Undo()
    }
    class ChangeStatusCommand {
        <<abstract>>
        #Student Student
        -IEnrollmentState _previousState
        +Execute()
        +Undo()
        #Apply()*
    }
    class SuspendStudentCommand {
        <<ConcreteCommand>>
        #Apply()
    }
    class ReinstateStudentCommand {
        <<ConcreteCommand>>
        #Apply()
    }
    class GraduateStudentCommand {
        <<ConcreteCommand>>
        #Apply()
    }
    class CommandHistory {
        <<Invoker>>
        -Stack~IStudentCommand~ _undo
        -Stack~IStudentCommand~ _redo
        +Execute(IStudentCommand)
        +Undo() string
        +Redo() string
        +ExecutedActions
    }
    class Student {
        <<Receiver>>
        +AddGrade(int)
        +Suspend()
        +Reinstate()
        +Graduate()
        ~RemoveGradeAt(int)
        ~RestoreState(IEnrollmentState)
    }
    class ConsoleMenu {
        <<Client>>
    }
    IStudentCommand <|.. AddGradeCommand
    IStudentCommand <|.. ChangeStatusCommand
    ChangeStatusCommand <|-- SuspendStudentCommand
    ChangeStatusCommand <|-- ReinstateStudentCommand
    ChangeStatusCommand <|-- GraduateStudentCommand
    CommandHistory o--> IStudentCommand : history
    AddGradeCommand --> Student : receiver
    ChangeStatusCommand --> Student : receiver
    ConsoleMenu --> CommandHistory
    ConsoleMenu ..> IStudentCommand : creates
```

### Code snippet

[Behavioral/Command/CommandHistory.cs](Behavioral/Command/CommandHistory.cs) and [Behavioral/Command/ChangeStatusCommand.cs](Behavioral/Command/ChangeStatusCommand.cs)

```csharp
// Invoker
public void Execute(IStudentCommand command)
{
    command.Execute();      // if this throws, nothing is recorded
    _undo.Push(command);
    _redo.Clear();          // a new action makes the old redo branch invalid
}

public string? Undo()
{
    if (!CanUndo) return null;
    IStudentCommand command = _undo.Pop();
    command.Undo();
    _redo.Push(command);
    return command.Description;
}

// Concrete command: execute / undo pair
public void Execute()
{
    IEnrollmentState before = Student.State;
    Apply();                        // the current state accepts or rejects the transition
    _previousState = before;
}
public void Undo()
{
    if (_previousState is null) return;
    Student.RestoreState(_previousState);
    _previousState = null;
}
```

`CommandHistory` only knows `IStudentCommand`, so a new action (for example `ChangeGroupCommand`) is just a new class.
Each command stores exactly what it needs to reverse itself (the previous state object, or the position of the added grade),
and a command that the state rejects throws before being pushed, so the history never contains failed actions.

### Unit tests ([Tests/CommandPatternTests.cs](Tests/CommandPatternTests.cs))

| Test | What it checks |
|---|---|
| `Command: execute then undo AddGrade restores original grades` | Grades after undo are exactly the original list. |
| `Command: undo of a status change restores the previous state` | Suspend + undo gives back `ActiveState`. |
| `Command: redo re-applies the undone command` | Graduate, undo, redo → Graduated again; redo stack is empty. |
| `Command: a rejected command is not recorded in history` | A grade rejected by `SuspendedState` is not added to the history. |

---

## 2.3 Observer

**Problem it solves here.** Several parts of the system care about changes to a student: the console, an audit log and the
dean's office risk alert. Without Observer, `Student` (or every menu option) would have to call each of them directly.
With Observer, `Student` only knows `IStudentObserver`; new listeners are added as new classes and can be (un)subscribed at runtime.

### Class diagram

```mermaid
classDiagram
    class IStudentSubject {
        <<interface>>
        +Subscribe(IStudentObserver)
        +Unsubscribe(IStudentObserver)
    }
    class Student {
        <<ConcreteSubject>>
        -List~IStudentObserver~ _observers
        +Subscribe(IStudentObserver)
        +Unsubscribe(IStudentObserver)
        -Notify(StudentEvent)
    }
    class IStudentObserver {
        <<interface>>
        +Update(StudentEvent)
    }
    class ConsoleNotifier {
        <<ConcreteObserver>>
        -TextWriter _output
        +Update(StudentEvent)
    }
    class AuditLogObserver {
        <<ConcreteObserver>>
        +Entries
        +Update(StudentEvent)
    }
    class AcademicRiskAlert {
        <<ConcreteObserver>>
        +Alerts
        +Update(StudentEvent)
    }
    class StudentEvent {
        <<record>>
        +Student Student
        +StudentEventKind Kind
        +string Description
    }
    class NotificationSettings {
        <<Client helper>>
        +Register(name, observer, enabled)
        +Toggle(name)
    }
    IStudentSubject <|.. Student
    IStudentObserver <|.. ConsoleNotifier
    IStudentObserver <|.. AuditLogObserver
    IStudentObserver <|.. AcademicRiskAlert
    Student o--> IStudentObserver : observers
    Student ..> StudentEvent : sends
    NotificationSettings --> IStudentSubject : subscribe / unsubscribe
```

### Code snippet

[Domain/Student.cs](Domain/Student.cs)

```csharp
public void Subscribe(IStudentObserver observer)
{
    if (!_observers.Contains(observer)) _observers.Add(observer);
}

public void Unsubscribe(IStudentObserver observer) => _observers.Remove(observer);

private void Notify(StudentEvent studentEvent)
{
    // Iterate over a copy so an observer may unsubscribe while being notified.
    foreach (IStudentObserver observer in _observers.ToList())
        observer.Update(studentEvent);
}

// Called by a state object to move the student to the next state.
internal void TransitionTo(IEnrollmentState next)
{
    string previous = State.Name;
    State = next;
    Notify(new StudentEvent(this, StudentEventKind.StateChanged, $"status {previous} -> {next.Name}"));
}
```

The subject keeps a list of `IStudentObserver` and loops over it after every change; it never references a concrete observer.
Observers only read the student, so a notification cannot trigger another change and cause an update loop.

### Unit tests ([Tests/ObserverPatternTests.cs](Tests/ObserverPatternTests.cs))

| Test | What it checks |
|---|---|
| `Observer: all subscribed observers are notified` | Two subscribers each receive one `GradeAdded` event. |
| `Observer: unsubscribed observer is not notified` | After `Unsubscribe`, the observer receives nothing. |
| `Observer: academic risk alert reacts only when average < 5` | The alert fires only when the average drops below 5. |
| `Interaction: Command -> State -> Observer (execute and undo both notify)` | Executing and undoing a suspension both reach the audit log. |

---

## 2.4 Test run

Command: `dotnet run -- --test`

```
--- Lab 2: structural patterns ---
PASS: Decorator: header adds information
PASS: Decorator: two wrappers work in either order
PASS: Proxy: blocks students without grades
PASS: Proxy: returns unchanged report for valid students
PASS: Facade: generates a decorated report
PASS: Facade: refuses unknown student ID

--- Lab 3: State ---
PASS: State: valid transitions Active -> Suspended -> Active -> Graduated
PASS: State: forbidden transitions are rejected and state is unchanged
PASS: State: same action behaves differently per state (AddGrade)
PASS: State: graduation requires a passing average

--- Lab 3: Command ---
PASS: Command: execute then undo AddGrade restores original grades
PASS: Command: undo of a status change restores the previous state
PASS: Command: redo re-applies the undone command
PASS: Command: a rejected command is not recorded in history

--- Lab 3: Observer ---
PASS: Observer: all subscribed observers are notified
PASS: Observer: unsubscribed observer is not notified
PASS: Observer: academic risk alert reacts only when average < 5
PASS: Interaction: Command -> State -> Observer (execute and undo both notify)

Tests: 18 passed, 0 failed.
```

## 2.5 Demo output (excerpt, `dotnet run -- --demo`)

```
=== 3. STATE: suspend, then try actions the Suspended state forbids ===
> Suspend Maria Ciobanu
  [Notification] Maria Ciobanu: status Active -> Suspended
> Add grade 10 to Maria Ciobanu
  Rejected: Cannot add a grade: student is Suspended.
> Graduate Maria Ciobanu
  Rejected: Cannot graduate: student is Suspended.
  Status: Suspended | Grades: 4, 9

=== 4. COMMAND + STATE: undo the suspension restores the Active state ===
  [Notification] Maria Ciobanu: status Suspended -> Active (undo)
Undone: Suspend Maria Ciobanu
  Status: Active | Grades: 4, 9
```

---

## 3. Conclusions

**Harder than expected.**
- *Undo for state changes.* Undo cannot simply call the "opposite" action: there is no legal way out of `Graduated`, so
  "un-graduate" would be rejected by the state itself. The command therefore saves the previous state object and the receiver
  offers `RestoreState`, used only by undo. Because the history is a stack (LIFO), the student is always in the state the
  command left it in when undo runs.
- *Where the rules live.* It was tempting to check "is the student suspended?" inside the commands. Keeping that rule only in
  the state classes means the menu, the demo and the commands all get the same behaviour for free.
- *Grades had to be encapsulated.* In Lab 2 `Grades` was a public `List<int>`, so any code could bypass the state. It is now an
  `IReadOnlyList<int>`, and grades can only be added through the current state.

**What I would design differently.**
- Observers are attached per student; with many students a central event bus (or Mediator) would be simpler than
  subscribing every observer to every student (`NotificationSettings` does this work now).
- Adding a new *action* (not a new state) still requires adding a method to `IEnrollmentState`. A Visitor-like or
  command-based state API would avoid that, at the cost of more complexity.
- With more time I would move the tests to a separate xUnit project so they run with `dotnet test`.

### Patterns not implemented (short notes)
| Pattern | Where it could fit in this project |
|---|---|
| Chain of Responsibility | Validating a new grade through a chain: range check → state check → duplicate check. |
| Iterator | Traversing students by group or by average without exposing the registry list. |
| Mediator | A central coordinator between menu, registry and notifications instead of direct references. |
| Memento | Saving a full snapshot of a student (grades + state) for undo, instead of per-command data. |
| Strategy | `IGradeCalculator` (average / highest) from Lab 1 is already a Strategy. |
| Template Method | `ChangeStatusCommand.Execute()` is a small template method: fixed steps, `Apply()` varies. |
| Visitor | Exporting students to different formats (CSV, JSON) without changing `Student`. |
| Interpreter | Parsing simple queries like `average > 8 and group = FAF-242`. |
