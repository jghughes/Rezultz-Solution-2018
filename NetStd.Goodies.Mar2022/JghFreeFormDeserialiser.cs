using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NetStd.Goodies.Mar2022;

/// <summary>
///     A nifty mapper for a single flat XElement or row of csv. Maps the data line-item into a keyed dictionary of its
///     constituent values.
/// </summary>
public class JghFreeFormDeserialiser
{
    #region ctors

    /// <summary>
    ///     Maps the child XElements within a parent XElement into a keyed dictionary of the values.
    /// </summary>
    /// <param name="newKeySourceElementNamePairs"></param>
    public JghFreeFormDeserialiser(IEnumerable<KeyValuePair<string, string>> newKeySourceElementNamePairs)
    {
        AssembleDictionaryOfCleanSourceXElementNamePairs(newKeySourceElementNamePairs);
    }

    /// <summary>
    ///     Maps the cells in a row of csv data into a keyed dictionary of the values.
    /// </summary>
    /// <param name="csvHeaderRow">A row of Excel-style column headers.</param>
    /// <param name="newKeySourceElementNamePairs"></param>
    public JghFreeFormDeserialiser(string csvHeaderRow, IEnumerable<KeyValuePair<string, string>> newKeySourceElementNamePairs)
    {
        _csvHeaderRow = csvHeaderRow;

        AssembleDictionaryOfSourceCsvElementNamePairs(newKeySourceElementNamePairs);
    }

    #endregion

    #region props

    public List<KeyValuePair<string, string>> RejectedKeySourceElementNamePairs { get; } = [];

    public List<KeyValuePair<string, string>> AcceptedKeySourceElementNamePairs => _dictionaryOfNewKeySourceElementNamePairs.FindAllElements().ToList();

    public List<KeyValuePair<string, string>> ValuesPluckedOutOfSourceElement => _dictionaryOfElementValuesPluckedOutOfSource.FindAllElements().ToList();

    #endregion

    #region helpers

    private void PluckValuesOutOfXElement()
    {
        foreach (var kvp in _dictionaryOfNewKeySourceElementNamePairs)
        {
            var sourceElementNames = kvp.Value.ToArray();

            var newKey = kvp.Key;

            foreach (var elementName in sourceElementNames)
            {
                var candidateXName = XName.Get(elementName, string.Empty);

                var sourceElement = _xElementBeingDeserialized.Element(candidateXName); // blows up if name contains a blank or invalid character

                if (sourceElement is null) continue;

                _dictionaryOfElementValuesPluckedOutOfSource.Add(newKey, sourceElement.Value);
            }
        }
    }

    private void PluckValuesOutOfRowOfCsv()
    {
        if (string.IsNullOrWhiteSpace(_csvHeaderRow)) return;

        var columnHeadings = _csvHeaderRow.Split(',');

        var cellValues = _rowOfCsvBeingDeserialized.Split(',');

        var maxColumns = Math.Min(columnHeadings.Length, cellValues.Length); // in case the row has more columns than the header, or vice versa

        for (var i = 0; i < maxColumns; i++)
        {
            var columnHeading = columnHeadings[i]; // the name of this cell

            if (string.IsNullOrWhiteSpace(columnHeading)) continue;

            var sourceElementValue = cellValues[i];

            var candidateNewKeys = _dictionaryOfNewKeySourceElementNamePairs.FindKeysByValue(columnHeading).ToArray(); //from the provided map, dig out the key/s associated with the heading of this cell

            var newKey = candidateNewKeys.FirstOrDefault(); // valid for there to be multiple keys, the kludge is to use the first occurring

            if (string.IsNullOrWhiteSpace(newKey)) continue;

            _dictionaryOfElementValuesPluckedOutOfSource.Add(newKey, sourceElementValue);
        }
    }

    private void AssembleDictionaryOfCleanSourceXElementNamePairs(IEnumerable<KeyValuePair<string, string>> newKeySourceElementNamePairs)
    {
        foreach (var kvp in newKeySourceElementNamePairs)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                continue;

            try
            {
                var dummy = XName.Get(kvp.Key, string.Empty);
                var dummy2 = XName.Get(kvp.Value, string.Empty);
                // many different characters are forbidden in XNames, hence the need to skip the name if XName.Get() blows up.
                // Forbidden characters include common characters such as spaces,:,#,!,@,*,(,),/, \
                // Access automatically replaces forbidden characters with their hexadecimal characters when exporting a worksheet as xml, but Excel does not when exporting csv. Anyone's guess what a user might do.
            }
            catch (Exception)
            {
                RejectedKeySourceElementNamePairs.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value));

