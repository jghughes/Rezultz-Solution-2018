namespace Tool04;

internal class MyLapsResultObject
{
    public string Bib { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public TimeSpan Duration { get; set; } = TimeSpan.MinValue;

    public string RaceGroup { get; set; } = string.Empty;
}