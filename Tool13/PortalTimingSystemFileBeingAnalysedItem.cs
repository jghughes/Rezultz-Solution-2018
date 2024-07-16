using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool13;

internal class PortalTimingSystemFileBeingAnalysedItem
{
    public FileInfo SingleRezultzFileInfo { get; set; } = new("DummyFileName.txt");

    public List<ResultDto> SingleRezultzFileContentsAsResultsDataTransferObjects { get; set; } = [];

    public List<InputFileItem> PublishedResultsFileItems { get; set; } = [];


}