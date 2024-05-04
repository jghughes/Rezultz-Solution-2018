using Rezultz.DataTypes.Nov2023.PortalHubItems;

namespace Rezultz.DataTypes.Nov2023.PortalSplitIntervalItems
{
    public class SplitIntervalAsPairOfTimeStampsItem
    {
        public TimeStampHubItem BeginningTimeStamp { get; set; }

        public TimeStampHubItem EndingTimeStamp { get; set; }

        public long CalculatedIntervalDurationTicks { get; set; }
    }
}