using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repository_interfaces;

namespace Rezultz.Library01.Mar2024.Repositories
{
    /// <summary>
    ///     A container for props required for carrying the current state of an ongoing session
    /// </summary>
    /// <seealso cref="ISessionState" />
    public class SessionState : ISessionState
    {

        //public string CurrentPageClassName { get; set; } = string.Empty;
        public string CurrentAmbientPageFooterMessage { get; set; } = string.Empty;


        // Rezultz

        //public SeriesItem CurrentCboSeriesItemLookupOnLeaderboardPage { get; set; } = new();
        //public EventItem CurrentCboEventItemLookupOnLeaderboardPage { get; set; } = new();
        //public MoreInformationItem CurrentCboMoreInfoItemLookupOnLeaderboardPage { get; set; } = new();
        public MoreInformationItem CurrentCboMoreInfoItemLookupOnSeriesStandingsPage { get; set; } = new();
        //public Dictionary<int, string> CurrentTxxColumnHeadersLookupTableOnSingleEventLeaderboardPage { get; set; } = new();

        // Portal

        //public bool TimeKeepingRoleIsProperlyInitialised { get; set; }
        //public bool ParticipantAdminRoleIsProperlyInitialised { get; set; }
        //public bool PublishingRoleIsProperlyInitialised { get; set; }



    }
}