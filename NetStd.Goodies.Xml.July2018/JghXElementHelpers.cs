using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;

namespace NetStd.Goodies.Xml.July2018
{
    // N.B. be sure not to improve these to be extension methods and invoke them as such. I did so, and it came back to bite me because of the occurance of nulls. lesson learned.

    public static class JghXElementHelpers
    {
        private const string Locus2 = nameof(JghXElementHelpers);
        private const string Locus3 = "[NetStd.Goodies.Xml.July2018]";

        public const string JghTimeNowFormat = "dddd MMMM dd, yyyy  hh:mm tt";

        public static string AsTmLr(string someString)
    {
        return someString?.Trim().ToLowerInvariant() ?? string.Empty;
    }

        public static string AsTmlr(XElement someElement)
    {
        return someElement is null
            ? string.Empty
            : AsTmLr(someElement.Value);
    }

        public static string AsTrimmedString(XElement someElement)
    {
        // NB don't use TmLr here. this method must'nt suppress case. We want to retain case sensitivity

        return someElement is null
            ? string.Empty
            : string.IsNullOrWhiteSpace(someElement.Value)
                ? string.Empty
                : someElement.Value.Trim();
    }

        public static int AsInt32(XElement someElement)
    {
        if (someElement is null) return 0;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return 0;


        var conversionSucceeded = int.TryParse(AsTmLr(someElement.Value), out var parseResult);

        return conversionSucceeded ? parseResult : 0;
    }

        public static long AsInt64(XElement someElement)
    {
        if (someElement is null) return 0;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return 0;

        var conversionSucceeded = int.TryParse(AsTmLr(someElement.Value), out var parseResult);

        return conversionSucceeded ? parseResult : 0;
    }

        public static double AsDouble(XElement someElement)
    {
        if (someElement is null) return 0;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return 0;

        var conversionSucceeded = double.TryParse(AsTmLr(someElement.Value), out var parseResult);

        return conversionSucceeded ? parseResult : 0;
    }

        public static bool AsBool(XElement someElement)
    {
        if (someElement is null) return false;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return false;

        return AsBool(someElement.Value);
    }

        private static bool AsBool(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        return (AsTmLr(value) == "1") | (AsTmLr(value) == "true") |
               (AsTmLr(value) == "yes") | (AsTmLr(value) == "y");
    }


        public static string AsTimespanString(XElement someElement)
    {
        if (someElement is null) return null;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return null;

        var conversionSucceeded = TimeSpan.TryParse(AsTmLr(someElement.Value), out var parseResult);

        return conversionSucceeded ? parseResult.ToString("G") : null;
    }

        public static DateTime AsDateTime(XElement someElement)
    {
        var zz = DateTime.MaxValue;

        if (someElement is null) return zz;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return zz;

        var conversionSucceeded = DateTime.TryParse(AsTmLr(someElement.Value), out var parseResult);

        return conversionSucceeded ? parseResult : zz;
    }

        public static string AsStringRepresentingBestGuessTypeOrBlankIfZero(XElement someElement)
    {
        if (someElement is null)
            return null;

        if (string.IsNullOrWhiteSpace(someElement.Value))
            return string.Empty;

        return JghString.ToBestGuessStringOrBlankIfNearZero(someElement.Value, 2,
            JghFormatSpecifiers.DecimalFormat2Dp);
    }

        public static string ChildElementValueAsString(XElement parentXe, string nameOfChild)
    {
        var childXe = parentXe?.Element(nameOfChild);

        if (childXe is null)
            return string.Empty;

        var value = childXe.Value; // empirically this blows up when the Value itself contains xml : for that use our cunning alternative method: GetChildElementValueVerbatim()

        return value.Trim();
    }

        public static string NameOfElementOrBlankIfElementIsNull(XElement somElement)
    {
        return somElement?.Name.ToString() ?? string.Empty;
    }

        public static XElement ElementOrDummyIfNull(XElement someElement)
    {
        return someElement ?? new XElement("Dummy", "dummy");
    }

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
                        string formattedValue;

                        if (candidateDate.Hour == 0 && candidateDate.Minute == 0 && candidateDate.Second == 0 && candidateDate.Millisecond == 0)
                        {
                            formattedValue = candidateDate.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            formattedValue = candidateDate.ToString("yyyy-MM-dd HH:mm:ss"); // Excel can't handle milliseconds so we have to drop them
                        }
                        listOfCsvFields.Add(JghString.Enclose(formattedValue, '"'));
                        continue;
                    }


                    // because it is a string, the value must be enclosed in double quotes, but only if it is not already enclosed

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

