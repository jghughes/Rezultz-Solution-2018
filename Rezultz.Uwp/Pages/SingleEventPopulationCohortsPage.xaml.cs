using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.Strings;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;
using Rezultz.Library02.Mar2024.PageViewModels;
using Rezultz.Uwp.DependencyInjection;
using Rezultz.Uwp.Strings;


// ReSharper disable UnusedParameter.Local

namespace Rezultz.Uwp.Pages
{
    public sealed partial class SingleEventPopulationCohortsPage
    {
        private const string Locus2 = nameof(SingleEventPopulationCohortsPage);
        private const string Locus3 = "[Rezultz.Uwp]";


        private SingleEventPopulationCohortsPageViewModel ViewModel => DataContext as SingleEventPopulationCohortsPageViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources[StringsForXamlPages.DependencyInjectionLocator] as DependencyInjectionLocator;


        public SingleEventPopulationCohortsPage()
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
                DependencyLocator.RegisterIPopulationCohortsPresentationServiceProvider(XamlElementPopulationCohortsFormatPageUserControl.MyPopulationCohortsDataGridUserControl);

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.Working + "fetching cohort info");

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
            DependencyLocator.DeRegisterIPopulationCohortsPresentationServiceProvider();

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


        // ReSharper disable once UnusedMember.Local
        private async void BtnSaveCohortAnalysisAsHtmlWebpage_OnClick(object sender, RoutedEventArgs e)
        {
            const string failure = "Unable to save file.";
            const string locus = "[BtnSaveCohortAnalysisAsHtmlWebpage_OnClick]";

            try
            {
                if (ViewModel is null)
                    throw new ArgumentNullException(nameof(ViewModel));

                if (ViewModel.SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm.IsAuthorisedToOperate == false)
                    return;

                ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

                #region  do work

                #region prepare save file picker

                var hopefullyValidFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix("cohorts");

                var fileSavePicker = new FileSavePicker
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    DefaultFileExtension = @".htm",
                    SuggestedFileName = hopefullyValidFileName
                };

                fileSavePicker.FileTypeChoices.Add("html file", new List<string> { ".htm" });
                // Dropdown of file types the user can save the file as

                var file = await PickSaveFile(fileSavePicker);

                if (file is null)
                {
                    ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                    return;
                }

                #endregion

                #region prepare file contents

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.WorkingFormatting);

                if (ViewModel.PopulationCohortsDataGridDesigner?.DesignerIsInitialisedForPopulationCohortItemDisplayObjects != true)
                {
                    ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                    return;
                }

                var htmWebPageAsString = await ViewModel.PopulationCohortsDataGridDesigner
                    .GetCohortAnalysisAsPrettyPrintedWebPageAsync();

                var answerAsBytes = JghConvert.ToBytesUtf8FromString(htmWebPageAsString);

                #endregion

                #region save file

                await SaveFileToHardDriveAsync(file, answerAsBytes,
                    ViewModel.PopulationCohortsDataGridDesigner.GetCohortLineItemsCount());

                #endregion

                #endregion

                ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                // move Gui forward upon success. nothing to do here. the saving of files is
                // orthogonal to the processing pipeline. all we do is restore prior state regardless.
            }

            #region trycatch

            catch (Exception ex)
            {
                ViewModel?.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                // move Gui forward upon success.
                // nothing to do here. the saving of files is orthogonal to the processing pipeline. all we do is restore prior state regardless.

                await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        private async Task<StorageFile> PickSaveFile(FileSavePicker fileSavePicker)
        {
            StorageFile file;
            try
            {
                file = await fileSavePicker.PickSaveFileAsync();

                if (string.IsNullOrWhiteSpace(file?.Name))
                    return null;
            }
            catch (SecurityException)
            {
                throw new JghAlertMessageException(
                    StringsUwpCommon.SaveFileDialogSecurityExceptionMessage);
            }
            catch (InvalidOperationException)
            {
                throw new JghAlertMessageException(
                    StringsUwpCommon.SaveFileDialogInvalidOperationExceptionMessage);
            }

            return file;
        }

        private async Task SaveFileToHardDriveAsync(StorageFile file, byte[] bytesToBeSaved, int countOfLineItems)
        {
            const string failure = "Unable to save file.";
            const string locus = "[SaveFileToHardDriveAsync]";

            try
            {
                #region null value error handling

                if (file is null) throw new ArgumentNullException(nameof(file));
                if (bytesToBeSaved is null) throw new ArgumentNullException(nameof(bytesToBeSaved));

                #endregion

                #region save file

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.WorkingSaving);


                await FileIO.WriteBytesAsync(file, bytesToBeSaved);
                // write to file

                var status = await CachedFileManager.CompleteUpdatesAsync(file);
                // Let Windows know that we're finished changing the file.
                // Completing updates may require Windows to ask for user input.

                if (status == FileUpdateStatus.Complete)
                    await ShowOkAsync(
                        $"Saved as {file.Name}. {JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length)} {countOfLineItems} line-items.");
                else
                    await ShowOkAsync("Document couldn't be saved.");

                #endregion
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }



    }
}
