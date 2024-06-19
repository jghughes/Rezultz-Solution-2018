using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class KeepTimeFindAnomaliesPage
    {
        private const string Locus2 = nameof(KeepTimeFindAnomaliesPage);
        private const string Locus3 = "[RezultzPortal.Uwp]";


        public KeepTimeFindAnomaliesPage()
        {
            Loaded += OnPageHasCompletedLoading;

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            XamlElementGridOfAllPageContents.Visibility = Visibility.Collapsed;

            XamlElementGridOfAllPageContents.IsPaneOpen = false;

            PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;

            PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;

            PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;

            RadioBtnCloseAllPanels.IsChecked = true;

        }


        private KeepTimeViewModel PagesViewModel => DataContext as KeepTimeViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources[StringsForXamlPages.DependencyInjectionLocator] as DependencyInjectionLocator;


        protected override async void OnPageHasCompletedLoading(object sender, RoutedEventArgs e)
        {
            const string failure = StringsForXamlPages.ExceptionCaughtAtPageLevel;
            const string locus = $"[{nameof(OnPageHasCompletedLoading)}]";

            try
            {
                if (PagesViewModel is null)
                    throw new ArgumentNullException(StringsForXamlPages.DataContextIsNull);

                DependencyLocator.RegisterIAlertMessageServiceProvider(this);
                DependencyLocator.RegisterIConsolidatedSplitIntervalsPresentationServiceProvider(XamlElementSplitItemsDataGridUserControl);

                PagesViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModeIsNull();

                PagesViewModel.ThrowIfWorkSessionNotProperlyInitialised();

                PagesViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                await PagesViewModel.RefreshAllDataGridsAndListViewsAsync();

                XamlElementGridOfAllPageContents.Visibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                //GlobalProgressIndicatorVm.FreezeProgressIndicator();

                XamlElementGridOfAllPageContents.Visibility = Visibility.Collapsed;

                await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, exception);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DependencyLocator.DeRegisterIAlertMessageServiceProvider();
            DependencyLocator.DeRegisterIConsolidatedSplitIntervalsPresentationServiceProvider();

            base.OnNavigatingFrom(e);
        }

        private void RadioBtnSelectDesiredControlPanelToBeDisplayed_OnChecked(object sender, RoutedEventArgs e)
        {
            if (RadioBtnShowPanelForCheckBoxesForFilteringRowsAndColumnsOfDataGrid.IsChecked is not null && (bool)RadioBtnShowPanelForCheckBoxesForFilteringRowsAndColumnsOfDataGrid.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Visible;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;
            }
            else if (RadioBtnShowPanelForSearchBoxUserControl.IsChecked is not null && (bool)RadioBtnShowPanelForSearchBoxUserControl.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Visible;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;
            }

            else if (RadioBtnCloseAllPanels.IsChecked is not null && (bool)RadioBtnCloseAllPanels.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;

            }
            else if (RadioBtnShowPanelForAddingParticipantInfo.IsChecked is not null && (bool)RadioBtnShowPanelForAddingParticipantInfo.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Visible;
            }

        }
    }
}
