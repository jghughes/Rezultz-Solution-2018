using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetStd.Goodies.Mar2022;

namespace NetStd.Goodies.Xml.July2018
{
    public static class JghXmlNormalisationHelpers
    {

        public const string LookupTableIdElementName = "ID";
        public const string LookupTableValueElementName = "Value";

        // ReSharper disable once InconsistentNaming
        public static XElement GenerateXmlNormalisationLookupTableOfIDAndStringValue(XElement sourceDocument, string nameOfParentOfTheDesiredLookupElement, string nameOfDesiredLookupElement, string desiredNameOfLookupTable)
        {
            var subGroups = sourceDocument.Descendants(nameOfParentOfTheDesiredLookupElement)
                .Elements(nameOfDesiredLookupElement)
                .Where(xe => xe != null)
                .GroupBy(xe => JghString.TmLr(xe.Value)).ToArray();

            var collectionOfUniqueValuesOfTheThing = subGroups.OrderBy(xe => xe.Key).Select(xe => xe.Key).ToArray();

            var arrayOfLookupTableEntry = new List<XElement>();

            var i = 1;

            foreach (var value in collectionOfUniqueValuesOfTheThing)
            {
                var entry = new XElement(
                    nameOfDesiredLookupElement,
                    new XElement(LookupTableIdElementName, i),
                    new XElement(LookupTableValueElementName, value));

                arrayOfLookupTableEntry.Add(entry);

                i++;
            }

            var lookupTable =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent(desiredNameOfLookupTable,
                    arrayOfLookupTableEntry);

            return lookupTable;
        }

        public static XElement FindElementInXmlNormalisationLookupTableByStringValue(XContainer lookupTable, string value)
        {
            if (lookupTable == null) throw new ArgumentNullException(nameof(lookupTable));
            if (value == null) throw new ArgumentNullException(nameof(value));

            var entries = lookupTable.Descendants(LookupTableValueElementName)
                .ToArray();

            if (!entries.Any()) return null;

            var entryIAmLookingFor = (from e in entries
                where JghString.AreEqualIgnoreOrdinalCase(e.Value, value)
                select e.Parent).FirstOrDefault();

            return entryIAmLookingFor;
        }

        public static XElement FindElementInXmlNormalisationLookupTableById(XContainer lookupTable, int id)
        {
            if (lookupTable == null) throw new ArgumentNullException(nameof(lookupTable));

            var iDElements = lookupTable.Descendants(LookupTableIdElementName)
                .ToArray();

            if (!iDElements.Any()) return null;

            var lookupId = 0;

            var entryIAmLookingFor = (from xe in iDElements
                let isInt32 = int.TryParse(xe.Value, out lookupId)
                where isInt32 && lookupId == id
                select xe.Parent).FirstOrDefault();

            return entryIAmLookingFor;
        }

        public static string FindEntryInXmlNormalisationLookupTableById(XContainer lookupTable, int id)
        {
            if (lookupTable == null) throw new ArgumentNullException(nameof(lookupTable));

            string answer = null;

            var itemElement = FindElementInXmlNormalisationLookupTableById(lookupTable, id);

            if (itemElement == null) return null;

            var xe = itemElement.Element(LookupTableValueElementName);

            if (xe != null)
                answer = xe.Value;

            return answer;
        }

        public static int FindIdInXmlNormalisationLookupTableByStringValue(XElement lookupTable, string value)
        {
            if (lookupTable == null) throw new ArgumentNullException(nameof(lookupTable));

            var answer = 0;

            var itemElement = FindElementInXmlNormalisationLookupTableByStringValue(lookupTable, value);

            var xe = itemElement?.Element(LookupTableIdElementName);

            if (xe == null) return 0;

            if (int.TryParse(xe.Value, out var parsedAnswer))
                answer = parsedAnswer;

            return answer;
        }

        public static KeyValuePair<int, string>[] TransformXmlNormalisationTableToCollectionOfKeyValuePairOfIdAndStringValue(XElement lookupTable)
        {
            var answer = new List<KeyValuePair<int, string>>();

            if (lookupTable == null)
                return answer.ToArray();

            if (!lookupTable.Descendants(LookupTableIdElementName).Any())
                return answer.ToArray();

            foreach (var idElement in lookupTable.Descendants(LookupTableIdElementName))
            {
                var id = JghXElementHelpers.AsInt32(idElement);

                if (idElement.Parent == null) continue;

                var value = JghXElementHelpers.AsTmlr(
                    idElement.Parent.Element(LookupTableValueElementName));

                var kvp = new KeyValuePair<int, string>(id,
                    value); // aloocation id to kvp.Key is critical. it's used as a primary key all down the line

                answer.Add(kvp);
            }

            return answer.ToArray();
        }
    }
}