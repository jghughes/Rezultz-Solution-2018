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
using Rezultz.Uwp.DependencyInjection;
using Rezultz.Uwp.PageViewModels;
using Rezultz.Uwp.Strings;
using Rezultz.Uwp.UserSettings;

namespace Rezultz.Uwp.Pages
{
    // This is the landing page as defined in App.CreateActivationService(), called in App ctor

    public sealed partial class HomePage
    {
        private const string Locus2 = nameof(HomePage);
        private const string Locus3 = "[Rezultz.Uwp]";

        private HomePageViewModel ViewModel => DataContext as HomePageViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources[StringsForXamlPages.DependencyInjectionLocator] as DependencyInjectionLocator;


        public HomePage()
        {
            Loaded += OnPageHasCompletedLoading;

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }


        protected override async void OnPageHasCompletedLoading(object sender, RoutedEventArgs e)
        {
            const string failure = StringsForXamlPages.ExceptionCaughtAtPageLevel;
            const string locus = $"[{nameof(OnPageHasCompletedLoading)}]";

            try
            {
                if (ViewModel is null)
                    throw new ArgumentNullException(StringsForXamlPages.DataContextIsNull);

                DependencyLocator.RegisterIAlertMessageServiceProvider(this);

                if (!AppSettings.IsFirstTimeThroughOnLandingPage)
                    return;

                if (AppSettings.IsInDebugMode)
                    await InitialiseDebugSettingsAsync();

                await InitialiseAppSettingsAsync();

                AppSettings.IsFirstTimeThroughOnLandingPage = false;

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.WakingServer);

                var messageOk = await ViewModel.BeInitialisedFromPageCodeBehindOrchestrateAsync();

                //GlobalSeasonProfileJsonFilenameValidationVm.SeasonMetadataProgressIndicatorVm.FreezeProgressIndicator();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await ShowOkAsync(messageOk);
            }
            catch (Exception exception)
            {
                //GlobalSeasonProfileJsonFilenameValidationVm.SeasonMetadataProgressIndicatorVm.FreezeProgressIndicator();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, exception);
            }
            finally
            {
                //GlobalSeasonProfileJsonFilenameValidationVm.SeasonMetadataProgressIndicatorVm.CloseProgressIndicator();

                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DependencyLocator.DeRegisterIAlertMessageServiceProvider();

            base.OnNavigatingFrom(e);
        }

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

        private async Task InitialiseDebugSettingsAsync()
        {
            //await ThingsPersistedInLocalStorage.ClearAllSettingsAsync(); //  DEBUG ONLY : if you want to debug what happens when a user downloads the app and signs on for the very first time. Comment this out for production

            await ThingsPersistedInLocalStorage.SaveMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync(AppSettings.MustSelectCategoryOfResultsForSingleParticipantIdOnLaunch);

            await ThingsPersistedInLocalStorage.SaveTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync(AppSettings.TargetParticipantIdForSelectedCategoryOfResultsOnLaunch);

            await ThingsPersistedInLocalStorage.SaveNavigationContextQueryStringAsync(new Dictionary<string, string>()); // deprecated. hails back to Silverlight and Win 8.1
        }

        private async Task InitialiseAppSettingsAsync()
        {
            await ThingsPersistedInLocalStorage.SaveMustSelectAllRacesOnFirstTimeThroughForAnEventAsync(AppSettings.MustSelectAllRacesOnFirstTimeThroughForAnEvent); // NB. this is a significant AppSetting

            await ThingsPersistedInLocalStorage.SaveMustUsePreviewDataOnLaunchAsync(AppSettings.MustUseDraftDataOnLaunch);
        }

    }
}
