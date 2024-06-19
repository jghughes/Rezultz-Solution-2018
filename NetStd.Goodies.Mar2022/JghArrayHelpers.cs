using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Interfaces01.July2018.HasProperty;

namespace NetStd.Goodies.Mar2022
{
    public static class JghArrayHelpers
    {
        /// <summary>
        ///     Adds an item to an array, excluding duplicates.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="manyItems">the array</param>
        /// <param name="theItem">item to be added</param>
        /// <returns>The resultant array</returns>
        public static T[] AddNoDuplicates<T>(T[] manyItems, T theItem) where T : class
        {
            manyItems ??= [];

            if (theItem is null)
                return manyItems;

            if (manyItems.Contains(theItem))
                return manyItems;

            var answer = manyItems.ToList();

            answer.Add(theItem);

            return answer.ToArray();
        }

        /// <summary>Safely selects an item in an array according to a specified value for the item's array index.</summary>
        /// <typeparam name="T">Nullable reference type.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchIndex">The specified Index.</param>
        /// <returns>Null if array is null or empty or index is out of bounds.</returns>
        public static T SelectItemFromArrayByArrayIndex<T>(T[] manyItems, int searchIndex) where T : class
        {
            if (manyItems is null || !manyItems.Any())
                return null;

            if ((searchIndex < 0) | (searchIndex > manyItems.Count() - 1))
                return null;

            return manyItems[searchIndex];
        }

        /// <summary>Safely selects an item in an array according to a specified value for the item's UniqueItemIdproperty.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasItemID.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameter">The specified Guid as a string.</param>
        /// <returns>The first occurrence of the item. Null if array is null or empty or searchParameter is Guid.Empty.</returns>
        public static T SelectItemFromArrayByItemGuidString<T>(T[] manyItems, string searchParameter) where T : class, IHasGuid
		{
			if (manyItems is null || !manyItems.Any() || searchParameter is null)
				return null;

			if (string.IsNullOrWhiteSpace(searchParameter))
				return null;

			var answer = (from item in manyItems
						  where item != null
						  where item.Guid == searchParameter
						  select item).FirstOrDefault(); // default is null by definition for reference types

			return answer;
		}

		/// <summary>Safely selects an item in an array according to a specified value for the item's ID property.</summary>
		/// <typeparam name="T">Nullable reference type satisfying IHasItemID.</typeparam>
		/// <param name="manyItems">The array of items.</param>
		/// <param name="searchParameter">The specified ID.</param>
		/// <returns>The first occurrence of the item. Null if array is null or empty or ID not found.</returns>
		public static T SelectItemFromArrayByItemId<T>(T[] manyItems, int searchParameter) where T : class, IHasItemID
        {
	        if (manyItems is null || !manyItems.Any())
		        return null;

	        var answer = (from item in manyItems
		        where item != null
		        where item.ID == searchParameter
		        select item).FirstOrDefault(); // default is null by definition for reference types

	        return answer;
        }

        /// <summary>Safely selects an item in an array according to a specified value for the item's Bib property.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasBib.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameter">The specified Bib.</param>
        /// <returns>The first occurrence of the item. Null if array is null or empty or Bib not found.</returns>
        public static T SelectItemFromArrayByItemBib<T>(T[] manyItems, string searchParameter) where T : class, IHasBib
        {
            if (manyItems is null || !manyItems.Any() || searchParameter is null)
                return null;

            var answer = (from item in manyItems
                where item != null
                where JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(item.Bib, searchParameter)
                select item).FirstOrDefault();

            return answer;
        }

        /// <summary>Safely selects an item in an array according to a specified value for the item's Bib property.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasBib.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameter">The specified Bib.</param>
        /// <returns>All occurrences of the item just in case duplicates exist. Null if array is null or empty or Bib not found.</returns>
        public static T[] SelectItemsFromArrayByItemBib<T>(T[] manyItems, string searchParameter)
            where T : class, IHasBib
        {
            if (manyItems is null || !manyItems.Any() || searchParameter is null)
                return null;

            var answer = (from item in manyItems
                where item != null
                where JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(item.Bib, searchParameter)
                select item).ToArray();

            return answer;
        }

