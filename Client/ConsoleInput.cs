using System;

public static class ConsoleInput
{
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = (Console.ReadLine() ?? "").Trim();
            if (value.Length > 0) return value;
            Console.WriteLine("This field is required.");
        }
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Enter a number between {min} and {max}.");
        }
    }

    public static bool ReadYes(string prompt)
    {
        Console.Write(prompt);
        return string.Equals((Console.ReadLine() ?? "").Trim(), "yes", StringComparison.OrdinalIgnoreCase);
    }
}
