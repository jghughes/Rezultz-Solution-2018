using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library01.Mar2024.Repository_algorithms
{
    public static class AlgorithmForAverageSplitTimes
    {
	    private const string Locus2 = nameof(AlgorithmForAverageSplitTimes);
	    private const string Locus3 = "[Rezultz.Library01.Mar2024]";


        public static ResultItem[] GeneratePseudoAverageSplitTimes(ResultItem[] allPlacedResults,
            EventProfileItem eventProfileToWhichThisRepositoryBelongs)
        {
            const string failure = "Calculating average split times for individuals in this event.";
            const string locus = "[InsertAverageSplitTimes]";

            try
            {
                #region get ready

                if (eventProfileToWhichThisRepositoryBelongs is null)
                    return allPlacedResults;


                if (allPlacedResults is null)
                    throw new JghNullObjectInstanceException(nameof(allPlacedResults));

                #endregion

                var pseudoSingleSplitResults = new List<ResultItem>();

                foreach (var finisher in allPlacedResults.Where(z => z is not null).Where(z => z.DerivedData is not null))
                {
                    if (!finisher.DerivedData.IsValidDuration) continue; // belt and braces

                    #region erect a pseudo split

                    var pseudoSplitResult = finisher.ShallowMemberwiseCloneCopyExcludingDerivedData;

                    ZeroiseAllTx(pseudoSplitResult);

                    // clever trick. shift Race field to UtilityClassification

                    pseudoSplitResult.UtilityClassification = finisher.RaceGroup; 

                    ////NB add 1 to all the RaceIDs to make doubly sure none of them = 0. this is a requirement

                    pseudoSplitResult.UtilityClassification = finisher.RaceGroup;

                    // neutralise Race field - temporary measure (reinstated after this method returns)

                    pseudoSplitResult.RaceGroup = "n/a"; // NB

                    //pseudoSplitResult.RaceID = 1; // NB

                    #endregion

                    #region calculate av split time in seconds

                    double averageSplitTimeSec;

                    if (finisher.DerivedData.CalculatedNumOfSplitsCompleted == 0)
                    {
                        averageSplitTimeSec = 0;
                    }
                    else
                    {
                        var matchingRaceSpec = eventProfileToWhichThisRepositoryBelongs.EventSettingsItem.RaceSpecificationItems
                         .FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Label, finisher.RaceGroup));

                        double normalisationFactor;


                        if (matchingRaceSpec is null)
                        {
                            normalisationFactor = 1.0;
                        }
                        else
                        {
                            normalisationFactor =
                                matchingRaceSpec.MultiplicationFactorForNormalisationOfAverageSplitDuration;
                        }

                        if (normalisationFactor == 0)
                            normalisationFactor = 1.0; // default. fallback

                        var normalisedTotalTimeSec = finisher.DerivedData.TotalDurationFromAlgorithmInSeconds
                                                * normalisationFactor;
                        
                        averageSplitTimeSec = normalisedTotalTimeSec
                                            / Convert.ToDouble(finisher.DerivedData.CalculatedNumOfSplitsCompleted);
                    }

                    #endregion

                    pseudoSplitResult.T01 = TimeSpan.FromSeconds(averageSplitTimeSec).ToString(JghTimeSpan.DdhhmmssfPattern);

                    pseudoSingleSplitResults.Add(pseudoSplitResult);
                }

                return pseudoSingleSplitResults.ToArray();
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private static void ZeroiseAllTx(ResultItem pseudoSplit)
        {
	        pseudoSplit.T01 = string.Empty;
	        pseudoSplit.T02 = string.Empty;
	        pseudoSplit.T03 = string.Empty;
	        pseudoSplit.T04 = string.Empty;
	        pseudoSplit.T05 = string.Empty;
	        pseudoSplit.T06 = string.Empty;
	        pseudoSplit.T07 = string.Empty;
	        pseudoSplit.T08 = string.Empty;
	        pseudoSplit.T09 = string.Empty;
	        pseudoSplit.T10 = string.Empty;
	        pseudoSplit.T11 = string.Empty;
	        pseudoSplit.T12 = string.Empty;
	        pseudoSplit.T13 = string.Empty;
	        pseudoSplit.T14 = string.Empty;
	        pseudoSplit.T15 = string.Empty;
        }

    }
}