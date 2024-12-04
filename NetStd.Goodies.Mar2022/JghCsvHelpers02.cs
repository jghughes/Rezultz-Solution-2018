using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NetStd.Goodies.Mar2022
{
    public class JghCsvHelpers02
    {
        private const string Locus2 = nameof(JghCsvHelpers02);
        private const string Locus3 = "[NetStd.Goodies.Mar2022]";

        public static string TransformXElementContainingSimpleChildrenToRowOfCsvForExcel(XElement inputXElement, bool mustTransformElementNamesNotValues)
        {
            const string failure = "Unable to transform xml XElement to string of comma separated values.";
            const string locus = "[TransformXElementContainingSimpleChildrenToRowOfCsvForExcel]";


            if (inputXElement is null)
                if (inputXElement is null)
                    throw new ArgumentNullException(nameof(inputXElement));

            try
            {
                var listOfCsvFields = new List<string>();

                if (mustTransformElementNamesNotValues)
                {
                    foreach (var xElement in inputXElement.Elements())
                        listOfCsvFields.Add(xElement.Name.ToString());
                }
                else
                {
                    foreach (var xElement in inputXElement.Elements())
                    {
                        var candidateValue = JghString.TmLr(xElement.Value);

                        // CSV is a terrible format, but we have to live with it. we are forced to remove any commas from the value because commas are used as the delimiter

                        candidateValue = candidateValue.Replace(",", string.Empty);

                        var isInt32 = int.TryParse(candidateValue, out _);
                        var isInt64 = long.TryParse(candidateValue, out _);
                        var isDouble = double.TryParse(candidateValue, out _);
                        var isByte = byte.TryParse(candidateValue, out _);
                        var isTimeSpan = TimeSpan.TryParse(candidateValue, out var candidateTimeSpan);
                        var isDate = DateTime.TryParse(candidateValue, out var candidateDate);

                        // NB. this test must come first
                        if (isInt32 || isInt64 || isDouble || isByte)
                        {
                            listOfCsvFields.Add(candidateValue);
                            continue;
                        }

                        if (isTimeSpan)
                        {
                            var formattedValue = candidateTimeSpan.ToString("G"); // this format is essential for Excel to reliably recognise the value for what it is
                            listOfCsvFields.Add(JghString.Enclose(formattedValue, '"'));
                            continue;
                        }
                        if (isDate)
                        {
                            // Note: to successfully interpret and display date and DateTime formats in Excel, you must apply the appropriate format through 
                            // the Format Cells dialog. For DateTimes, in the Type field, enter the custom format: yyyy-mm-dd hh:mm:ss AM/PM

                            string formattedValue;

                            if (candidateDate.Hour == 0 && candidateDate.Minute == 0 && candidateDate.Second == 0 && candidateDate.Millisecond == 0)
                            {
                                formattedValue = candidateDate.ToString("yyyy-MM-dd"); // the only date format excel will reliably recognise as a date.
                            }
                            else
                            {
                                formattedValue = candidateDate.ToString("yyyy-MM-dd HH:mm:ss"); // Excel can't handle milliseconds so we have to drop them
                            }
                            listOfCsvFields.Add(JghString.Enclose(formattedValue, '"'));
                            continue;
                        }

                        // because it is a string, the value must be enclosed in double quotes, but only if it is not already enclosed

                        candidateValue = Encoding.UTF8.GetString(Encoding.Default.GetBytes(candidateValue)); // properly handle UTF8 characters - such as apostrophe for example

                        if (candidateValue.StartsWith("\"") && candidateValue.EndsWith("\""))
                        {
                            listOfCsvFields.Add(candidateValue);
                            continue;
                        }

                        listOfCsvFields.Add(JghString.Enclose(candidateValue, '"'));
                    }
                }

                var resultingRowOfCsv = string.Join(",", listOfCsvFields);

                return resultingRowOfCsv;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        public static string TransformXElementContainingArrayOfChildElementsToCsvFileContentsForExcel(XElement parentElementContainingListOfChildElements)
        {
            const string failure = "Unable to transform xml data into the contents for a comma separated file.";
            const string locus = "[TransformXElementContainingArrayOfChildElementsToCsvFileContentsForExcel]";

            try
            {
                #region null checks

                if (parentElementContainingListOfChildElements is null || parentElementContainingListOfChildElements.IsEmpty)
                    throw new JghAlertMessageException("Nothing found. Nothing to convert to .csv data.");

                #endregion

                var items = parentElementContainingListOfChildElements.Elements().ToList();

                if (!items.Any())
                    throw new JghAlertMessageException("No elements of data.");

                var sb = new StringBuilder();

                var headerLineOfCsv = TransformXElementContainingSimpleChildrenToRowOfCsvForExcel(items.First(), true); // arbitrarily use first element to deduce csv column headers

                sb.AppendLine(headerLineOfCsv);

                foreach (var item in items)
                {
                    var lineOfCsv = TransformXElementContainingSimpleChildrenToRowOfCsvForExcel(item, false);

                    sb.AppendLine(lineOfCsv);
                }

                var csvFileAsSingleLongStringWithMultipleLineBreaks = sb.ToString();

                return csvFileAsSingleLongStringWithMultipleLineBreaks;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }
    }
}
