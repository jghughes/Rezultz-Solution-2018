﻿using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.Strings;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Xml.July2018;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDesigners;

using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.PageViewModels;
using RezultzPortal.Uwp.Strings;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class RegisterParticipantsToolsPage
    {
        private const string Locus2 = nameof(RegisterParticipantsToolsPage);
        private const string Locus3 = "[RezultzPortal.Uwp]";


        public RegisterParticipantsToolsPage()
    {
        Loaded += OnPageHasCompletedLoading;

        InitializeComponent();

        NavigationCacheMode = NavigationCacheMode.Enabled;

        XamlElementGridOfAllPageContents.Visibility = Visibility.Collapsed;
    }


        private RegisterParticipantsViewModel ViewModel => DataContext as RegisterParticipantsViewModel;

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

        public IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem> RepositoryOfParticipantHubItemEntries
        {
            get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem>>();
            }
            catch (Exception ex)
            {
                var msg =
                    JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                        "<IRepositoryOfParticipantStorage>");

                const string locus = "Property getter of <RepositoryOfParticipantHubItemEntries]";
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
            if (ViewModel is null)
                throw new ArgumentNullException(StringsForXamlPages.DataContextIsNull);

            DependencyLocator.RegisterIAlertMessageServiceProvider(this);

            ViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModeIsNull();

            ViewModel.ThrowIfWorkSessionNotProperlyInitialised();

            ViewModel.ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

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

        base.OnNavigatingFrom(e);
    }

        private async void BtnExportChosenData_OnClick(object sender, RoutedEventArgs e)
    {
        const string failure = "Unable to save file.";
        const string locus = "[BtnExportChosenData_OnClick]";


        try
        {
            #region null value error handling

            if (ViewModel is null)
                throw new ArgumentNullException(nameof(ViewModel));

            #endregion

            #region process the sender command parameter

            var xamlButtonCommandParameter = ((Button)sender).CommandParameter?.ToString();

            if (string.IsNullOrWhiteSpace(xamlButtonCommandParameter)) return;

            const string exportParticipantsButtonCommand = "ParticipantMasterList"; // must match the xaml button command parameter

            #endregion

            #region can execute?

            switch (xamlButtonCommandParameter)
            {
                case exportParticipantsButtonCommand:
                {
                    if (!ViewModel.ExportConsolidatedParticipantDataFromLocalStorageButtonVm.IsAuthorisedToOperate)
                        return;
                    break;
                }
                default:
                {
                    throw new MissingFieldException("Sorry. Coding error. Missing case in switch statement.");
                }
            }

            #endregion

            ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

            #region do work

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____processing);

            #region prepare file contents as byte[]

            var desiredFileFormatEnum = ViewModel.CboLookUpFileFormatsVm.CurrentItem?.EnumString;

            if (string.IsNullOrWhiteSpace(desiredFileFormatEnum)) throw new JghInvalidValueException("Coding error. Enum for selected file format is blank.");

            int numberOfItemsInFile;

            byte[] fileContentAsBytes;

            if (XamlElementRadioButtonSystemMasterList.IsChecked is not null && (bool)XamlElementRadioButtonSystemMasterList.IsChecked)
            {
                var participantDatabase = new ParticipantDatabase();

                participantDatabase.LoadDatabaseV2(RepositoryOfParticipantHubItemEntries);

                var hubItems = participantDatabase.GetMasterList();

                var dataTransferObjects = ParticipantHubItem.ToDataTransferObject(hubItems);

                if (dataTransferObjects is null)
                {
                    ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(
                        EnumsForGui.Restore);

                    return;
                }

                numberOfItemsInFile = dataTransferObjects.Length;

                fileContentAsBytes = await GenerateAnswerAsBytes(desiredFileFormatEnum, dataTransferObjects);
            }
            else if (XamlElementRadioButtonDisplayVersion.IsChecked is not null && (bool)XamlElementRadioButtonDisplayVersion.IsChecked)
            {
                var dataGridPresenter = ViewModel.DataGridOfItemsInRepository;

                if (dataGridPresenter is null)
                {
                    ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                    return;
                }

                var dataGridDesigner = ViewModel.DataGridDesignerForItemsInRepository;

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.WorkingFormatting);

                if (dataGridDesigner?.DesignerIsInitialisedForParticipantHubItemDisplayObjects != true)
                {
                    ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                    return;
                }

                numberOfItemsInFile = dataGridPresenter.ItemsSource.Count;

                fileContentAsBytes = await GenerateAnswerAsBytes(desiredFileFormatEnum, dataGridDesigner);
            }
            else
            {
                throw new JghAlertMessageException("Data source not recognised. Please choose a data source.");
            }

            #endregion

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            #region save file

            var fileSavePicker = CreateFileSavePicker(desiredFileFormatEnum, "Participants");

            var file = await PickSaveFile(fileSavePicker);

            if (file is null)
            {
                ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                return;
            }

            await SaveFileToHardDriveAsync(file, fileContentAsBytes, numberOfItemsInFile);

            #endregion

            #endregion

            ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            // move Gui forward upon success
            // nothing to do here. the saving of files is orthogonal to the processing pipeline. all we do is restore prior state regardless.
        }

        #region trycatch

        catch (Exception ex)
        {
            ViewModel?.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            // nothing to do here. the saving of files is orthogonal to the processing pipeline. all we do is restore prior state regardless.

            await ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        private static FileSavePicker CreateFileSavePicker(string desiredFileFormatEnum, string fileNamePrefix)
    {
        const string failure = "Unable to populate save file dialogue.";
        const string locus = "[CreateFileSavePicker]";


        var fileSavePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };

        switch (desiredFileFormatEnum)
        {
            case EnumStrings.AsCsvFile:
            {
                AddFileTypesForCsv(fileSavePicker);
                break;
            }
            case EnumStrings.AsFlatFileXml:
            {
                AddFileTypesForXml(fileSavePicker);
                break;
            }
            case EnumStrings.AsFlatFileJson:
            {
                AddFileTypesForJson(fileSavePicker);
                break;
            }
            default:
            {
                throw new JghAlertMessageException($"[{failure}] [{locus}] File format not supported. ");
            }
        }

        fileSavePicker.SuggestedFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(fileNamePrefix);
        return fileSavePicker;
    }

        private async Task<StorageFile> PickSaveFile(FileSavePicker fileSavePicker)
    {
        const string failure = "Unable to select a filename.";
        const string locus = "[PickSaveFile]";

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
            throw new JghAlertMessageException($"{StringsUwpCommon.SaveFileDialogInvalidOperationExceptionMessage} [{failure}] [{locus}]");
        }
        catch (Exception ex)
        {
            throw new JghAlertMessageException($"Unable to pick name of file to save. [{failure}] [{locus}]", ex);
        }

        return file;
    }

        private static async Task<byte[]> GenerateAnswerAsBytes<T>(string desiredFileFormatEnum, T[] dataTransferObjects)
    {
        const string failure = "Unable to generate contents of file.";
        const string locus = "[GenerateAnswerAsBytes]";


        byte[] answerAsBytes;

        switch (desiredFileFormatEnum)
        {
            case EnumStrings.AsCsvFile:
            {
                var itemsWrappedInAParentXe = JghSerialisation.ToXElementWithoutWhiteSpaceFromObject(dataTransferObjects, [typeof(T)]);
                var csvDocumentAsString = JghXmlToCsvHelpers.TransformXElementContainingArrayOfChildElementsToCsvFileContentsForExcel(itemsWrappedInAParentXe);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(csvDocumentAsString);
                break;
            }
            case EnumStrings.AsFlatFileXml:
            {
                var itemsAsStringOfXml = JghSerialisation.ToXmlFromObject(dataTransferObjects, [typeof(T)]);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(itemsAsStringOfXml);
                break;
            }
            case EnumStrings.AsFlatFileJson:
            {
                var itemsAsStringOfJson = JghSerialisation.ToJsonFromObject(dataTransferObjects);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(itemsAsStringOfJson);
                break;
            }
            default:
            {
                throw new JghAlertMessageException($"File format option not available. [{failure}] [{locus}]");
            }
        }

        return await Task.FromResult(answerAsBytes);
    }

        private static async Task<byte[]> GenerateAnswerAsBytes(string desiredFileFormatEnum, DataGridDesigner printer)
    {
        const string failure = "Unable to generate contents of file.";
        const string locus = "[GenerateAnswerAsBytes]";

        byte[] answerAsBytes;

        switch (desiredFileFormatEnum)
        {
            case EnumStrings.AsCsvFile:
            {
                var childrenWrappedInAParentXe = await printer.GetParticipantHubItemArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat.SameAsGuiLayout);
                var csvDocumentAsString = JghXmlToCsvHelpers.TransformXElementContainingArrayOfChildElementsToCsvFileContentsForExcel(childrenWrappedInAParentXe);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(csvDocumentAsString);
                break;
            }
            case EnumStrings.AsFlatFileXml:
            {
                var childrenWrappedInAParentXe = await printer.GetParticipantHubItemArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat.SameAsGuiLayout);
                answerAsBytes = JghXElementHelpers.TransformXElementToBytesUtf8(childrenWrappedInAParentXe, SaveOptions.None);
                break;
            }
            case EnumStrings.AsFlatFileJson:
            {
                throw new JghAlertMessageException("JSON file format not valid for this dataset.");
            }
            default:
            {
                throw new JghAlertMessageException($"Coding error. Unrecognised file format. [{failure}] [{locus}]");
            }
        }

        return answerAsBytes;
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

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____saving);

            ViewModel.UpdateLogOfActivity($"Saving '{file.Name}' ....");

            await FileIO.WriteBytesAsync(file, bytesToBeSaved);
            // write to file

            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            // Let Windows know that we're finished changing the file.

            if (status == FileUpdateStatus.Complete)
            {
                ViewModel.UpdateLogOfFilesThatWereTransferred(file.Name, "was saved to",
                    "your documents", JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length));
                await ShowOkAsync(
                    $"Saved as {file.Name}. {JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length)} {countOfLineItems} line-items.");
            }
            else
            {
                ViewModel.UpdateLogOfFilesThatWereTransferred(file.Name, "couldn't be saved to",
                    "your documents", JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length));
                await ShowOkAsync("Document couldn't be saved.");
            }

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

        private static void AddFileTypesForCsv(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".csv";
        savePicker.FileTypeChoices.Add("Comma separated values data", new List<string> {".csv"});
        savePicker.FileTypeChoices.Add("Text document", new List<string> {".txt"});
    }

        private static void AddFileTypesForXml(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".xml";
        savePicker.FileTypeChoices.Add("XML data", new List<string> {".xml"});
        savePicker.FileTypeChoices.Add("Text document", new List<string> {".txt"});
    }

        private static void AddFileTypesForJson(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".json";
        savePicker.FileTypeChoices.Add("JSON data", new List<string> {".json"});
        savePicker.FileTypeChoices.Add("Text document", new List<string> {".txt"});
    }
    }
}
