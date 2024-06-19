using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repositories;

// ReSharper disable IdentifierTypo

// ReSharper disable InconsistentNaming

namespace RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;

public class MyLaps2024HelperXml
{
    #region primary method

    public static List<ResultItem> GenerateResultItemArrayFromMyLapsFile(MyLapsFile myLapsFile,
        Dictionary<string, ParticipantHubItem> dictionaryOfParticipants, AgeGroupSpecificationItem[] ageGroupSpecificationItems, DateTime dateOfThisEvent,
        JghStringBuilder conversionReportSb, int lhsWidth)
    {
        #region declarations

        var i = 0;

        var durationAsPossiblyNastyString = string.Empty;

        List<ResultItem> answerAsResultItems = [];

        #endregion

        #region try parse file contents as .xml

        var fileContentsAsXElement = XElement.Parse(myLapsFile.FileContents); // will blow if parsing fails

        var repeatingChildElements = fileContentsAsXElement.Elements().ToArray(); // Note: this returns the first level children i.e. all the individual MyLaps line items

        // bale if we don't have any data rows
        if (!repeatingChildElements.Any())
            return answerAsResultItems;

        #endregion

        #region process all the line items in the .xml file

        conversionReportSb.AppendLine($"{JghString.LeftAlign("MyLaps line items extracted", lhsWidth)} : {repeatingChildElements.Length}");

        conversionReportSb.AppendLine("Processing line items one by one.");

        foreach (var thisRepeatingChildElement in repeatingChildElements)
        {
            #region declarations

            ResultItem thisRepeatingResultItem;

            var mustSkipThisRowBecauseGunTimeIsInValid = false; // initial default

            #endregion

            #region skip if can't see a bib number in thisRepeatingChildElement

            var bibOfThisRepeatingXe = thisRepeatingChildElement.Elements(SrcXeBib).FirstOrDefault();

            if (bibOfThisRepeatingXe is null) continue;

            if (string.IsNullOrWhiteSpace(bibOfThisRepeatingXe.Value)) continue;

            #endregion

            #region if can see a bib number, try find the matching participant in the master list that was previously uploaded (having been generated from the hub)

            var participantIsFound = dictionaryOfParticipants.TryGetValue(bibOfThisRepeatingXe.Value, out var participantHubItem);

            #endregion

            #region new up a ResultItem depending on whether or not the matching bib number is found

            if (participantIsFound)
            {
                thisRepeatingResultItem = new ResultItem
                {
                    Bib = participantHubItem.Bib,
                    Rfid = participantHubItem.Rfid,
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
                thisRepeatingResultItem = new ResultItem
                {
                    Bib = bibOfThisRepeatingXe.Value,
                    LastName = GetTmlrValueOfXElementOrStringEmpty(SrcXeFullName, thisRepeatingChildElement),
                    Gender = GetTmlrValueOfXElementOrStringEmpty(SrcXeGender, thisRepeatingChildElement),
                    Age = JghConvert.ToInt32(GetTmlrValueOfXElementOrStringEmpty(SrcXeAge, thisRepeatingChildElement)),
                    RaceGroup = GetTmlrValueOfXElementOrStringEmpty(SrcXeRaceGroup, thisRepeatingChildElement),
                    IsSeries = false // no option but to assume this. play safe
                };

                conversionReportSb.AppendLine(
                    $"Warning! Participant database on the hub fails to contain a Bib number for <{thisRepeatingResultItem.Bib} {thisRepeatingResultItem.LastName} {thisRepeatingResultItem.RaceGroup}>.");
            }

            #endregion

            #region figure out TO1 (Gun Time) and Dnx (Overall)

            var possibleDnx = GetTmlrValueOfXElementOrStringEmpty(SrcXeOverall, thisRepeatingChildElement); //in MyLaps, DNF is in the column "Overall"

            if (possibleDnx == JghString.TmLr(SrcValueDnf))
            {
                thisRepeatingResultItem.T01 = string.Empty;
                thisRepeatingResultItem.DnxString = Symbols.SymbolDnf;
            }
            else
            {
                durationAsPossiblyNastyString = GetTmlrValueOfXElementOrStringEmpty(SrcXeGunTime, thisRepeatingChildElement);

                if (TryConvertTextToTimespan(durationAsPossiblyNastyString, out var myTimeSpan, out var conversionReport01))
                {
                    thisRepeatingResultItem.T01 = myTimeSpan.ToString("G");
                    thisRepeatingResultItem.DnxString = string.Empty;
                }
                else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dnf"))
                {
                    thisRepeatingResultItem.T01 = string.Empty;
                    thisRepeatingResultItem.DnxString = Symbols.SymbolDnf;
                }
                else
                {
                    thisRepeatingResultItem.T01 = $"<{SrcXeGunTime}> is invalid. {conversionReport01}";
                    mustSkipThisRowBecauseGunTimeIsInValid = true;
                }
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

    #region xelements in xml files originating from Access and before that from MyLaps Excel spreadsheets from Andrew

    private const string SrcXeBib = "Bib_x0023_"; // the repeating element of the array
    private const string SrcXeGunTime = "Gun_x0020_Time";
    private const string SrcXeOverall = "Overall";
    private const string SrcXeFullName = "Athlete";
    private const string SrcXeGender = "Gender";
    private const string SrcXeAge = "Age";
    private const string SrcXeRaceGroup = "Race";

    private const string SrcValueDnf = "DNF"; // not a name. a value

    #endregion

    #region helpers

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

    public static string GetTmlrValueOfXElementOrStringEmpty(string name, XElement xE)
    {
        var textItem = xE.Elements(name).FirstOrDefault();

        return JghString.TmLr(textItem?.Value ?? string.Empty);
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

        var failure = "Conversion failure.";
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