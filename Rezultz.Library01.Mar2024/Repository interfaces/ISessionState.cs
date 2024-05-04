using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    /// <summary>
    ///     Uses an in-memory dictionary of certain key variables during a session, which can be saved/recovered as a
    ///     constituent of session state if desired
    /// </summary>
    public interface ISessionState
    {
        //string CurrentPageClassName { get; set; }
        string CurrentAmbientPageFooterMessage { get; set; }

        //SeriesItem CurrentCboSeriesItemLookupOnLeaderboardPage { get; set; }
        //EventItem CurrentCboEventItemLookupOnLeaderboardPage { get; set; }
        //MoreInformationItem CurrentCboMoreInfoItemLookupOnLeaderboardPage { get; set; }
        MoreInformationItem CurrentCboMoreInfoItemLookupOnSeriesStandingsPage { get; set; }
        //Dictionary<int, string> CurrentTxxColumnHeadersLookupTableOnSingleEventLeaderboardPage { get; set; }

        //bool TimeKeepingRoleIsProperlyInitialised { get; set; }
        //bool ParticipantAdminRoleIsProperlyInitialised { get; set; }
        //bool PublishingRoleIsProperlyInitialised { get; set; }

    }
}