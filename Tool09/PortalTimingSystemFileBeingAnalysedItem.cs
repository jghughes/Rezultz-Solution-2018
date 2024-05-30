using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool09;

internal class PortalTimingSystemFileBeingAnalysedItem
{
    public FileInfo SingleRezultzFileInfo { get; set; } = new("DummyFileName.txt");

    public List<ResultDto> SingleRezultzFileContentsAsResultsDataTransferObjects { get; set; } = [];

    public List<MyLapsFileItem> ListOfMyLapsFileObjects { get; set; } = [];

    public List<ResultDto> CalculatedListOfPortalResultsThatAreMissingFromMyLapsFiles { get; set; } = [];

    public List<MyLapsResultItem> CalculatedListOfMyLapsResultsThatAreMissingFromPortalFile { get; set; } = [];
}