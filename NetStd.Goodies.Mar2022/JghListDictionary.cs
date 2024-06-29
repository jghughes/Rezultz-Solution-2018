using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// a very handy Dictionary inspired by the Prism team - but modified
// https://raw.githubusercontent.com/PrismLibrary/Prism/master/src/Prism.Core/Common/JghListDictionary.cs

namespace NetStd.Goodies.Mar2022;

/// <summary>
///     A dictionary of key value pairs where each value is a list.
/// </summary>
/// <typeparam name="TKey">The key to use for lists.</typeparam>
/// <typeparam name="TValue">The type of the value held by lists.</typeparam>
public sealed class JghListDictionary<TKey, TValue> : IDictionary<TKey, IList<TValue>>
{
    private readonly Dictionary<TKey, IList<TValue>> _innerValues = new();

    #region IEnumerable<KeyValuePair<TKey,List<TValue>>> Members

    /// <summary>
    ///     See <see cref="IEnumerable{TValue}.GetEnumerator" /> for more information.
    /// </summary>
    IEnumerator<KeyValuePair<TKey, IList<TValue>>> IEnumerable<KeyValuePair<TKey, IList<TValue>>>.GetEnumerator()
    {
        return _innerValues.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    ///     See <see cref="System.Collections.IEnumerable.GetEnumerator" /> for more information.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _innerValues.GetEnumerator();
    }

    #endregion

    #region helpers

    private List<TValue> CreateNewList(TKey key)
    {
        List<TValue> values = [];

        _innerValues.Add(key, values);

        return values;
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///     If a list does not already exist, it will be created automatically.
    /// </summary>
    /// <param name="key">The key of the list that will hold the value.</param>
    public void Add(TKey key)
    {
        if (key is null) return;

        CreateNewList(key);
    }

    /// <summary>
    ///     Adds a value to a list with the given key. If a list does not already exist,
    ///     it will be created automatically.
    /// </summary>
    /// <param name="key">The key of the list that will hold the value.</param>
    /// <param name="value">The value to add to the list under the given key.</param>
    public void Add(TKey key, TValue value)
    {
        if (key is null) return;

        if (value is null) return;

        if (_innerValues.TryGetValue(key, out var innerValue))
        {
            innerValue.Add(value);
        }
        else
        {
            var values = CreateNewList(key);
            values.Add(value);
        }
    }

    /// <summary>
    ///     Removes all entries in the dictionary.
    /// </summary>
    public void Clear()
    {
        _innerValues.Clear();
    }

    /// <summary>
    ///     Determines whether the dictionary contains the given key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>true if the dictionary contains the given key; otherwise, false.</returns>
    public bool ContainsKey(TKey key)
    {
        return _innerValues.ContainsKey(key);
    }

    /// <summary>
    ///     Determines whether the dictionary contains the specified value.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>true if the dictionary contains the value in any list; otherwise, false.</returns>
    public bool ContainsValue(TValue value)
    {
        if (value is null) return false;

        foreach (var pair in _innerValues)
            if (pair.Value.Contains(value))
                return true;

        return false;
    }

    /// <summary>
    ///     Retrieves all elements of the list associated with a key.
    ///     Empty if nothing found.
    /// </summary>
    /// <param name="key">The key of the list to access.</param>
    /// <returns>The list associated with the key.</returns>
    public IEnumerable<TValue> FindValues(TKey key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        return !_innerValues.TryGetValue(key, out var value) ? new List<TValue>() : value;
    }

    /// <summary>
    ///     Retrieves all the elements from the lists which have a key that matches the condition
    ///     defined by the specified predicate.
    /// </summary>
    /// <param name="keyFilter">The filter with the condition to use to filter lists by their key.</param>
    /// <returns>The elements that have a key that matches the condition defined by the specified predicate.</returns>
    public IEnumerable<TValue> FindAllValuesByKey(Predicate<TKey> keyFilter)
    {
        foreach (var pair in this)
            if (keyFilter(pair.Key))
                foreach (var value in pair.Value)
                    yield return value;
    }

    /// <summary>
    ///     Retrieves all the keys associated with lists that contain the value.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>The keys associated with lists that contain the value.</returns>
    public IEnumerable<TKey> FindKeysByValue(TValue value)
    {
        foreach (var pair in this)
            if (pair.Value.Contains(value))
                yield return pair.Key;
    }

    /// <summary>
    ///     Retrieves all keys associated with lists that contain a value matching the condition defined by the specified predicate.
    /// </summary>
    /// <param name="valueFilter">The filter with the condition to use to filter values.</param>
    /// <returns>The keys associated with lists that contain a value matching the condition defined by the specified predicate.</returns>
    public IEnumerable<TKey> FindAllKeysByValue(Predicate<TValue> valueFilter)
    {
        foreach (var pair in this)
        foreach (var value in pair.Value)
        {
            if (!valueFilter(value))
                continue;

            yield return pair.Key;
            break;
        }
    }

    /// <summary>
    ///     Retrieves all the values that match the condition defined by the specified predicate.
    /// </summary>
    /// <param name="valueFilter">The filter with the condition to use to filter values.</param>
    /// <returns>The elements that match the condition defined by the specified predicate.</returns>
    public IEnumerable<TValue> FindAllValues(Predicate<TValue> valueFilter)
    {
        return from pair in this from value in pair.Value where valueFilter(value) select value;
    }

    /// <summary>
    ///     Retrieves all the elements in the ListDictionary.
    /// </summary>
    /// <returns>The elements that match the condition defined by the specified predicate.</returns>
    public IEnumerable<KeyValuePair<TKey, TValue>> FindAllElements()
    {
        return from pair in this from value in pair.Value select new KeyValuePair<TKey, TValue>(pair.Key, value);
    }

    /// <summary>
    ///     Removes a list by key.
    /// </summary>
    /// <param name="key">The key of the list to remove.</param>
    /// <returns><see langword="true" /> if the element was removed.</returns>
    public bool Remove(TKey key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        return _innerValues.Remove(key);
    }

    /// <summary>
    ///     Removes a value from the list with the given key.
    /// </summary>
    /// <param name="key">The key of the list where the value exists.</param>
    /// <param name="value">The value to remove.</param>
    public void RemoveValue(TKey key, TValue value)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (_innerValues.ContainsKey(key))
        {
            var innerList = (List<TValue>)_innerValues[key];
            innerList.RemoveAll(delegate(TValue item) { return value.Equals(item); });
        }
    }