                continue;
            }

            _dictionaryOfNewKeySourceElementNamePairs.Add(kvp.Key, kvp.Value);
        }
    }

    private void AssembleDictionaryOfSourceCsvElementNamePairs(IEnumerable<KeyValuePair<string, string>> newKeySourceElementNamePairs)
    {
        foreach (var kvp in newKeySourceElementNamePairs)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                continue;

            _dictionaryOfNewKeySourceElementNamePairs.Add(kvp.Key, kvp.Value);
        }
    }

    #endregion

    #region variables

    private XElement _xElementBeingDeserialized;

    private readonly string _csvHeaderRow = string.Empty;

    private string _rowOfCsvBeingDeserialized;

    private readonly JghListDictionary<string, string> _dictionaryOfNewKeySourceElementNamePairs = []; // key is the destination name of the element, value is the list of source names for the destination element

    private JghListDictionary<string, string> _dictionaryOfElementValuesPluckedOutOfSource = []; // using a list dict because handling duplicates is a requirement - a clever feature

    #endregion

    #region public methods

    public List<KeyValuePair<string, string>> Deserialise(XElement xElementBeingDeserialized)
    {
        _xElementBeingDeserialized = xElementBeingDeserialized;

        _dictionaryOfElementValuesPluckedOutOfSource = [];

        PluckValuesOutOfXElement();

        return ValuesPluckedOutOfSourceElement;
    }

    public List<KeyValuePair<string, string>> Deserialise(string rowOfCsvBeingDeserialized)
    {
        _rowOfCsvBeingDeserialized = rowOfCsvBeingDeserialized;

        _dictionaryOfElementValuesPluckedOutOfSource = [];

        PluckValuesOutOfRowOfCsv();

        return ValuesPluckedOutOfSourceElement;
    }

    /// <summary>
    ///     returns first occurrence of the non-empty value for the key, or string.Empty if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetFirstOrBlankAsString(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (!_dictionaryOfElementValuesPluckedOutOfSource.ContainsKey(key))
            return string.Empty;

        var candidate = _dictionaryOfElementValuesPluckedOutOfSource[key].FirstOrDefault(z => !string.IsNullOrWhiteSpace(z));

        if (string.IsNullOrWhiteSpace(candidate))
            return string.Empty;

        return candidate;
    }

    /// <summary>
    ///     returns the first occurrence of the value for the key that successfully parses to bool, or false if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetFirstOrDefaultAsBool(string key)
    {
        var candidate = GetFirstOrBlankAsString(key);

        if (string.IsNullOrWhiteSpace(candidate))
            return false;

        if (bool.TryParse(candidate, out var result))
            return result;

        return false;
    }

    /// <summary>
    ///     returns the first occurrence of the value for the key that successfully parses to DateTime, or DateTime.MinValue if
    ///     not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public DateTime GetFirstOrFallbackAsDateTime(string key)
    {
        var candidate = GetFirstOrBlankAsString(key);

        if (string.IsNullOrWhiteSpace(candidate))
            return DateTime.MinValue;

        if (DateTime.TryParse(candidate, out var result))
            return result;

        return DateTime.MinValue;
    }

    /// <summary>
    ///     returns the first occurrence of the value for the key that successfully parses to int, or 0 if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int GetFirstOrDefaultAsInt(string key)
    {
        var candidate = GetFirstOrBlankAsString(key);

        if (string.IsNullOrWhiteSpace(candidate))
            return 0;

        if (int.TryParse(candidate, out var result))
            return result;

        return 0;
    }

    /// <summary>
    ///     returns the first occurrence of the value for the key that successfully parses to double, or 0 if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public double GetFirstOrDefaultAsDouble(string key)
    {
        var candidate = GetFirstOrBlankAsString(key);

        if (string.IsNullOrWhiteSpace(candidate))
            return 0;

        if (double.TryParse(candidate, out var result))
            return result;

        return 0;
    }

    /// <summary>
    ///     returns the first occurrence of the value for the key that successfully parses to decimal, or 0 if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public decimal GetFirstOrDefaultAsDecimal(string key)
    {
        var candidate = GetFirstOrBlankAsString(key);

        if (string.IsNullOrWhiteSpace(candidate))
            return 0;

        if (decimal.TryParse(candidate, out var result))
            return result;

        return 0;
    }

    #endregion
}