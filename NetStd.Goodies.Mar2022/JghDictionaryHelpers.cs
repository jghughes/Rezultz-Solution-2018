using System;
using System.Collections.Generic;
using System.Linq;

namespace NetStd.Goodies.Mar2022
{
    public static class JghDictionaryHelpers
    {
        /// <summary>
        ///     Converts the key/value pairs in the dictionary to an array of the corresponding values. Order is preserved.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theDictionary">The dictionary.</param>
        /// <returns>The array.</returns>
        /// <exception cref="System.ArgumentNullException">The Dictionary cannot be null.</exception>
        public static TValue[] ConvertDictionaryToArray<TKey, TValue>(IDictionary<TKey, TValue> theDictionary)
        {
            if (theDictionary == null)
                throw new ArgumentNullException(nameof(theDictionary));

            return !theDictionary.Any() ? [] : theDictionary.Where(kvp => kvp.Value != null).Select(kvp => kvp.Value).ToArray();
        }

        /// <summary>
        ///     Converts a dictionary to an array of corresponding KeyValuePairs. Order is preserved.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theDictionary">The dictionary.</param>
        /// <returns>The array.</returns>
        /// <exception cref="System.ArgumentNullException">The Dictionary cannot be null.</exception>
        public static KeyValuePair<TKey, TValue>[] ConvertDictionaryToKeyValuePairArray<TKey, TValue>(IDictionary<TKey, TValue> theDictionary)
        {
            if (theDictionary == null)
                throw new ArgumentNullException(nameof(theDictionary));

            return !theDictionary.Any() ? [] : theDictionary.ToArray();
        }

        /// <summary>
        ///     Converts an array of KeyValuePairs to a dictionary.
        ///     The array cannot be null.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theArray">The array</param>
        /// <returns>The dictionary keyed on TKey with values of TValue.</returns>
        /// <exception cref="System.ArgumentNullException">The array cannot be null.</exception>
        public static Dictionary<TKey, TValue> ConvertKeyValuePairArrayToDictionary<TKey, TValue>(KeyValuePair<TKey, TValue>[] theArray)
        {
            if (theArray == null)
                throw new ArgumentNullException(nameof(theArray));

            return !theArray.Any() ? new Dictionary<TKey, TValue>() : theArray.ToDictionary(z => z.Key, z => z.Value);
        }

        /// <summary>
        ///     Converts an array of to a sequentially integer indexed dictionary.
        ///     The array can be null.
        ///     Starting index is zero.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theArray">The array</param>
        /// <returns>The dictionary keyed on index with values of TValue.</returns>
        public static Dictionary<int, TValue> ConvertArrayToIndexedDictionary<TValue> (TValue[] theArray)
        {
            if (theArray == null)
                return new Dictionary<int, TValue>();

            if(!theArray.Any())
                return new Dictionary<int, TValue>();

            var answer = new Dictionary<int, TValue>();

            for (int i = 0; i < theArray.Length; i++)
            {
                var item = theArray[i];

                if (item == null)
                    continue;

                answer.Add(i, item);
            }

            return answer;

        }

