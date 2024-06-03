using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool04
{
    internal class RezultzFileItem
    {
        public FileInfo RezultzFileInfo { get; set; } = new("DummyFileName.txt");

        public string RezultzFileContentsAsText { get; set; } = string.Empty;

        public List<ResultDto> RezultzFileContentsAsResultsDataTransferObjects { get; set; } = [];

        public List<MyLapsFileObject> ListOfMyLapsFileItems { get; set; } = [];

        public List<ResultDto> ModifiedRezultzFileContentsAsResultsDataTransferObjects { get; set; } = [];
    }
}