        /// <summary>Safely selects an item in an array according to a specified value for the item's EnumString property.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IEnumString.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameter">The specified EnumString.</param>
        /// <returns>The first occurrence of the item. Null if array is null or empty or EnumString not found.</returns>
        public static T SelectItemFromArrayByItemLabel<T>(T[] manyItems, string searchParameter)
	        where T : class, IHasLabel
        {
	        if (manyItems is null || !manyItems.Any() || searchParameter is null)
		        return null;

	        var answer = (from item in manyItems
		        where item != null
		        where JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(item.Label, searchParameter)
		        select item).FirstOrDefault();

	        return answer;
        }

        /// <summary>Safely selects an item in an array according to a specified value for the item's EnumString property.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IEnumString.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameter">The specified EnumString.</param>
        /// <returns>The first occurrence of the item. Null if array is null or empty or EnumString not found.</returns>
        public static T SelectItemFromArrayByItemEnumString<T>(T[] manyItems, string searchParameter)
	        where T : class, IHasEnumString
        {
	        if (manyItems is null || !manyItems.Any() || searchParameter is null)
		        return null;

	        var answer = (from item in manyItems
		        where item != null
		        where JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(item.EnumString, searchParameter)
		        select item).FirstOrDefault();

	        return answer;
        }
        
        /// <summary>
        ///     Determines the array index of an item in an array that is the same as a specified item on the basis of their
        ///     default equality comparer.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchItem">The specified item.</param>
        /// <returns>-1 if array is null or empty or no matching item exists.</returns>
        public static int SelectArrayIndexOfItemInArrayByItemEquality<T>(T[] manyItems, T searchItem)
        {
            if (manyItems is null || !manyItems.Any())
                return -1;

            var arrayIndexOfTheItem = Array.IndexOf(manyItems, searchItem); // method returns -1 if not found

            return arrayIndexOfTheItem;
        }

        /// <summary>Determines the array index of an item in an array according to a specified value for the item's ID property.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasItemID.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameter">The specified ItemId.</param>
        /// <returns>Index of first matching occurrence or -1 if array is null or empty or no matching items exist.</returns>
        public static int SelectArrayIndexOfItemInArrayByItemId<T>(T[] manyItems, int searchParameter) where T : class, IHasItemID
        {
            if (manyItems is null || !manyItems.Any())
                return -1;

            var collectionToBeSearched = (from item in manyItems
                where item != null
                select item.ID).ToArray();

            if (!collectionToBeSearched.Any())
                return -1;

            var theIndex = Array.IndexOf(collectionToBeSearched, searchParameter); // method returns -1 if not found

            return theIndex;
        }

        /// <summary>
        ///     Determines the array index of an item in an array according to a specified value for the item's EnumString
        ///     property.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasEnumString.</typeparam>
        /// <param name="manyItems">The array of many items.</param>
        /// <param name="searchParameter">The specified EnumString.</param>
        /// <returns>Index of first matching occurrence or -1 if array is null or empty or no matching items exist.</returns>
        public static int SelectArrayIndexOfItemInArrayByItemEnumString<T>(T[] manyItems, string searchParameter)
            where T : class, IHasEnumString
        {
            if (manyItems is null || !manyItems.Any())
                return -1;

            var collectionToBeSearched = (from item in manyItems
                where item != null
                where item.EnumString != null
                select item.EnumString).ToList();

            if (!collectionToBeSearched.Any())
                return -1;

            var theIndex = collectionToBeSearched.IndexOf(searchParameter); // method returns -1 if not found

            return theIndex;
        }


