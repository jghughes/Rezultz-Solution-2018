using System.Xml.Linq;

namespace Tool09;

internal class FileItem
{
    public FileInfo FileInfo { get; set; } = new("DummyFileName.txt");

    public string FileContentsAsText { get; set; } = string.Empty;

    public XElement FileContentsAsXElement { get; set; } = new("Rubbish");

    public string OutputSubFolderName { get; set; } = string.Empty;
}