using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
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

    public static List<ResultItem> GenerateResultItemArrayFromMyLapsFile(MyLapsFileItem myLapsFileItem, Dictionary<string, ParticipantHubItem> dictionaryOfParticipants, 
        AgeGroupSpecificationItem[] ageGroupSpecificationItems, DateTime dateOfThisEvent, JghStringBuilder conversionReportSb, int lhsWidth)
    {
        #region declarations

        var i = 0;

        List<ResultItem> answerAsResultItems = [];

        #endregion

        #region try parse file contents as single XElement container

        var fileContentsAsXElement = XElement.Parse(myLapsFileItem.FileContents); // will blow if parsing fails

        var repeatingXElementsInFileContents = fileContentsAsXElement.Elements().ToArray(); // Note: this returns the first level children i.e. all the individual MyLaps line items

       if (!repeatingXElementsInFileContents.Any())
            return []; // bale if we don't have any data rows

        #endregion

        #region deserialise all the repeating child XElements in the parent XElement

        conversionReportSb.AppendLine($"{JghString.LeftAlign("MyLaps line-item XElements extracted", lhsWidth)} : {repeatingXElementsInFileContents.Length}");

        conversionReportSb.AppendLine("Processing repeating child XElements in XML file...");

        var deserialiser = new JghFreeFormDeserialiser(NewKeyMyLapsNamePairs); // use our custom mapper for xml

        foreach (var thisRepeatingXElement in repeatingXElementsInFileContents)
        {
            #region deserialise

            deserialiser.Deserialise(thisRepeatingXElement);

            var myLapsAsResultDto = new ResultDto
            {
                Bib = JghString.TmLr(deserialiser.GetFirstOrBlankAsString(ResultDto.XeBib)),
                Last = JghString.TmLr(deserialiser.GetFirstOrBlankAsString(ResultDto.XeFullName)),
                Sex = JghString.TmLr(deserialiser.GetFirstOrBlankAsString(ResultDto.XeSex)),
                RaceGroup = JghString.TmLr(deserialiser.GetFirstOrBlankAsString(ResultDto.XeRace)),
                Age = deserialiser.GetFirstOrDefaultAsInt(ResultDto.XeAge)
            };

            if (string.IsNullOrWhiteSpace(myLapsAsResultDto.Bib)) continue; //skip iff can't see a bib number in this row of .csv

            #endregion

            #region first things first, figure out if TO1 or Dnx

            string bestGuessDuration;

            var bestGuessDnx = string.Empty;

            var bestGuessComment = string.Empty;

            var durationAsPossiblyNastyString = deserialiser.GetFirstOrBlankAsString(ResultDto.XeT01);

            var mustSkipThisRowBecauseGunTimeIsInValid = false; // initial default

            if (TryConvertTextToTimespan(durationAsPossiblyNastyString, out var calculatedDuration, out var conversionReport01))
            {
                bestGuessDuration = calculatedDuration.ToString("G");
                bestGuessDnx = string.Empty;
            }
            else if (JghString.TmLr(durationAsPossiblyNastyString).Contains(MyLapsSymbolForDnf))
            {
                bestGuessDuration = string.Empty;
                bestGuessDnx = Symbols.SymbolDnf;
            }
            else
            {
                bestGuessDuration = string.Empty;
                bestGuessComment = $"<{MyLapsGunTime}> is invalid. {conversionReport01}";
                mustSkipThisRowBecauseGunTimeIsInValid = true;
            }

            #endregion

            #region if we succeed in seeing a bib number, try find the matching participant in the Rezultz Portal master list that was uploaded a moment ago in the publishing sequence from the portal by the user (having been generated from the hub)

            ParticipantHubItem participantHubItem = null;

            var participantIsDiscovered = false;

            if (dictionaryOfParticipants is not null)
                participantIsDiscovered = dictionaryOfParticipants.TryGetValue(myLapsAsResultDto.Bib, out participantHubItem);

            #endregion

            #region new up a ResultItem depending on whether or not the matching bib number is found

            ResultItem computedResultItemForThisRow;

            if (participantIsDiscovered && participantHubItem is not null)
            {
                computedResultItemForThisRow = new ResultItem
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
                    DnxString = bestGuessDnx,
                    T01 = bestGuessDuration,
                    Comment = bestGuessComment
                };
            }
            else
            {
                myLapsAsResultDto.DnxString = bestGuessDnx;
                myLapsAsResultDto.T01 = bestGuessDuration;
                myLapsAsResultDto.Comment = bestGuessComment;

                computedResultItemForThisRow = ResultItem.FromDataTransferObject(myLapsAsResultDto);

                if (dictionaryOfParticipants is not null)
                    conversionReportSb.AppendLine(
                        $"Warning! Participant master list fails to have a Bib number for <{computedResultItemForThisRow.Bib} {computedResultItemForThisRow.LastName} {computedResultItemForThisRow.RaceGroup}>");
            }

            #endregion

            #region figure out the RaceGroup

            // Note: the following is a bit of a hack. we are using the RaceGroup field to store the RaceGroup value from the MyLaps row.
            // Only if it is empty do we fall back on the RaceGroup field in the hub.

            var myLapsRaceGroup = myLapsAsResultDto.RaceGroup;

            computedResultItemForThisRow.RaceGroup = string.IsNullOrWhiteSpace(myLapsRaceGroup)
                ? FigureOutRaceGroup(ParticipantHubItem.ToDataTransferObject(participantHubItem), dateOfThisEvent)
                : myLapsRaceGroup;

            #endregion

            #region around we go

            i += 1;

            conversionReportSb.AppendLine(WriteOneLineReport(i, computedResultItemForThisRow, durationAsPossiblyNastyString));

            if (!mustSkipThisRowBecauseGunTimeIsInValid)
                answerAsResultItems.Add(computedResultItemForThisRow);

            #endregion
        }

        #endregion

        return answerAsResultItems;
    }

    #endregion

    #region NewKey/SourceElementName pairs for xelements in xml files originating from Jgh Access worksheet and before that from MyLaps Excel spreadsheets from Andrew

    private const string MyLapsBib = "Bib_x0023_"; // the repeating element of the array
    private const string MyLapsGunTime = "Gun_x0020_Time";
    private const string MyLapsFullName = "Athlete";
    private const string MyLapsGender = "Gender";
    private const string MyLapsAge = "Age";
    private const string MyLapsRaceGroup = "Race";
    private const string MyLapsSymbolForDnf = "DNF"; // not a name. a value

    private static KeyValuePair<string, string>[] NewKeyMyLapsNamePairs =>
    [
        new KeyValuePair<string, string>(ResultDto.XeBib, MyLapsBib),
        new KeyValuePair<string, string>(ResultDto.XeT01, MyLapsGunTime),
        new KeyValuePair<string, string>(ResultDto.XeFullName, MyLapsFullName),
        new KeyValuePair<string, string>(ResultDto.XeSex, MyLapsGender),
        new KeyValuePair<string, string>(ResultDto.XeAge, MyLapsAge),
        new KeyValuePair<string, string>(ResultDto.XeRace, MyLapsRaceGroup)
    ];


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
        var answer = string.IsNullOrWhiteSpace(inputDuration)
            ? $"{index,3} Bib: {resultItem.Bib,3} {$"{resultItem.FirstName} {resultItem.LastName}",-25} {resultItem.T01,-17} {resultItem.DnxString,3}"
            : $"{index,3} Bib: {resultItem.Bib,3} {$"{resultItem.FirstName} {resultItem.LastName}",-25} {resultItem.T01,-17} {resultItem.DnxString,3} Source value: {inputDuration,-17}";

        return answer;
    }

    #endregion
}