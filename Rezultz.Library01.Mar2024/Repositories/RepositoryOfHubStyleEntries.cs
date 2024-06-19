using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces01.July2018.Objects;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.Library01.Mar2024.Repository_interfaces;

namespace Rezultz.Library01.Mar2024.Repositories
{
	/// <summary>
	/// repository is a database of items where key is item.GetBothGuids()
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RepositoryOfHubStyleEntries<T> : IRepositoryOfHubStyleEntries<T>
		where T : class, IHubItem, IHasGuid, new()
	{
		#region ctor

		#endregion

		#region constants

		#endregion

		#region fields

		private bool SequenceIsOutOfDate { get; set; }

		private bool SequenceIsPristine => !SequenceIsOutOfDate;

        private bool DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate { get; set; }

		private readonly List<T> _nastyLittleMirror = [];

		private readonly List<T> _everythingOrderedByDescendingTimestamp = [];

		private readonly ConcurrentDictionary<string, T> _dictionaryOfEverythingKeyedbyBothGuids = new();

        private readonly ConcurrentDictionary<string, T> _dictionaryOfMostRecentItemKeyedByOriginatingItemGuid = new();

		#endregion

		#region props

		public int DesiredHeightOfShortList { get; set; } = 10; 
        //arbitrary default. you can set this to anything you like externally. in AppSettings.DesiredHeightOfShortListOfHubItemsDefault

        public int Count => _dictionaryOfEverythingKeyedbyBothGuids.Count;

		#endregion

		#region methods to add/change/delete data

		/// <summary>
		///  database key is item.GetBothGuids()
		/// </summary>
		/// <param name="item"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool TryAddNoDuplicate(T item, out string errorMessage)
		{
            #region null checks

			if (item is null)
			{
				errorMessage = "Item is null. Data error";

				return false;
			}

			if (string.IsNullOrWhiteSpace(item.Guid))
			{
				errorMessage = "Item Guid property not specified. Data error";

				return false;
			}

			if (string.IsNullOrWhiteSpace(item.OriginatingItemGuid))
			{
				errorMessage = "Item OriginatingItemGuid property not specified. Data error";

				return false;
			}

			if (string.IsNullOrWhiteSpace(item.GetBothGuids()))
			{
				errorMessage = "Item BothGuids property not specified. Data error";

				return false;
			}

			if (string.IsNullOrWhiteSpace(item.GetBothGuids()))
			{
				errorMessage = "BothGuids property not specified. Data error";

				return false;
			}

			if (item.Equals(new T()))
			{
				errorMessage = "Item is blank.";

				return false;
			}

			#endregion

			#region fast insertion

			if (_dictionaryOfEverythingKeyedbyBothGuids.ContainsKey(item.GetBothGuids()))
			{
				_dictionaryOfEverythingKeyedbyBothGuids[item.GetBothGuids()] = item; // overwrite : don't just ignore

				errorMessage = string.Empty;
			}
			else
			{
				_dictionaryOfEverythingKeyedbyBothGuids[item.GetBothGuids()] = item;

				errorMessage = string.Empty;
			}

			SequenceIsOutOfDate = true;

			DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = true;

			#endregion

			AddOrOverwriteToDirtyLittleBabyMirror(item);

			return true;
		}

		/// <summary>
		/// database key is item.GetBothGuids()
		/// </summary>
		/// <param name="items"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool TryAddRangeNoDuplicates(IEnumerable<T> items, out string errorMessage)
		{
            if (items is null)
			{
				errorMessage = "Range of items is null. Data error.";

                return false;
			}

			foreach (var z in items)
			{
				if (TryAddNoDuplicate(z, out var localErrorMessage))
					continue;

				errorMessage = localErrorMessage;
                return false;
			}

			errorMessage = string.Empty;

            return true;
		}

		/// <summary>
		/// database key is item.GetBothGuids()
		/// </summary>
		/// <param name="item"></param>
		public void UpdateEntry(T item)
		{
			if (item is null || string.IsNullOrWhiteSpace(item.GetBothGuids()))
				return;

			if (!_dictionaryOfEverythingKeyedbyBothGuids.ContainsKey(item.GetBothGuids()))
				return;

			_dictionaryOfEverythingKeyedbyBothGuids[item.GetBothGuids()] = item; // overwrite

			SequenceIsOutOfDate = true;

			DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = true;
		}

		public T RemoveEntry(string bothGuidsAsKey)
		{
			if (string.IsNullOrWhiteSpace(bothGuidsAsKey))
				return default;

			if (!_dictionaryOfEverythingKeyedbyBothGuids.TryGetValue(bothGuidsAsKey, out var item))
				return null;

			_dictionaryOfEverythingKeyedbyBothGuids.TryRemove(bothGuidsAsKey, out _);

			SequenceIsOutOfDate = true;

			DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate =
				true; // not sure if this is necessary. hmm. anyhow, it doesn't do any harm except slow things down

			return item;
		}

        public void ReorderRawEntriesByDescendingTimestamp()
        {
            if (SequenceIsPristine)
                return;

            _everythingOrderedByDescendingTimestamp.Clear();

            _everythingOrderedByDescendingTimestamp.AddRange(_dictionaryOfEverythingKeyedbyBothGuids.Values
                .OrderByDescending(z => z.TimeStampBinaryFormat)
                .ThenByDescending(z => z.WhenTouchedBinaryFormat));

            _nastyLittleMirror.Clear();

            _nastyLittleMirror.AddRange(_everythingOrderedByDescendingTimestamp.Take(DesiredHeightOfShortList));

            SequenceIsOutOfDate = false;
        }

        public int ClearCache()
		{
			var countOfItemsCleared = _dictionaryOfEverythingKeyedbyBothGuids.Count;

            _everythingOrderedByDescendingTimestamp.Clear();

            _dictionaryOfEverythingKeyedbyBothGuids.Clear();

			_dictionaryOfMostRecentItemKeyedByOriginatingItemGuid.Clear();

			_nastyLittleMirror.Clear();

			SequenceIsOutOfDate = true; //nb. handle the subsequent first-time-thru correctly

			DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = true; // ditto

			return countOfItemsCleared;
		}

		#endregion

		#region methods to access data

        public bool ContainsKeyAsBothGuids(string bothGuidsAsKey)
        {
            return _dictionaryOfEverythingKeyedbyBothGuids.ContainsKey(bothGuidsAsKey);
        }

		public bool ContainsEntryWithMatchingBothGuids(T item)
		{
			var answer = item is not null && ContainsKeyAsBothGuids(item.GetBothGuids());

			return answer;
		}

		public T GetEntryByBothGuidsAsKey(string bothGuidsAsKey)
		{
			if (string.IsNullOrWhiteSpace(bothGuidsAsKey))
				return default;

			return _dictionaryOfEverythingKeyedbyBothGuids.TryGetValue(bothGuidsAsKey, out var item) ? item : default;
		}

        public T[] GetAllEntriesAsRawData()
		{
			if (SequenceIsPristine)
				return _everythingOrderedByDescendingTimestamp.ToArray();

			ReorderRawEntriesByDescendingTimestamp();


			var answer = _everythingOrderedByDescendingTimestamp.ToArray();

			return answer;
        }

        // Note: this is the end-of-the-line master list for timestamps, but NOT for participants. duplicate IDs are not permitted in the master list for participants, nor ditches 
        public T[] GetYoungestDescendentOfEachOriginatingItemGuidIncludingDitches() 
        {
            //	Step 1. update the dictionary of most recent item per OriginatingItemGuid

            MakeDictionaryOfMostRecentItemForEachOriginatingItemGuid();

            // Step 2. order nicely

            var answer = (from kvpHubItem
                        in _dictionaryOfMostRecentItemKeyedByOriginatingItemGuid
                    select kvpHubItem.Value)
                .OrderByDescending(z => z.TimeStampBinaryFormat)
                .ThenByDescending(z => z.WhenTouchedBinaryFormat).ToArray();

            return answer;
        }

        public T[] GetAllEntriesAsRawDataNotYetPushed()
        {
            var answer = GetAllEntriesAsRawData()
                .Where(z => z is not null)
                .Where(z => z.IsStillToBePushed);

            return answer.ToArray();
        }

		public T[] GetQuickAndDirtyShortListOfEntries()
		{
			return _nastyLittleMirror.ToArray();
		}


        public T GetMostRecentEntry()
		{
			if (_everythingOrderedByDescendingTimestamp is null || !_everythingOrderedByDescendingTimestamp.Any())
				return new();

			var answer = _everythingOrderedByDescendingTimestamp.FirstOrDefault();

			return answer ?? new T();
		}

		public T GetBestGuessHeadlineEntry()
		{
			var sensibleHeadlineItem = GetMostRecentEntry();

			return sensibleHeadlineItem;
		}

		public T GetYoungestDescendentWithSameOriginatingItemGuid(string candidateOriginatingItemGuid)
		{
			MakeDictionaryOfMostRecentItemForEachOriginatingItemGuid();

			if (_dictionaryOfMostRecentItemKeyedByOriginatingItemGuid.TryGetValue(candidateOriginatingItemGuid,
				out var discoveredItem))
				return discoveredItem;

			return null;
		}

        public T GetSingleMostRecentItemOfThisKindOfRecordingModeFromMasterList(string recordingModeEnum)
        {
            if (string.IsNullOrWhiteSpace(recordingModeEnum))
                return default;

            var answer = GetAllUnDitchedYoungestDescendentsWithSameOriginatingItemGuidAsMasterList()
                .Where(z => z is not null)
                .Where(z => z.RecordingModeEnum == recordingModeEnum).OrderBy(z => z.WhenTouchedBinaryFormat)
                .LastOrDefault();

            return answer;
        }

        public Dictionary<string, T> GetDictionaryOfIdentifiersWithTheirMostRecentItemForThisRecordingModeFromMasterList(string recordingModeEnum)
        {
            if (string.IsNullOrWhiteSpace(recordingModeEnum))
                return null;

            var itemsForThisRecordingMode = GetAllUnDitchedYoungestDescendentsWithSameOriginatingItemGuidAsMasterList()
                .Where(z => z is not null)
                .Where(z => z.RecordingModeEnum == recordingModeEnum)
                .ToArray();

            if (!itemsForThisRecordingMode.Any())
                return new();

            var hubItemsGroupedByIdentifier = HubItemBase.ToListDictionaryGroupedByBib(itemsForThisRecordingMode);

            var answer = new Dictionary<string, T>();

            foreach (var subgroupForThisIdentifierKvp in hubItemsGroupedByIdentifier)
            {
                var identifier = subgroupForThisIdentifierKvp.Key;

                var mostRecent = subgroupForThisIdentifierKvp.Value.OrderBy(z => z.WhenTouchedBinaryFormat).LastOrDefault();

                if (mostRecent is not null)
                {
                    answer.Add(identifier, mostRecent);
                }
            }

            return answer;
        }

        public JghListDictionary<string, T> GetDictionaryOfIdentifiersWithTheirMultipleItemsForThisRecordingModeFromMasterList(string recordingModeEnum)
        {
            if (string.IsNullOrWhiteSpace(recordingModeEnum))
                return null;

            var interimAnswer = GetAllUnDitchedYoungestDescendentsWithSameOriginatingItemGuidAsMasterList()
                .Where(z => z is not null)
                .Where(z => z.RecordingModeEnum == recordingModeEnum).ToArray();


            var answer = HubItemBase.ToListDictionaryGroupedByBib(interimAnswer);

            return answer;
        }

        public bool IsMostRecentEntryWithSameOriginatingItemGuid(T candidateMostRecentItem)
        {
            if (candidateMostRecentItem is null) return false;

            MakeDictionaryOfMostRecentItemForEachOriginatingItemGuid();

            if (_dictionaryOfMostRecentItemKeyedByOriginatingItemGuid.TryGetValue(
                    candidateMostRecentItem.OriginatingItemGuid,
                    out var actualMostRecentItem))
                return candidateMostRecentItem.WhenTouchedBinaryFormat >= actualMostRecentItem.WhenTouchedBinaryFormat;

            return true;
        }

		public Tuple<int, int> FlagAllEntriesAsSaved()
		{
			var totalSaves = _dictionaryOfEverythingKeyedbyBothGuids.Count;
			var newSaves = 0;

			foreach (var kvp in _dictionaryOfEverythingKeyedbyBothGuids.Where(z => z.Value.IsStillToBeBackedUp))
			{
				kvp.Value.IsStillToBeBackedUp = false;

				newSaves += 1;
			}

			if (newSaves > 0)
			{
				SequenceIsOutOfDate = true;

				DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = true;
			}

			return new(totalSaves, newSaves);
		}

        public Tuple<int, int> FlagAllEntriesAsPushed()
        {
            var totalPushes = _dictionaryOfEverythingKeyedbyBothGuids.Count;
            var newPushes = 0;

            foreach (var kvp in _dictionaryOfEverythingKeyedbyBothGuids.Where(z => z.Value.IsStillToBePushed))
            {
                kvp.Value.IsStillToBePushed = false;

                newPushes += 1;
            }

            if (newPushes > 0)
            {
                SequenceIsOutOfDate = true;

                DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = true;
            }

            return new(totalPushes, newPushes);
        }

		public int FlagIncrementalEntriesAsPushed(IEnumerable<T> pushed, bool trueIfPushedFalseIfUnpushed)
		{
			var newPushes = 0;

			var whenPushed = DateTime.Now.ToBinary();

			foreach (var item in pushed.Where(z => z is not null).Where(z => z.GetBothGuids() is not null))
			{
				if (!_dictionaryOfEverythingKeyedbyBothGuids.TryGetValue(item.GetBothGuids(), out var discovered)) continue;

				if (trueIfPushedFalseIfUnpushed)
				{
					discovered.IsStillToBePushed = false;
					discovered.WhenPushedBinaryFormat = whenPushed;
				}
				else
				{
					discovered.IsStillToBePushed = true;
					discovered.WhenPushedBinaryFormat = 0;
				}

				newPushes += 1;
			}

			if (newPushes > 0)
			{
				SequenceIsOutOfDate = true;
				DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = true;
			}

			return newPushes;
		}

		#endregion

		#region helpers

		private void AddOrOverwriteToDirtyLittleBabyMirror(T item)
		{
			#region null checks

			if (item is null)
				return;

			if (string.IsNullOrWhiteSpace(item.GetBothGuids()))
				return;

			if (item.Equals(new T()))
				return; // policy is to ignore attempted additions of "blank" i.e. unpopulated "new" items

			#endregion

			#region fast update

			if (_nastyLittleMirror.Contains(item))
			{
				var index = _nastyLittleMirror.IndexOf(item);

				_nastyLittleMirror.Insert(index, item);
			}
			else
			{
				if (DesiredHeightOfShortList < 1)
					return;

				_nastyLittleMirror.Insert(0, item); // O(n) unfortunately, so keep the list short

				if (_nastyLittleMirror.Count > DesiredHeightOfShortList)
					_nastyLittleMirror.RemoveAt(_nastyLittleMirror.Count - 1); // ditch the oldest one
			}

			#endregion
		}


        protected void MakeDictionaryOfMostRecentItemForEachOriginatingItemGuid()
        {
            if (!DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate) return;

            // for ALL records (DatabaseActionEnum=Add and =Edit) - group them by  OriginatingItemGuid

            var allHubItems = GetAllEntriesAsRawData()
                    .GroupBy(z => z.OriginatingItemGuid)
                    .Where(subGroup => subGroup.Any())
                    .Select(subGroup => subGroup.OrderBy(z => z.WhenTouchedBinaryFormat).LastOrDefault())
                    .Where(mostRecent => mostRecent is not null);

            foreach (var hubItem in allHubItems)
            {
                _dictionaryOfMostRecentItemKeyedByOriginatingItemGuid[hubItem.OriginatingItemGuid] = hubItem;
            }

            DictionaryOfMostRecentItemPerOriginatingItemGuidIsOutOfDate = false;
        }

        private T[] GetAllUnDitchedYoungestDescendentsWithSameOriginatingItemGuidAsMasterList()
        {
            //	Step 1. update the dictionary of most recent item per OriginatingItemGuid

            MakeDictionaryOfMostRecentItemForEachOriginatingItemGuid();

            // Step 2. exclude Ditches and order nicely

            var answer = (from kvpHubItem
                        in _dictionaryOfMostRecentItemKeyedByOriginatingItemGuid
                    where !kvpHubItem.Value.MustDitchOriginatingItem
                    select kvpHubItem.Value)
                .OrderByDescending(z => z.TimeStampBinaryFormat)
                .ThenByDescending(z => z.WhenTouchedBinaryFormat).ToArray();

            return answer;
        }


        #endregion
    }
}
