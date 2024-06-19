// ReSharper disable RedundantUsingDirective
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.OnBoardServices;
using Jgh.Xamarin.Common.Jan2019;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices01.July2018.Persistence;
using NetStd.OnBoardServices02.July2018.FileStoreForRezultz;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
//using NetStd.ViewModels01.April2022;
using Rezultz.Uwp.In_app_services;
using Rezultz.Uwp.Local_storage;
using Rezultz.Uwp.NavigationServiceJgh;
using Rezultz.Uwp.UserSettings;
using System;
using Windows.UI.Xaml.Data;
using NetStd.DependencyInjection.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces02.July2018.Interfaces_dummies;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDummiesRezultz;
using Rezultz.Library02.Mar2024.DataGridInterfaces;
using Rezultz.Library02.Mar2024.PageNavigation;
using Rezultz.Library02.Mar2024.PageViewModels;
using Rezultz.Library02.Mar2024.ValidationViewModels;
using Rezultz.Uwp.Pages;
using Rezultz.Uwp.PageViewModels;
using RezultzSvc.Agents.Mar2024.SvcAgents;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService02;
using RezultzSvc.Clients.Wcf.Mar2023.ServiceClients;

// ReSharper disable InconsistentNaming

/* This pivotal class is instantiated declaratively in App xaml when the app launches.
 * See <Application.Resources> <ResourceDictionary> <viewModelLocator:ViewModelLocator x:Key="DependencyInjectionLocator" />
 *
 * It's crucial purpose is to instantiate a DI container, registering singletons and service classes that can be accessed in code
 * anywhere and everywhere, such as in library projects in foreign classes using Microsoft.CommonServiceLocator, and in xaml 
 * by way the ViewModelLocator's static properties for view models used as the data context for xaml pages. For data contexts, 
 * it is referenced from Application.Current.Resources resource dictionary in the code-behind of each page, and then that
 * reference with its static public prop for each registered page vm is assigned in xaml as the DataContext for the page. As follows: -
 * 
 * private static ViewModelLocator DependencyInjectionLocator => Application.Current.Resources["DependencyInjectionLocator"] as ViewModelLocator;
 * DataContext="{Binding PortalAboutPageVm, Source={StaticResource DependencyInjectionLocator}}"
 *
 * When required on rare occasions (undesirable anti-pattern), the DataContext can be referenced in a circular manner so that the
 * vm can be accessed in the code behind of the page. As follows: -
 *
 * private PortalAboutPageViewModel ViewModel => DataContext as PortalAboutPageViewModel;
 *
 * Once registered in the DI container, the DI container automatically injects the services and singletons into all classes that
 * depend on them by means of constructor injection. Alternatively they can be referenced manually using the CommonServiceLocator in property getters
 * using the ServiceLocator. Like so:
 *
 *  var something = ServiceLocator.Current.GetInstance<IRepositoryOfResultsForSingleEvent>();
 *
 * In our UWP app design, we choose to orchestrate page Navigation in the Shell xaml using the NavigationViewItem class which
 * in turn uses a singleton, RezultzPortal.Uwp.Services.NavigationServiceEx, which is instantiated and registered
 * in the DI container here. Each page must be added into a dictionary of pages in the singleton. Like so;-
 *
 *  Services.NavigationServiceEx navigationServiceExInstance = ConfigureNavigationServiceExSingleton();
 *  svc.Configure(typeof(Shell).FullName, typeof(Shell));
 *
 * using:RezultzPortal.Uwp.Helpers
 *  <NavigationViewItem helpers:NavHelper.NavigateTo="RezultzPortal.Uwp.Views___pages.AboutPage" />

*/

namespace Rezultz.Uwp.DependencyInjection
{
    [Bindable]
    public class DependencyInjectionLocator : IAlertMessageServiceLocator
    {
        private const string Locus2 = nameof(DependencyInjectionLocator);
        private const string Locus3 = "[Rezultz.Uwp]";

        #region ctor - the meat

