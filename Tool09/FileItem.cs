using System.Xml.Linq;

namespace Tool09;

internal class FileItem
{
    public FileInfo FileInfo { get; set; } = new("DummyFileName.txt");

    public string FileContentsAsText { get; set; } = string.Empty;

    public XDocument FileContentsAsXDocument { get; set; } = new();

    public string OutputFileName { get; set; } = string.Empty;
}