    /// <summary>
    ///     Removes a value from all lists where it may be found.
    /// </summary>
    /// <param name="value">The value to remove.</param>
    public void RemoveValue(TValue value)
    {
        foreach (var pair in _innerValues) RemoveValue(pair.Key, value);
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets a shallow copy of all values in all lists.
    /// </summary>
    /// <value>List of values.</value>
    public IList<TValue> Values
    {
        get
        {
            List<TValue> values = [];

            foreach (var list in _innerValues.Values) values.AddRange(list);

            return values;
        }
    }

    /// <summary>
    ///     Gets the list of keys in the dictionary.
    /// </summary>
    /// <value>Collection of keys.</value>
    public ICollection<TKey> Keys => _innerValues.Keys;

    /// <summary>
    ///     Gets or sets the list associated with the given key. The
    ///     access always succeeds, eventually returning an empty list.
    /// </summary>
    /// <param name="key">The key of the list to access.</param>
    /// <returns>The list associated with the key.</returns>
    public IList<TValue> this[TKey key]
    {
        get
        {
            if (_innerValues.ContainsKey(key) == false) _innerValues.Add(key, []);
            return _innerValues[key];
        }
        set => _innerValues[key] = value;
    }

    /// <summary>
    ///     Gets the number of lists in the dictionary.
    /// </summary>
    /// <value>Value indicating the values count.</value>
    public int Count => _innerValues.Count;

    #endregion

    #region IDictionary<TKey,List<TValue>> Members

    /// <summary>
    ///     See <see cref="IDictionary{TKey,TValue}.Add" /> for more information.
    /// </summary>
    void IDictionary<TKey, IList<TValue>>.Add(TKey key, IList<TValue> value)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        _innerValues.Add(key, value);
    }

    /// <summary>
    ///     See <see cref="IDictionary{TKey,TValue}.TryGetValue" /> for more information.
    /// </summary>
    bool IDictionary<TKey, IList<TValue>>.TryGetValue(TKey key, out IList<TValue> value)
    {
        value = this[key];
        return true;
    }

    /// <summary>
    ///     See <see cref="IDictionary{TKey,TValue}.Values" /> for more information.
    /// </summary>
    ICollection<IList<TValue>> IDictionary<TKey, IList<TValue>>.Values => _innerValues.Values;

    #endregion

    #region ICollection<KeyValuePair<TKey,List<TValue>>> Members

    /// <summary>
    ///     See <see cref="ICollection{TValue}.Add" /> for more information.
    /// </summary>
    void ICollection<KeyValuePair<TKey, IList<TValue>>>.Add(KeyValuePair<TKey, IList<TValue>> item)
    {
        ((ICollection<KeyValuePair<TKey, IList<TValue>>>)_innerValues).Add(item);
    }

    /// <summary>
    ///     See <see cref="ICollection{TValue}.Contains" /> for more information.
    /// </summary>
    bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Contains(KeyValuePair<TKey, IList<TValue>> item)
    {
        return ((ICollection<KeyValuePair<TKey, IList<TValue>>>)_innerValues).Contains(item);
    }

    /// <summary>
    ///     See <see cref="ICollection{TValue}.CopyTo" /> for more information.
    /// </summary>
    void ICollection<KeyValuePair<TKey, IList<TValue>>>.CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, IList<TValue>>>)_innerValues).CopyTo(array, arrayIndex);
    }

    /// <summary>
    ///     See <see cref="ICollection{TValue}.IsReadOnly" /> for more information.
    /// </summary>
    bool ICollection<KeyValuePair<TKey, IList<TValue>>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, IList<TValue>>>)_innerValues).IsReadOnly;

    /// <summary>
    ///     See <see cref="ICollection{TValue}.Remove" /> for more information.
    /// </summary>
    bool ICollection<KeyValuePair<TKey, IList<TValue>>>.Remove(KeyValuePair<TKey, IList<TValue>> item)
    {
        return ((ICollection<KeyValuePair<TKey, IList<TValue>>>)_innerValues).Remove(item);
    }

    #endregion
}