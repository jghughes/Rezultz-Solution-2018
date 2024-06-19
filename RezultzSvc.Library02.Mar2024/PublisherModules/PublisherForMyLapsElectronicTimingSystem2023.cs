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
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repositories;
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
public class PublisherForMyLapsElectronicTimingSystem2023 : PublisherBase
{
    private const string Locus2 = nameof(PublisherForMyLapsElectronicTimingSystem2023);
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

        conversionReportSb.AppendLineFollowedByOne("Processing report:");

        List<ResultItem> allComputedResults = [];

        try
        {
            #region null checks

            //throw new ArgumentNullException(nameof(publisherInputItem), "This is a showstopping exception thrown solely for the purpose of testing and debugging. Be sure to delete it when testing is finished.");

            if (publisherInputItem is null)
                throw new ArgumentNullException(nameof(publisherInputItem), "Remote publishing service received an input object that was null.");

            if (!string.IsNullOrWhiteSpace(publisherInputItem.NullChecksFailureMessage))
                throw new ArgumentException(publisherInputItem.NullChecksFailureMessage); // Note: might look odd, but isn't. The pre-formatted message is the exception message

            #endregion

            #region fetch previously uploaded  MyLaps timing data files

            conversionReportSb.AppendLine("Doing null checks to confirm that we have one or more files of timing data originating from MyLaps ...");

            var myLapsFileTargets = publisherInputItem.DeduceStorageLocations(IdentifierOfDatasetOfMyLapsTimingData) ??
                                      throw new JghAlertMessageException("MyLaps file/s not specified. EntityLocationItem array is null.");

            if (!myLapsFileTargets.Any())
                throw new JghAlertMessageException("MyLaps file/s not specified. Please import one or more MyLaps files.");

            List<MyLapsFile> myLapsFiles = [];

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

            #region fetch previously uploaded participant master list

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

            #region do all computations and calculations - process the csv data in one or more files

            foreach (var myLapsFile in myLapsFiles)
            {
                conversionReportSb.AppendLinePrecededByOne($"Processing MyLaps file <{myLapsFile.FileName}>...");

                #region step 1 interpret ObtainedDatasetToBeProcessedAsRawString as csv data if we can

                var allRowsOfCsvText = myLapsFile.FileContents.Split(["\r\n", "\r", "\n"], StringSplitOptions.None).ToList(); // remove carriage returns and line breaks

                var relevantRowsOfCsvText =
                    allRowsOfCsvText.Where(z => !string.IsNullOrWhiteSpace(z)).Where(z => z.Contains(','))
                        .ToList(); // eliminate blank lines and lines that are non-data lines in the MyLaps files - for starters there is a pair of blank lines at the bottom of the file

                List<string> relevantRowsWithoutEscapeLiterals = [];

                foreach (var rowOfCsv in relevantRowsOfCsvText)
                {
                    var thisRowOfCsv = rowOfCsv;
                    thisRowOfCsv = thisRowOfCsv.Replace(@"\", string.Empty);
                    thisRowOfCsv = thisRowOfCsv.Replace(@"""", string.Empty);

                    if (!string.IsNullOrWhiteSpace(thisRowOfCsv))
                        relevantRowsWithoutEscapeLiterals.Add(thisRowOfCsv);
                }

                conversionReportSb.AppendLine($"{JghString.LeftAlign("Rows of CSV text extracted", lhsWidth)} : {relevantRowsWithoutEscapeLiterals.Count}");

                // bale if we don't have at least a heading row and one data row
                if (relevantRowsWithoutEscapeLiterals.Count() < NumberOfRowsPrecedingRowOfColumnHeadings + 2) throw new JghAlertMessageException("Unable to extract any rows of CSV data in the file.");

                #endregion

                #region step 2 process the headings row (which might or might not be the first row) - locate the column headings

                conversionReportSb.AppendLine("Extracting column headings from header row in CSV file...");

                var arrayOfColumnHeadings = relevantRowsWithoutEscapeLiterals.Skip(NumberOfRowsPrecedingRowOfColumnHeadings).FirstOrDefault()?.Split(',');

                if (arrayOfColumnHeadings is null) throw new JghAlertMessageException("Unable to find any column headings in the provided MyLaps data. It is therefore impossible to interpret the data.");

                // example of headings from MyLaps : "Pos","Bib#","Athlete","Finish Time","Race","Age","Gender"
                var indexOfBibColumn = Array.IndexOf(arrayOfColumnHeadings, "Bib#");
                var indexOfTotalDuration = Array.IndexOf(arrayOfColumnHeadings, "Finish Time");
                var indexOfFullName = Array.IndexOf(arrayOfColumnHeadings, "Athlete");
                var indexOfRaceGroup = Array.IndexOf(arrayOfColumnHeadings, "Race");
                var indexOfAge = Array.IndexOf(arrayOfColumnHeadings, "Age");
                var indexOfGender = Array.IndexOf(arrayOfColumnHeadings, "Gender");

                if (indexOfBibColumn == -1 && indexOfTotalDuration == -1 && indexOfFullName == -1 && indexOfRaceGroup == -1 && indexOfAge == -1 && indexOfGender == -1)
                    throw new JghAlertMessageException("Unable to find any of the specific column headings which this conversion module absolutely requires to interpret the MyLaps data.");

                #endregion

                #region step 3 process all the subsequent rows (which should be legit rows of data by now)

                var rowsOfData = relevantRowsWithoutEscapeLiterals.Skip(NumberOfRowsPrecedingRowOfColumnHeadings + 1).ToList(); // skip first line i.e. the header row we just processed in Step 2

                conversionReportSb.AppendLine("Processing rows of results in CSV file...");

                if (!rowsOfData.Any())
                    // bale if we don't have any data rows
                    throw new JghAlertMessageException("Unable to extract any rows of csv rows in the provided MyLaps file.");

                var i = 1;

                List<ResultItem> answerAsResultItems = [];

                foreach (var row in rowsOfData)
                {
                    var arrayOfDataInRow = row.Split(',');

                    #region instantiate ResultItemDataTransferObject for this row and populate with particulars from participant master list iff Bib is found in master list

                    var identifierOfParticipant = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfBibColumn);

                    if (string.IsNullOrWhiteSpace(identifierOfParticipant)) continue;

                    var participantItem = masterListOfParticipantHubItemDTo.FirstOrDefault(z => z.Bib == identifierOfParticipant);

                    ResultItem thisResultItem;

                    if (participantItem is null)
                    {
                        // "Pos","Bib#","Athlete","Finish Time","Race","Age","Gender"

                        thisResultItem = new ResultItem
                        {
                            Bib = identifierOfParticipant,
                            LastName = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfFullName),
                            Gender = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfGender),
                            Age = JghConvert.ToInt32(GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfAge)),
                            RaceGroup = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfRaceGroup),
                            IsSeries = false
                        };

                        conversionReportSb.AppendLine(
                            $"Warning! Participant master list fails to have a Bib number for <{thisResultItem.Bib} {thisResultItem.LastName} {thisResultItem.RaceGroup}>");
                    }
                    else
                    {
                        thisResultItem = new ResultItem
                        {
                            Bib = participantItem.Bib,
                            FirstName = participantItem.FirstName,
                            LastName = participantItem.LastName,
                            MiddleInitial = participantItem.MiddleInitial,
                            Gender = participantItem.Gender,
                            Age = ParticipantDatabase.ToAgeFromBirthYear(participantItem.BirthYear),
                            AgeGroup = ParticipantDatabase.ToAgeCategoryDescriptionFromBirthYear(participantItem.BirthYear, ageGroupSpecificationItems),
                            City = participantItem.City,
                            Team = participantItem.Team,
                            IsSeries = participantItem.IsSeries
                        };

                        #region figure out RaceGroup

                        thisResultItem.RaceGroup = FigureOutRaceGroup(participantItem, dateOfThisEvent);

                        #endregion
                    }


                    #region figure out TO1 and Dnx regardless of whether participant is in master list

                    var mustSkipThisRowBecauseGunTimeIsNonsense = false; // initial default

                    var durationAsPossiblyNastyString = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfTotalDuration);

                    if (TryConvertTextToTimespan(durationAsPossiblyNastyString, out var myTimeSpan, out var conversionReport01))
                    {
                        thisResultItem.T01 = myTimeSpan.ToString("G");
                        thisResultItem.DnxString = string.Empty;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dnf"))
                    {
                        thisResultItem.T01 = string.Empty;
                        thisResultItem.DnxString = Symbols.SymbolDnf;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dns"))
                    {
                        thisResultItem.T01 = string.Empty;
                        thisResultItem.DnxString = Symbols.SymbolDns;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dq"))
                    {
                        thisResultItem.T01 = string.Empty;
                        thisResultItem.DnxString = Symbols.SymbolDq;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("tbd"))
                    {
                        thisResultItem.T01 = string.Empty;
                        thisResultItem.DnxString = Symbols.SymbolTbd;
                    }
                    else
                    {
                        thisResultItem.T01 = $"Gun Time is nonsense. {conversionReport01}";
                        mustSkipThisRowBecauseGunTimeIsNonsense = true;
                    }

                    #endregion

                    conversionReportSb.AppendLine(WriteOneLineReport(i, thisResultItem, durationAsPossiblyNastyString));

                    i += 1;

                    if (mustSkipThisRowBecauseGunTimeIsNonsense)
                        continue;

                    answerAsResultItems.Add(thisResultItem);
                }

                #endregion

                #endregion

                #region step 4 report progress

                var j = 1;

                foreach (var resultItemDataTransferObject in answerAsResultItems.OrderBy(z => z.T01))
                {
                    conversionReportSb.AppendLine(WriteOneLineReport(j, resultItemDataTransferObject, string.Empty));

                    j += 1;
                }

                #endregion

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
                ComputedResults = []
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

    #region settings

    private const string IdentifierOfDatasetOfMyLapsTimingData = "MyLaps2023AsCsv"; // the identifier here originates from the profile file. Can be anything you like there, but must be kept in sync here

    private const string EnumForParticipantListFromPortal = EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub;

    private const int NumberOfRowsPrecedingRowOfColumnHeadings = 0;
    // Note: The value of this constant of 0 is normal for csv files exported manually by Jgh from the Excel exported from MyLaps.
    // It is 1 for csv files exported directly from MyLaps. They have some sort of title row before the field names row.

    #endregion

    #region fields

    private readonly AzureStorageServiceMethodsHelper _storage = new(new AzureStorageAccessor());


    #endregion

    #region helpers

    private static string FigureOutRaceGroup(ParticipantHubItemDto participantItem, DateTime dateOfEvent)
    {
        var isTransitionalParticipant = participantItem.RaceGroupBeforeTransition != participantItem.RaceGroupAfterTransition;

        string answer;

        if (isTransitionalParticipant)
        {
            if (DateTime.TryParse(participantItem.DateOfRaceGroupTransitionAsString, out var dateOfTransition))
            {
                var eventIsBeforeRaceGroupTransition = dateOfEvent < dateOfTransition;

                answer = eventIsBeforeRaceGroupTransition ? participantItem.RaceGroupBeforeTransition : participantItem.RaceGroupAfterTransition;

                return answer;
            }

            answer = participantItem.RaceGroupBeforeTransition;
        }
        else
        {
            answer = participantItem.RaceGroupBeforeTransition;
        }

        return answer;
    }

    private string GetTextItemFromArrayByIndexOrStringEmpty(string[] arrayOfText, int indexOfDataItem)
    {
        var textItem = JghArrayHelpers.SelectItemFromArrayByArrayIndex(arrayOfText, indexOfDataItem);

        return JghString.TmLr(textItem ?? string.Empty);
    }

    private AgeGroupSpecificationItem[] GetAgeGroupSpecificationItems(SeriesProfileItem seriesMetaDataItem)
    {
        var arrayOfAgeGroupSpecificationItem = seriesMetaDataItem.DefaultEventSettingsForAllEvents.AgeGroupSpecificationItems;

        return arrayOfAgeGroupSpecificationItem;
    }

    private static bool TryConvertTextToTimespan(string purportedTimeSpanAsText, out TimeSpan answer, out string conversionReport)
    {
        static bool TryGetFrontComponentAsInteger(string[] subStrings, out int firstValue)
        {
            if (!subStrings.Any())
            {
                firstValue = 0;
                return true;
            }

            if (int.TryParse(subStrings[0], out firstValue))
                return true;

            firstValue = 0;
            return false;
        }

        var failure = "Conversion anomaly.";
        const string locus = "[TryConvertTextToTimespan]";

        conversionReport = string.Empty;

        var defaultConversionReport = $"{failure} Purported timespan is [{purportedTimeSpanAsText}] whereas [d.hh:mm:ss.ff] is required.";

        int days = 0, hours = 0, minutes = 0, seconds = 0, decimalSeconds = 0;

        try
        {
            // Examples:
            //12:1:07:23.47
            // 0:1:07:23.47
            //   1:07:23.47
            //   0:07:23.47
            //      7:23.47
            //      0:23.47
            //        23.47
            //         0.47
            //          .47

            #region null and format checks

            if (string.IsNullOrWhiteSpace(purportedTimeSpanAsText))
            {
                answer = TimeSpan.MinValue;

                conversionReport = $"{failure} The text is null or whitespace.";

                return false;
            }

            #endregion

            #region get the fractional seconds  - special case

            string[] components;

            if (purportedTimeSpanAsText.Contains('.')) // there is a fractional seconds component
            {
                var oneOrTwoComponents = purportedTimeSpanAsText.Split('.').Reverse().ToArray();

                if (!TryGetFrontComponentAsInteger(oneOrTwoComponents, out decimalSeconds))
                {
                    conversionReport = defaultConversionReport + " Fractional seconds look wrong.";
                    decimalSeconds = 0;
                }

                components = purportedTimeSpanAsText.Split(':', '.').Reverse().Skip(1).ToArray();
            }
            else
            {
                components = purportedTimeSpanAsText.Split(':').Reverse().ToArray();
            }

            #endregion

            #region get the rest of the time span components

            for (var i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out seconds))
                        {
                            conversionReport = defaultConversionReport + " Seconds look wrong.";
                            seconds = 0;
                        }

                        break;
                    }
                    case 1:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out minutes))
                        {
                            conversionReport = defaultConversionReport + " Minutes look wrong.";
                            minutes = 0;
                        }

                        break;
                    }
                    case 2:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out hours))
                        {
                            conversionReport = defaultConversionReport + " Hours look wrong.";
                            hours = 0;
                        }

                        break;
                    }
                    case 3:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out days))
                        {
                            conversionReport = defaultConversionReport + " Days look wrong.";
                            days = 0;
                        }

                        break;
                    }
                }

                components = components.Skip(1).ToArray();
            }

            #endregion

            double.TryParse($"0.{decimalSeconds}", out var fractionalSeconds); // the special case

            try
            {
                answer = new TimeSpan(days, hours, minutes, seconds) + TimeSpan.FromSeconds(fractionalSeconds);
            }
            catch (Exception)
            {
                answer = TimeSpan.MinValue;

                conversionReport = $"{failure} One or more of the number of hours, minutes, seconds, and/or fractional seconds exceeds the logical maximum. (23,59, 59, and .999)";

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }
    }

    private static string WriteOneLineReport(int index, ResultItem resultItem, string inputDuration)
    {
        string answer;

        if (string.IsNullOrWhiteSpace(inputDuration))
            answer = $"{index,-3}  {resultItem.Bib,-3}  {resultItem.FirstName,-15} {resultItem.LastName,-15}  {resultItem.T01,-15}  {resultItem.DnxString,-3}";
        else
            answer = $"{index,-3}  {resultItem.Bib,-3}  {resultItem.FirstName,-15} {resultItem.LastName,-15}  {resultItem.T01,-15}  {resultItem.DnxString,-3}  ({inputDuration,-15})";

        return answer;
    }

    #endregion
}