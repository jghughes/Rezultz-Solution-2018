using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool10
{
    internal class RezultzFileItem : FileItem
    {
        public List<ResultDto> RezultzFileContentsAsResultsDataTransferObjects { get; set; }= [];

    }
}