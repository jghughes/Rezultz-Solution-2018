using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NetStd.Goodies.Mar2022;

/// <summary>
///     A nifty translator for a single line-item of raw data in the form of a flat XElement or a row of csv. Translates the line-item into a collection of key-value pairs with designated names.
/// </summary>
public class JghMapper
{
    #region ctor

    /// <summary>
    ///     Translates the child XElements within a parent XElement into (a subset? of) designated-name/value properties.
    /// </summary>
    /// <param name="xElementBeingTranslated">The XElement containing the child XElements, one or more of which has a known or suspected name.</param>
    /// <param name="elementNameMappings">A mapping of key/value pairs where each key is the designated name of a desired property and each key maps to the known or suspected name/s of the associated child XElement. Duplicate keys (catering for multiple candidates) are permitted.</param>
    public JghMapper(XElement xElementBeingTranslated, IEnumerable<KeyValuePair<string, string>> elementNameMappings)
    {
        _xElementBeingTranslated = xElementBeingTranslated;

        ConvertElementNameMappingsListToDictionary(elementNameMappings);

        DoXmlMapping();
    }

    /// <summary>
    ///     Translates the cells in a row of csv data into (a subset? of) designated-name/value properties. 
    /// </summary>
    /// <param name="csvHeaderRow">A row of Excel column headers. The headers are the names of the cells in the row.</param>
    /// <param name="rowOfCsvBeingTranslated">THe row of csv values. Each value corresponds to a cell in Excel.</param>
    /// <param name="elementNameMappings">A mapping of key/value pairs where each key is the designated name of a desired property and each value maps to the known or suspected name/s of an associated cell. Duplicate keys (catering for multiple candidates) are permitted.</param>
    public JghMapper(string csvHeaderRow, string rowOfCsvBeingTranslated, IEnumerable<KeyValuePair<string, string>> elementNameMappings)
    {
        _rowOfCsvBeingTranslated = rowOfCsvBeingTranslated;

        ConvertElementNameMappingsListToDictionary(elementNameMappings);

        DoCsvMapping(csvHeaderRow);
    }

    #endregion

    #region helpers

    private void DoXmlMapping()
    {
        foreach (var kvp in _mapOfElementNamesAsDict)
        {
            var candidatesInSourceLineItem = kvp.Value.ToArray();

            var mapKey = kvp.Key;

            foreach (var candidate in candidatesInSourceLineItem)
            {
                var childElement = _xElementBeingTranslated.Element(candidate);

                if (childElement is null) continue;

                if (!_mappedLineItemAsDict.ContainsKey(mapKey))
                    _mappedLineItemAsDict.Add(mapKey, childElement.Value);
            }
        }
    }

    private void DoCsvMapping(string headerRow)
    {
        var columnHeadings = headerRow.Split(',');

        var cellValues = _rowOfCsvBeingTranslated.Split(',');

        var maxColumns = Math.Min(columnHeadings.Length, cellValues.Length); // in case the row has more columns than the header, or vice versa

        for (var i = 0; i < maxColumns; i++)
        {
            var columnHeading = columnHeadings[i];

            if (string.IsNullOrWhiteSpace(columnHeading)) continue;

            var cellValue = cellValues[i];

            var candidateMapKeys = _mapOfElementNamesAsDict.FindKeysByValue(columnHeading).ToArray(); //map the key to the destination name

            var mapKey = candidateMapKeys.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mapKey)) continue;

            if (!_mappedLineItemAsDict.ContainsKey(mapKey))
                _mappedLineItemAsDict.Add(mapKey, cellValue);
        }
    }

    private void ConvertElementNameMappingsListToDictionary(IEnumerable<KeyValuePair<string, string>> theKeyValuePairs)
    {
        foreach (var kvp in theKeyValuePairs)
            if (!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                _mapOfElementNamesAsDict.Add(kvp.Key, kvp.Value);
    }

    #endregion

    #region variables

    private readonly XElement _xElementBeingTranslated;

    private readonly string _rowOfCsvBeingTranslated;

    private readonly JghListDictionary<string, string> _mapOfElementNamesAsDict = []; // key is the destination name of the element, value is the list of source names for the destination element

    private readonly JghListDictionary<string, string> _mappedLineItemAsDict = [];

    #endregion

    #region public methods

    /// <summary>
    ///     returns first occurrence of the non-empty value for the key, or string.Empty if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetAsString(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (!_mappedLineItemAsDict.ContainsKey(key))
            return string.Empty;

        var candidate = _mappedLineItemAsDict[key].FirstOrDefault(z => !string.IsNullOrWhiteSpace(z));

        if (string.IsNullOrWhiteSpace(candidate))
            return string.Empty;

        return candidate;
    }

    /// <summary>
    ///     returns the first occurrence of the value for the key that successfully parses to bool, or false if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetAsBool(string key)
    {
        var candidate = GetAsString(key);

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
    public DateTime GetAsDateTime(string key)
    {
        var candidate = GetAsString(key);

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
    public int GetAsInt(string key)
    {
        var candidate = GetAsString(key);

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
    public double GetAsDouble(string key)
    {
        var candidate = GetAsString(key);

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
    public decimal GetAsDecimal(string key)
    {
        var candidate = GetAsString(key);

        if (string.IsNullOrWhiteSpace(candidate))
            return 0;

        if (decimal.TryParse(candidate, out var result))
            return result;

        return 0;
    }

    #endregion
}