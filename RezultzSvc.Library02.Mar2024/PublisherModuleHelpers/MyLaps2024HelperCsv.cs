using System;
using System.Collections.Generic;
using System.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repositories;

namespace RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;

public class MyLaps2024HelperCsv
{

    #region primary method

    public static List<ResultItem> GenerateResultItemArrayFromMyLapsFile(MyLapsFile myLapsFile,
        Dictionary<string, ParticipantHubItem> dictionaryOfParticipants, AgeGroupSpecificationItem[] ageGroupSpecificationItems, DateTime dateOfThisEvent,
        JghStringBuilder conversionReportSb, int lhsWidth)
    {
        #region declarations

        var i = 0;

        List<ResultItem> answerAsResultItems = [];

        #endregion

        #region step 1 interpret ObtainedDatasetToBeProcessedAsRawString as csv data if we can

        var allRowsOfCsvText = myLapsFile.FileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList(); // remove carriage returns and line breaks

        var relevantRowsOfCsvText =
            allRowsOfCsvText.Where(z => !string.IsNullOrWhiteSpace(z)).Where(z => z.Contains(','))
                .ToList(); // eliminate blank lines and lines that are non-data lines in the MyLaps files - for starters there is a pair of blank lines at the bottom of the file

        List<string> relevantRowsWithoutEscapeLiterals = new();

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

        if (arrayOfColumnHeadings == null) throw new JghAlertMessageException("Unable to find any column headings in the provided MyLaps data. It is therefore impossible to interpret the data.");

        // example of headings from MyLaps : "Pos","Bib#","Athlete","Finish Time","Race","Age","Gender"
        var indexOfBibColumn = Array.IndexOf(arrayOfColumnHeadings, SrcXeBib);
        var indexOfTotalDuration = Array.IndexOf(arrayOfColumnHeadings, SrcXeGunTime);
        var indexOfFullName = Array.IndexOf(arrayOfColumnHeadings, SrcXeFullName);
        var indexOfRaceGroup = Array.IndexOf(arrayOfColumnHeadings, SrcXeRaceGroup);
        var indexOfAge = Array.IndexOf(arrayOfColumnHeadings, SrcXeAge);
        var indexOfGender = Array.IndexOf(arrayOfColumnHeadings, SrcXeGender);

        if (indexOfBibColumn == -1 && indexOfTotalDuration == -1 && indexOfFullName == -1 && indexOfRaceGroup == -1 && indexOfAge == -1 && indexOfGender == -1)
            throw new JghAlertMessageException("Unable to find any of the specific column headings which this conversion module absolutely requires to interpret the MyLaps data.");

        #endregion

        #region step 3 process all the subsequent rows (which should be legit rows of data by now)

        var rowsOfData = relevantRowsWithoutEscapeLiterals.Skip(NumberOfRowsPrecedingRowOfColumnHeadings + 1).ToList(); // skip first line i.e. the header row we just processed in Step 2

        conversionReportSb.AppendLine("Processing rows of results in CSV file...");

        if (!rowsOfData.Any())
            // bale if we don't have any data rows
            throw new JghAlertMessageException("Unable to extract any rows of csv in the provided MyLaps file.");


        foreach (var row in rowsOfData)
        {
            var arrayOfDataInRow = row.Split(',');

            #region skip if can't see a bib number in this row of .csv

            var bibOfThiRepeatingRow = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfBibColumn);

            if (string.IsNullOrWhiteSpace(bibOfThiRepeatingRow)) continue;

            #endregion

            #region if can see a bib number in the .csv, try find the matching participant in the master list that was previously uploaded (having been generated from the hub)

            var participantIsFound = dictionaryOfParticipants.TryGetValue(bibOfThiRepeatingRow, out var participantHubItem);

            #endregion

            #region new up a ResultItem depending on whether or not the matching bib number is found

            ResultItem thisRepeatingResultItem;

            if (participantIsFound)
            {
                thisRepeatingResultItem = new ResultItem
                {
                    Bib = participantHubItem.Bib,
                    FirstName = participantHubItem.FirstName,
                    LastName = participantHubItem.LastName,
                    MiddleInitial = participantHubItem.MiddleInitial,
                    Gender = participantHubItem.Gender,
                    Age = ParticipantDatabase.ToAgeFromBirthYear(participantHubItem.BirthYear),
                    AgeGroup = ParticipantDatabase.ToAgeCategoryDescriptionFromBirthYear(participantHubItem.BirthYear, ageGroupSpecificationItems),
                    City = participantHubItem.City,
                    Team = participantHubItem.Team,
                    IsSeries = participantHubItem.IsSeries,
                    RaceGroup = FigureOutRaceGroup(ParticipantHubItem.ToDataTransferObject(participantHubItem), dateOfThisEvent)
                };

            }
            else
            {
                // "Pos","Bib#","Athlete","Finish Time","Race","Age","Gender"

                thisRepeatingResultItem = new ResultItem
                {
                    Bib = bibOfThiRepeatingRow,
                    LastName = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfFullName),
                    Gender = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfGender),
                    Age = JghConvert.ToInt32(GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfAge)),
                    RaceGroup = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfRaceGroup),
                    IsSeries = false
                };

                conversionReportSb.AppendLine(
                    $"Warning! Participant master list fails to have a Bib number for <{thisRepeatingResultItem.Bib} {thisRepeatingResultItem.LastName} {thisRepeatingResultItem.RaceGroup}>");
            }

            #endregion

            #region figure out if TO1 or Dnx 

            var mustSkipThisRowBecauseGunTimeIsInValid = false; // initial default

            var durationAsPossiblyNastyString = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfTotalDuration);

            if (TryConvertTextToTimespan(durationAsPossiblyNastyString, out var myTimeSpan, out var conversionReport01))
            {
                thisRepeatingResultItem.T01 = myTimeSpan.ToString("G");
                thisRepeatingResultItem.DnxString = string.Empty;
            }
            else if (JghString.TmLr(durationAsPossiblyNastyString).Contains(SrcValueDnf))
            {
                thisRepeatingResultItem.T01 = string.Empty;
                thisRepeatingResultItem.DnxString = Symbols.SymbolDnf;
            }
            else
            {
                thisRepeatingResultItem.T01 = $"<{SrcXeGunTime}> is invalid. {conversionReport01}";
                mustSkipThisRowBecauseGunTimeIsInValid = true;
            }

            #endregion

            #region around we go

            i += 1;

            conversionReportSb.AppendLine(WriteOneLineReport(i, thisRepeatingResultItem, durationAsPossiblyNastyString));

            if (!mustSkipThisRowBecauseGunTimeIsInValid)
                answerAsResultItems.Add(thisRepeatingResultItem);

            #endregion

        }

        #endregion

        return answerAsResultItems;
    }

    #endregion

    #region headings in .csv columns

    private const string SrcXeBib = "Bib#"; // the repeating element of the array
    private const string SrcXeGunTime = "Finish Time";
    //private const string SrcXeOverall = "Overall";
    private const string SrcXeFullName = "Athlete";
    private const string SrcXeGender = "Gender";
    private const string SrcXeAge = "Age";
    private const string SrcXeRaceGroup = "Race";

    private const string SrcValueDnf = "dnf"; // not a name. a value

    #endregion

    #region settings

    private const int NumberOfRowsPrecedingRowOfColumnHeadings = 0;
    // Note: The value of this constant of 0 is normal for csv files exported manually by Jgh from the Excel exported from MyLaps.
    // It is 1 for csv files exported directly from MyLaps. They have some sort of title row before the field names row.

    #endregion

    #region helpers

    private static string GetTextItemFromArrayByIndexOrStringEmpty(string[] arrayOfText, int indexOfDataItem)
    {
        var textItem = JghArrayHelpers.SelectItemFromArrayByArrayIndex(arrayOfText, indexOfDataItem);

        return JghString.TmLr(textItem ?? string.Empty);
    }

    public static string FigureOutRaceGroup(ParticipantHubItemDto participantItem, DateTime dateOfEvent)
    {
        var isTransitionalParticipant = participantItem.RaceGroupBeforeTransition != participantItem.RaceGroupAfterTransition;

        string answerAsRaceGroup;

        if (isTransitionalParticipant)
        {
            if (DateTime.TryParse(participantItem.DateOfRaceGroupTransitionAsString, out var dateOfTransition))
            {
                var eventIsBeforeRaceGroupTransition = dateOfEvent < dateOfTransition;

                answerAsRaceGroup = eventIsBeforeRaceGroupTransition ? participantItem.RaceGroupBeforeTransition : participantItem.RaceGroupAfterTransition;

                return answerAsRaceGroup;
            }

            answerAsRaceGroup = participantItem.RaceGroupBeforeTransition;
        }
        else
        {
            answerAsRaceGroup = participantItem.RaceGroupBeforeTransition;
        }

        return answerAsRaceGroup;
    }

    public static bool TryConvertTextToTimespan(string purportedTimeSpanAsText, out TimeSpan answer, out string conversionReport)
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

    public static string WriteOneLineReport(int index, ResultItem resultItem, string inputDuration)
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