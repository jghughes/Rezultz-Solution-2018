using System.Collections.Generic;

namespace Rezultz.Library02.Mar2024.PageNavigation
{
    /// <summary>
    ///     Navigation methods that Rezultz ViewModels can avail themselves of
    /// </summary>
    public interface INavigationServiceForRezultz
    {
        #region Rezultz pages

        void NavigateToAverageSplitTimesPage(Dictionary<string, string> navigationContext);
        void NavigateToSingleEventPopulationCohortsPage(Dictionary<string, string> navigationContext);
        void NavigateToSeriesStandingsPage(Dictionary<string, string> navigationContext);
        void NavigateToSeriesPopulationCohortsPage(Dictionary<string, string> navigationContext);

        #endregion

    }
}