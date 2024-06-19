// ReSharper disable RedundantUsingDirective
using System;
using Windows.UI.Xaml.Data;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.OnBoardServices;
using Jgh.Xamarin.Common.Jan2019;
using NetStd.DependencyInjection.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces02.July2018.Interfaces_dummies;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices01.July2018.Persistence;
using NetStd.OnBoardServices02.July2018.FileStoreForRezultz;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
using RezultzPortal.Uwp.In_app_services;
using RezultzPortal.Uwp.Local_storage;
using RezultzPortal.Uwp.NavigationServiceJgh;
using RezultzPortal.Uwp.Pages;
using RezultzPortal.Uwp.PageViewModels;
using RezultzPortal.Uwp.Strings;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDummiesPortal;
using Rezultz.Library02.Mar2024.DataGridDummiesRezultz;
using Rezultz.Library02.Mar2024.DataGridInterfaces;
using Rezultz.Library02.Mar2024.PageNavigation;
using RezultzSvc.Agents.Mar2024.SvcAgents;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService02;
using RezultzSvc.Clients.Wcf.Mar2023.ServiceClients;


/* This pivotal class is instantiated declaratively in App xaml when the app launches.
 *   See <Application.Resources> <ResourceDictionary> <viewModelLocator:DependencyInjectionLocator x:Key="DependencyInjectionLocator" />
 *
 * It's crucial purpose is to instantiate a DI container, registering singletons and service classes that can be accessed in code
 * anywhere and everywhere, such as in library projects in foreign classes using Microsoft.CommonServiceLocator, and in xaml 
 * by way the DependencyInjectionLocator's static properties for view models used as the data context for xaml pages. For data contexts, 
 * it is referenced from Application.Current.Resources resource dictionary in the code-behind of each page, and then that
 * reference with its static public prop for each registered page vm is assigned in xaml as the DataContext for the page. As follows: -
 * 
 * private static DependencyInjectionLocator DependencyInjectionLocator => Application.Current.Resources["DependencyInjectionLocator"] as DependencyInjectionLocator;
 * DataContext="{Binding AboutPageVm, Source={StaticResource DependencyInjectionLocator}}"
 *
 * When required on rare occasions (undesirable anti-pattern), the DataContext can be referenced in a circular manner so that the
 * vm can be accessed in the code behind of the page. As follows: -
 *
 * private AboutPageViewModel PagesViewModel => DataContext as AboutPageViewModel;
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

namespace RezultzPortal.Uwp.DependencyInjection
{
    [Bindable]
    public class DependencyInjectionLocator : IAlertMessageServiceLocator
    {
        private const string Locus2 = nameof(DependencyInjectionLocator);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        #region ctor - the meat

        public DependencyInjectionLocator()
        {
            ServiceLocator.SetLocatorProvider(() => _dependencyContainer); //NB

            #region page navigation

            var navigationServiceExInstance = ConfigureNavigationServiceExSingleton();

            _dependencyContainer.Register(() => navigationServiceExInstance);
            // the static object that all things in this app that aren't viewmodels - such as any other classes and pages and usercontrols for instance - can (and should)
            // reference for navigation via the static property of this DependencyInjectionLocator called NavigationService e.g. see the multiple references in ActivationServiceJgh

            _dependencyContainer.Register<INavigationServiceForRezultz, NavigationServiceExForRezultzPortalViewModels>();
            // INavigationServiceForRezultz is the interface that Rezultz view models can access by means of dependency injection. it's a wrapper for NavigationService

            #endregion

            #region local storage

            _dependencyContainer.Register<IStorageDirectoryPaths, IsolatedStoragePaths>(); // drags in nothing
            _dependencyContainer.Register<ILocalStorageService, LocalStorageServiceUwp>(); // recommended. drags in nothing
            //_dependencyContainer.Register<ILocalStorageService, IsolatedStorageService>(); // alternative. drags in nothing

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

            #region mvc/wcf remote services

            if (AppSettings.MustUseMvcNotWcfForRemoteServices)
            {
                //_dependencyContainer.Register<ILeaderboardResultsServiceClient, LeaderboardResultsServiceClientMvc>();
                //_dependencyContainer.Register<ITimeKeepingServiceClient, TimeKeepingServiceClientMvc>();
                //_dependencyContainer.Register<IParticipantRegistrationServiceClient, ParticipantRegistrationServiceClientMvc>();
                //_dependencyContainer.Register<IAzureStorageServiceClient, AzureStorageServiceClientMvc>();
                //_dependencyContainer.Register<IRaceResultsPublishingServiceClient, RaceResultsPublishingServiceClientMvc>();

                _dependencyContainer.Register<ILeaderboardResultsServiceClient, LeaderboardResultsServiceClientMvc02>();
                _dependencyContainer.Register<ITimeKeepingServiceClient, TimeKeepingServiceClientMvc02>();
                _dependencyContainer.Register<IParticipantRegistrationServiceClient, ParticipantRegistrationServiceClientMvc02>();
                _dependencyContainer.Register<IAzureStorageServiceClient, AzureStorageServiceClientMvc02>();
                _dependencyContainer.Register<IRaceResultsPublishingServiceClient, RaceResultsPublishingServiceClientMvc02>();
            }
            else
            {
                _dependencyContainer.Register<ILeaderboardResultsServiceClient, LeaderboardResultsServiceClientWcf>();
                _dependencyContainer.Register<ITimeKeepingServiceClient, TimeKeepingServiceClientWcf>();
                _dependencyContainer.Register<IParticipantRegistrationServiceClient, ParticipantRegistrationServiceClientWcf>();
                _dependencyContainer.Register<IAzureStorageServiceClient, AzureStorageServiceClientWcf>();
                _dependencyContainer.Register<IRaceResultsPublishingServiceClient, RaceResultsPublishingServiceClientWcf>();
            }

            _dependencyContainer.Register<ILeaderboardResultsSvcAgent, LeaderboardResultsSvcAgent>();
            _dependencyContainer.Register<ITimeKeepingSvcAgent, TimeKeepingSvcAgent>();
            _dependencyContainer.Register<IRegistrationSvcAgent, ParticipantRegistrationSvcAgent>();
            _dependencyContainer.Register<IAzureStorageSvcAgent, AzureStorageSvcAgent>();
            _dependencyContainer.Register<IRaceResultsPublishingSvcAgent, RaceResultsPublishingSvcAgent>();

            #endregion

            #region local dynamic services - obtained live by property getter

            /* we choose to not have a fixed app-wide IAlertMessageService,
            * but rather to let each page or user control provide the service. in this approach each page registers 
            * itself as the provider in its OnLoaded handler, and de-registers itself in its OnUnloaded handler. the Dummy here is a merely a 
            * transient placeholder until a proper service is registered by means of RegisterIAlertMessageServiceProvider() below
            */

            _dependencyContainer.Register<IAlertMessageService, AlertMessageServiceDummy>();

            // neither of these are used in the portal of course, but dummies are essential to be here. do not omit them - TODO TO BE DELETED
            _dependencyContainer.Register<ILeaderboardDataGridPresentationService, LeaderboardDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<IFavoritesDataGridPresentationService, FavoritesDataGridPresentationServiceDummy>();

            /* we choose these services to be handled by pages that might display
             * one or more data grids or might do something else exotic.The typical hierarchy
             * is that IParticipantMasterListDataGridPresentationService is implemented
             * by ParticipantListPresentationServiceUserControl which is contained by
             * a page such as ParticipantMasterListPage. Whenever the page loads it registers itself
             * as the transient provider of IParticipantMasterListDataGridPresentationService
             * in its OnLoading method by calling the method RegisterIParticipantEntriesAsMasterListPresentationServiceProvider
             * below. dummy loaded for now. The exact same applies to IParticipantPossibleAnomaliesDataGridPresentationService
             * and all the others.
             */

            _dependencyContainer.Register<IParticipantEntriesInLocalStorageDataGridPresentationService, ParticipantEntriesInLocalStorageDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<IParticipantEntriesInMemoryCacheDataGridPresentationService, ParticipantEntriesInMemoryCacheDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<IParticipantMasterListDataGridPresentationService, ParticipantMasterListDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<IParticipantPossibleAnomaliesDataGridPresentationService, ParticipantPossibleAnomaliesDataGridPresentationServiceDummy>();

            _dependencyContainer.Register<ITimeStampEntriesInLocalStorageDataGridPresentationService, TimeStampEntriesInLocalStorageDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<ITimeStampEntriesInMemoryCacheDataGridPresentationService, TimeStampEntriesInMemoryCacheDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<ITimeStampEntriesInClustersDataGridPresentationService, TimeStampEntriesInClustersDataGridPresentationServiceDummy>();
            _dependencyContainer.Register<ITimeStampEntriesMasterListDataGridPresentationService, TimeStampEntriesMasterListDataGridPresentationServiceDummy>();

            _dependencyContainer.Register<ISplitIntervalsPerPersonDataGridPresentationService, SplitIntervalsPerPersonDataGridPresentationServiceDummy>();

            #endregion

            #region global singletons

            _dependencyContainer.Register<ISessionState, SessionState>();
            _dependencyContainer.Register<IProgressIndicatorViewModel, ProgressIndicatorViewModelXamarin>();

            _dependencyContainer.Register<IRepositoryOfHubStyleEntriesWithStorageBackup<TimeStampHubItem>, RepositoryOfHubStyleEntriesWithStorageBackup<TimeStampHubItem>>();
            _dependencyContainer.Register<IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem>, RepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem>>();

            #endregion

            #region viewmodels for the public static vm props for datacontexts for views

            _dependencyContainer.Register<ShellViewModel>();

            _dependencyContainer.Register<HomePageViewModel>();
            _dependencyContainer.Register<AboutPageViewModel>();

            _dependencyContainer.Register<KeepTimeViewModel>();
            _dependencyContainer.Register<RegisterParticipantsViewModel>();
            _dependencyContainer.Register<PublishSingleEventResultsViewModel>();

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
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[NavigationServiceEx]");

                    const string locus = "Property getter of [NavigationService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
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
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[ShellViewModel]");

                    const string locus = "Property getter of [ShellViewModel]";
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
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[RezultzPortalAboutPageViewModel]");

                    const string locus = "Property getter of [RezultzPortalAboutPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public KeepTimeViewModel KeepTimeVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<KeepTimeViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[KeepTimeViewModel]");

                    const string locus = "Property getter of [PortalTimeStampPagesVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public RegisterParticipantsViewModel RegisterParticipantsVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<RegisterParticipantsViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[PortalParticipantAdminPagesViewModel]");

                    const string locus = "Property getter of [PortalParticipantAdminPagesVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        public PublishSingleEventResultsViewModel PublishSingleEventResultsVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<PublishSingleEventResultsViewModel>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[PublishSingleEventResultsViewModel]");

                    const string locus = "Property getter of [PublishSingleEventResultsVm]";
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

        public void DeRegisterIParticipantEntriesInStoragePresentationServiceProvider()
        {
            _dependencyContainer.Register<IParticipantEntriesInLocalStorageDataGridPresentationService, ParticipantEntriesInLocalStorageDataGridPresentationServiceDummy>();
        }
        public void RegisterIParticipantEntriesInStoragePresentationServiceProvider(IParticipantEntriesInLocalStorageDataGridPresentationService implementingObject)
        {
            if (implementingObject is null)
                _dependencyContainer.Register<IParticipantEntriesInLocalStorageDataGridPresentationService, ParticipantEntriesInLocalStorageDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => implementingObject);
        }

        public void DeRegisterIParticipantEntriesInMemoryCachePresentationServiceProvider()
        {
            _dependencyContainer.Register<IParticipantEntriesInMemoryCacheDataGridPresentationService, ParticipantEntriesInMemoryCacheDataGridPresentationServiceDummy>();
        }
        public void RegisterIParticipantEntriesInMemoryCachePresentationServiceProvider(IParticipantEntriesInMemoryCacheDataGridPresentationService implementingObject)
        {
            if (implementingObject is null)
                _dependencyContainer.Register<IParticipantEntriesInMemoryCacheDataGridPresentationService, ParticipantEntriesInMemoryCacheDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => implementingObject);
        }

        public void DeRegisterITimeStampEntriesInStoragePresentationServiceProvider()
        {
            _dependencyContainer.Register<ITimeStampEntriesInLocalStorageDataGridPresentationService, TimeStampEntriesInLocalStorageDataGridPresentationServiceDummy>();
        }
        public void RegisterITimeStampEntriesInStoragePresentationServiceProvider(ITimeStampEntriesInLocalStorageDataGridPresentationService implementingObject)
        {
            if (implementingObject is null)
                _dependencyContainer.Register<ITimeStampEntriesInLocalStorageDataGridPresentationService, TimeStampEntriesInLocalStorageDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => implementingObject);
        }

        public void DeRegisterITimeStampEntriesInMemoryCachePresentationServiceProvider()
        {
            _dependencyContainer.Register<ITimeStampEntriesInMemoryCacheDataGridPresentationService, TimeStampEntriesInMemoryCacheDataGridPresentationServiceDummy>();
        }
        public void RegisterITimeStampEntriesInMemoryCachePresentationServiceProvider(ITimeStampEntriesInMemoryCacheDataGridPresentationService implementingObject)
        {
            if (implementingObject is null)
                _dependencyContainer
                    .Register<ITimeStampEntriesInMemoryCacheDataGridPresentationService, TimeStampEntriesInMemoryCacheDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => implementingObject);
        }

        public void DeRegisterIConsolidatedSplitIntervalsPresentationServiceProvider()
        {
            _dependencyContainer.Register<ISplitIntervalsPerPersonDataGridPresentationService, SplitIntervalsPerPersonDataGridPresentationServiceDummy>();
        }
        public void RegisterIConsolidatedSplitIntervalsPresentationServiceProvider(ISplitIntervalsPerPersonDataGridPresentationService implementingObject)
        {
            if (implementingObject is null)
                _dependencyContainer
                    .Register<ISplitIntervalsPerPersonDataGridPresentationService, SplitIntervalsPerPersonDataGridPresentationServiceDummy>();
            else
                _dependencyContainer.Register(() => implementingObject);
        }

        #endregion

        #region navigation - pages

        private static NavigationServiceEx ConfigureNavigationServiceExSingleton()
        {
            var svc = new NavigationServiceEx();

            svc.Configure(typeof(Shell).FullName, typeof(Shell));

            svc.Configure(typeof(HomePage).FullName, typeof(HomePage));
            svc.Configure(typeof(AboutPage).FullName, typeof(AboutPage));
            svc.Configure(typeof(KeepTimeLaunchPage).FullName, typeof(KeepTimeLaunchPage));
            svc.Configure(typeof(KeepTimeWorkingPage).FullName, typeof(KeepTimeWorkingPage));
            svc.Configure(typeof(KeepTimeToolsPage).FullName, typeof(KeepTimeToolsPage));
            svc.Configure(typeof(KeepTimeFindAnomaliesPage).FullName, typeof(KeepTimeFindAnomaliesPage));
            svc.Configure(typeof(RegisterParticipantsLaunchPage).FullName, typeof(RegisterParticipantsLaunchPage));
            svc.Configure(typeof(RegisterParticipantsWorkingPage).FullName, typeof(RegisterParticipantsWorkingPage));
            svc.Configure(typeof(RegisterParticipantsToolsPage).FullName, typeof(RegisterParticipantsToolsPage));
            svc.Configure(typeof(PublishSingleEventResultsLaunchPage).FullName, typeof(PublishSingleEventResultsLaunchPage));


            return svc;
        }

        #endregion

    }
}
