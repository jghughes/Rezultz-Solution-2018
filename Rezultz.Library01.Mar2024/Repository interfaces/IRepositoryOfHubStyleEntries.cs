using System;
using System.Collections.Generic;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.Objects;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
	public interface IRepositoryOfHubStyleEntries<T> where T : IHubItem, new()
	{
		int Count { get; }

		int DesiredHeightOfShortList { get; set; }

        bool IsMostRecentEntryWithSameOriginatingItemGuid(T candidateMostRecentItem);

        Tuple<int, int> FlagAllEntriesAsSaved();

		int ClearCache();

		bool TryAddNoDuplicate(T item, out string errorMessage);

		bool TryAddRangeNoDuplicates(IEnumerable<T> items, out string errorMessage);

		bool ContainsEntryWithMatchingBothGuids(T item);


		T GetEntryByBothGuidsAsKey(string bothGuidsAsKey);

		T GetBestGuessHeadlineEntry();

        T GetYoungestDescendentWithSameOriginatingItemGuid(string candidateOriginatingItemGuid);

        T GetSingleMostRecentItemOfThisKindOfRecordingModeFromMasterList(string recordingModeEnum);

        Dictionary<string, T> GetDictionaryOfIdentifiersWithTheirMostRecentItemForThisRecordingModeFromMasterList(string recordingModeEnum);

        JghListDictionary<string, T> GetDictionaryOfIdentifiersWithTheirMultipleItemsForThisRecordingModeFromMasterList(string recordingModeEnum);

        T[] GetAllEntriesAsRawData();

        T[] GetYoungestDescendentOfEachOriginatingItemGuidIncludingDitches(); // this is primarily what we use for generating a participant repository

        T[] GetAllEntriesAsRawDataNotYetPushed(); 

        T[] GetQuickAndDirtyShortListOfEntries();
	}
}