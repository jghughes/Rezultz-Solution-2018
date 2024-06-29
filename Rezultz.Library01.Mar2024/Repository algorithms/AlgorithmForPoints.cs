using System;
using System.Collections.Generic;
using System.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library01.Mar2024.Repository_algorithms;

public static class AlgorithmForPoints
{
    private const string Locus2 = nameof(AlgorithmForPoints);
    private const string Locus3 = "[Rezultz.Library01.Mar2024]";


    #region methods

    public static ResultItem[] InsertPoints(EventProfileItem eventProfileToWhichThisRepositoryBelongs, ResultItem[] finisherResults, ResultItem[] allComputedResultsDnxIncluded)
    {
        const string failure = "Populating calculating points for individuals in this event.";
        const string locus = "[InsertPoints]";

        try
        {
            #region get ready

            if (eventProfileToWhichThisRepositoryBelongs is null)
                throw new JghNullObjectInstanceException(nameof(eventProfileToWhichThisRepositoryBelongs));

            if (finisherResults is null)
                throw new JghNullObjectInstanceException(nameof(finisherResults));

            if (allComputedResultsDnxIncluded is null)
                throw new JghNullObjectInstanceException(nameof(allComputedResultsDnxIncluded));

            #endregion

            var dictionaryOfPointsForFinishers = GenerateDictionaryOfPointsForAllFinishers(finisherResults, eventProfileToWhichThisRepositoryBelongs);

            foreach (var result in allComputedResultsDnxIncluded.Where(z => z is not null)
                         .Where(z => z.DerivedData is not null))
                result.DerivedData.PointsCalculated = dictionaryOfPointsForFinishers.TryGetValue(result.Identifier, out var pointsForFinisher)
                    ? pointsForFinisher
                    : 0;

            return allComputedResultsDnxIncluded;
        }

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public static bool JekyllIsMoreSeniorAtTheEndOfTheKelsoSeriesThanTheBeginning(SeriesProfileItem seriesProfileToWhichThisRepositoryBelongs, SequenceContainerItem mostRecentEvent, SequenceContainerItem earliestEvent)
    {
        var becameMoreSenior = ObtainSeniorityRankForPointsTransfer(mostRecentEvent.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup, seriesProfileToWhichThisRepositoryBelongs)
                               < ObtainSeniorityRankForPointsTransfer(earliestEvent.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup, seriesProfileToWhichThisRepositoryBelongs);

        return becameMoreSenior;
    }

    #endregion

    #region helpers

    private static Dictionary<Tuple<string, string, string, string>, double> GenerateDictionaryOfPointsForAllFinishers(ResultItem[] allFinisherResults, EventProfileItem eventProfileToWhichThisRepositoryBelongs)
    {
        #region get ready

        var dictionaryOfCalculatedPointsForAllFinishers =
            new Dictionary<Tuple<string, string, string, string>, double>();

        if (allFinisherResults is null)
            return dictionaryOfCalculatedPointsForAllFinishers;

        if (eventProfileToWhichThisRepositoryBelongs is null)
            return dictionaryOfCalculatedPointsForAllFinishers;

        if (eventProfileToWhichThisRepositoryBelongs.EventSettingsItem is null)
            return dictionaryOfCalculatedPointsForAllFinishers;

        var algorithmForPointsEnum =
            eventProfileToWhichThisRepositoryBelongs.EventSettingsItem.AlgorithmForCalculatingPointsEnumString;

        if (string.IsNullOrWhiteSpace(algorithmForPointsEnum))
            return dictionaryOfCalculatedPointsForAllFinishers;

        #endregion

        /*  nothing required to be calculated or recalculated at the collection level
         *  we have all the metrics we need in the DerivedData for each finisher.
         */
        foreach (var individualResult in allFinisherResults
                     .Where(z => z is not null)
                     .Where(z => z.DerivedData is not null)
                     .Where(z => z.DerivedData.IsValidDuration = true))
        {
            double trophyPointsForThisRace = 0;

            Dictionary<int, double> pointsScaleAsDictionary = new();

            var raceSpecificationForThisRace = eventProfileToWhichThisRepositoryBelongs.EventSettingsItem.RaceSpecificationItems?
                .FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Label, individualResult.RaceGroup));

            if (raceSpecificationForThisRace is not null)
                switch (algorithmForPointsEnum)
                {
                    case EnumStringsForSeriesProfile.ProportionalToSpeed:
                    case EnumStringsForSeriesProfile.SimpleCountDownFromTrophyPoints:
                        trophyPointsForThisRace = raceSpecificationForThisRace.TrophyPoints;
                        break;
                    case EnumStringsForSeriesProfile.PointsScaleForEachRace:
                    {
                        var pointsScaleForThisRaceAsStringArray = raceSpecificationForThisRace.PointsScaleAsCsv.Split(',');

                        var i = 1;

                        foreach (var pointsValueAsString in pointsScaleForThisRaceAsStringArray)
                        {
                            if (!JghConvert.TryConvertToDouble(pointsValueAsString, out var pointsValueAsDouble,
                                    out _)) continue;

                            pointsScaleAsDictionary.Add(i, pointsValueAsDouble);

                            i += 1;
                        }

                        break;
                    }
                }


            dictionaryOfCalculatedPointsForAllFinishers[individualResult.Identifier] =
                CalculatePointsForThisCompetitor(individualResult, algorithmForPointsEnum, trophyPointsForThisRace, pointsScaleAsDictionary, eventProfileToWhichThisRepositoryBelongs);
        }