        public DependencyInjectionLocator()
        {
            ServiceLocator.SetLocatorProvider(() => _dependencyContainer);

            #region page navigation

            var navigationServiceExInstance = ConfigureNavigationServiceExSingleton();

            _dependencyContainer.Register(() => navigationServiceExInstance);
            // the static object that all things in this app that aren't viewmodels - such as any other classes and pages and user controls for instance - can (and should)
            // reference for navigation via the static property of this ViewModelLocator called NavigationService e.g. see the multiple references in ActivationServiceJgh

            _dependencyContainer.Register<INavigationServiceForRezultz, NavigationServiceExForRezultzViewModels>();
            // INavigationServiceForRezultz is the interface that Rezultz view models can access by means of dependency injection. NavigationServiceExForRezultzViewModels uses NavigationService for all navigation

            #endregion

            #region local storage

            _dependencyContainer.Register<IStorageDirectoryPaths, IsolatedStoragePaths>(); // drags in nothing
            _dependencyContainer.Register<ILocalStorageService, LocalStorageServiceUwp>(); // recommended. drags in nothing
            //_container.Register<ILocalStorageService, IsolatedStorageService>(); this is the way to go in future because it works with everything including Xamarin. drags in IStorageDirectoryPaths

            #endregion

            #region user settings

            _dependencyContainer.Register<ISettingsService, SettingsServiceUwp>(); // drags in nothing
            _dependencyContainer.Register<IThingsPersistedInLocalStorage, ThingsPersistedInLocalStorage>(); // drags in ISettingsService

            #endregion

            #region file service

            // - not used. not tested

            _dependencyContainer.Register<IFileService, FileService>(); // drags in ILocalStorageService
            _dependencyContainer.Register<IFileServiceForRezultz, FileServiceForRezultz>(); // drags in IFileService and IStorageDirectoryPaths

            #endregion

            #region wcf/REST services

            if (AppSettings.MustUseMvcNotWcfForRemoteServices)
            {
                //_dependencyContainer.Register<IAzureStorageServiceClient, AzureStorageServiceClientMvc>();
                //_dependencyContainer.Register<ILeaderboardResultsServiceClient, LeaderboardResultsServiceClientMvc>();
                //_dependencyContainer.Register<IRaceResultsPublishingServiceClient, RaceResultsPublishingServiceClientMvc>();

                _dependencyContainer.Register<IAzureStorageServiceClient, AzureStorageServiceClientMvc02>();
                _dependencyContainer.Register<ILeaderboardResultsServiceClient, LeaderboardResultsServiceClientMvc02>();
                _dependencyContainer.Register<IRaceResultsPublishingServiceClient, RaceResultsPublishingServiceClientMvc02>();
            }
            else
            {
                _dependencyContainer.Register<IAzureStorageServiceClient, AzureStorageServiceClientWcf>();
                _dependencyContainer.Register<ILeaderboardResultsServiceClient, LeaderboardResultsServiceClientWcf>();
                _dependencyContainer.Register<IRaceResultsPublishingServiceClient, RaceResultsPublishingServiceClientWcf>();
            }

            _dependencyContainer.Register<IAzureStorageSvcAgent, AzureStorageSvcAgent>();
            _dependencyContainer.Register<ILeaderboardResultsSvcAgent, LeaderboardResultsSvcAgent>();
            _dependencyContainer.Register<IRaceResultsPublishingSvcAgent, RaceResultsPublishingSvcAgent>();

            #endregion

            #region local dynamic services - obtained live by property getter

            /*
            * we choose to not have a fixed app-wide IAlertMessageService,
            * but rather to let each page or user control provide the service. in this approach each page registers 
            * itself as the provider in its OnLoaded handler, and de-registers itself in its OnUnloaded handler. the Dummy here is a merely a 
            * transient placeholder until a proper service is registered by means of RegisterIAlertMessageServiceProvider() below
            */
            _dependencyContainer.Register<IAlertMessageService, AlertMessageServiceDummy>();


            /* 
             * we choose these services to be handled by pages that might display one or more DataGrids or might do something else exotic .
             * The typical hierarchy is that IResultsListPresentationService is implemented by LeaderboardDataGridPresentationServiceUserControl
             * which is contained by PageContentsForLeaderboardStylePageUserControl which in turn is contained by a page such as SingleEventLeaderboardPage.
             * Whenever the page loads it registers itself as the transient provider of IResultsListPresentationService in its OnLoading method by calling
             * the method RegisterILeaderboardListPresentationServiceProvider below. dummy for now
             */
            _dependencyContainer.Register<ILeaderboardDataGridPresentationService, LeaderboardDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<IFavoritesDataGridPresentationService, FavoritesDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<IPopulationCohortsDataGridPresentationService, PopulationCohortsDataGridPresentationServiceDummy>();

            #endregion

            #region global singletons

            _dependencyContainer.Register<ISessionState, SessionState>();
            _dependencyContainer.Register<ISeasonProfileAndIdentityValidationViewModel, SeasonProfileAndIdentityValidationViewModel>();
            _dependencyContainer.Register<IUserSettingsServiceViewModel, UserSettingsServiceViewModel>();
            _dependencyContainer.Register<IProgressIndicatorViewModel, ProgressIndicatorViewModelXamarin>();

            /* NB. beware that the following registration of RepositoryOfResultsForSingleEvent is a tad deceptive because
             * the class is also instantiated explicitly in SingleEventAverageSplitTimesPageViewModel and RepositoryOfSeriesStandings.
             * It has to be done this way because it is not a global singleton there. It is used as a transient calculating machine.
             * If you introduce a different version, you unfortunately need to explicitly update the code. The DI does not handle this.
             */

            _dependencyContainer.Register<IRepositoryOfResultsForSingleEvent, RepositoryOfResultsForSingleEvent>();
            _dependencyContainer.Register<IRepositoryOfSeriesStandings, RepositoryOfSeriesStandings>();

            #endregion

            #region viewmodels for the public static vm props for datacontexts for views

            _dependencyContainer.Register<ShellViewModel>();

            _dependencyContainer.Register<HomePageViewModel>();
            _dependencyContainer.Register<AboutPageViewModel>();
            _dependencyContainer.Register<PreferencesPageViewModel>();

            _dependencyContainer.Register<SingleEventLeaderboardPageViewModel>();
            _dependencyContainer.Register<SingleEventPopulationCohortsPageViewModel>();
            _dependencyContainer.Register<SingleEventAverageSplitTimesPageViewModel>();
            _dependencyContainer.Register<SeriesStandingsLeaderboardPageViewModel>();
            _dependencyContainer.Register<SeriesPopulationCohortsPageViewModel>();

            #endregion
        }

