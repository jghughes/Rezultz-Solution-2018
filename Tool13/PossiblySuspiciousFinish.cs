using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Tool13;

internal class PossiblySuspiciousFinish
{
    public EventProfileDto EventProfileDto = new ();

    public ResultDto ResultDto = new ();

    public string OfficialRaceGroup { get; set; } = string.Empty;

}