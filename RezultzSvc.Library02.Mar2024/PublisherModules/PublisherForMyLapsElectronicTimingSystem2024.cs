using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;
using RezultzSvc.Library02.Mar2024.SvcHelpers;

namespace RezultzSvc.Library02.Mar2024.PublisherModules;

/// <summary>
///     Takes the CSV data files received manually from Andrew Haddow from the MyLaps electronic timing system.
///     Each race has its own CSV file. In each file, the  relevant columns are 'Gun Time' and 'Bib'.
///     Takes the JSON participant master list from the Rezultz portal, and using the JSON seriesMetadata
///     from Rezultz, does all translations and computation and deduces a resulting array of ResultItems
///     and generates an XML file in the format required by the Rezultz 2018 import API.
///     Note: on occasions when Andrew is not using Rezultz, which was for all events except Event #01,
///     he chooses Excel as his preferred MyLaps export format and then he cuts and pasts selected
///     columns from Excel into a TextBox in RaceRoster, assigning the columns to the correct fields in RaceRoster.
///     Specified in
///     https://systemrezultzlevel1.blob.core.windows.net/publishingmoduleprofiles/publisherprofile-23mylaps.xml
///     In future, don't do things the way we jury-rigged them in 2023 as described above.
///     The MyLaps CSV direct export function is buggy and can't handle times exceeding one hour.
///     Rather export from MyLaps into Excel, then into Access, and then into XML. This might improve your chances.
///     It will mean not having to work with MyLaps directly, or with infernal .csv files.
///     At time of writing, the conversion module specification file specifies two ComputerGuiButtonProfileItems.
///     The two DatasetIdentifier are:
///     Jgh.SymbolsStringsConstants.Mar2022.EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub
///     and "MyLaps2023AsCsv".
///     These are the buttons that the user clicks to get info from the hub and browse the hard drive and these are the
///     datasets expected here.
/// </summary>
public class PublisherForMyLapsElectronicTimingSystem2024 : PublisherBase
{
    private const string Locus2 = nameof(PublisherForMyLapsElectronicTimingSystem2024);
    private const string Locus3 = "[RezultzSvc.Library02.Mar2024]";

    #region variables

    private readonly AzureStorageServiceMethodsHelper _storage = new(new AzureStorageAccessor());

    #endregion

    #region helpers

    private static AgeGroupSpecificationItem[] GetAgeGroupSpecificationItems(SeriesProfileItem seriesMetaDataItem)
    {
        var arrayOfAgeGroupSpecificationItem = seriesMetaDataItem.DefaultEventSettingsForAllEvents.AgeGroupSpecificationItems;

        return arrayOfAgeGroupSpecificationItem;
    }

    #endregion

    #region local class

    public class MyLapsFile
    {
        public MyLapsFile()
        {
        }

        public MyLapsFile(string identifierOfDataSet, string fileName, string contents)
        {
            IdentifierOfDataset = identifierOfDataSet ?? string.Empty;
            FileName = fileName ?? string.Empty;
            FileContents = contents ?? string.Empty;
        }

        public string IdentifierOfDataset { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileContents { get; set; } = string.Empty;
    }

    #endregion

    #region methods

    public override void ExtractCustomXmlInformationFromAssociatedPublisherProfileFile()
    {
        throw new NotImplementedException(); // nothing required at time of writing
    }