        #endregion

        #region field

        private readonly DependencyInjectionContainer _dependencyContainer = new();

        #endregion

        #region props

        // page navigation

        public NavigationServiceEx NavigationService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<NavigationServiceEx>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[Services.NavigationServiceEx]");

                    const string locus = "Property getter of [NavigationService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }

            // P.S. don't refactor the name of this property. The name is referred to all over the place in Widows Template Studio generated classes
        }


        // DataContexts for pages

        public ShellViewModel ShellVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ShellViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[ShellVm]");

                    const string locus = "Property getter of [ShellVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public HomePageViewModel HomePageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<HomePageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[HomePageViewModel]");

                    const string locus = "Property getter of [HomePageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public AboutPageViewModel AboutPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<AboutPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[AboutPageViewModel]");

                    const string locus = "Property getter of [AboutPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public PreferencesPageViewModel PreferencesPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<PreferencesPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[PreferencesPageViewModel]");

                    const string locus = "Property getter of [PreferencesPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public SingleEventLeaderboardPageViewModel SingleEventLeaderboardPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<SingleEventLeaderboardPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[SingleEventLeaderboardPageViewModel]");

                    const string locus = "Property getter of [SingleEventLeaderboardPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public SingleEventAverageSplitTimesPageViewModel SingleEventAverageSplitTimesPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<SingleEventAverageSplitTimesPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[SingleEventAverageSplitTimesPageViewModel]");

                    const string locus = "Property getter of [SingleEventAverageSplitTimesPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public SingleEventPopulationCohortsPageViewModel SingleEventPopulationCohortsPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<SingleEventPopulationCohortsPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[SingleEventPopulationCohortsPageViewModel]");

                    const string locus = "Property getter of [SingleEventPopulationCohortsPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public SeriesStandingsLeaderboardPageViewModel SeriesStandingsLeaderboardPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<SeriesStandingsLeaderboardPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[SeriesStandingsLeaderboardPageViewModel]");

                    const string locus = "Property getter of [SeriesStandingsLeaderboardPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public SeriesPopulationCohortsPageViewModel SeriesPopulationCohortsPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<SeriesPopulationCohortsPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(
                            StringsForXamlPages.UnableToRetrieveInstance,
                            "[SeriesPopulationCohortsPageViewModel]");

                    const string locus = "Property getter of [SeriesPopulationCohortsPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        // DataContexts for views

        public static IUserSettingsServiceViewModel GlobalUserSettingsServiceVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IUserSettingsServiceViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, $"[{nameof(IUserSettingsServiceViewModel)}]");

                    const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(GlobalUserSettingsServiceVm)}]";

                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public IProgressIndicatorViewModel GlobalProgressIndicatorVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IProgressIndicatorViewModel>();
                }
                catch (Exception ex)
                {
                    var msg = JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, $"[{nameof(IProgressIndicatorViewModel)}]");

                    const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(GlobalProgressIndicatorVm)}]";

                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public static ISeasonProfileAndIdentityValidationViewModel GlobalSeasonProfileAndIdentityValidationVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ISeasonProfileAndIdentityValidationViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, $"[{nameof(ISeasonProfileAndIdentityValidationViewModel)}]");

                    const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(GlobalSeasonProfileAndIdentityValidationVm)}]";

                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region dynamic registrations

        public void DeRegisterIAlertMessageServiceProvider()
        {
            _dependencyContainer.Register<IAlertMessageService, AlertMessageServiceDummy>();
        }
        public void RegisterIAlertMessageServiceProvider(IAlertMessageService implementingObject)
        {
            if (implementingObject is null)
                _dependencyContainer.Register<IAlertMessageService, AlertMessageServiceDummy>();
            else
                _dependencyContainer.Register(() => implementingObject);
        }

        public void DeRegisterILeaderboardListPresentationServiceProvider()
        {
            _dependencyContainer.Register<ILeaderboardDataGridPresentationService, LeaderboardDataGridPresentationServiceDummy>();
        }
        public void RegisterILeaderboardListPresentationServiceProvider(ILeaderboardDataGridPresentationService serviceImplementingObject)
        {
            if (serviceImplementingObject is null)
                _dependencyContainer.Register<ILeaderboardDataGridPresentationService, LeaderboardDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => serviceImplementingObject);
        }

        public void DeRegisterIFavoritesListPresentationServiceProvider()
        {
            _dependencyContainer.Register<IFavoritesDataGridPresentationService, FavoritesDataGridPresentationServiceDummy>();
        }
        public void RegisterIFavoritesListPresentationServiceProvider(IFavoritesDataGridPresentationService serviceImplementingObject)
        {
            if (serviceImplementingObject is null)
                _dependencyContainer
                    .Register<IFavoritesDataGridPresentationService, FavoritesDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => serviceImplementingObject);
        }

        public void DeRegisterIPopulationCohortsPresentationServiceProvider()
        {
            _dependencyContainer.Register<IPopulationCohortsDataGridPresentationService, PopulationCohortsDataGridPresentationServiceDummy>();
        }
        public void RegisterIPopulationCohortsPresentationServiceProvider(IPopulationCohortsDataGridPresentationService serviceImplementingObject)
        {
            if (serviceImplementingObject is null)
                _dependencyContainer
                    .Register<IPopulationCohortsDataGridPresentationService, PopulationCohortsDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => serviceImplementingObject);
        }

        #endregion

        #region navigation - pages

        private static NavigationServiceEx ConfigureNavigationServiceExSingleton()
        {
            var svc = new NavigationServiceEx();

            svc.Configure(typeof(Shell).FullName, typeof(Shell));

            svc.Configure(typeof(HomePage).FullName, typeof(HomePage));
            svc.Configure(typeof(AboutPage).FullName, typeof(AboutPage));
            svc.Configure(typeof(PreferencesPage).FullName, typeof(PreferencesPage));

            svc.Configure(typeof(SingleEventLeaderboardPage).FullName, typeof(SingleEventLeaderboardPage));
            svc.Configure(typeof(SingleEventAverageSplitTimesPage).FullName, typeof(SingleEventAverageSplitTimesPage));
            svc.Configure(typeof(SingleEventPopulationCohortsPage).FullName, typeof(SingleEventPopulationCohortsPage));

            svc.Configure(typeof(SeriesStandingsLeaderboardPage).FullName, typeof(SeriesStandingsLeaderboardPage));
            svc.Configure(typeof(SeriesPopulationCohortsPage).FullName, typeof(SeriesPopulationCohortsPage));

            return svc;
        }

        #endregion

    }
}
