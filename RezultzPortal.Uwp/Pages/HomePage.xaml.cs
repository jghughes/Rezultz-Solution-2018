using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;

using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;
using RezultzPortal.Uwp.Strings;

namespace RezultzPortal.Uwp.Pages
{
    // This is the landing page as defined in App.CreateActivationService(), called in App ctor

    public sealed partial  class HomePage
    {
        private const string Locus2 = nameof(HomePage);
        private const string Locus3 = "[RezultzPortal.Uwp]";


        public HomePage()
        {
            Loaded += OnPageHasCompletedLoading;

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private HomePageViewModel ViewModel => DataContext as HomePageViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources[StringsForXamlPages.DependencyInjectionLocator] as DependencyInjectionLocator;

        public static IThingsPersistedInLocalStorage ThingsPersistedInLocalStorage
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IThingsPersistedInLocalStorage>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance,
                            $"'{nameof(IThingsPersistedInLocalStorage)}'");

                    const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(ThingsPersistedInLocalStorage)}]";

                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        private static IProgressIndicatorViewModel GlobalProgressIndicatorVm
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

        protected override async void OnPageHasCompletedLoading(object sender, RoutedEventArgs e)
        {
            const string failure = StringsForXamlPages.ExceptionCaughtAtPageLevel;
            const string locus = $"[{nameof(OnPageHasCompletedLoading)}]";

            try
            {
                if (ViewModel == null)
                    throw new ArgumentNullException(StringsForXamlPages.DataContextIsNull);

                DependencyLocator.RegisterIAlertMessageServiceProvider(this);

                if (!AppSettings.IsFirstTimeThroughOnLandingPage)
                    return;

                if (AppSettings.IsInDebugMode)
                    await InitialiseDebugSettingsAsync();

                InitialiseAppSettings();

                AppSettings.IsFirstTimeThroughOnLandingPage = false;

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.WakingServer);

                var messageOk = await ViewModel.BeInitialisedFromPageCodeBehindOrchestrateAsync();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await ShowOkAsync(messageOk);
            }
            catch (Exception exception)
            {
                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, exception);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DependencyLocator.DeRegisterIAlertMessageServiceProvider();

            base.OnNavigatingFrom(e);
        }

        private async Task InitialiseDebugSettingsAsync()
        {
            //await ThingsPersistedInLocalStorage.ClearAllSettingsAsync(); //  DEBUG ONLY : if you want to debug what happens when a user downloads the app and signs on for the very first time. Comment this out for production

            await ThingsPersistedInLocalStorage.SaveNavigationContextQueryStringAsync(new Dictionary<string, string>()); // deprecated. hails back to Silverlight and Win 8.1
        }

        private void InitialiseAppSettings()
        {
            DependencyLocator.KeepTimeVm.RepositoryOfHubStyleEntries.DesiredHeightOfShortList = AppSettings.DesiredHeightOfShortListOfHubItemsDefault;

            DependencyLocator.RegisterParticipantsVm.RepositoryOfHubStyleEntries.DesiredHeightOfShortList = AppSettings.DesiredHeightOfShortListOfHubItemsDefault;
        }
    }
}