        /// <summary>
        ///     Gets the value associated with the specified key safely.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of the key, for which the default equality comparer will be used for the lookup if it
        ///     is a reference type.
        /// </typeparam>
        /// <typeparam name="TValue">The type of the value, which must be a reference type.</typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="theDictionary">The dictionary, which can be null or empty.</param>
        /// <returns>The value associated with the key, or null if the dictionary is null or empty or the key is not found.</returns>
        public static TValue LookUpValueSafely<TKey, TValue>(TKey key, IDictionary<TKey, TValue> theDictionary) where TValue : class
        {
            if (theDictionary == null || !theDictionary.Any() || key == null)
                return null;

            return theDictionary.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        ///     Gets the value associated with the specified key safely.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of the key, for which the default equality comparer will be used for the lookup if it
        ///     is a reference type.
        /// </typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="theDictionary">The dictionary, which can be null or empty.</param>
        /// <returns>
        ///     The value associated with the key, or an empty string if the dictionary is null or empty or the key is not
        ///     found.
        /// </returns>
        public static string LookUpValueSafely<TKey>(TKey key, IDictionary<TKey, string> theDictionary)
        {
            if (theDictionary == null || !theDictionary.Any() || key == null)
                return null;

            var value = theDictionary.TryGetValue(key, out var value1) ? value1 : null;

            return value ?? string.Empty;
        }

        /// <summary>
        ///     Gets the value associated with the specified key safely.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="theDictionary">The dictionary, which can be null or empty.</param>
        /// <returns>
        ///     The value associated with the key, or null if the dictionary is null or empty or the key is null or whitespace
        ///     or the key is not found.
        /// </returns>
        public static string LookUpValueSafely(string key, IDictionary<string, string> theDictionary)
        {
            if (theDictionary == null || !theDictionary.ContainsKey(key) || string.IsNullOrWhiteSpace(key) )
                return null;

            var answer = theDictionary[key];

            return answer;
        }

        /// <summary>
        ///     Gets the value associated with the specified key safely.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of the key, for which the default equality comparer will be used for the lookup if it
        ///     is a reference type.
        /// </typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="theDictionary">The dictionary, which can be null or empty.</param>
        /// <returns>The value associated with the key, or zero if the dictionary is null or empty or the key is not found.</returns>
        public static int LookUpValueSafely<TKey>(TKey key, IDictionary<TKey, int> theDictionary)
        {
	        if (theDictionary == null || !theDictionary.Any() || key == null)
		        return 0;

	        var value = theDictionary.TryGetValue(key, out var value1) ? value1 : 0;

	        return value;
        }

        /// <summary>
        ///     Gets the value associated with the specified key safely.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of the key, for which the default equality comparer will be used for the lookup if it
        ///     is a reference type.
        /// </typeparam>
        /// <param name="key">The specified key.</param>
        /// <param name="theDictionary">The dictionary, which can be null or empty.</param>
        /// <returns>The value associated with the key, or zero if the dictionary is null or empty or the key is not found.</returns>
        public static double LookUpValueSafely<TKey>(TKey key, IDictionary<TKey, double> theDictionary)
        {
	        if (theDictionary == null || !theDictionary.Any() || key == null)
		        return 0;

	        var value = theDictionary.TryGetValue(key, out var value1) ? value1 : 0;

	        return value;
        }

        /// <summary>
        ///     Determines if a matching key/value pair exists.
        /// </summary>
        /// <param name="key">The specified keywhich can be null.</param>
        /// <param name="value">The specified value which can be null.</param>
        /// <param name="theDictionary">The dictionary, which can be null or empty.</param>
        /// <returns>
        ///     True if the key/value is found or false if the dictionary is null or empty, the key is null or whitespace, or
        ///     the key is not found, or the value is null or whitespace.
        /// </returns>
        public static bool StringDictionaryKvpValueIsMatch(string key, string value, IDictionary<string, string> theDictionary)
        {
            if (theDictionary == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return false;

            var valueObtained = LookUpValueSafely(key, theDictionary);

            return JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(valueObtained, value);
        }

        /// <summary>
        ///     Merges a collection of dictionaries into a single dictionary, dictated by whether or not the first occurrence of a
        ///     repeated key takes precedence.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of the key, for which the default equality comparer will be used for the lookup if it
        ///     is a reference type.
        /// </typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="arrayOfDictionaries">The array of dictionaries which can itself be null or can contain null elements.</param>
        /// <param name="firstKvpTakePrecedence">
        ///     If set to <c>true</c> the first occurrence of a key takes precedence, otherwise
        ///     the last.
        /// </param>
        /// <returns>The merged dictionary or an empty dictionary if the input array of dictionaries is null.</returns>
        public static Dictionary<TKey, TValue> MergeMultipleDictionaries<TKey, TValue>(IDictionary<TKey, TValue>[] arrayOfDictionaries, bool firstKvpTakePrecedence)
        {
            if (arrayOfDictionaries == null) return new Dictionary<TKey, TValue>();

            var omnibusListOfKvps = new List<KeyValuePair<TKey, TValue>>();

            foreach (var dictionary in arrayOfDictionaries.Where(z => z != null))
                omnibusListOfKvps.AddRange(dictionary);

            var mergedDictionary = new Dictionary<TKey, TValue>();

            foreach (var kvp in omnibusListOfKvps)
                if (firstKvpTakePrecedence)
                {
                    // don't overwrite
                    if (!mergedDictionary.ContainsKey(kvp.Key))
                        mergedDictionary.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    // overwrite
                    mergedDictionary.Remove(kvp.Key);
                    mergedDictionary.Add(kvp.Key, kvp.Value);
                }

            return mergedDictionary;
        }

        /// <summary>
        ///     Tries the get value from dictionary safely.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theLookupKey">The lookup key.</param>
        /// <param name="theDiscoveredValue">
        ///     The value associated with the specified key, if the key is found; otherwise, the
        ///     default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <param name="theDictionary">The dictionary, which can be null.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise false.</returns>
        public static bool TryGetValueSafely<TValue>(int theLookupKey, out TValue theDiscoveredValue, IDictionary<int, TValue> theDictionary) where TValue : class
        {
            theDiscoveredValue = null; // dummy. only applicable if the dictionary is null.

            if (theDictionary == null)
                return false;

            var answer = theDictionary.TryGetValue(theLookupKey, out theDiscoveredValue);

            return answer;
        }

        /// <summary>
        ///     Adds or updates key/value pair to the dictionary safely. Null values are rejected.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theLookupKey">The lookup key, a string, which can be null or whitespace.</param>
        /// <param name="theValue">Value to be added.</param>
        /// <param name="theDictionary">The dictionary, which can be null.</param>
        /// <returns>True if the value is not null and the operation completes successfully, false otherwise.</returns>
        public static bool AddOrUpdateSafely<TValue>(string theLookupKey, TValue theValue, IDictionary<string, TValue> theDictionary) where TValue : class
        {
	        if (theDictionary == null)
		        return false;

	        if (string.IsNullOrWhiteSpace(theLookupKey))
		        return false;

	        if (theValue == null)
		        return false;

	        if (theDictionary.ContainsKey(theLookupKey))
		        theDictionary[theLookupKey] = theValue;
	        else
		        theDictionary.Add(theLookupKey, theValue);

	        return true;
        }

        /// <summary>
        ///     Adds or updates key/value pair to the dictionary safely. Null values are rejected.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theLookupKey">The lookup key, a string, which can be null or whitespace.</param>
        /// <param name="theValue">Value to be added.</param>
        /// <param name="theDictionary">The dictionary, which can be null.</param>
        /// <returns>True if the value is not null and the operation completes successfully, false otherwise.</returns>
        public static bool AddOrUpdateSafely<TValue>(int theLookupKey, TValue theValue, IDictionary<int, TValue> theDictionary) where TValue : class
        {
	        if (theDictionary == null)
		        return false;

	        if (theValue == null)
		        return false;

	        if (theDictionary.ContainsKey(theLookupKey))
		        theDictionary[theLookupKey] = theValue;
	        else
		        theDictionary.Add(theLookupKey, theValue);

	        return true;
        }

        /// <summary>
        ///     Deletes key/value pair to the dictionary safely.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="theLookupKey">The lookup key, a string, which can be null or whitespace.</param>
        /// <param name="theDictionary">The dictionary, which can be null.</param>
        /// <returns>True if the value is not null and the operation completes successfully, false otherwise.</returns>
        public static bool DeleteSafely<TValue>(int theLookupKey, IDictionary<int, TValue> theDictionary) where TValue : class
        {
	        if (theDictionary == null)
		        return false;
            
		    theDictionary.Remove(theLookupKey);

	        return true;
        }


    }
}