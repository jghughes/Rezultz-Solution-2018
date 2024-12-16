using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Uwp.Common.July2018.Strings;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Xml.July2018;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;
using Rezultz.Library02.Mar2024.DataGridDesigners;
using Rezultz.Library02.Mar2024.DataGridViewmodels;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Uwp.Strings;
using Rezultz.Uwp.UserControls_DataGrid;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Rezultz.Uwp.UserControls;

public sealed partial class PageContentsForLeaderboardStylePageUserControl
{
    private const string Locus2 = nameof(PageContentsForLeaderboardStylePageUserControl);
    private const string Locus3 = "[Rezultz.Uwp]";


    public PageContentsForLeaderboardStylePageUserControl()
    {
        InitializeComponent();

        XamlElementSplitView.IsPaneOpen = false;
    }

    private BaseLeaderboardStylePageViewModel ViewModel => DataContext as BaseLeaderboardStylePageViewModel;

    public FavoritesDataGridPresentationServiceUserControl MyFavoritesDataGridUserControl => XamlElementFavoritesDataGridPresentationServiceUserControl;
    public LeaderboardDataGridPresentationServiceUserControl MyLeaderboardDataGridUserControl => XamlElementLeaderboardDataGridPresentationServiceUserControl;


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

                const string locus = "Property getter of [AlertMessageService]";
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

    private void BtnToggleVisibilityOfSplitViewPane_OnClick(object sender, RoutedEventArgs e)
    {
        XamlElementSplitView.IsPaneOpen = !XamlElementSplitView.IsPaneOpen;
    }

    private async void MyAutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        const string failure = "Unable to save file.";
        const string locus = "[MyAutoSuggestBox_OnTextChanged]";

        try
        {
            if (ViewModel is null)
                throw new NullReferenceException(
                    "The Datacontext is not an object of type LeaderboardStylePageViewModelBase. This is mandatory.");

            // We only want to get results when it was a user typing, otherwise we assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;

            var matchingSuggestions =
                await ViewModel.OnShortlistOfQuerySuggestionsRequestedFromTheSearchUniverseAsync(sender.Text);

            var listOfMatchingSuggestions = matchingSuggestions.ToList();

            if (listOfMatchingSuggestions.Count == 0) listOfMatchingSuggestions.Add("No results found");

            sender.ItemsSource = listOfMatchingSuggestions;
        }

        #region trycatch

