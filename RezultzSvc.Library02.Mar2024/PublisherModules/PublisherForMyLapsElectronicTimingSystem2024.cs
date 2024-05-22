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
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
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

    public override void ExtractCustomXmlInformationFromAssociatedPublisherProfileFile()
    {
        throw new NotImplementedException(); // nothing required at time of writing
    }

    public override async Task<PublisherOutputItem> DoAllTranslationsAndComputationsToGenerateResultsAsync(PublisherInputItem publisherInputItem)
    {
        const string failure = "Unable to compute results for specified event based on datasets loaded.";
        const string locus = "[DoAllTranslationsAndComputationsToGenerateResultsAsync()]";

        const int lhsWidth = 50;
        const int lhsWidthPlus1 = lhsWidth + 1;
        //const int lhsWidthLess1 = lhsWidth - 1;
        //const int lhsWidthLess2 = lhsWidth - 2;
        //const int lhsWidthLess3 = lhsWidth - 3;
        const int lhsWidthLess4 = lhsWidth - 4;
        const int lhsWidthLess5 = lhsWidth - 5;
        //const int lhsWidthLess6 = lhsWidth - 6;
        const int lhsWidthLess7 = lhsWidth - 7;


        var conversionReportSb = new JghStringBuilder();
        var ranToCompletionMsgSb = new JghStringBuilder();
        var startDateTime = DateTime.UtcNow;

        conversionReportSb.AppendLineFollowedByOne("Conversion report:");

        List<ResultItem> allComputedResults = new();

        try
        {
            #region null checks

            //throw new ArgumentNullException(nameof(publisherInputItem), "This is a showstopping exception thrown solely for the purpose of testing and debugging. Be sure to delete it when testing is finished.");

            if (publisherInputItem == null)
                throw new ArgumentNullException(nameof(publisherInputItem), "Remote publishing service received an input object that was null.");

            if (!string.IsNullOrWhiteSpace(publisherInputItem.NullChecksFailureMessage))
                throw new ArgumentException(publisherInputItem.NullChecksFailureMessage); // Note: might look odd, but isn't. The pre-formatted message is the exception message

            #endregion

            #region fetch MyLaps timing data files

            conversionReportSb.AppendLine("Doing null checks to confirm that we have one or more files of timing data originating from MyLaps ...");

            var myLapsFileTargets = publisherInputItem.DeduceStorageLocations(IdentifierOfDatasetOfMyLapsTimingData) ??
                                      throw new JghAlertMessageException("MyLaps file/s not specified. EntityLocationItem array is null.");

            if (!myLapsFileTargets.Any())
                throw new JghAlertMessageException("MyLaps file/s not specified. Please import one or more MyLaps files.");

            List<MyLapsFile> myLapsFiles = new();

            foreach (var target in myLapsFileTargets)
            {
                if (!await _storage.GetIfBlobExistsAsync(target.AccountName, target.ContainerName, target.EntityName))
                    throw new JghAlertMessageException($"Error. Missing file. Specified MyLaps file not found. <{target.EntityName}> ");

                var myLapsTimingDataAsCsv = JghConvert.ToStringFromUtf8Bytes(
                    await _storage.DownloadBlockBlobAsBytesAsync(target.AccountName, target.ContainerName, target.EntityName));

                if (string.IsNullOrWhiteSpace(myLapsTimingDataAsCsv))
                    throw new JghAlertMessageException($"Specified MyLaps file is empty. <{target.EntityName}>");

                myLapsFiles.Add(new MyLapsFile(IdentifierOfDatasetOfMyLapsTimingData, target.EntityName, myLapsTimingDataAsCsv));

                conversionReportSb.AppendLine($"MyLaps file found <{target.EntityName}>...");
            }

            #endregion

            #region fetch participant master list


            Dictionary<string, ParticipantHubItem> dictionaryOfParticipants = RepositoryOfHubStyleEntries.GetDictionaryOfIdentifiersWithTheirMostRecentItemForThisRecordingModeFromMasterList();








            conversionReportSb.AppendLine("Doing null checks to confirm that we have the master list of participants ...");

            var participantFileTarget = publisherInputItem.DeduceStorageLocation(EnumForParticipantListFromPortal) ??
                                     throw new JghAlertMessageException("Participant file not identified. Please import a participant file");

            if (!await _storage.GetIfBlobExistsAsync(participantFileTarget.AccountName, participantFileTarget.ContainerName, participantFileTarget.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected Participant file not found. <{participantFileTarget.EntityName}>");

            var participantFileContentsAsJson = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(participantFileTarget.AccountName, participantFileTarget.ContainerName, participantFileTarget.EntityName));

            if (string.IsNullOrWhiteSpace(participantFileContentsAsJson))
                throw new JghAlertMessageException($"Participant file is empty. <{participantFileTarget.EntityName}>");

            conversionReportSb.AppendLine("Participant file obtained ...");

            List<ParticipantHubItemDto> masterListOfParticipantHubItemDTo;

            try
            {
                masterListOfParticipantHubItemDTo = JghSerialisation.ToObjectFromJson<List<ParticipantHubItemDto>>(participantFileContentsAsJson);
            }
            catch (Exception)
            {
                throw new JghAlertMessageException(
                    "Content and/or format of Participant file is not what publisher was coded for. JSON deserialisation threw an exception.");
            }

            conversionReportSb.AppendLine($"{JghString.LeftAlign("Participant file", lhsWidth)} : {participantFileTarget.EntityName}");
            conversionReportSb.AppendLine($"{JghString.LeftAlign("Participants", lhsWidth)} : {masterListOfParticipantHubItemDTo.Count}");















            #endregion

            #region dig required info out of SeriesProfile

            conversionReportSb.AppendLine("Extracting information out of SeriesProfile...");

            var ageGroupSpecificationItems = GetAgeGroupSpecificationItems(publisherInputItem.SeriesProfile);

            var thisEvent = publisherInputItem.SeriesProfile.EventProfileItems.FirstOrDefault(z => z.Label == publisherInputItem.EventLabelAsEventIdentifier);

            var dateOfThisEvent = thisEvent?.AdvertisedDate ?? DateTime.MinValue;

            #endregion

            #region do all computations and calculations - process one or more files

            foreach (var myLapsFile in myLapsFiles)
            {
                conversionReportSb.AppendLinePrecededByOne($"Processing MyLaps file: <{myLapsFile.FileName}>");

                var answerAsResultItems = MyLaps2024Helper.GenerateResultItemArrayFromMyLapsFile(myLapsFile, conversionReportSb, lhsWidth, dictionaryOfParticipants, ageGroupSpecificationItems, dateOfThisEvent);

                allComputedResults.AddRange(answerAsResultItems);

                conversionReportSb.AppendLineWrappedByOne($"{JghString.LeftAlign("Results synthesised from this file", lhsWidth)} : {answerAsResultItems.Count}");

            }

            #endregion

            #region report progress and wrap up

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Computations ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (participants):", lhsWidthLess7)} <{participantFileTarget.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{participantFileTarget.ContainerName}>");
            foreach (var myLapsFileTarget in myLapsFileTargets)
            {
                ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (MyLaps):", lhsWidthLess7)} <{myLapsFileTarget.EntityName}>");
                ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{myLapsFileTarget.ContainerName}>");
            }
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Results computed:", lhsWidthLess4)} {allComputedResults.Count} results");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. {allComputedResults.Count} results computed.");

            var answer2 = new PublisherOutputItem
            {
                LabelOfEventAsIdentifier = publisherInputItem.EventLabelAsEventIdentifier,
                RanToCompletionMessage = ranToCompletionMsgSb.ToString(),
                ConversionReport = conversionReportSb.ToString(),
                ConversionDidFail = false,
                ComputedResults = allComputedResults.ToArray()
            };

            #endregion

            return await Task.FromResult(answer2);
        }

        #region try-catch

        catch (JghAlertMessageException ex)
        {
            conversionReportSb.AppendLinePrecededByOne(ex.Message);

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLinePrecededByOne($"{JghString.LeftAlign("Outcome:", lhsWidth)} Conversion interrupted. Conversion ran into a problem.");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Message:", lhsWidthPlus1)} {ex.Message}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conversion duration:", lhsWidthLess5)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Results computed:", lhsWidthLess4)} none");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidth)} Failure. Please read the conversion report for more information.");

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

    #region const

    private const string IdentifierOfDatasetOfMyLapsTimingData = "MyLaps2023AsCsv"; // the identifier here originates from the profile file. Can be anything you like there, but must be kept in sync here

    private const string EnumForParticipantListFromPortal = EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub;

    #endregion

    #region variables

    private readonly AzureStorageServiceMethodsHelper _storage = new(new AzureStorageAccessor());

    public static readonly IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem> RepositoryOfHubStyleEntries = new RepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem>(new IsolatedStorageService());


    #endregion

    #region helpers



    private static AgeGroupSpecificationItem[] GetAgeGroupSpecificationItems(SeriesProfileItem seriesMetaDataItem)
    {
        var arrayOfAgeGroupSpecificationItem = seriesMetaDataItem.DefaultEventSettingsForAllEvents.AgeGroupSpecificationItems;

        return arrayOfAgeGroupSpecificationItem;
    }



    #endregion
}