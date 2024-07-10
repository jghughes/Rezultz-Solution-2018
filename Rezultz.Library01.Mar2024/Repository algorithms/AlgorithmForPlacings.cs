using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace Rezultz.Library01.Mar2024.Repository_algorithms
{
    public static class AlgorithmForPlacings
	{
		private const string Locus2 = nameof(AlgorithmForPlacings);
		private const string Locus3 = "[Rezultz.Library01.Mar2024]";

		#region methods

		public static async Task<ResultItem[]> PopulateAllPlacingsForThisEventAsync(ResultItem[] results)
		{
			const string failure = "Populating positions and placings of individuals in this event.";
			const string locus = "[PopulateAllPlacingsForThisEventAsync]";

			try
			{
				if (results is null)
					throw new JghNullObjectInstanceException(nameof(results));
                
                int i = 1;

                foreach (var person in results)
                {
                    person.ID = i;
                    i++;
                }

				var aa = await results.PopulatePlacingStringsForSubsetsOfRaceWithinEventAsync();

				var bb = await aa.PopulatePlacingStringsForSubsetsOfSexWithinRaceAsync();

				var cc = await bb.PopulatePlacingStringsForSubsetsOfAgeGroupWithinSexWithinRaceAsync();

				return cc.ToArray();
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

        public static Task<PopulationCohortItem[]> ComposeTableOfCohortHistogramsGroupedByStringAsync(IGrouping<string, SequenceContainerItem>[] groupingsOfPlacedResults)
        {
            const string failure = "Composing population counts, totals and total percentages for histogram.";
            const string locus = "[ComposeTableOfCohortHistogramsGroupedByStringAsync]";

            var tcs = new TaskCompletionSource<PopulationCohortItem[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            if (groupingsOfPlacedResults is null)
            {
                tcs.TrySetResult([]);

                return tcs.Task;
            }

            var answer = new Dictionary<string, PopulationCohortItem>();

            try
            {
                foreach (var group in groupingsOfPlacedResults)
                {
                    if (group is null) continue;

                    var lineItem = new PopulationCohortItem
                    {
                        NameOfCohort = group.Key,
                        SexMaleCount = group.Where(z => z is not null).Count(z => z.MostRecentResultItemToWhichThisSequenceApplies.Gender == Symbols.SymbolMale),
                        SexFemaleCount = group.Where(z => z is not null).Count(z => z.MostRecentResultItemToWhichThisSequenceApplies.Gender == Symbols.SymbolFemale),
                        SexOtherCount = group.Where(z => z is not null).Count(z => z.MostRecentResultItemToWhichThisSequenceApplies.Gender == Symbols.SymbolNonBinary),
                    };

                    answer.Add(lineItem.NameOfCohort, lineItem);
                }

                var conciseAnswer = answer.Values.ToArray();

                foreach (var lineItem in conciseAnswer)
                    lineItem.TotalCount = lineItem.SexMaleCount + lineItem.SexFemaleCount + lineItem.SexOtherCount;

                var totalPopulationInTable = conciseAnswer.Sum(lineItem => lineItem.TotalCount);

                foreach (var lineItem in conciseAnswer)
                    lineItem.Percent =
                        JghMath.Round(
                            1000 * JghMath.GuardAgainstDivisionByZero(lineItem.TotalCount, totalPopulationInTable, 0)) / 10;

                tcs.TrySetResult(conciseAnswer);

                return tcs.Task;
            }
            catch (Exception ex)
            {
                tcs.TrySetException(JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex));
                return tcs.Task;
            }
        }

        public static Task<PopulationCohortItem[]> ComposeTableOfCohortHistogramsGroupedByStringAsync(IGrouping<string, ResultItem>[] groupingsOfPlacedResults)
        {
            const string failure = "Composing population counts, totals and total percentages for histogram.";
            const string locus = "[ComposeTableOfCohortHistogramsGroupedByStringAsync]";

            var tcs = new TaskCompletionSource<PopulationCohortItem[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            if (groupingsOfPlacedResults is null)
            {
                tcs.TrySetResult([]);

                return tcs.Task;
            }

            var answer = new Dictionary<string, PopulationCohortItem>();

            try
            {
                foreach (var group in groupingsOfPlacedResults)
                {
                    if (group is null) continue;

                    var lineItem = new PopulationCohortItem
                    {
                        NameOfCohort = group.Key,
                        SexMaleCount = group.Where(z => z is not null).Count(z => z.Gender == Symbols.SymbolMale),
                        SexFemaleCount = group.Where(z => z is not null).Count(z => z.Gender == Symbols.SymbolFemale),
                        SexOtherCount = group.Where(z => z is not null).Count(z => z.Gender == Symbols.SymbolNonBinary),
                    };

                    answer.Add(lineItem.NameOfCohort, lineItem);
                }

                var conciseAnswer = answer.Values.ToArray();

                foreach (var lineItem in conciseAnswer)
                    lineItem.TotalCount = lineItem.SexMaleCount + lineItem.SexFemaleCount + lineItem.SexOtherCount;

                var totalPopulationInTable = conciseAnswer.Sum(lineItem => lineItem.TotalCount);

                foreach (var lineItem in conciseAnswer)
                    lineItem.Percent =
                        JghMath.Round(
                            1000 * JghMath.GuardAgainstDivisionByZero(lineItem.TotalCount, totalPopulationInTable, 0)) / 10;

                tcs.TrySetResult(conciseAnswer);

                return tcs.Task;
            }
            catch (Exception ex)
            {
                tcs.TrySetException(JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex));
                return tcs.Task;
            }
        }

		#endregion

		#region helpers

		private static async Task<ResultItem[]> PopulatePlacingStringsForSubsetsOfRaceWithinEventAsync(this ResultItem[] results)
		{
			const string failure = "Adding to database. Calculating placings and rankings by race.";
			const string locus = "[PopulatePlacingStringsForSubsetsOfRaceWithinEventAsync]";

			try
			{
				if (results is null)
					throw new JghNullObjectInstanceException(nameof(results));

				//var results = individuals;

				if (!results.Any())
					return [];

				var dictionaryOfAllResults = results.ConvertArrayToDictionaryKeyedOnId();

				// in this case grouping is by raceCODE: because it's a simple key be sure to use a LINQ declaration like this not a method chain to cope with the situation where raceCODE is blank throughout, have to do it this way

				var subGroups = from kvp in dictionaryOfAllResults
								where kvp.Value is not null
								where kvp.Value.DerivedData is not null
								where kvp.Value.DerivedData.IsValidDuration
								let theRaceCode = kvp.Value.RaceGroup
								group kvp by theRaceCode
					into myGroup
								orderby myGroup.Key
								select myGroup;

				foreach (var subgroup in subGroups)
				{
					var dictionaryOfResultsInSubgroup = subgroup.ToDictionary(x => x.Key, x => x.Value);

					var scratchPad = await CalculateScratchPadOfRankingsForArbitrarySubsetOfResultsAsync(dictionaryOfResultsInSubgroup);

					foreach (var kvp in dictionaryOfResultsInSubgroup)
					{
                        // method assumes that the ID property of the item and its key in the dictionary are one and the same
						if (!JghDictionaryHelpers.TryGetValueSafely(kvp.Key, out var theDiscoveredScratchPadItemValue, scratchPad))
							continue;

						// NB be sure to paste in data fields into dictionaryOfAllIndividualResults, not dictionaryOfIndividualResultsInSubgroup

						dictionaryOfAllResults[kvp.Key].DerivedData.TotalFinishersInRace = theDiscoveredScratchPadItemValue.TotalItemsInSubset;
                        dictionaryOfAllResults[kvp.Key].DerivedData.PlaceCalculatedOverallInt = theDiscoveredScratchPadItemValue.RankInSubsetInt;
                        dictionaryOfAllResults[kvp.Key].DerivedData.PlaceCalculatedOverallIntExcludingNonSeriesParticipants = theDiscoveredScratchPadItemValue.RankInSubsetIntSkippingOverNonSeriesParticipants;
						dictionaryOfAllResults[kvp.Key].DerivedData.TimeGapBehindWinnerOfRaceInSeconds = theDiscoveredScratchPadItemValue.TimeBehindWinnerOfSubsetInSeconds;
						dictionaryOfAllResults[kvp.Key].DerivedData.SplitsBehindWinnerOfRace = theDiscoveredScratchPadItemValue.SplitsBehindWinnerOfSubset;
					}
				}

				var answer = JghDictionaryHelpers.ConvertDictionaryToArray(dictionaryOfAllResults);

				return answer;
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

		private static async Task<ResultItem[]> PopulatePlacingStringsForSubsetsOfSexWithinRaceAsync(this ResultItem[] results)
		{
			const string failure = "Adding to database. Calculating placings and rankings by Sex within a race.";
			const string locus = "[PopulatePlacingStringsForSubsetsOfSexWithinRaceAsync]";

			try
			{
				if (results is null)
					throw new JghNullObjectInstanceException(nameof(results));


				if (!results.Any())
					return [];

				var dictionaryOfAllResults = results.ConvertArrayToDictionaryKeyedOnId();

				// in this case grouping is by race and Sex together: because it's a compound key, be sure to use a method chain like this not a linq select statement, otherwise it won't work as expected when all three compnents are empty

				if (dictionaryOfAllResults is not null)
				{
					var subGroups = dictionaryOfAllResults
						.Where(item => item.Value.DerivedData is not null)
						.Where(item => item.Value.DerivedData.IsValidDuration)
						.GroupBy(item => new {Race = item.Value.RaceGroup, item.Value.Gender })
                        .ToArray();


					foreach (var subgroup in subGroups)
					{
						var dictionaryOfResultsInSubgroup = subgroup.ToDictionary(x => x.Key, x => x.Value);

						var scratchPad = await CalculateScratchPadOfRankingsForArbitrarySubsetOfResultsAsync(dictionaryOfResultsInSubgroup);

						foreach (var kvp in dictionaryOfResultsInSubgroup)
						{
                            // method assumes that the ID property of the item and its key in the dictionary are one and the same
							if (!JghDictionaryHelpers.TryGetValueSafely(kvp.Key,
								out var theDiscoveredScratchPadItemValue, scratchPad))
								continue;

							// NB be sure to paste data fields into dictionaryOfAllIndividualResults, not dictionaryOfIndividualResultsInSubgroup
							dictionaryOfAllResults[kvp.Key].DerivedData.TotalFinishersInSubsetOfSexWithinRace = theDiscoveredScratchPadItemValue.TotalItemsInSubset;
                            dictionaryOfAllResults[kvp.Key].DerivedData.CalculatedRankInSubsetOfSexWithinRace = theDiscoveredScratchPadItemValue.RankInSubsetInt;
                            dictionaryOfAllResults[kvp.Key].DerivedData.CalculatedRankInSubsetOfSexWithinRaceExcludingNonSeriesParticipants = theDiscoveredScratchPadItemValue.RankInSubsetIntSkippingOverNonSeriesParticipants;
							dictionaryOfAllResults[kvp.Key].DerivedData.TimeGapBehindWinnerOfSubsetOfSexWithinRaceInSeconds = theDiscoveredScratchPadItemValue.TimeBehindWinnerOfSubsetInSeconds;
							dictionaryOfAllResults[kvp.Key].DerivedData.SplitsBehindWinnerOfSubsetOfSexWithinRace = theDiscoveredScratchPadItemValue.SplitsBehindWinnerOfSubset;
						}
					}
				}

				var answer = JghDictionaryHelpers.ConvertDictionaryToArray(dictionaryOfAllResults);

				return answer;
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

		private static async Task<ResultItem[]> PopulatePlacingStringsForSubsetsOfAgeGroupWithinSexWithinRaceAsync(this ResultItem[] results)
		{
			const string failure =
				"Adding to database. Calculating placings and rankings by race and Sex in combination with category.";
			const string locus = "[PopulatePlacingStringsForSubsetsOfAgeGroupWithinSexWithinRaceAsync]";

			try
			{
				if (results is null)
					throw new JghNullObjectInstanceException(nameof(results));

				if (!results.Any())
					return [];

				var dictionaryOfAllResults = results.ConvertArrayToDictionaryKeyedOnId();

				// in this case grouping is by race and Sex and age group together: because it's a compound key, be sure to use a method chain like this not a linq select statement, otherwise it won't work as expected when all three compnents are empty

				if (dictionaryOfAllResults is not null)
				{
					var subGroups = dictionaryOfAllResults
						.Where(item => item.Value.DerivedData is not null)
						.Where(item => item.Value.DerivedData.IsValidDuration)
						.GroupBy(item => new {Race = item.Value.RaceGroup, item.Value.Gender, CategoryID = item.Value.AgeGroup });

                    foreach (var subgroup in subGroups)
					{
						var dictionaryOfResultsInSubgroup = subgroup.ToDictionary(x => x.Key, x => x.Value);

						var scratchPad = await CalculateScratchPadOfRankingsForArbitrarySubsetOfResultsAsync(dictionaryOfResultsInSubgroup);

						foreach (var kvp in dictionaryOfResultsInSubgroup)
						{
                            // method assumes that the ID property of the item and its key in the dictionary are one and the same
							if (!JghDictionaryHelpers.TryGetValueSafely(kvp.Key, out var theDiscoveredScratchPadItemValue, scratchPad))
								continue;

							// NB be sure to paste data fields into dictionaryOfAllIndividualResults, not dictionaryOfIndividualResultsInSubgroup

							dictionaryOfAllResults[kvp.Key].DerivedData.TotalFinishersInSubsetOfAgeGroupWithinSexWithinRace = theDiscoveredScratchPadItemValue.TotalItemsInSubset;
                            dictionaryOfAllResults[kvp.Key].DerivedData.CalculatedRankInSubsetOfAgeGroupWithinSexWithinRace = theDiscoveredScratchPadItemValue.RankInSubsetInt;
                            dictionaryOfAllResults[kvp.Key].DerivedData.CalculatedRankInSubsetOfAgeGroupWithinSexWithinRaceExcludingNonSeriesParticipants = theDiscoveredScratchPadItemValue.RankInSubsetIntSkippingOverNonSeriesParticipants;
							dictionaryOfAllResults[kvp.Key].DerivedData.TimeGapBehindWinnerOfSubsetOfAgeGroupWithinSexWithinRaceInSeconds = theDiscoveredScratchPadItemValue.TimeBehindWinnerOfSubsetInSeconds;
							dictionaryOfAllResults[kvp.Key].DerivedData.SplitsBehindWinnerOfSubsetOfAgeGroupWithinSexWithinRace = theDiscoveredScratchPadItemValue.SplitsBehindWinnerOfSubset;
						}
					}
				}
				var answer = JghDictionaryHelpers.ConvertDictionaryToArray(dictionaryOfAllResults);

				return answer;
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

		private static Task<Dictionary<int, ScratchPadItemForDerivedResultData>> CalculateScratchPadOfRankingsForArbitrarySubsetOfResultsAsync(this Dictionary<int, ResultItem> results)
		{
			const string failure = "Helper method. Calculating relative placings and proportional speeds.";
			const string locus = "[CalculateScratchPadOfRankingsForArbitrarySubsetOfResultsAsync]";

			var tcs = new TaskCompletionSource<Dictionary<int, ScratchPadItemForDerivedResultData>>(TaskCreationOptions.RunContinuationsAsynchronously);

			try
			{
				if (results is null)
				{
					tcs.TrySetException(new JghNullObjectInstanceException(nameof(results)));
					return tcs.Task;
				}

				if (!results.Any())
				{
					tcs.TrySetResult(new());
					return tcs.Task;
				}

				var answer = new Dictionary<int, ScratchPadItemForDerivedResultData>();

				// find the winner and calculate placings and percentiles off him
				// NB always be sure to sidestep rubbish results i.e. those with durations less than 1 sec - this protection is empirical!

                var participantsInCorrectOrder = results
                    .Where(kvp => kvp.Value.DerivedData is not null)
                    .Where(kvp => kvp.Value.DerivedData.IsValidDuration)
                    .Where(kvp => kvp.Value.DerivedData.TotalDurationFromAlgorithmInSeconds > 1)
                    .OrderByDescending(kvp => kvp.Value.DerivedData.CalculatedNumOfSplitsCompleted)
                    .ThenBy(kvp => kvp.Value.DerivedData.TotalDurationFromAlgorithmInSeconds)
                    .ToArray();

				var winnerKvp = participantsInCorrectOrder.FirstOrDefault();

				if (winnerKvp.Value?.DerivedData is null)
				{
					tcs.TrySetResult(new());
					return tcs.Task;
				}

				var winningDurationSeconds = winnerKvp.Value.DerivedData.TotalDurationFromAlgorithmInSeconds;

				var winningNumOfSplits = winnerKvp.Value.DerivedData.CalculatedNumOfSplitsCompleted;

                // OK. placing data

                var i = 1;
                var j = 1;

                var numInSubset = participantsInCorrectOrder.Length;

				foreach (var kvp in participantsInCorrectOrder)
				{
					if (kvp.Value is null) continue;

					var scratchPadItem = new ScratchPadItemForDerivedResultData(kvp.Value.DerivedData); // don't do what resharper suggests. don't use initialiser. risky

					scratchPadItem.TotalItemsInSubset = numInSubset;

					scratchPadItem.RankInSubsetInt = i;

                    //if (kvp.Value.FirstName == "nial")
                    //{
                    //    var rub = 1; // stop here
                    //}

                    if (!kvp.Value.IsSeries || kvp.Value.IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons)
                    {
                        scratchPadItem.RankInSubsetIntSkippingOverNonSeriesParticipants = 0;
                    }
                    else
                    {
                        scratchPadItem.RankInSubsetIntSkippingOverNonSeriesParticipants = j;
                        j++;
                    }

                    scratchPadItem.TimeBehindWinnerOfSubsetInSeconds = kvp.Value.DerivedData.TotalDurationFromAlgorithmInSeconds - winningDurationSeconds;

					scratchPadItem.SplitsBehindWinnerOfSubset = winningNumOfSplits - kvp.Value.DerivedData.CalculatedNumOfSplitsCompleted;

					answer.Add(kvp.Key, scratchPadItem);

					i++;
				}

				tcs.TrySetResult(answer);
				return tcs.Task;
			}
			catch (Exception ex)
			{
				tcs.TrySetException(JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex));
				return tcs.Task;
			}
		}

		#endregion

	}
}