using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool13;

internal class InputFileItem
{
    public FileInfo FileInfo { get; set; } = new("DummyFileName.txt");

    public string ContentsAsText { get; set; } = string.Empty;

    public List<PossiblySuspiciousFinish> ContentsAsCollectionOfPossiblySuspiciousFinishes { get; set; } = [];
}