using NetStd.Goodies.Mar2022;
using NetStd.ServiceLocation.Aug2022;
using Rezultz.Uwp.In_app_services;

using System;
using System.Collections.Generic;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.Library02.Mar2024.PageNavigation;
using Rezultz.Uwp.Pages;

//using CommonServiceLocator;

namespace Rezultz.Uwp.NavigationServiceJgh
{
    /// <inheritdoc />
    /// <summary>
    ///     The purpose of this service is to provide an implementation of INavigationServiceForRezultz
    ///     that can be registered in the Ioc for Rezultz viewmodels to use for navigation.
    ///     it's not primarily intended for code-behind page navigation: for that
    ///     it's simpler to employ the NavigationServiceEx singleton which is also registered in the Ioc.
    ///     This class employs singleton.
    /// </summary>
    public class NavigationServiceExForRezultzViewModels : INavigationServiceForRezultz
    {
        private const string Locus2 = nameof(NavigationServiceExForRezultzViewModels);
        private const string Locus3 = "[Rezultz.Uwp]";

        #region navigation service

        private static NavigationServiceEx NavigationServiceExSingleton
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<NavigationServiceEx>();
                }
                catch (Exception ex)
                {
                    var msg = JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, "<NavigationServiceEx>");

                    const string locus = "Property getter of <NavigationService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region Rezultz only - not used in Portal


        public void NavigateToAverageSplitTimesPage(Dictionary<string, string> navigationContext)
        {
            NavigationServiceExSingleton.Navigate(typeof(SingleEventAverageSplitTimesPage).FullName, navigationContext);
        }

        public void NavigateToSingleEventPopulationCohortsPage(Dictionary<string, string> navigationContext)
        {
            NavigationServiceExSingleton.Navigate(typeof(SingleEventPopulationCohortsPage).FullName, navigationContext);
        }

        public void NavigateToSeriesStandingsPage(Dictionary<string, string> navigationContext)
        {
            NavigationServiceExSingleton.Navigate(typeof(SeriesStandingsLeaderboardPage).FullName, navigationContext);
        }

        public void NavigateToSeriesPopulationCohortsPage(Dictionary<string, string> navigationContext)
        {
            NavigationServiceExSingleton.Navigate(typeof(SeriesPopulationCohortsPage).FullName, navigationContext);
        }

        #endregion

    }
}
