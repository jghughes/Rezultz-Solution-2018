using System.Text;

namespace Tool13
{
    public class JghConsoleHelperV2
    {
        private readonly StringBuilder _sb = new();

        #region methods

        public void Write(string text)
    {
        Console.Write(text);
        _sb.AppendLine(text);
    }

        public string ReadLine()
    {
        return Console.ReadLine() ?? string.Empty;
    }

        public void WriteLine()
    {
        Console.WriteLine();
        _sb.AppendLine("");
    }

        public void WriteLine(string line)
    {
        Console.WriteLine(line);
        _sb.AppendLine(line);
    }


        public void WriteLinePrecededByTwo(string line)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(line);
        _sb.AppendLine("");
        _sb.AppendLine("");
        _sb.AppendLine(line);
    }

        public void WriteLinePrecededByOne(string line)
    {
        Console.WriteLine();
        Console.WriteLine(line);
        _sb.AppendLine("");
        _sb.AppendLine(line);
    }

        public void WriteLineFollowedByTwo(string line)
    {
        Console.WriteLine(line);
        Console.WriteLine();
        Console.WriteLine();
        _sb.AppendLine(line);
        _sb.AppendLine("");
        _sb.AppendLine("");
    }

        public void WriteLineFollowedByOne(string line)
    {
        Console.WriteLine(line);
        Console.WriteLine();
        _sb.AppendLine(line);
        _sb.AppendLine("");
    }

        public void WriteLineWrappedByOne(string line)
    {
        Console.WriteLine();
        Console.WriteLine(line);
        Console.WriteLine();
        _sb.AppendLine("");
        _sb.AppendLine(line);
        _sb.AppendLine("");
    }

        public void WriteLineWrappedByTwo(string line)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(line);
        Console.WriteLine();
        Console.WriteLine();
        _sb.AppendLine("");
        _sb.AppendLine("");
        _sb.AppendLine(line);

        _sb.AppendLine("");
        _sb.AppendLine("");
    }

        public override string ToString()
    {
        return _sb.ToString();
    }
        #endregion
    }
}