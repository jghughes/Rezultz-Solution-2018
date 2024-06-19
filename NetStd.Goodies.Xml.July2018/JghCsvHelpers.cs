using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetStd.Goodies.Mar2022;

namespace NetStd.Goodies.Xml.July2018
{
    public static class JghCsvHelpers
    {
        public static bool TryConvertRegistrationDeskDataFromCsvToXml(
            string[] arrayOfCsvStrings,
            string desiredNameOfRoot,
            string desiredNameOfChildElement,
            out XDocument resultingXDocument,
            out string errorMessage)
        {
            if (arrayOfCsvStrings is null)
            {
                errorMessage = "Null argument. csv text is null.";
                resultingXDocument = null;
                return false;
            }
            if (string.IsNullOrWhiteSpace(desiredNameOfRoot))
            {
                errorMessage = "name of root element must be specified.";
                resultingXDocument = null;
                return false;
            }
            if (string.IsNullOrWhiteSpace(desiredNameOfChildElement))
            {
                errorMessage = "name of child elements must be specified.";
                resultingXDocument = null;
                return false;
            }
            try
            {
                var listOfCsvStrings = arrayOfCsvStrings.ToList();

                var listOfXElements = new List<XElement>();

                foreach (var rowOfCommaSeparatedValues in listOfCsvStrings)
                {
                    var values = rowOfCommaSeparatedValues.Split(',');

                    var childElement = new XElement(
                        desiredNameOfChildElement,
                        new XElement("Bib", values[0]),
                        new XElement("First", values[1]),
                        new XElement("Last", values[2]),
                        new XElement("Age", values[3]),
                        new XElement("Cat", values[4]),
                        new XElement("Sex", values[5]),
                        new XElement("City", values[6]),
                        new XElement("Race", values[7]),
                        new XElement("Course", values[8]),
                        new XElement("Splits", values[9]),
                        new XElement("TrophyPts", values[10]));

                    listOfXElements.Add(childElement);
                }

                var rootElement = new XElement(
                    desiredNameOfRoot,
                    new XAttribute("generated", DateTime.UtcNow),
                    new XAttribute("number of items", arrayOfCsvStrings.Count().ToString()));

                rootElement.Add(listOfXElements);

                var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("registration desk data"));

                xDoc.Add(rootElement);

                //success!

                errorMessage = null;

                resultingXDocument = xDoc;

                return true;
            }
            catch (Exception ex)
            {
                errorMessage =
                    $"Exception occurred. Conversion of {arrayOfCsvStrings.Count()} rows of comma separated values to xml failed. {ex.Message}";
                resultingXDocument = null;
                return false;
            }
        }

        public static bool TryConvertOneOrMoreRowsOfCsvFieldsToArrayOfConstituentFields(string[] rowsOfCsvStrings,
            out string[] resultingArray, out string errorMessage)
        {
            if (rowsOfCsvStrings is null || !rowsOfCsvStrings.Any())
            {
                errorMessage = "Null argument. list of expected field names is null or empty.";
                resultingArray = null;
                return false;
            }
            try
            {
                List<string> constituentStrings = [];

                foreach (string[] fields in rowsOfCsvStrings.Select(rowOfCommaSeparatedValues =>
                    rowOfCommaSeparatedValues.Split(',')))
                    constituentStrings.AddRange(fields);

                //success!

                errorMessage = null;

                resultingArray = constituentStrings.ToArray();

                return true;
            }
            catch (Exception ex)
            {
                errorMessage =
                    $"Exception occurred. Conversion of {rowsOfCsvStrings.Count()} rows of comma separated values to array of constituent strings failed. {ex.Message}";
                resultingArray = null;
                return false;
            }
        }

        public static bool TryConvertRowsOfCsvToXml(
            string[] arrayOfCsvStrings,
            string[] expectedFieldNames,
            string desiredNameOfRootParentElement,
            string desiredNameOfChildElement,
            out XDocument resultingXDocument,
            out string errorMessage)
        {
            if (arrayOfCsvStrings is null)
            {
                errorMessage = "Null argument. csv text is null.";
                resultingXDocument = null;
                return false;
            }
            if (expectedFieldNames is null || !expectedFieldNames.Any())
            {
                errorMessage = "Null argument. list of expected field names is null or empty.";
                resultingXDocument = null;
                return false;
            }

            if (string.IsNullOrWhiteSpace(desiredNameOfRootParentElement))
            {
                errorMessage =
                    "Error. Blank parameter. Desired name of root element of Xml document to be generated must be specified.";
                resultingXDocument = null;
                return false;
            }
            if (string.IsNullOrWhiteSpace(desiredNameOfChildElement))
            {
                errorMessage =
                    "Error. Blank parameter. Desired name of child elements of Xml document to be generated must be specified.";
                resultingXDocument = null;
                return false;
            }
            try
            {
                var listOfCsvStrings = arrayOfCsvStrings.ToList();

                var childElements = new List<XElement>();

                foreach (var rowOfCommaSeparatedValues in listOfCsvStrings)
                {
                    var fields = rowOfCommaSeparatedValues.Split(',');

                    var childElement = new XElement(desiredNameOfChildElement);

                    var numberOfFieldsToInspect = Math.Min(fields.Count(), expectedFieldNames.Count());
                    //var numberOfFieldsToInspect = JghMath.Min(fields.Count(), expectedFieldNames.Count());

                    for (var index = 0; index < numberOfFieldsToInspect; index++)
                        childElement.Add(new XElement(expectedFieldNames[index], fields[index]));

                    childElements.Add(childElement);
                }

                var rootElement = new XElement(
                    desiredNameOfRootParentElement,
                    new XAttribute("when", DateTime.Now.ToString(JghDateTime.Iso8601Pattern)),
                    new XAttribute("itemCount", childElements.Count().ToString()),
                    new XAttribute("generated", DateTime.UtcNow));

                rootElement.Add(childElements);

                var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("------- data converted from csv format -------"));

                xDoc.Add(rootElement);

                //success!

                errorMessage = null;

                resultingXDocument = xDoc;

                return true;
            }
            catch (Exception ex)
            {
                errorMessage =
                    $"Exception occurred. Conversion of {arrayOfCsvStrings.Count()} rows of comma separated values to xml failed. {ex.Message}";
                resultingXDocument = null;
                return false;
            }
        }
    }
}