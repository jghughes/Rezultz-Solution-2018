using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class RegisterParticipantsWorkingPage
    {
        private const string Locus2 = nameof(RegisterParticipantsWorkingPage);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        public RegisterParticipantsWorkingPage()
        {
            Loaded += OnPageHasCompletedLoading;

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            XamlElementGridOfAllPageContents.Visibility = Visibility.Collapsed;

            XamlElementGridOfAllPageContents.IsPaneOpen = false;

            PanelForEnteringNewParticipantProfileId.Visibility = Visibility.Collapsed;

            XamlElementPanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;

            PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;

            RadioBtnCloseAllPanels.IsChecked = true;

            XamlElementButtonForEnteringParticipantProfileId.Content = $"Click to add a participant ID";
        }

        private RegisterParticipantsViewModel PagesViewModel => DataContext as RegisterParticipantsViewModel;

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
                DependencyLocator.RegisterIParticipantEntriesInMemoryCachePresentationServiceProvider(XamlElementRawDataDataGridUserControl);

                PagesViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModeIsNull();

                PagesViewModel.ThrowIfWorkSessionNotProperlyInitialised();

                PagesViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                await PagesViewModel.RefreshAllDataGridsAndListViewsAsync();

                XamlElementGridOfAllPageContents.Visibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                XamlElementGridOfAllPageContents.Visibility = Visibility.Collapsed;

                await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, exception);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DependencyLocator.DeRegisterIAlertMessageServiceProvider();
            DependencyLocator.DeRegisterIParticipantEntriesInMemoryCachePresentationServiceProvider();

            base.OnNavigatingFrom(e);
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(XamlElementTextBoxForEnteringGunStartIndividualId.Text))
            {
                XamlElementButtonForEnteringParticipantProfileId.Content = $"Click to add a participant ID";

                return;
            }

            XamlElementButtonForEnteringParticipantProfileId.Content = $"Click to add {XamlElementTextBoxForEnteringGunStartIndividualId.Text}";
        }

        private void RadioBtnSelectDesiredControlPanelToBeDisplayed_OnChecked(object sender, RoutedEventArgs e)
        {
            if (RadioBtnShowPanelForAddingNewParticipantId.IsChecked is not null && (bool)RadioBtnShowPanelForAddingNewParticipantId.IsChecked)
            {
                XamlElementPanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringNewParticipantProfileId.Visibility = Visibility.Visible;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
            }
            else if (RadioBtnShowPanelForCheckBoxesForFilteringRowsAndColumnsOfDataGrid.IsChecked is not null && (bool)RadioBtnShowPanelForCheckBoxesForFilteringRowsAndColumnsOfDataGrid.IsChecked)
            {
                XamlElementPanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Visible;
                PanelForEnteringNewParticipantProfileId.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
            }
            else if (RadioBtnShowPanelForSearchBoxUserControl.IsChecked is not null && (bool)RadioBtnShowPanelForSearchBoxUserControl.IsChecked)
            {
                XamlElementPanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringNewParticipantProfileId.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Visible;
            }

            else if (RadioBtnCloseAllPanels.IsChecked is not null && (bool)RadioBtnCloseAllPanels.IsChecked)
            {
                XamlElementPanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringNewParticipantProfileId.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnToggleVisibilityOfSplitViewPane_OnClick(object sender, RoutedEventArgs e)
        {
            XamlElementGridOfAllPageContents.IsPaneOpen = !XamlElementGridOfAllPageContents.IsPaneOpen;
        }
    }
}
