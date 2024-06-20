using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NetStd.Goodies.Mar2022;

/// <summary>
///     A nifty mapper for a single flat XElement or row of csv. Maps the data line-item into a keyed dictionary of its constituent values.
/// </summary>
public class JghMapper
{
    #region ctor

    /// <summary>
    ///     Maps the child XElements within a parent XElement into a keyed dictionary of its constituent values. 
    /// </summary>
    /// <param name="xElementBeingMapped">The XElement containing the named child XElements with values.</param>
    /// <param name="newNameDirtyNamePairs">A map of key/name pairs where the key is used to look up the mapped value of the named element. Duplicate names are permitted for added flexibility.</param>
    public JghMapper(XElement xElementBeingMapped, IEnumerable<KeyValuePair<string, string>> newNameDirtyNamePairs)
    {
        _xElementBeingMapped = xElementBeingMapped;

        AssembleDictionaryOfDirtyNameOldNamePairs(newNameDirtyNamePairs);

        PluckNewNameValuePairsOutOfXElement();
    }

    /// <summary>
    ///     Maps the values in a row of csv data into a keyed dictionary of the values. 
    /// </summary>
    /// <param name="csvHeaderRow">A row of Excel-style column headers.</param>
    /// <param name="rowOfCsvBeingMapped">THe row of csv containing values associated with specified column headers.</param>
    /// <param name="newNameDirtyNamePairs">A map of key/name pairs where the key is used to look up the mapped value of the named element. Duplicate names are permitted for added flexibility.</param>
    public JghMapper(string csvHeaderRow, string rowOfCsvBeingMapped, IEnumerable<KeyValuePair<string, string>> newNameDirtyNamePairs)
    {
        _rowOfCsvBeingMapped = rowOfCsvBeingMapped;

        AssembleDictionaryOfDirtyNameOldNamePairs(newNameDirtyNamePairs);

        PluckNewNameValuePairsOutOfRowOfCsv(csvHeaderRow);
    }

    #endregion

    #region helpers

    private void PluckNewNameValuePairsOutOfXElement()
    {
        foreach (var kvp in _dictionaryOfNewNameOldNamePairs)
        {
            var associatedDataNames = kvp.Value.ToArray();

            var accessKey = kvp.Key;

            foreach (var name in associatedDataNames)
            {

                XName kosherName;

                try
                {

                    kosherName = XName.Get(name, string.Empty);
                    // many different characters are forbidden in XNames, hence the need to skip the name if XName.Get() blows up.
                    // Forbidden characters include common characters such as spaces,:,#,!,@,*,(,),/, \
                    // Access automatically replaces forbidden characters with their hexadecimal characters when exporting a worksheet as xml, but Excel does not when exporting csv. Anyone's guess what a user might do.
                }
                catch (Exception e)
                {
                    continue;
                }


                var childElement = _xElementBeingMapped.Element(kosherName); // blows up if name contains a blank

                if (childElement is null) continue;

                if (!_dictionaryOfDataValuesPluckedOutOfSource.ContainsKey(accessKey))
                {
                    _dictionaryOfDataValuesPluckedOutOfSource.Add(accessKey, childElement.Value);

                }
            }
        }
    }

    private void PluckNewNameValuePairsOutOfRowOfCsv(string headerRow)
    {
        var columnHeadings = headerRow.Split(',');

        var cellValues = _rowOfCsvBeingMapped.Split(',');

        var maxColumns = Math.Min(columnHeadings.Length, cellValues.Length); // in case the row has more columns than the header, or vice versa

        for (var i = 0; i < maxColumns; i++)
        {
            var columnHeading = columnHeadings[i]; // the name of this cell

            if (string.IsNullOrWhiteSpace(columnHeading)) continue;

            var cellValue = cellValues[i];

            var candidateMapKeys = _dictionaryOfNewNameOldNamePairs.FindKeysByValue(columnHeading).ToArray(); //from the provided map, dig out the key/s associated with the heading of this cell

            var key = candidateMapKeys.FirstOrDefault();  // valid for there to be multiple keys, the kludge is to use the first occurring

            if (string.IsNullOrWhiteSpace(key)) continue;

            if (!_dictionaryOfDataValuesPluckedOutOfSource.ContainsKey(key))
                _dictionaryOfDataValuesPluckedOutOfSource.Add(key, cellValue);
        }
    }

    private void AssembleDictionaryOfDirtyNameOldNamePairs(IEnumerable<KeyValuePair<string, string>> newNameOldNamePairs)
    {
        foreach (var kvp in newNameOldNamePairs)
            if (!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                _dictionaryOfNewNameOldNamePairs.Add(kvp.Key, kvp.Value);
    }

    #endregion

    #region variables

    private readonly XElement _xElementBeingMapped;

    private readonly string _rowOfCsvBeingMapped;

    private readonly JghListDictionary<string, string> _dictionaryOfNewNameOldNamePairs = []; // key is the destination name of the element, value is the list of source names for the destination element

    private readonly JghListDictionary<string, string> _dictionaryOfDataValuesPluckedOutOfSource = [];

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

        if (!_dictionaryOfDataValuesPluckedOutOfSource.ContainsKey(key))
            return string.Empty;

        var candidate = _dictionaryOfDataValuesPluckedOutOfSource[key].FirstOrDefault(z => !string.IsNullOrWhiteSpace(z));

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