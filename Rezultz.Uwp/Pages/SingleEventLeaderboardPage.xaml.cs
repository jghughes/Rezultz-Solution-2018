using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;
using Rezultz.Library02.Mar2024.PageViewModels;
using Rezultz.Library02.Mar2024.ValidationViewModels;
using Rezultz.Uwp.DependencyInjection;
using Rezultz.Uwp.Strings;

namespace Rezultz.Uwp.Pages
{
    public sealed partial class SingleEventLeaderboardPage
    {
        private const string Locus2 = nameof(SingleEventLeaderboardPage);
        private const string Locus3 = "[Rezultz.Uwp]";

        private SingleEventLeaderboardPageViewModel ViewModel => DataContext as SingleEventLeaderboardPageViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources[StringsForXamlPages.DependencyInjectionLocator] as DependencyInjectionLocator;


        public SingleEventLeaderboardPage()
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
                DependencyLocator.RegisterIFavoritesListPresentationServiceProvider(XamlElementLeaderboardFormatPageUserControl.MyFavoritesDataGridUserControl);
                DependencyLocator.RegisterILeaderboardListPresentationServiceProvider(XamlElementLeaderboardFormatPageUserControl.MyLeaderboardDataGridUserControl);

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.Working + "fetching results");

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
            DependencyLocator.DeRegisterIFavoritesListPresentationServiceProvider();
            DependencyLocator.DeRegisterILeaderboardListPresentationServiceProvider();

            base.OnNavigatingFrom(e);
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

    }
}
