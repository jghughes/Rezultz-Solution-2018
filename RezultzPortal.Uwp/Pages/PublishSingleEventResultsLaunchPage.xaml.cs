using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.Strings;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces03.Apr2022;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.UserControls;
using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;
using RezultzPortal.Uwp.Strings;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class PublishSingleEventResultsLaunchPage
    {
        private const string Locus2 = nameof(PublishSingleEventResultsLaunchPage);
        private const string Locus3 = "[RezultzPortal.Uwp]";


        public PublishSingleEventResultsLaunchPage()
    {
        Loaded += OnPageHasCompletedLoading;

        InitializeComponent();

        NavigationCacheMode = NavigationCacheMode.Enabled;
    }

        private PublishSingleEventResultsViewModel PagesViewModel => DataContext as PublishSingleEventResultsViewModel;

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

        private static IRaceResultsPublishingSvcAgent GlobalRaceResultsPublishingSvcAgent
        {
            get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IRaceResultsPublishingSvcAgent>();
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, $"[{nameof(IRaceResultsPublishingSvcAgent)}]");

                const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(GlobalRaceResultsPublishingSvcAgent)}]";

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

        private async void BtnBrowseHardDriveForFileAndUploadAsSourceDataset_OnClick(object sender, RoutedEventArgs e)
    {
        const string failure = "Unable to import and upload file.";
        const string locus = "[BtnBrowseHardDriveForFileAndUploadAsSourceData_OnClick]";

        #region const

        const int lhsWidth = 30;
        const int lhsWidthPlus1 = lhsWidth + 1;
        //const int lhsWidthPlus2 = lhsWidth + 2;
        const int lhsWidthPlus3 = lhsWidth + 3;
        const int lhsWidthPlus5 = lhsWidth + 5;
        //const int lhsWidthLess3 = lhsWidth - 3;
        const int lhsWidthLess4 = lhsWidth - 4;
        //const int lhsWidthLess6 = lhsWidth - 6;

        #endregion

        try
        {
            #region can execute?

            var xamlButtonThatUserClicked = sender as Button;

            if (xamlButtonThatUserClicked is not {IsEnabled: true})
                throw new JghAlertMessageException("Not enabled. Please wait.");

            var buttonProfile = PagesViewModel?.PublishingModuleProfile?.GuiButtonProfilesForBrowsingFileSystemForDatasets?
                .FirstOrDefault(z => z.IdentifierOfAssociatedDataset == xamlButtonThatUserClicked.Tag as string);

            if (buttonProfile is null)
                throw new Exception("GUI Button Tag not found in list of AssociatedDataset for this processing module.");

            #endregion

            #region deaden Gui

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working______uploading);

            PagesViewModel.DeadenGui();

            #endregion

            #region launch FileOpenPicker - bale if user opts out of importing the file

            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            if (!string.IsNullOrWhiteSpace(buttonProfile.FileNameExtensionFiltersForBrowsingHardDrive))
                foreach (var fileTypeFilter in buttonProfile.FileNameExtensionFiltersForBrowsingHardDrive.Split(','))
                    openPicker.FileTypeFilter.Add(fileTypeFilter);

            var importFile = await openPicker.PickSingleFileAsync();

            if (importFile is null)
                throw new JghAlertMessageException("Cancelled");

            #endregion

            #region clear updateable datafields in buttonvm to cater for the possibility that this is not the first time through which is quite normal

            if (xamlButtonThatUserClicked.DataContext is not PublishingModuleButtonControlViewModel buttonVm)
                throw new Exception("The xamlButtonThatUserClicked DataContext as PublishingModuleButtonControlViewModel on parent vm PublishSingleEventResultsViewModel is null. Please report this problem.");

            buttonVm.DatasetHasBeenUploaded = false;
            buttonVm.DatasetFileNameForUpload = string.Empty;
            buttonVm.DatasetFileUploadOutcomeReport = string.Empty;
            buttonVm.DatasetAsRawString = string.Empty;

            #endregion

            #region import user-selected file unquestioningly

            PagesViewModel.AppendToConversionReportLog(StringsPortal.Working_____importing);

            var dataSetAsString = await FileIO.ReadTextAsync(importFile);

            var fileSizeDescription = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.ToBytesUtf8FromString(dataSetAsString).Length);

            var fileImportReport = $"{JghString.LeftAlign("File path:", lhsWidthPlus3)} <{importFile.Path}>";

            PagesViewModel.AppendToConversionReportLog(fileImportReport);

            PagesViewModel.UpdateLogOfFilesThatWereTransferred(importFile.Name, "was imported into", "this app", fileSizeDescription);

            #endregion

            var startDateTime = DateTime.UtcNow;

            #region upload dataset

            PagesViewModel.AppendToConversionReportLog(StringsPortal.Working______uploading);

            var accountName = PagesViewModel.SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DatabaseAccountName;
            var containerName = PagesViewModel.SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DataContainerName;
            var datasetEntityName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', $"{DateTime.Now.ToString(JghDateTime.SortablePattern)}+{importFile.Name}");

            var uploadDidSucceed = await GlobalRaceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(buttonVm.DatasetIdentifyingEnum,
                new EntityLocationItem(accountName, containerName, datasetEntityName),
                dataSetAsString,
                CancellationToken.None);

            if (!uploadDidSucceed)
            {
                var uploadDatasetFailureReport = $"{JghString.LeftAlign("Dataset upload:", lhsWidthLess4)} Failure. <{datasetEntityName}> failed to upload to <{containerName}>";

                PagesViewModel.AppendToConversionReportLog(uploadDatasetFailureReport);

                throw new JghAlertMessageException(uploadDatasetFailureReport);
            }

            var uploadDatasetSuccessReport = $"{JghString.LeftAlign("Dataset uploaded:", lhsWidthLess4)} <{datasetEntityName}> {fileSizeDescription}";

            PagesViewModel.AppendToConversionReportLog(uploadDatasetSuccessReport);

            PagesViewModel.UpdateLogOfFilesThatWereTransferred($"{datasetEntityName}", "was uploaded in preparation for processing to", $"{containerName}", fileSizeDescription);

            #endregion

            #region update GUI

            await PagesViewModel.CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject {Label = buttonVm.Content.ToString(), Blurb = dataSetAsString, EnumString = "dummy"});

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            var ranToCompletionMsgSb = new JghStringBuilder();
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Operation ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine(fileImportReport);
            ranToCompletionMsgSb.AppendLine(uploadDatasetSuccessReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{containerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("File size:", lhsWidthPlus5)} {fileSizeDescription}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. Dataset uploaded into position for processing.");

            #endregion

            #region update button

            buttonVm.DatasetHasBeenUploaded = true;
            buttonVm.DatasetFileNameForUpload = datasetEntityName;
            buttonVm.DatasetFileUploadOutcomeReport = ranToCompletionMsgSb.ToString();
            buttonVm.DatasetAsRawString = dataSetAsString;

            #endregion

            if (!PagesViewModel.MakeListOfDatasetInputButtonVms().Any(z => z.IsDesignated && !z.DatasetHasBeenUploaded)) PagesViewModel.NextThingToDoEnum = PublishSingleEventResultsViewModel.NextThingToDo.MakeControlsForPreprocessingActive;

            #region enliven Gui

            PagesViewModel.EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            #endregion

            await ShowOkAsync(ranToCompletionMsgSb.ToString());
        }

        #region trycatch

        catch (Exception ex)
        {
            PagesViewModel?.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                // a JghAlertMessageException is deemed a successful outcome
                PagesViewModel?.EvaluateGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        private async void BtnSaveProcessingReportToHardDrive_OnClick(object sender, RoutedEventArgs e)
    {
        const string failure = "Unable to save file.";
        const string locus = "[BtnSaveProcessingReportToHardDrive_OnClick]";

        var savedFileName = string.Empty;

        try
        {
            if (PagesViewModel.ExportProcessingReportToHardDriveButtonVm.IsAuthorisedToOperate == false)
                return;

            #region null checks

            // return if nothing available to save yet

            if (string.IsNullOrWhiteSpace(PagesViewModel.ProcessingReportTextVm.Text))
            {
                await ShowOkAsync("Nothing to save. Saved file would be empty.");
                return;
            }

            #endregion

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____saving);

            PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

            #region do work

            #region prepare save file picker

            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = @".xml",
                SuggestedFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix("ProcessingReport")
            };

            fileSavePicker.FileTypeChoices.Add("Text file", new List<string> {".txt"});

            #endregion

            #region pick a file location

            StorageFile file;
            try
            {
                file = await fileSavePicker.PickSaveFileAsync();

                // bale if user opts out
                if (string.IsNullOrWhiteSpace(file?.Name))
                {
                    PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                    savedFileName = StringsPortal.Incomplete__nothing_saved;
                    PagesViewModel.SavedFileNameOfProcessingReportTextVm.Text = savedFileName;
                    return;
                }
                else
                {
                    savedFileName = file.Name;
                    PagesViewModel.SavedFileNameOfProcessingReportTextVm.Text = savedFileName;
                }
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

            #endregion

            #region prepare file contents

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____processing);

            var contentToBeSaved = PagesViewModel.ProcessingReportTextVm.Text;

            var answerAsBytes = JghConvert.ToBytesUtf8FromString(contentToBeSaved);

            #endregion

            #region save file

            var message = await SaveFileToHardDriveAsync(file, answerAsBytes, 1);

            #endregion

            #endregion

            PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            PagesViewModel.EvaluateGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await ShowOkAsync(message);
        }

        #region trycatch

        catch (Exception ex)
        {
            PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
            {
                PagesViewModel.EvaluateGui();
                PagesViewModel.SavedFileNameOfProcessingReportTextVm.Text = savedFileName;
            }
            else
            {
                PagesViewModel.SavedFileNameOfProcessingReportTextVm.Text = StringsPortal.Incomplete__nothing_saved;
            }

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        private async void BtnSaveSuccessfullyComputedLeaderboardToHardDrive_OnClick(object sender, RoutedEventArgs e)
    {
        const string failure = "Unable to save file.";
        const string locus = "[BtnSaveSuccessfullyComputedLeaderboardToHardDrive_OnClick]";

        var savedFileName = string.Empty;

        try
        {
            if (PagesViewModel.ExportLeaderboardToHardDriveButtonVm.IsAuthorisedToOperate == false)
                return;

            #region null checks

            // return if nothing available to save yet

            if (string.IsNullOrWhiteSpace(PagesViewModel.SuccessfullyComputedLeaderboardAsXml))
            {
                await ShowOkAsync("Nothing to save. Saved file would be empty.");
                return;
            }

            #endregion

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____saving);

            PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

            #region do work

            #region prepare save file picker

            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = @".xml",
                SuggestedFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(PagesViewModel.BlobTargetOfPublishedResultsUponLaunchOfWorkSession?.Label)
            };

            // digression. if content is not valid xml, rather save as a .txt file so that file is openable by user

            fileSavePicker.FileTypeChoices.Add("XML file", new List<string> {".xml"});
            fileSavePicker.FileTypeChoices.Add("Text file", new List<string> {".txt"});

            #endregion

            #region pick a file location

            StorageFile file;
            try
            {
                file = await fileSavePicker.PickSaveFileAsync();

                // bale if user opts out
                if (string.IsNullOrWhiteSpace(file?.Name))
                {
                    PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                    savedFileName = StringsPortal.Incomplete__nothing_saved;
                    PagesViewModel.SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm.Text = savedFileName;
                    return;
                }
                else
                {
                    savedFileName = file.Name;
                    PagesViewModel.SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm.Text = savedFileName;
                }
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

            #endregion

            #region prepare file contents

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____processing);

            var contentToBeSaved = PagesViewModel.SuccessfullyComputedLeaderboardAsXml;

            var answerAsBytes = JghConvert.ToBytesUtf8FromString(contentToBeSaved);

            #endregion

            #region save file

            var message = await SaveFileToHardDriveAsync(file, answerAsBytes, 1);

            #endregion

            #endregion

            PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            PagesViewModel.EvaluateGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await ShowOkAsync(message);
        }

        #region trycatch

        catch (Exception ex)
        {
            PagesViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
            {
                PagesViewModel.EvaluateGui();
                PagesViewModel.SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm.Text = savedFileName;
            }
            else
            {
                PagesViewModel.SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm.Text = StringsPortal.Incomplete__nothing_saved;
            }

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        private async Task<string> SaveFileToHardDriveAsync(StorageFile file, byte[] bytesToBeSaved, int countOfLineItems)
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

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____saving);

            PagesViewModel.AppendToConversionReportLog($"Saving <{file.Name}> ....");


            await FileIO.WriteBytesAsync(file, bytesToBeSaved);
            // write to file

            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            // Let Windows know that we're finished changing the file.
            // Completing updates may require Windows to ask for user input.

            string message;

            if (status == FileUpdateStatus.Complete)
            {
                PagesViewModel.UpdateLogOfFilesThatWereTransferred(file.Name, "was saved to",
                    "your file system", JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length));

                message = $"Saved as {file.Name}. {JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length)} {countOfLineItems} line-items.";
            }
            else
            {
                PagesViewModel.UpdateLogOfFilesThatWereTransferred(file.Name, "couldn't be saved to",
                    "your file system", JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length));

                message = "File couldn't be saved.";
            }

            return message;

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