    public override async Task<PublisherOutputItem> DoAllTranslationsAndComputationsToGenerateResultsAsync(PublisherInputItem publisherInputItem)
    {
        const string failure = "Unable to compute results for specified event based on datasets uploaded for processing.";
        const string locus = "[DoAllTranslationsAndComputationsToGenerateResultsAsync()]";

        #region declarations

        var startDateTime = DateTime.UtcNow;

        JghStringBuilder ranToCompletionMsgSb = new();

        JghStringBuilder conversionReportSb = new(); conversionReportSb.AppendLineFollowedByOne("Processing report:");

        List<MyLapsFile> myLapsFiles = [];

        List<ResultItem> allComputedResults = [];

        #endregion

        try
        {
            #region null checks

            //throw new ArgumentNullException(nameof(publisherInputItem), "This is a showstopping exception thrown solely for the purpose of testing and debugging. Be sure to delete it when testing is finished.");

            if (publisherInputItem == null)
                throw new ArgumentNullException(nameof(publisherInputItem), "Remote publishing service received an input object that was null <PublisherInputItem>.");

            if (!string.IsNullOrWhiteSpace(publisherInputItem.NullChecksFailureMessage))
                throw new ArgumentException(publisherInputItem.NullChecksFailureMessage); // Note: might look odd, but isn't. The pre-formatted message is the exception message

            #endregion

            #region look in PublisherInputItem for names of previously uploaded MyLaps timing data files

            conversionReportSb.AppendLine("Doing null checks to confirm that we have one or more files of timing data originating from MyLaps.");

            var myLapsFileTargets = publisherInputItem.DeduceStorageLocations(IdentifierOfDatasetOfMyLapsTimingData) ??
                                    throw new JghAlertMessageException("MyLaps file/s not specified. EntityLocationItem array is null.");

            if (!myLapsFileTargets.Any())
                throw new JghAlertMessageException("No MyLaps files specified. Please import and upload one or more files of MyLaps timing data in order to proceed.");

            #endregion

            #region fetch MyLaps timing data files from Azure

            foreach (var target in myLapsFileTargets)
            {
                if (!await _storage.GetIfBlobExistsAsync(target.AccountName, target.ContainerName, target.EntityName))
                    throw new JghAlertMessageException($"Error. Missing file. Specified MyLaps file not found. <{target.EntityName}> ");

                var myLapsTimingDataAsString = JghConvert.ToStringFromUtf8Bytes(
                    await _storage.DownloadBlockBlobAsBytesAsync(target.AccountName, target.ContainerName, target.EntityName));

                if (string.IsNullOrWhiteSpace(myLapsTimingDataAsString))
                    throw new JghAlertMessageException($"Specified MyLaps file is blank. <{target.EntityName}>");

                conversionReportSb.AppendLine($"MyLaps file found. <{target.EntityName}>");

                myLapsFiles.Add(new MyLapsFile(IdentifierOfDatasetOfMyLapsTimingData, target.EntityName, myLapsTimingDataAsString));
            }

            #endregion

            #region fetch previously uploaded participant master list

            conversionReportSb.AppendLine("Doing null checks to confirm that we have a previously uploaded master list of participants.");

            var participantFileTarget = publisherInputItem.DeduceStorageLocation(EnumForParticipantListFromPortal) ??
                                        throw new JghAlertMessageException("Name of file containing master list of participants not identified. Please import and upload a participant file.");

            if (!await _storage.GetIfBlobExistsAsync(participantFileTarget.AccountName, participantFileTarget.ContainerName, participantFileTarget.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected file containing master list of participants not found. <{participantFileTarget.EntityName}>");

            var participantFileContentsAsJson = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(participantFileTarget.AccountName, participantFileTarget.ContainerName, participantFileTarget.EntityName));

            if (string.IsNullOrWhiteSpace(participantFileContentsAsJson))
                throw new JghAlertMessageException($"Participant file is empty. <{participantFileTarget.EntityName}>");

            conversionReportSb.AppendLine("Success. File containing master list of participants obtained.");

            List<ParticipantHubItemDto> masterListOfParticipantHubItemDTo;

            try
            {
                masterListOfParticipantHubItemDTo = JghSerialisation.ToObjectFromJson<List<ParticipantHubItemDto>>(participantFileContentsAsJson);
            }
            catch (Exception)
            {
                throw new JghAlertMessageException(
                    "Content and/or format of file containing master list of participants is not what publisher was coded to expect. JSON deserialisation threw an exception.");
            }

            conversionReportSb.AppendLine($"{JghString.LeftAlign("Participant file", LhsWidth)} : {participantFileTarget.EntityName}");
            conversionReportSb.AppendLine($"{JghString.LeftAlign("Participants", LhsWidth)} : {masterListOfParticipantHubItemDTo.Count}");

            #endregion

            #region convert participant master list to dictionary

            var masterListOfParticipantsAsDictionary = masterListOfParticipantHubItemDTo.ToDictionary(z => z.Bib, ParticipantHubItem.FromDataTransferObject);

            #endregion

            #region get ready - dig required info out of SeriesProfile

            conversionReportSb.AppendLine("Extracting information out of SeriesProfile.");

            var ageGroupSpecificationItems = GetAgeGroupSpecificationItems(publisherInputItem.SeriesProfile);

            var thisEvent = publisherInputItem.SeriesProfile.EventProfileItems.FirstOrDefault(z => z.Label == publisherInputItem.EventLabelAsEventIdentifier);

            var dateOfThisEvent = thisEvent?.AdvertisedDate ?? DateTime.MinValue;

            #endregion

            #region do all computations and calculations - process one or more files

            foreach (var myLapsFile in myLapsFiles)
            {
                conversionReportSb.AppendLinePrecededByOne($"{JghString.LeftAlign("Processing MyLaps file", LhsWidth)} : {myLapsFile.FileName}");

                var answerAsResultItemsInThisFile = MyLaps2024Helper.GenerateResultItemArrayFromMyLapsFile(myLapsFile, masterListOfParticipantsAsDictionary, ageGroupSpecificationItems, dateOfThisEvent, conversionReportSb, LhsWidth); // the MEAT

                conversionReportSb.AppendLineWrappedByOne($"{JghString.LeftAlign("Results synthesised from this file", LhsWidth)} : {answerAsResultItemsInThisFile.Count}");

                allComputedResults.AddRange(answerAsResultItemsInThisFile.OrderBy(z => z.T01));
            }

            #endregion

            #region report conclusion of processing

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", LhsWidth)} Computations ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", LhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (participants):", LhsWidthLess7)} <{participantFileTarget.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", LhsWidthPlus1)} <{participantFileTarget.ContainerName}>");
            foreach (var myLapsFileTarget in myLapsFileTargets)
            {
                ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (MyLaps):", LhsWidthLess7)} <{myLapsFileTarget.EntityName}>");
                ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", LhsWidthPlus1)} <{myLapsFileTarget.ContainerName}>");
            }

            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Results computed:", LhsWidthLess4)} {allComputedResults.Count} results");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", LhsWidthPlus1)} Success. {allComputedResults.Count} results computed.");

            #endregion

            #region return answer

            var answer2 = new PublisherOutputItem
            {
                LabelOfEventAsIdentifier = publisherInputItem.EventLabelAsEventIdentifier,
                RanToCompletionMessage = ranToCompletionMsgSb.ToString(),
                ConversionReport = conversionReportSb.ToString(),
                ConversionDidFail = false,
                ComputedResults = allComputedResults.ToArray()
            };

            return await Task.FromResult(answer2);

            #endregion
        }

        #region try-catch

        catch (JghAlertMessageException ex)
        {
            conversionReportSb.AppendLinePrecededByOne(ex.Message);

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLinePrecededByOne($"{JghString.LeftAlign("Outcome:", LhsWidth)} Conversion interrupted. Conversion ran into a problem.");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Message:", LhsWidthPlus1)} {ex.Message}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conversion duration:", LhsWidthLess5)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Results computed:", LhsWidthLess4)} none");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", LhsWidth)} Failure. Please read the conversion report for more information.");

            var answer2 = new PublisherOutputItem
            {
                LabelOfEventAsIdentifier = publisherInputItem?.EventLabelAsEventIdentifier,
                RanToCompletionMessage = ranToCompletionMsgSb.ToString(),
                ConversionReport = conversionReportSb.ToString(),
                ConversionDidFail = true,
                ComputedResults = Array.Empty<ResultItem>()
            };

            return await Task.FromResult(answer2);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region constants

    private const string IdentifierOfDatasetOfMyLapsTimingData = "MyLaps"; // the identifier for MyLaps timing data files here originates from the profile file. Can be anything you like there, but must be kept in sync here

    private const string EnumForParticipantListFromPortal = EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub;

    private const int LhsWidth = 50;
    private const int LhsWidthPlus1 = LhsWidth + 1;
    private const int LhsWidthLess4 = LhsWidth - 4;
    private const int LhsWidthLess5 = LhsWidth - 5;
    private const int LhsWidthLess7 = LhsWidth - 7;

    #endregion
}