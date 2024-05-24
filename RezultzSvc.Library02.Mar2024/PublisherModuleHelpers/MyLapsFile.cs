namespace RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;

public class MyLapsFile
{
    public MyLapsFile()
    {
    }

    public MyLapsFile(string identifierOfDataSet, string fileName, string contents)
    {
        IdentifierOfDataset = identifierOfDataSet ?? string.Empty;
        FileName = fileName ?? string.Empty;
        FileContents = contents ?? string.Empty;
    }

    public string IdentifierOfDataset { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileContents { get; set; } = string.Empty;
}
