using System;
using System.Collections.Generic;
using Rezultz.Library02.Mar2024.PageNavigation;

namespace RezultzPortal.Uwp.NavigationServiceJgh
{
    /// <inheritdoc />
    /// <summary>
    ///     The purpose of this service is to provide an implementation of INavigationServiceForRezultz
    ///     that can be registered in the Ioc for Rezultz viewmodels to use for navigation.
    ///     it's not primarily intended for code-behind page navigation: for that
    ///     it's simpler to employ the NavigationService singleton which is also registered in the Ioc.
    ///     This class is merely a wrapper for that singleton.
    /// </summary>
    public class NavigationServiceExForRezultzPortalViewModels : INavigationServiceForRezultz
    {

        #region Rezultz only - not used in Portal

        public void NavigateToAverageSplitTimesPage(Dictionary<string, string> navigationContext)
        {
            throw new NotImplementedException();
        }

        public void NavigateToSingleEventPopulationCohortsPage(Dictionary<string, string> navigationContext)
        {
            throw new NotImplementedException();
        }

        public void NavigateToSeriesStandingsPage(Dictionary<string, string> navigationContext)
        {
            throw new NotImplementedException();
        }

        public void NavigateToSeriesPopulationCohortsPage(Dictionary<string, string> navigationContext)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
