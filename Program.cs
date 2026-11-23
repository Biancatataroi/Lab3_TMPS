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
