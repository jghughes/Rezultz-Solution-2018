namespace Tool10;

public static class JghConsoleHelper
{

    #region methods

    public static void Write(string text)
    {
        Console.Write(text);
    }

    public static string ReadLine()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    public static void WriteLine()
    {
        Console.WriteLine();
    }

    public static void WriteLine(string line)
    {
        Console.WriteLine(line);
    }


    
    public static void WriteLinePrecededByTwo(string line)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(line);
    }

    public static void WriteLinePrecededByOne(string line)
    {
        Console.WriteLine();
        Console.WriteLine(line);
    }

    public static void WriteLineFollowedByTwo(string line)
    {
        Console.WriteLine(line);
        Console.WriteLine();
        Console.WriteLine();
    }

    public static void WriteLineFollowedByOne(string line)
    {
        Console.WriteLine(line);
        Console.WriteLine();
    }

    public static void WriteLineWrappedInOne(string line)
    {
        Console.WriteLine();
        Console.WriteLine(line);
        Console.WriteLine();
    }

    public static void WriteLineWrappedInTwo(string line)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(line);
        Console.WriteLine();
        Console.WriteLine();
    }


    #endregion
}