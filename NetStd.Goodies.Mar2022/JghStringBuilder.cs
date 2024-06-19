using System.Text;

namespace NetStd.Goodies.Mar2022
{
    public class JghStringBuilder
    {
        private readonly StringBuilder _sb;

        #region ctor

        public JghStringBuilder()
    {
        _sb = new StringBuilder();
    }

        #endregion

        #region methods

        public void Append(string text)
        {
            _sb.Append(text);
        }

        public void AppendAsNewSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            _sb.Append(" ");

            _sb.Append(text);
        }


        public void AppendLine()
    {
        _sb.AppendLine();
    }

        public void AppendLine(string line)
    {
        _sb.AppendLine(line);
    }

        public void AppendLine(StringBuilder sb)
    {
        if (sb is null || sb.Length == 0) return;

        _sb.AppendLine(sb.ToString());
    }

        public void AppendLine(JghStringBuilder sb)
    {
        if (sb is null) return;

        _sb.AppendLine(sb.ToString());
    }

        public void AppendLinePrecededByTwo(string line)
    {
        _sb.AppendLine("");
        _sb.AppendLine("");
        _sb.AppendLine(line);
    }

        public void AppendLinePrecededByOne(string line)
    {
        _sb.AppendLine("");
        _sb.AppendLine(line);
    }

        public void AppendLineFollowedByTwo(string line)
    {
        _sb.AppendLine(line);
        _sb.AppendLine("");
        _sb.AppendLine("");
    }

        public void AppendLineFollowedByOne(string line)
    {
        _sb.AppendLine(line);
        _sb.AppendLine("");
    }

        public void AppendLineWrappedByOne(string line)
    {
        _sb.AppendLine("");
        _sb.AppendLine(line);
        _sb.AppendLine("");
    }

        public string AppendLineThenToString(JghStringBuilder sb)
    {
        if (sb is null) return null;

        _sb.AppendLine(sb.ToString());

        return _sb.ToString();
    }


        public void Clear()
    {
        _sb.Clear();
    }

        public new string ToString()
    {
        return _sb.ToString();
    }

        #endregion
    }
}