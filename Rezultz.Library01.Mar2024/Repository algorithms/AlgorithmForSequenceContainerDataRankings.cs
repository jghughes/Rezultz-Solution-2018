using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace Rezultz.Library01.Mar2024.Repository_algorithms
{

    /// <summary>
    /// be sure that whenever you use this method, and it's helpers, the seriesStandingLineItems have already been allocated a unique ID, i.e. a Guid.NewGuid().ToString
    /// </summary>
    public static class AlgorithmForSequenceContainerDataRankings
    {
	    private const string Locus2 = nameof(AlgorithmForSequenceContainerDataRankings);
	    private const string Locus3 = "[Rezultz.Library01.Mar2024]";

        public static async Task<SequenceContainerItem[]> DoRankingsOfTotalsForAllIndividualsRelativeToTheirCompetitorsAsync(
            bool seriesTotalIsOrderedByDescending, SequenceContainerItem[] seriesStandingLineItems)
        {
            const string failure = "Adding to repository. Calculating rankings.";

            const string locus = "[DoRankingsOfTotalsForAllIndividualsRelativeToTheirCompetitorsAsync]";

            try
            {
                if (seriesStandingLineItems is null)
                    throw new JghNullObjectInstanceException(nameof(seriesStandingLineItems));


                var seriesStandingsList = seriesStandingLineItems.Where(z => z is not null).ToArray();

                var answer = await PopulatePlacingInSubsetsOfRaceAndSexAndAgeGroupCombinedAsync(
                    seriesTotalIsOrderedByDescending,
                    await PopulatePlacingsInSubsetsOfRaceAndSexCombinedAsync(seriesTotalIsOrderedByDescending,
                        await PopulatePlacingsInRaceAsync(seriesTotalIsOrderedByDescending, seriesStandingsList)));

                return answer;
            }

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private static async Task<SequenceContainerItem[]> PopulatePlacingsInRaceAsync(bool seriesTotalIsOrderedByDescending,
            SequenceContainerItem[] items)
        {
            const string failure = "Adding to database. Calculating points rankings by race.";

            const string locus = "[PopulatePlacingsInRaceAsync]";

            if (items is null)
                throw new JghNullObjectInstanceException(nameof(items));

            var lineItems = items;

            if (!lineItems.Any())
                return [];

            try
            {
                var dictionaryOfAllLineItems = lineItems.Where(z => z is not null).ToArray()
                    .ConvertArrayOfSequenceContainersToDictionaryKeyedOnUniqueId();

                // in this case grouping is by raceCODE: because it's a simple key 
                // be sure to use a LINQ declaration like this not a method chain 
                // to cope with the situation where raceCODE is blank throughout, have to do it this way

                var subGroups = from kvp in dictionaryOfAllLineItems
                    where kvp.Value is not null
                    where kvp.Value.MostRecentResultItemToWhichThisSequenceApplies is not null
                    where !string.IsNullOrWhiteSpace(kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup)
                    let aString = kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup
                    group kvp by aString
                    into myGroup
                    orderby myGroup.Key
                    select myGroup;

                foreach (var subgroup in subGroups)
                {
                    var dictionaryOfLineItemsInSubgroup = subgroup.ToDictionary(x => x.Key, x => x.Value);

                    var scratchPad =
                        await CalculateScratchPadForArbitrarySubsetOfSeasonStandingsTableLineItemsAsync(
                            seriesTotalIsOrderedByDescending, dictionaryOfLineItemsInSubgroup);

                    foreach (var kvp in dictionaryOfLineItemsInSubgroup)
                    {
	                    if (!TryGetScratchPadItemByUniqueItemId(scratchPad, kvp.Key, out var theDiscoveredScratchPadItemValue))
                            continue;

                        // NB paste in data fields into dictionaryOfAllLineItems, not dictionaryOfLineItemsInSubgroup

                        dictionaryOfAllLineItems[kvp.Key].SequenceTotalRankInt =
                            theDiscoveredScratchPadItemValue.RankInSubsetInt;

                        dictionaryOfAllLineItems[kvp.Key].RelativeRankInRaceAsDecimalRatio =
                            theDiscoveredScratchPadItemValue.RelativeRankInSubsetAsDecimalRatio;

                        dictionaryOfAllLineItems[kvp.Key].FractionalRankInRaceInNumeratorOverDenominatorFormat =
                            theDiscoveredScratchPadItemValue.FractionalRankInSubsetAsNumeratorOverDenominator;

                        dictionaryOfAllLineItems[kvp.Key].GapBehindBestInRaceInPrevailingUnitsOfSequence =
                            theDiscoveredScratchPadItemValue.GapBehindBestInSubsetInPrevailingUnitsOfSequence;
                    }
                }

                var answer = JghDictionaryHelpers.ConvertDictionaryToArray(dictionaryOfAllLineItems);

                return answer;

            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private static async Task<SequenceContainerItem[]> PopulatePlacingsInSubsetsOfRaceAndSexCombinedAsync(
            bool seriesTotalIsOrderedByDescending, SequenceContainerItem[] items)
        {
            const string failure =
                "Adding to database. Calculating placings and rankings by Sex and points within each race.";
            const string locus = "[PopulatePlacingsInSubsetsOfRaceAndSexCombinedAsync]";

            if (items is null) throw new JghNullObjectInstanceException(nameof(items));

            var lineItems = items;

            if (!lineItems.Any()) return [];

            try
            {
                var dictionaryOfAllLineItems = lineItems.ConvertArrayOfSequenceContainersToDictionaryKeyedOnUniqueId();

                // in this case grouping is by race and Sex together: because it's a compound key, be sure to use a method chain like this not a linq select statement, otherwise it won't work as expected when all three compnents are empty

                if (dictionaryOfAllLineItems is not null)
                {
                    var subGroups = dictionaryOfAllLineItems
                        .Where(kvp => kvp.Value is not null)
                        .Where(kvp => kvp.Value.MostRecentResultItemToWhichThisSequenceApplies is not null)
                        .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup))
                        .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.Gender))
                        .GroupBy(kvp => new {Race = kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup, Sex = kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.Gender});
                    ////.Where(kvp => !String.IsNullOrWhiteSpace(kvp.Value.Result.SexCODE))
                    //.GroupBy(kvp => new { RaceCODE = kvp.Value.Result.Race, kvp.Value.Result.IsFemale });

                    foreach (var subgroup in subGroups)
                    {
                        var dictionaryOfLineItemsInSubgroup = subgroup.ToDictionary(x => x.Key, x => x.Value);

                        var scratchPad =
                            await CalculateScratchPadForArbitrarySubsetOfSeasonStandingsTableLineItemsAsync(
                                seriesTotalIsOrderedByDescending, dictionaryOfLineItemsInSubgroup);

                        foreach (var kvp in dictionaryOfLineItemsInSubgroup)
                        {
                            if (!TryGetScratchPadItemByUniqueItemId(scratchPad, kvp.Key,
                                    out var theDiscoveredScratchPadItemValue))
                                continue;

                            // NB be sure to paste data fields into dictionaryOfAllLineItems, not dictionaryOfLineItemsInSubgroup

                            dictionaryOfAllLineItems[kvp.Key].FractionalRankBySexInNumeratorOverDenominatorFormat =
                                theDiscoveredScratchPadItemValue.FractionalRankInSubsetAsNumeratorOverDenominator;
                        }
                    }
                }

                var answer = JghDictionaryHelpers.ConvertDictionaryToArray(dictionaryOfAllLineItems);

                return answer;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private static async Task<SequenceContainerItem[]> PopulatePlacingInSubsetsOfRaceAndSexAndAgeGroupCombinedAsync(
            bool seriesTotalIsOrderedByDescending, SequenceContainerItem[] items)
        {
            const string failure = "Adding to database. Calculating rankings by race, Sex and category.";
            const string locus = "[PopulatePlacingInSubsetsOfRaceAndSexAndAgeGroupCombinedAsync]";

            try
            {
                if (items is null)
                    throw new JghNullObjectInstanceException(nameof(items));

                var lineItems = items;

                if (!lineItems.Any())
                    return [];

                var dictionaryOfAllLineItems = lineItems.ConvertArrayOfSequenceContainersToDictionaryKeyedOnUniqueId();

                // in this case grouping is by race and Sex and category together: because it's a compound key, be sure to use a method chain like this not a linq select statement, otherwise it won't work as expected when all three compnents are empty

                if (dictionaryOfAllLineItems is not null)
                {
                    var subGroups = dictionaryOfAllLineItems
                        .Where(kvp => kvp.Value is not null)
                        .Where(kvp => kvp.Value.MostRecentResultItemToWhichThisSequenceApplies is not null)
                        .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup))
                        .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.Gender))
                        .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.AgeGroup))
                        .GroupBy(kvp => new {Race = kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup, Sex = kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.Gender, Category = kvp.Value.MostRecentResultItemToWhichThisSequenceApplies.AgeGroup});
                    //.GroupBy(kvp => new { RaceCODE = kvp.Value.Result.Race, kvp.Value.Result.IsFemale, CategoryCODE = kvp.Value.Result.AgeGroup });

                    foreach (var subgroup in subGroups)
                    {
                        var dictionaryOfLineItemsInSubgroup = subgroup.ToDictionary(x => x.Key, x => x.Value);

                        var scratchPad =
                            await CalculateScratchPadForArbitrarySubsetOfSeasonStandingsTableLineItemsAsync(
                                seriesTotalIsOrderedByDescending, dictionaryOfLineItemsInSubgroup);

                        foreach (var kvp in dictionaryOfLineItemsInSubgroup)
                        {
                            if (!TryGetScratchPadItemByUniqueItemId(scratchPad, kvp.Key,
                                    out var theDiscoveredScratchPadItemValue))
                                continue;

                            // NB be sure to paste data fields into dictionaryOfAllLineItems, not dictionaryOfLineItemsInSubgroup

                            dictionaryOfAllLineItems[kvp.Key]
                                    .FractionalRankBySexPlusCategoryInNumeratorOverDenominatorFormat =
                                theDiscoveredScratchPadItemValue.FractionalRankInSubsetAsNumeratorOverDenominator;
                        }
                    }
                }

                var answer = JghDictionaryHelpers.ConvertDictionaryToArray(dictionaryOfAllLineItems);

                return answer;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private static Task<Dictionary<int, ScratchPadItemForSeriesStandingsLineItem>>
            CalculateScratchPadForArbitrarySubsetOfSeasonStandingsTableLineItemsAsync(
                bool seriesTotalIsOrderedByDescending, Dictionary<int, SequenceContainerItem> lineItems)
        {
            const string failure = "Helper method. Calculating relative placing.";

            const string locus = "[CalculateScratchPadForArbitrarySubsetOfindividualResults]";

            var tcs = new TaskCompletionSource<Dictionary<int, ScratchPadItemForSeriesStandingsLineItem>>(TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                if (lineItems is null) throw new JghNullObjectInstanceException(nameof(lineItems));

                if (!lineItems.Any())
                {
                    tcs.TrySetResult(new());

                    return tcs.Task;
                }

                var answer = new Dictionary<int, ScratchPadItemForSeriesStandingsLineItem>();

                // find the winner and calculate placings off him : for points calculations do OrderByDescending

                var finishers = seriesTotalIsOrderedByDescending
                    ? lineItems.Where(kvp => kvp.Value is not null).OrderByDescending(kvp => kvp.Value.SequenceTotal)
                        .ToArray()
                    : lineItems.Where(kvp => kvp.Value is not null).OrderBy(kvp => kvp.Value.SequenceTotal).ToArray();

                // OK. placing data

                var i = 1;

                if (finishers.Any())
                {
                    var numInSubset = finishers.Count();

                    var winningPerformance = finishers.FirstOrDefault();

                    var winningScore = winningPerformance.Value.SequenceTotal;


                    // ditto
                    foreach (var kvp in finishers)
                    {
                        //if (kvp.Value is null) continue;

                        var scratchPadItem = new ScratchPadItemForSeriesStandingsLineItem(kvp.Value)
                        {
                            RankInSubsetInt = i,
                            FractionalRankInSubsetAsNumeratorOverDenominator = $"{i}/{numInSubset}",
                            GapBehindBestInSubsetInPrevailingUnitsOfSequence = winningScore - kvp.Value.SequenceTotal
                        };

                        answer.Add(kvp.Key, scratchPadItem);

                        i++;
                    }
                }

                tcs.TrySetResult(answer);

                return tcs.Task;
            }
            catch (Exception ex)
            {
                tcs.TrySetException(JghExceptionHelpers.ConvertToCarrier(failure, locus, ex));
                return tcs.Task;
            }
        }

        private static bool TryGetScratchPadItemByUniqueItemId(
            IDictionary<int, ScratchPadItemForSeriesStandingsLineItem> theDictionary, int theLookupKey,
            out ScratchPadItemForSeriesStandingsLineItem theDiscoveredValue)
        {
            theDiscoveredValue = new();

            if (theDictionary is null)
                return false;

            try
            {
                return theDictionary.TryGetValue(theLookupKey, out theDiscoveredValue);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Dictionary<int, SequenceContainerItem> ConvertArrayOfSequenceContainersToDictionaryKeyedOnUniqueId(
	        this SequenceContainerItem[] sequenceItems)
        {
	        const string failure = "Converting SequenceContainerItem[] to type dictionary keyed on object ID.";

	        const string locus = "[ConvertArrayOfSequenceContainersToDictionaryKeyedOnUniqueId]";

	        var arrayOfTypeKelso2013SequenceContainers = sequenceItems.ToArray();


	        var answer = new Dictionary<int, SequenceContainerItem>();

	        try
	        {
		        if (!arrayOfTypeKelso2013SequenceContainers.Any())
			        return new();

		        foreach (var container in arrayOfTypeKelso2013SequenceContainers)
		        {
			        if (container is null)
				        continue;

			        if (answer.ContainsKey(container.ID)) continue;

			        answer.Add(container.ID, container);
		        }
	        }

	        #region try catch handling - the specific exceptions screened here intended to be those from which it is vaguely possible to continue in this particular context, subject to displaying an informative error message and returning a sensible return value

	        catch (Exception ex)
	        {
		        throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
	        }

	        #endregion


	        return answer;
        }

    }
}