        return dictionaryOfCalculatedPointsForAllFinishers;
    }

    private static double CalculatePointsForThisCompetitor(ResultItem individualResult, string algorithmForPointsEnum,
        double trophyPointsForThisRace, Dictionary<int, double> pointsScaleForThisRaceAsDictionary, EventProfileItem eventProfileToWhichThisRepositoryBelongs)
    {
        double answer;

        switch (algorithmForPointsEnum)
        {
            case EnumStringsForSeriesProfile.ProportionalToSpeed:
            {
                // mtb

                answer = CalculateRelativeSpeedInSubsetAsDecimalRatio(
                             individualResult.DerivedData.TotalDurationFromAlgorithmInSeconds,
                             individualResult.DerivedData.TimeGapBehindWinnerOfSubsetOfSexWithinRaceInSeconds)
                         * trophyPointsForThisRace;
                break;
            }
            case EnumStringsForSeriesProfile.PointsScaleForEachRace:
            {
                try
                {
                    if (eventProfileToWhichThisRepositoryBelongs.MustRankGendersTogetherForSeriesPoints)
                    {
                        if (eventProfileToWhichThisRepositoryBelongs.MustExcludeNonSeriesParticipantsFromRankingForSeriesPoints)
                            answer = JghDictionaryHelpers.LookUpValueSafely(
                                individualResult.DerivedData.PlaceCalculatedOverallIntExcludingNonSeriesParticipants, pointsScaleForThisRaceAsDictionary);
                        else
                            answer = JghDictionaryHelpers.LookUpValueSafely(
                                individualResult.DerivedData.PlaceCalculatedOverallInt, pointsScaleForThisRaceAsDictionary);
                    }
                    else
                    {
                        if (eventProfileToWhichThisRepositoryBelongs.MustExcludeNonSeriesParticipantsFromRankingForSeriesPoints)
                            answer = JghDictionaryHelpers.LookUpValueSafely(
                                individualResult.DerivedData.CalculatedRankInSubsetOfSexWithinRaceExcludingNonSeriesParticipants, pointsScaleForThisRaceAsDictionary);
                        else
                            answer = JghDictionaryHelpers.LookUpValueSafely(
                                individualResult.DerivedData.CalculatedRankInSubsetOfSexWithinRace, pointsScaleForThisRaceAsDictionary);
                    }
                }
                catch (Exception)
                {
                    return 0;
                }

                break;
            }

            default:
                answer = 0;
                break;
        }




        return answer;
    }

    private static int ObtainSeniorityRankForPointsTransfer(string raceLabel,
        SeriesProfileItem seriesProfileToWhichThisRepositoryBelongs)
    {
        if (string.IsNullOrWhiteSpace(raceLabel) || seriesProfileToWhichThisRepositoryBelongs?.DefaultEventSettingsForAllEvents.RaceSpecificationItems is null)
            return 0;

        var matchingRaceSpec = seriesProfileToWhichThisRepositoryBelongs.DefaultEventSettingsForAllEvents.RaceSpecificationItems
            .FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Label, raceLabel));

        if (matchingRaceSpec is null)
            return 0;

        var answer = matchingRaceSpec.SeniorityRankForPointsTransfer;

        return answer;
    }

    private static double CalculateRelativeSpeedInSubsetAsDecimalRatio(double durationInSeconds,
        double timeGapBehindWinnerInSeconds)
    {
        var winningDurationInSeconds = durationInSeconds - timeGapBehindWinnerInSeconds;

        return JghMath.GuardAgainstDivisionByZero(winningDurationInSeconds, durationInSeconds, 0);
    }

    #endregion
}