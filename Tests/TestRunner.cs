using System;

public static class TestRunner
{
    private static int _passed;
    private static int _failed;

    public static void RunAll()
    {
        Console.WriteLine("--- Lab 2: structural patterns ---");
        StructuralPatternTests.Run();
        Console.WriteLine("\n--- Lab 3: State ---");
        StatePatternTests.Run();
        Console.WriteLine("\n--- Lab 3: Command ---");
        CommandPatternTests.Run();
        Console.WriteLine("\n--- Lab 3: Observer ---");
        ObserverPatternTests.Run();

        Console.WriteLine($"\nTests: {_passed} passed, {_failed} failed.");
        if (_failed > 0) Environment.ExitCode = 1;
    }

    public static void Check(string name, Action test)
    {
        try { test(); Console.WriteLine($"PASS: {name}"); _passed++; }
        catch (Exception ex) { Console.WriteLine($"FAIL: {name} - {ex.Message}"); _failed++; }
    }

    public static void Assert(bool condition, string message = "Assertion failed")
    {
        if (!condition) throw new Exception(message);
    }

    public static void AssertThrows<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new Exception($"Expected {typeof(TException).Name}");
    }

    public static Student MakeStudent(params int[] grades)
    {
        var builder = new StudentBuilder().SetId(900).SetName("Test Student")
            .SetFaculty("FCIM").SetGroup("FAF-242")
            .SetStudyYear(2).SetEmail("test@example.com");
        foreach (int grade in grades) builder.AddGrade(grade);
        return builder.Build();
    }
}
