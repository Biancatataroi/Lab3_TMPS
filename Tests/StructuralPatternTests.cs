using static TestRunner;

public static class StructuralPatternTests
{
    public static void Run()
    {
        Check("Decorator: header adds information", () =>
        {
            var student = MakeStudent(8, 10);
            IAcademicReport report = new HeaderDecorator(new AverageReport());
            string result = report.Generate(student);
            Assert(result.Contains("ACADEMIC REPORT") && result.Contains("9.00"));
        });

        Check("Decorator: two wrappers work in either order", () =>
        {
            var student = MakeStudent(9, 9);
            IAcademicReport first = new HeaderDecorator(new HonorsDecorator(new AverageReport()));
            IAcademicReport second = new HonorsDecorator(new HeaderDecorator(new AverageReport()));
            Assert(first.Generate(student).Contains("Classification: Excellent"));
            Assert(second.Generate(student).Contains("ACADEMIC REPORT"));
            Assert(second.Generate(student).Contains("Classification: Excellent"));
        });

        Check("Proxy: blocks students without grades", () =>
        {
            IAcademicReport report = new ReportProxy(new AverageReport());
            AssertThrows<InvalidOperationException>(() => report.Generate(MakeStudent()));
        });

        Check("Proxy: returns unchanged report for valid students", () =>
        {
            var student = MakeStudent(7, 9);
            IAcademicReport real = new AverageReport();
            IAcademicReport proxy = new ReportProxy(real);
            Assert(proxy.Generate(student) == real.Generate(student));
        });

        Check("Facade: generates a decorated report", () =>
        {
            var registry = StudentRegistry.Instance;
            var student = MakeStudent(10, 10);
            if (registry.FindById(student.Id) is null) registry.Add(student);
            var facade = new AcademicReportFacade(registry);
            string result = facade.Generate(student.Id, new AverageReportCreator(), true, true);
            Assert(result.Contains("ACADEMIC REPORT"));
            Assert(result.Contains("Average grade: 10.00"));
            Assert(result.Contains("Classification: Excellent"));
        });

        Check("Facade: refuses unknown student ID", () =>
        {
            var facade = new AcademicReportFacade(StudentRegistry.Instance);
            AssertThrows<InvalidOperationException>(() => facade.Generate(-1, new HighestGradeReportCreator()));
        });
    }
}