        /// <summary>
        ///     Determines the array index of an item in an array according to a specified value for the item's Label
        ///     property.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasLabel.</typeparam>
        /// <param name="manyItems">The array of many items.</param>
        /// <param name="searchParameter">The specified Label.</param>
        /// <returns>Index of first matching occurrence or -1 if array is null or empty or no matching items exist.</returns>
        public static int SelectArrayIndexOfItemInArrayByItemLabel<T>(T[] manyItems, string searchParameter)
	        where T : class, IHasLabel
        {
	        if (manyItems is null || !manyItems.Any() || searchParameter is null)
		        return -1;

	        var collectionToBeSearched = (from item in manyItems
		        where item != null
		        where item.Label != null
		        select item.Label).ToList();

	        if (!collectionToBeSearched.Any())
		        return -1;

	        var theIndex = collectionToBeSearched.IndexOf(searchParameter); // method returns -1 if not found

	        return theIndex;
        }
        
        /// <summary>
        ///     Safely determines the value of EnumString property of an item in an array according to a specified value for
        ///     the item's array index.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasEnumString.</typeparam>
        /// <param name="manyEnums">The array of items.</param>
        /// <param name="searchIndex">The specified index.</param>
        /// <returns>Null if array is null or empty or index is out of bounds.</returns>
        public static string SelectEnumStringFromEnumStyleItemArrayByArrayIndex<T>(T[] manyEnums, int searchIndex)
            where T : class, IHasEnumString, new()
        {
            var intermediateAnswer = SelectItemFromArrayByArrayIndex(manyEnums, searchIndex);

            return intermediateAnswer?.EnumString;
        }

        /// <summary>
        ///     Selects a subset of items from an array of items according to a specified value for their CodeNameOfSuperSet
        ///     property.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasCodeNameOfSuperset.</typeparam>
        /// <param name="manyItems">The array of items.</param>
        /// <param name="searchParameterOfSuperset">The specified CodeName.</param>
        /// <returns>Empty array if the collection is null or empty or no matching items exist.</returns>
        public static T[] SelectItemsByCodeNameOfSuperset<T>(T[] manyItems, string searchParameterOfSuperset)
            where T : class, IHasCodeNameOfSuperset
        {
            if (manyItems is null || !manyItems.Any() || searchParameterOfSuperset is null)
                return [];

            var candidates = (from thisItem in manyItems
                where thisItem != null
                where JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(thisItem.CodeNameOfSuperset,
                    searchParameterOfSuperset)
                select thisItem).ToArray();

            return candidates;
        }

        /// <summary>Populates the ID property of items in a collection with sequential ascending values starting with one.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasItemID.</typeparam>
        /// <param name="manyItems">The collection.</param>
        /// <returns>Populated array. Empty array if the collection is null or empty.</returns>
        public static T[] PopulateItemsInArrayWithSequentialIDsStartingWithOne<T>(T[] manyItems) where T : class, IHasItemID
        {
            if (manyItems is null || !manyItems.Any())
                return [];

            var i = 1;
            // NB don't start with zero. we use 0 to mean "all" in our filtering system. we use "-1" to mean "n/a"

            foreach (var item in manyItems.Where(z => z != null))
            {
                item.ID = i;

                i++;
            }
            return manyItems;
        }

        /// <summary>
        ///     Selects the most recent item in a collection from among those whose AdvertisedDate property value is
        ///     before DateTime Now.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasAdvertisedDate.</typeparam>
        /// <param name="items">The collection of items.</param>
        /// <returns>
        ///     The most recent item that occurred in the past. Null if the collection is null or empty or contains no items
        ///     that occurred in the past.
        /// </returns>
        public static T SelectMostRecentItemInItemArrayBeforeDateTimeNow<T>(T[] items)
            where T : class, IHasAdvertisedDate, new()
        {
            if (items is null) return null;

            if (!items.Any()) return null;

            var answers = (from item in items
                where item != null
                where item.AdvertisedDate < DateTime.Now
                orderby item.AdvertisedDate descending
                select item).ToArray();

            return answers.FirstOrDefault();
        }