        public static byte[] TransformXElementToBytesUtf8(XElement inputXElement, SaveOptions saveOption)
    {
        const string failure = "Unable to transform xml XElement to string then byte array";
        const string locus = "[TransformXElementToBytesUtf8]";

        byte[] answerAsBytes;

        try
        {
            if (inputXElement is null) throw new ArgumentNullException(nameof(inputXElement));

            var answerAsString = inputXElement.ToString(saveOption);

            answerAsBytes = JghConvert.ToBytesUtf8FromString(answerAsString);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        return answerAsBytes;
    }

        public static byte[] TransformXDocumentToBytesUtf8(XDocument inputXDocument, SaveOptions saveOption)
    {
        const string failure = "Unable to transform xml XDocument to string then byte array";
        const string locus = "[TransformXDocumentToBytesUtf8]";

        if (inputXDocument is null) throw new ArgumentNullException(nameof(inputXDocument));

        byte[] answerAsBytes;

        try
        {
            var answerAsString = inputXDocument.ToString(saveOption);

            answerAsBytes = JghConvert.ToBytesUtf8FromString(answerAsString);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        return answerAsBytes;
    }

        public static XElement TransformBytesToXElement(byte[] inputByteArray)
    {
        var failure = "Unable to transform bytes to xml XElement.";
        const string locus = "[TransformXDocumentToBytesUtf8]";

        XElement answerAsXElement;

        try
        {
            if (inputByteArray is null) throw new ArgumentNullException(nameof(inputByteArray));

            using var memStream = new MemoryStream();

            memStream.Write(inputByteArray, 0, inputByteArray.Length);

            answerAsXElement = XElement.Load(new MemoryStream(memStream.ToArray()));
        }
        catch (Exception ex)
        {
            failure = JghString.ConcatAsSentences(failure,
                "Among other reasons, this can occur if the bytes are unusable because they are compressed or if the underlying file they represent is not a valid xml file or if there is a syntax or semantic or format error in the xml text.");
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        return answerAsXElement;
    }

        public static XElement WrapXElementInNewParentElement(string desiredNameOfNewParentXElement,
            XElement inputXElement)
    {
        const string failure = "Unable to wrap xml XElement in new parent.";
        const string locus = "[WrapXElementInNewParentElement]";

        try
        {
            if (desiredNameOfNewParentXElement is null)
                throw new ArgumentNullException(nameof(desiredNameOfNewParentXElement));

            if (inputXElement is null) throw new ArgumentNullException(nameof(inputXElement));

            var answerAsNewParent = new XElement(
                desiredNameOfNewParentXElement,
                new XAttribute("itemCount", "1"),
                inputXElement
            );

            return answerAsNewParent;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XElement WrapCollectionOfXElementsInNewParent(string desiredNameOfNewParentXElement,
            IEnumerable<XElement> inputCollectionOfXElements)
    {
        const string failure = "Unable to wrap collection of xml XElements in new parent.";
        const string locus = "[WrapCollectionOfXElementsInNewParent]";

        try
        {
            if (desiredNameOfNewParentXElement is null)
                throw new ArgumentNullException(nameof(desiredNameOfNewParentXElement));

            if (inputCollectionOfXElements is null)
                throw new ArgumentNullException(nameof(inputCollectionOfXElements));

            var newParent = new XElement(
                desiredNameOfNewParentXElement,
                new XAttribute("itemCount", inputCollectionOfXElements.Count()),
                new XAttribute("generated", $"{DateTime.Now.DayOfWeek} {DateTime.Now:f}"),
                inputCollectionOfXElements
            );

            return newParent;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XElement WrapCollectionOfXElementsInNewParentWithNoAttributes(string desiredNameOfNewParentXElement,
            IEnumerable<XElement> inputCollectionOfXElements)
    {
        const string failure = "Unable to wrap collection of xml XElements in new parent.";
        const string locus = "[WrapCollectionOfXElementsInNewParent]";

        try
        {
            if (desiredNameOfNewParentXElement is null)
                throw new ArgumentNullException(nameof(desiredNameOfNewParentXElement));

            if (inputCollectionOfXElements is null)
                throw new ArgumentNullException(nameof(inputCollectionOfXElements));

            var newParent = new XElement(
                desiredNameOfNewParentXElement,
                inputCollectionOfXElements
            );

            return newParent;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XDocument WrapXElementInNewXDocument(string desiredNameOfNewParent, XElement inputXElement)
    {
        const string failure = "Unable to wrap xml XElement in new parent as an XDocument.";
        const string locus = "[WrapXElementInNewXDocument]";

        try
        {
            if (desiredNameOfNewParent is null) throw new ArgumentNullException(nameof(desiredNameOfNewParent));

            if (inputXElement is null) throw new ArgumentNullException(nameof(inputXElement));

            var rootXElement = new XElement(
                desiredNameOfNewParent,
                new XAttribute("generated", $"{DateTime.Now.DayOfWeek} {DateTime.Now:f}"),
                inputXElement);

            var answerAsXDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rootXElement);

            return answerAsXDocument;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XDocument WrapCollectionOfXElementsInNewXDocument(string desiredNameOfXDocumentRootElement,
            IEnumerable<XElement> inputCollectionOfXElements)
    {
        const string failure = "Unable to wrap collection of xml XElements in new parent as an XDocument.";
        const string locus = "[WrapCollectionOfXElementsInNewXDocument]";


        try
        {
            if (desiredNameOfXDocumentRootElement is null)
                throw new ArgumentNullException(nameof(desiredNameOfXDocumentRootElement));

            if (inputCollectionOfXElements is null)
                throw new ArgumentNullException(nameof(inputCollectionOfXElements));

            var rootXElement = new XElement(
                desiredNameOfXDocumentRootElement,
                new XAttribute("generated", $"{DateTime.Now.DayOfWeek} {DateTime.Now:f}"),
                new XAttribute("itemCount", inputCollectionOfXElements.Count()),
                inputCollectionOfXElements);

            var answerAsXDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rootXElement);

            return answerAsXDocument;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XElement RemoveDescendantsHavingSystemDefaultValues(XElement inputXElement)
    {
        const string failure =
            "Unable to remove redundant xml descendants with default values such as whitespace, 0, false, and time zero.";
        const string locus = "[RemoveDescendantsHavingSystemDefaultValues]";

        try
        {
            if (inputXElement is null) throw new ArgumentNullException(nameof(inputXElement));

            inputXElement.Descendants().Where(z => z.Value == string.Empty).Remove();
            inputXElement.Descendants().Where(z => z.Value == "0").Remove();
            inputXElement.Descendants().Where(z => z.Value == "false").Remove();
            inputXElement.Descendants().Where(z => z.Value == "0001-01-01T00:00:00").Remove();
            inputXElement.Descendants().Where(z => z.Value == "9999-12-31T23:59:59.9999999").Remove();
            inputXElement.Descendants().Where(z => z.Value == "00000000-0000-0000-0000-000000000000").Remove();

            return inputXElement;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XElement RenameChild(XElement parentXElement, string oldNameOfChild, string desiredNewNameOfChild)
    {
        const string failure = "Unable to rename one or more child xml elements";
        const string locus = "[RemoveDescendantsHavingSystemDefaultValues]";

        try
        {
            if (parentXElement is null) throw new ArgumentNullException(nameof(parentXElement));

            if (oldNameOfChild is null) throw new ArgumentNullException(nameof(oldNameOfChild));

            if (desiredNewNameOfChild is null) throw new ArgumentNullException(nameof(desiredNewNameOfChild));

            var oldChild = parentXElement.Element(oldNameOfChild);

            if (oldChild is null) return parentXElement;

            var renamedChild = new XElement(desiredNewNameOfChild, oldChild.Elements());

            foreach (var item in oldChild.Attributes())
                renamedChild.SetAttributeValue(item.Name, item.Value);

            parentXElement.Add(renamedChild);

            oldChild.Remove();

            return parentXElement;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public static XElement RemoveUnwantedChildren(XElement element, string[] namesOfChildrenToBeRemoved)
    {
        const string failure = "Unable to delete unwanted children in xml XElement.";
        const string locus = "[RemoveUnwantedChildren]";

        try
        {
            if (element is null) throw new ArgumentNullException(nameof(element));

            if (namesOfChildrenToBeRemoved is null)
                throw new ArgumentNullException(nameof(namesOfChildrenToBeRemoved));

            foreach (var child in element.Elements()
                         .Where(child => namesOfChildrenToBeRemoved.Contains(child.Name.ToString())))
                child.Remove();

            return element;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public static XElement RemoveUnwantedChildrenButOnlyIfEmpty(XElement element, string[] namesOfChildrenToBeRemoved)
    {
        const string failure = "Unable to delete unwanted children in xml XElement.";
        const string locus = "[RemoveUnwantedChildrenButOnlyIfEmpty]";

        try
        {
            if (element is null) throw new ArgumentNullException(nameof(element));

            if (namesOfChildrenToBeRemoved is null)
                throw new ArgumentNullException(nameof(namesOfChildrenToBeRemoved));

            foreach (var child in element.Elements())
                if (namesOfChildrenToBeRemoved.Contains(child.Name.ToString()))
                    child.Remove();

            return element;
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