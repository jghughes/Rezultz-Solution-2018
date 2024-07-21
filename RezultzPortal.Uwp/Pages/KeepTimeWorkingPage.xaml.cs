using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class KeepTimeWorkingPage
    {
        private const string Locus2 = nameof(KeepTimeWorkingPage);
        private const string Locus3 = "[RezultzPortal.Uwp]";


        public KeepTimeWorkingPage()
        {
            Loaded += OnPageHasCompletedLoading;

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            XamlElementGridOfAllPageContents.Visibility = Visibility.Collapsed;

            XamlElementGridOfAllPageContents.IsPaneOpen = false;

            PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Collapsed;

            PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Collapsed;

            PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;

            PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;

            PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;

            RadioBtnCloseAllPanels.IsChecked = true;

            XamlElementButtonForCreatingGunStartTimestamp.Content = $"Click to fire starter's gun now!";

            ButtonForEnteringIndividualIdForTimeStampOfTimingMat01.Content = $"Click to trigger mat now!";

            ButtonForEnteringIndividualIdForTimeStampOfTimingMat02.Content = $"Click to trigger mat now!";

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
                DependencyLocator.RegisterITimeStampEntriesInMemoryCachePresentationServiceProvider(XamlElementRawDataDataGridUserControl);


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
            DependencyLocator.DeRegisterITimeStampEntriesInMemoryCachePresentationServiceProvider();

            base.OnNavigatingFrom(e);
        }   

        private void RadioBtnDesiredControlPanelToBeDisplayed_OnChecked(object sender, RoutedEventArgs e)
        {
            if (RadioBtnShowPanelForGunStarts.IsChecked is not null && (bool)RadioBtnShowPanelForGunStarts.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Visible;
                PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;


            }
            else if (RadioBtnShowPanelForTimingMatSignals.IsChecked is not null && (bool)RadioBtnShowPanelForTimingMatSignals.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Collapsed;
                PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Visible;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;


            }
            else if (RadioBtnShowPanelForCheckBoxesForFilteringRowsAndColumnsOfDataGrid.IsChecked is not null && (bool)RadioBtnShowPanelForCheckBoxesForFilteringRowsAndColumnsOfDataGrid.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Visible;
                PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Collapsed;
                PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;


            }
            else if (RadioBtnShowPanelForSearchBoxUserControl.IsChecked is not null && (bool)RadioBtnShowPanelForSearchBoxUserControl.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Collapsed;
                PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Visible;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;

            }

            else if (RadioBtnCloseAllPanels.IsChecked is not null && (bool)RadioBtnCloseAllPanels.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Collapsed;
                PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Collapsed;

            }

            else if (RadioBtnShowPanelForDisplayingParticipantInfo.IsChecked is not null && (bool)RadioBtnShowPanelForDisplayingParticipantInfo.IsChecked)
            {
                PanelOfCheckBoxesForFilteringRowsAndColumnsOfDataGrid.Visibility = Visibility.Collapsed;
                PanelForEnteringGunStartTimeStamps.Visibility = Visibility.Collapsed;
                PanelForEnteringTimingMatTimeStamps.Visibility = Visibility.Collapsed;
                PanelContainingSearchUserControl.Visibility = Visibility.Collapsed;
                PanelContainingButtonToPullParticipants.Visibility = Visibility.Visible;
            }

        }

        private void TextBoxForEnteringGunStartIndividualId_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(XamlElementTextBoxForEnteringGunStartIndividualId.Text))
            {
                XamlElementButtonForCreatingGunStartTimestamp.Content = $"Click to start person now!";

                return;
            }

            XamlElementButtonForCreatingGunStartTimestamp.Content = $"Click to start {XamlElementTextBoxForEnteringGunStartIndividualId.Text} now!";
        }

        private void TextBoxForEnteringIndividualIdForTimeStampOfTimingMat01_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxForEnteringIndividualIdForTimeStampOfTimingMat01.Text))
            {
                ButtonForEnteringIndividualIdForTimeStampOfTimingMat01.Content = $"Click to trigger mat now!";

                return;
            }

            ButtonForEnteringIndividualIdForTimeStampOfTimingMat01.Content = $"Click {TextBoxForEnteringIndividualIdForTimeStampOfTimingMat01.Text} now!";

        }

        private void TextBoxForEnteringIndividualIdForTimeStampOfTimingMat02_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxForEnteringIndividualIdForTimeStampOfTimingMat02.Text))
            {
                ButtonForEnteringIndividualIdForTimeStampOfTimingMat02.Content = $"Click to trigger mat now!";

                return;
            }

            ButtonForEnteringIndividualIdForTimeStampOfTimingMat02.Content = $"Click {TextBoxForEnteringIndividualIdForTimeStampOfTimingMat02.Text} now!";
        }

        private void BtnToggleVisibilityOfSplitViewPane_OnClick(object sender, RoutedEventArgs e)
        {
            XamlElementGridOfAllPageContents.IsPaneOpen = !XamlElementGridOfAllPageContents.IsPaneOpen;
        }
    }
}