        /// <summary>
        ///     Selects the earliest item in a collection irrespective of whether its AdvertisedDate property is in the
        ///     past, present or future.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasAdvertisedDate.</typeparam>
        /// <param name="items">The collection of items.</param>
        /// <returns>The earliest item. Null if the collection is null or empty or contains no items that occurred in the past.</returns>
        public static T SelectEarliestItemInItemArray<T>(T[] items) where T : class, IHasAdvertisedDate, new()
        {
            if (items is null) return null;

            if (!items.Any()) return null;

            var answers = (from item in items
                where item != null
                orderby item.AdvertisedDate
                select item).ToArray();

            return answers.FirstOrDefault();
        }

        /// <summary>
        ///     Selects the most recent item in a collection whose AdvertisedDate property value is before DateTime Now,
        ///     or if such doesn't exist the soonest item in the future.
        /// </summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasAdvertisedDate.</typeparam>
        /// <param name="manyItems">The collection of items.</param>
        /// <returns>
        ///     The most recent item that occurred in the past or failing that the soonest item that will occur in future.
        ///     Null if the collection is null or empty or contains no items that occurred in the past.
        /// </returns>
        public static T SelectMostRecentItemBeforeDateTimeNowInArrayOfItemsOrFailingThatPickTheEarliest<T>(T[] manyItems)
            where T : class, IHasAdvertisedDate, new()
        {
            if (manyItems is null) return null;

            if (!manyItems.Any()) return null;

            var earliest = SelectEarliestItemInItemArray(manyItems);

            var beforeNow = SelectMostRecentItemInItemArrayBeforeDateTimeNow(manyItems);

            return beforeNow ?? earliest;
        }

        /// <summary>
        ///     Converts a collection of items to a dictionary of items keyed on a fresh ascending integer key starting from
        ///     specifiedStartingKey.
        /// </summary>
        /// <typeparam name="T">Reference type.</typeparam>
        /// <param name="manyItems">The collection of items.</param>
        /// <param name="specifiedStartingKey">Non-negative integer.</param>
        /// <returns>Dictionary with ascending key incremented by one each time.</returns>
        /// <exception cref="ArgumentNullException">The collection cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specifiedStartingKey cannot be negative.</exception>
        public static Dictionary<int, T> ConvertArrayToDictionaryKeyedOnFreshAscendingIndex<T>(T[] manyItems,
            int specifiedStartingKey) where T : class
        {
            if (manyItems is null)
                throw new ArgumentNullException(nameof(manyItems));

            if (!manyItems.Any())
                return new Dictionary<int, T>();

            if (specifiedStartingKey < 0)
                throw new ArgumentOutOfRangeException(nameof(specifiedStartingKey));

            var answer = new Dictionary<int, T>();

            var i = specifiedStartingKey;

            foreach (var item in manyItems)
            {
                answer.Add(i, item);

                i++;
            }

            return answer;
        }

        /// <summary>Converts a collection of items to a dictionary of items keyed on the value of the ID property of each item.</summary>
        /// <typeparam name="T">Nullable reference type satisfying IHasItemID.</typeparam>
        /// <param name="manyItems">The collection of items.</param>
        /// <returns>Dictionary of items, omitting null items, and all but the first occurrence of items sharing the same ID.</returns>
        /// <exception cref="System.ArgumentNullException">The collection cannot be null.</exception>
        public static Dictionary<int, T> ConvertArrayToDictionaryKeyedOnId<T>(this T[] manyItems)
            where T : class, IHasItemID
        {
            #region null checks

            if (manyItems is null)
                throw new ArgumentNullException(nameof(manyItems));

            if (!manyItems.Any())
                return new Dictionary<int, T>();

            #endregion

            var answer = new Dictionary<int, T>();

            foreach (var item in manyItems)
            {
                if (item is null)
                    continue;

                if (answer.ContainsKey(item.ID)) continue;

                answer.Add(item.ID, item);
            }

            return answer;
        }

        public static T[] AggregateArrays<T>(IEnumerable<T[]> arrays)
        {
            var answer = new List<T>();

            foreach (var array in arrays)
            {
                if (array is null)
                    continue;

                answer.AddRange(array);
            }

            return answer.ToArray();
        }
    }
}