        catch (Exception ex)
        {
            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3,
                ex);
        }

        #endregion
    }

    private void MyAutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        // nothing much i can think of. i don't need to do anything behind the scenes.
        // Anyhow, here is how you obtain the SelectedItem for whatever purpose you might choose in future

        // ReSharper disable once UnusedVariable
        var theSelectedSearchSuggestion = (string)args.SelectedItem;
    }

    private async void MyAutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        const string failure = "Unable to respond to submission of search query.";
        const string locus = "[MyAutoSuggestBox_OnQuerySubmitted]";

        try
        {
            if (ViewModel is null)
                throw new NullReferenceException(
                    "The Datacontext is not an object of type LeaderboardStylePageViewModelBase. This is mandatory.");


            if (args.ChosenSuggestion is not null)
            {
                // user selected an item, take an action on it here
                await ViewModel.OnFinalSearchQuerySubmittedAsTextAsync((string)args.ChosenSuggestion);
            }
            else
            {
                // Do a fuzzy search on the query text
                if (!string.IsNullOrWhiteSpace(args.QueryText))
                    await ViewModel.OnFinalSearchQuerySubmittedAsTextAsync(args.QueryText);
            }
        }

        #region trycatch

        catch (Exception ex)
        {
            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3,
                ex);
        }

        #endregion
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

            const string exportFavoritesButtonCommand = "Favorites"; // must match the xaml button command parameter
            const string exportLeaderboardButtonCommand = "Leaderboard"; // ditto

            #endregion

            #region can execute?

            switch (xamlButtonCommandParameter)
            {
                case exportFavoritesButtonCommand:
                {
                    if (ViewModel.ExportFavoritesButtonVm.IsAuthorisedToOperate == false)
                        return;
                    break;
                }
                case exportLeaderboardButtonCommand:
                {
                    if (ViewModel.ExportSingleEventLeaderboardButtonVm.IsAuthorisedToOperate == false)
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

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsRezultz.WorkingFormatting);

            #region prepare file contents

            var desiredFileFormatEnumString = ViewModel.CboLookUpFileFormatsVm.CurrentItem?.EnumString;

            if (string.IsNullOrWhiteSpace(desiredFileFormatEnumString)) throw new JghInvalidValueException("Coding error. Required parameter is null or white space. [ViewModel.CboLookUpFileFormatsVm.CurrentItem?.EnumString.]");

            LeaderboardStyleDataGridViewModel dataGridVm;
            DataGridDesigner dataGridDesigner;

            switch (xamlButtonCommandParameter)
            {
                case exportFavoritesButtonCommand:
                {
                    dataGridVm = ViewModel.DataGridOfFavoritesVm;

                    dataGridDesigner = ViewModel.DataGridDesignerForFavorites;

                    if (dataGridVm is null || dataGridDesigner?.DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects != true) // although misleadingly named, this is the correct property to check
                    {
                        ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                        return;
                    }

                    break;
                }
                case exportLeaderboardButtonCommand:
                {
                    dataGridVm = ViewModel.DataGridOfLeaderboardVm;

                    dataGridDesigner = ViewModel.DataGridDesignerForLeaderboard;

                    if (dataGridVm is null || dataGridDesigner?.DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects != true)
                    {
                        ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                        return;
                    }

                    break;
                }
                default:
                {
                    throw new MissingFieldException("Sorry. Coding error. Missing case in switch statement.");
                }
            }

            var fileContentAsBytes = await GenerateAnswerAsBytes(desiredFileFormatEnumString, dataGridDesigner);

            #endregion

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            #region save file

            var fileNamePrefix = dataGridDesigner.DatagridTitleAndBlurbInformationItem?.Title ?? string.Empty;

            var fileSavePicker = CreateFileSavePicker(desiredFileFormatEnumString, fileNamePrefix);

            var file = await PickSaveFile(fileSavePicker);

            if (file is null)
            {
                ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
                return;
            }

            _ = await SaveFileToHardDriveAsync(file, fileContentAsBytes);

            #endregion

            #endregion

            ViewModel.ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            //// move Gui forward upon success.
            //// nothing to do here. the saving of files is orthogonal to the processing pipeline. all we do is restore prior state regardless.
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
            case EnumStrings.AsHtmlDocument:
            {
                AddFileTypesForHtm(fileSavePicker);
                break;
            }

            case EnumStrings.AsTextDocument:
            {
                AddFileTypesForText(fileSavePicker);
                break;
            }
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
            //case EnumStrings.AsFlatFileJson:
            //{
            //    AddFileTypesForJson(fileSavePicker);
            //    break;
            //}
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

    private static async Task<byte[]> GenerateAnswerAsBytes(string desiredFileFormatEnumString, DataGridDesigner printer)
    {
        const string failure = "Unable to generate contents of file.";
        const string locus = "[GenerateAnswerAsBytes]";


        byte[] answerAsBytes;

        switch (desiredFileFormatEnumString)
        {
            case EnumStrings.AsHtmlDocument:
            {
                var htmWebDocumentAsString = await printer.GetLeaderboardStyleResultsArrayAsPrettyPrintedWebPageOrTextFileAsync(false);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(htmWebDocumentAsString);
                break;
            }

            case EnumStrings.AsTextDocument:
            {
                var textDocumentAsString = await printer.GetLeaderboardStyleResultsArrayAsPrettyPrintedWebPageOrTextFileAsync(true);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(textDocumentAsString);
                break;
            }

            case EnumStrings.AsCsvFile:
            {
                var resultsItemsWrappedInAParentXe = await printer.GetLeaderboardStyleResultsArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat.SameAsGuiLayout);
                var csvDocumentAsString = JghXmlToCsvHelpers.TransformXElementContainingArrayOfChildElementsToCsvFileContentsForExcel(resultsItemsWrappedInAParentXe);
                answerAsBytes = JghConvert.ToBytesUtf8FromString(csvDocumentAsString);
                break;
            }

            case EnumStrings.AsFlatFileXml:
            {
                var resultsItemsWrappedInAParentXe = await printer.GetLeaderboardStyleResultsArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat.SameAsGuiLayout);
                answerAsBytes = JghXElementHelpers.TransformXElementToBytesUtf8(resultsItemsWrappedInAParentXe, SaveOptions.None);
                break;
            }
            //case EnumStrings.AsSerialisedObjectXml:
            //{
            //    var resultsItemsWrappedInAParentXe = printer.GetLeaderboardAsResultItemsAsXml();
            //    answerAsBytes = JghXElementHelpers.TransformXElementToBytesUtf8(resultsItemsWrappedInAParentXe, SaveOptions.None);
            //    break;
            //}
            default:
            {
                throw new JghAlertMessageException($"Feature not available. [{failure}] [{locus}]");
            }
        }

        return answerAsBytes;
    }

    private async Task<bool> SaveFileToHardDriveAsync(StorageFile file, byte[] bytesToBeSaved)
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

            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            // Let Windows know that we're finished changing the file

            if (status == FileUpdateStatus.Complete)
                await AlertMessageService.ShowOkAsync(
                    $"Saved as {file.Name} ({JghConvert.SizeOfBytesInHighestUnitOfMeasure(bytesToBeSaved.Length)})");
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

        return true;
    }

    private static void AddFileTypesForHtm(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".htm";
        savePicker.FileTypeChoices.Add("Web page", new List<string> { ".htm" });
        savePicker.FileTypeChoices.Add("Text document", new List<string> { ".txt" });
    }

    private static void AddFileTypesForText(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".txt";
        savePicker.FileTypeChoices.Add("Text document", new List<string> { ".txt" });
    }

    private static void AddFileTypesForCsv(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".csv";
        savePicker.FileTypeChoices.Add("Comma separated values data", new List<string> { ".csv" });
        savePicker.FileTypeChoices.Add("Text document", new List<string> { ".txt" });
    }

    private static void AddFileTypesForXml(FileSavePicker savePicker)
    {
        savePicker.DefaultFileExtension = @".xml";
        savePicker.FileTypeChoices.Add("XML data", new List<string> { ".xml" });
        savePicker.FileTypeChoices.Add("Text document", new List<string> { ".txt" });
    }
}
