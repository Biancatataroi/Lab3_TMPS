// Entry point.
//   dotnet run              -> interactive menu
//   dotnet run -- --demo    -> scripted demo of all patterns
//   dotnet run -- --test    -> unit tests

// Same number format (9.00, not 9,00) on every machine.
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

if (args.Contains("--test"))
{
    TestRunner.RunAll();
    return;
}

if (args.Contains("--demo"))
{
    DemoScenario.Run();
    return;
}

new ConsoleMenu(StudentRegistry.Instance).Run();
