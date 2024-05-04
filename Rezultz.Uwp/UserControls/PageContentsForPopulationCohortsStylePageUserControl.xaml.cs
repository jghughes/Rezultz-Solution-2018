using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.Strings;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Uwp.Strings;
using Rezultz.Uwp.UserControls_DataGrid;


namespace Rezultz.Uwp.UserControls
{
    public sealed partial class PageContentsForPopulationCohortsStylePageUserControl
    {
        private const string Locus2 = nameof(PageContentsForPopulationCohortsStylePageUserControl);
        private const string Locus3 = "[Rezultz.Uwp]";

        private BasePopulationCohortsStylePageViewModel ViewModel => DataContext as BasePopulationCohortsStylePageViewModel;

        public PopulationCohortsDataGridPresentationServiceUserControl MyPopulationCohortsDataGridUserControl => XamlElementPopulationCohortsDataGridPresentationServiceUserControl;

        public PageContentsForPopulationCohortsStylePageUserControl()
        {
            this.InitializeComponent();
        }


        private void BtnToggleVisibilityOfSplitViewPane_OnClick(object sender, RoutedEventArgs e)
        {
            if (XamlElementSplitView.IsPaneOpen)
            {
                XamlElementSplitView.IsPaneOpen = false;
            }
            else
            {
                XamlElementSplitView.IsPaneOpen = true;
            }
        }


        private async void BtnSaveCohortAnalysisAsHtmlWebpage_OnClick(object sender, RoutedEventArgs e)
        {
            const string failure = "Unable to save file.";
            const string locus = "[BtnSaveCohortAnalysisAsHtmlWebpage_OnClick]";

            try
            {
                if (ViewModel == null)
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

                if (file == null)
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

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
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

                if (file == null) throw new ArgumentNullException(nameof(file));
                if (bytesToBeSaved == null) throw new ArgumentNullException(nameof(bytesToBeSaved));

                #endregion

                #region save file

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.WorkingSaving);

                await FileIO.WriteBytesAsync(file, bytesToBeSaved);

                var status = await CachedFileManager.CompleteUpdatesAsync(file);
                // Let Windows know that we're finished changing the file.

                if (status == FileUpdateStatus.Complete)
                    await AlertMessageService.ShowOkAsync(
                        $"Saved as {file.Name}. {JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length)} {countOfLineItems} line-items.");
                else
                    await AlertMessageService.ShowOkAsync("Document couldn't be saved.");

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

        private static IAlertMessageService AlertMessageService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IAlertMessageService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance,
                            "<IAlertMessageService>");

                    const string locus = "Property getter of <AlertMessageServiceInstance]";
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


    }
}
