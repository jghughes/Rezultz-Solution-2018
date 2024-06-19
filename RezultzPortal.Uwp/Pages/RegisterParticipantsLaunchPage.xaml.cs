using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;

using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;
using RezultzPortal.Uwp.Strings;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class RegisterParticipantsLaunchPage
    {
        private const string Locus2 = nameof(RegisterParticipantsLaunchPage);
        private const string Locus3 = "[RezultzPortal.Uwp]";


        public RegisterParticipantsLaunchPage()
        {
            Loaded += OnPageHasCompletedLoading;

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private RegisterParticipantsViewModel PagesViewModel => DataContext as RegisterParticipantsViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources[StringsForXamlPages.DependencyInjectionLocator] as DependencyInjectionLocator;

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
                if (PagesViewModel is null)
                    throw new ArgumentNullException(StringsForXamlPages.DataContextIsNull);

                DependencyLocator.RegisterIAlertMessageServiceProvider(this);

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.GettingReady);

                await PagesViewModel.BeInitialisedFromPageCodeBehindOrchestrateAsync();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                PagesViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModeIsNull();

                PagesViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();
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
    }
}
