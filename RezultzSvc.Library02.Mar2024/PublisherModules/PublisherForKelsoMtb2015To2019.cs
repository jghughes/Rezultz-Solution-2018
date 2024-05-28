using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Xml.July2018;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using RezultzSvc.Library02.Mar2024.SvcHelpers;

// ReSharper disable InconsistentNaming

// ReSharper disable UnusedVariable

namespace RezultzSvc.Library02.Mar2024.PublisherModules;

/// <summary>
///     Interprets the self-standing data in a single self-standing XML file exported from Access from
///     Simon Holden and AJ Leeming and does extensive field-mapping, arithmetic, reformatting, and renaming
///     to comply with Rezultz 2018 inputEntity API. Does not require any separate participant data or series metadata.
///     Specified in
///     https://systemrezultzlevel1.blob.core.windows.net/publishingmoduleprofiles/publisherprofile-15mtb.xml
///     At the time of writing, the conversion module profile file specifies just a single PublisherButtonProfileItem.
///     The DatasetIdentifier is 'FileFromSimonHoldenAsXml', this is the button that the user clicks to browse the hard
///     drive
///     for the file that Simon originates in Access and this is the dataset that is expected here.
/// </summary>
public class PublisherForKelsoMtb2015To2019 : PublisherBase
{
    private const string Locus2 = nameof(PublisherForKelsoMtb2015To2019);
    private const string Locus3 = "[RezultzSvc.Library02.Mar2024]";

    public override void ExtractCustomXmlInformationFromAssociatedPublisherProfileFile()
    {
        throw new NotImplementedException(); // nothing required at time of writing
    }

    public override async Task<PublisherOutputItem> DoAllTranslationsAndComputationsToGenerateResultsAsync(PublisherInputItem publisherInputItem)
    {
        const string failure = "Unable to compute results for specified event based on datasets loaded.";
        const string locus = "[DoAllTranslationsAndComputationsToGenerateResultsAsync()]";

        const int lhsWidth = 30;
        const int lhsWidthPlus1 = lhsWidth + 1;
        //const int lhsWidthLess1 = lhsWidth - 1;
        //const int lhsWidthLess2 = lhsWidth - 2;
        const int lhsWidthLess3 = lhsWidth - 3;
        const int lhsWidthLess4 = lhsWidth - 4;
        const int lhsWidthLess5 = lhsWidth - 5;
        //const int lhsWidthLess6 = lhsWidth - 6;


        var conversionReportSb = new JghStringBuilder();
        var ranToCompletionMsgSb = new JghStringBuilder();
        var startDateTime = DateTime.UtcNow;

        conversionReportSb.AppendLineFollowedByOne("Processing report:");

        try
        {
            #region null checks

            //throw new ArgumentNullException(nameof(publisherInputItem), "This is a showstopping exception thrown solely for the purpose of testing and debugging. Be sure to delete it when testing is finished.");

            if (publisherInputItem == null)
                throw new ArgumentNullException(nameof(publisherInputItem), "Remote publishing service received an input object that was null.");

            if (!string.IsNullOrWhiteSpace(publisherInputItem.NullChecksFailureMessage))
                throw new ArgumentException(publisherInputItem.NullChecksFailureMessage); // Note: might look odd, but isn't. The pre-formatted message is the exception message

            #endregion

            // Note: if you go look see in the xml profile, you will see that in this module we are only looking for the contents of a single file - EnumForResultItemsFromSystemHub

            #region results data from Access worksheet from AJ

            conversionReportSb.AppendLine("Doing null checks to confirm that we have a file of results from Access ...");

            var targetFileFromAJ = publisherInputItem.DeduceStorageLocation(IdentifierOfResultsFromAccessFromAJ) ??
                                   throw new JghAlertMessageException("Results file not identified. Please import a file from Access.");

            if (!await _storage.GetIfBlobExistsAsync(targetFileFromAJ.AccountName, targetFileFromAJ.ContainerName, targetFileFromAJ.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected file of Results from Access not found. <{targetFileFromAJ.EntityName}>");

            var simonHoldenAccessFileASXml = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(targetFileFromAJ.AccountName, targetFileFromAJ.ContainerName, targetFileFromAJ.EntityName));

            if (string.IsNullOrWhiteSpace(simonHoldenAccessFileASXml))
                throw new JghAlertMessageException($"Access file is empty. <{targetFileFromAJ.EntityName}>");

            #endregion

            #region do all computations and calculations

            #region before starting, parse to Xml - this will blow up if not well formed xml

            conversionReportSb.AppendLine("checking if Xml data is well formed ....");

            var dummyDoc = ParsePlainTextIntoXml(simonHoldenAccessFileASXml);

            #endregion

            #region before going any further, extract child elements. this will blow up if no repeating elements are found with the specified given name or with the wildcard "*" as the case may be

            conversionReportSb.AppendLine("checking if row titles match up ....");

            ExtractListOfIndividualResults(dummyDoc, "*"); // blow up?

            #endregion

            #region rename all specified XElement names by cunning means of editing the document as plain text

            conversionReportSb.AppendLine("renaming XElement names ....");

            var xmlAsStringUndergoingMappingOfSubStrings =
                MapXElementNames(simonHoldenAccessFileASXml, XElementNameMappingDictionary);

            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            conversionReportSb.AppendLine("verifying format after renaming ....");

            try
            {
                ParsePlainTextIntoXml(xmlAsStringUndergoingMappingOfSubStrings); // blow up?
            }
            catch (Exception)
            {
                throw new Exception(
                    "Very sorry. In the process of renaming XElements, the format of the data was corrupted. It no longer parses back into Xml. This is a program error. Please contact the administrator.");
            }

            #endregion

            #region rename all specified XElement names by cunning means of editing the document as plain text

            conversionReportSb.AppendLine("renaming XElement values ....");

            xmlAsStringUndergoingMappingOfSubStrings = MapXElementNames(xmlAsStringUndergoingMappingOfSubStrings,
                XElementValueMappingDictionary);

            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            conversionReportSb.AppendLine("verifying format after renaming ....");

            XElement withAllSubstringsMappedToRezultzApiAsXe;

            try
            {
                withAllSubstringsMappedToRezultzApiAsXe =
                    ParsePlainTextIntoXml(xmlAsStringUndergoingMappingOfSubStrings);
            }
            catch (Exception)
            {
                throw new Exception(
                    "Very sorry. In the process of renaming XElement values, the format of the data was corrupted. It no longer parses back into Xml. This is a program error. Please contact the administrator.");
            }

            #endregion

            #region create list of individual results from renamed doc

            conversionReportSb.AppendLine(
                "searching for repeating child XElements, where each child is a single individual's result ....");

            var arrayOfIndividualResultXes =
                GiveChildrenTheCorrectNameForRezultzApi(withAllSubstringsMappedToRezultzApiAsXe);

            #endregion

            #region process list for IsSeries, Dnx, Chip time, trophyPts

            conversionReportSb.AppendLine(
                $"processing {arrayOfIndividualResultXes.Length} occurrences individually ....");

            arrayOfIndividualResultXes =
                InsertTrophyPtsField(
                    CalculateChipField(TranslateDnxField(TranslateIsSeriesField(arrayOfIndividualResultXes))));

            #endregion

            #region remove superfluous child elements

            arrayOfIndividualResultXes = RemoveSuperfluousFields(arrayOfIndividualResultXes,
                XElementsThatCanBeDeletedAsTheFinalStepAfterConversionIsFinished);

            #endregion

            #region create answer as XML document. add XAttribute to document to provide the name of the inputEntity file as the primarysource - the start of a paper trail

            var answerDocument =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent(ResultDto.XeRootForContainerOfSimpleStandAloneArray, arrayOfIndividualResultXes);

            answerDocument.Add(new XAttribute(sourceFileXAttribute, targetFileFromAJ.EntityName));

            var text = answerDocument.ToString(SaveOptions.None);

            #endregion

            #region in the original version of this preprocessor we merely returned this pretty XML document. But in this 2023 update, we go further and return a PublisherOutput containing an array of ResultItemsDTo as a string

            var allComputedResults = ConvertArrayOfXElementsToArrayOfResultItemDataTransferObjects(arrayOfIndividualResultXes).OrderBy(z => z.T01).ToList();

            #endregion


            #endregion

            #region report progress and wrap up

            var j = 1;

            foreach (var resultItemDataTransferObject in allComputedResults)
            {
                conversionReportSb.AppendLine(WriteOneLineReport(j, resultItemDataTransferObject, string.Empty));

                j += 1;
            }

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Computations ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (from AJ):", lhsWidth)} <{targetFileFromAJ.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{targetFileFromAJ.ContainerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Results computed:", lhsWidthLess4)} {allComputedResults.Count} results");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. {allComputedResults.Count} results computed.");

            var answer2 = new PublisherOutputItem
            {
                LabelOfEventAsIdentifier = publisherInputItem.EventLabelAsEventIdentifier,
                RanToCompletionMessage = ranToCompletionMsgSb.ToString(),
                ConversionReport = conversionReportSb.ToString(),
                ConversionDidFail = false,
                ComputedResults = allComputedResults.ToArray()
            };

            #endregion

            return await Task.FromResult(answer2);
        }

        #region try-catch

        catch (JghAlertMessageException ex)
        {
            conversionReportSb.AppendLinePrecededByOne(ex.Message);

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLinePrecededByOne($"{JghString.LeftAlign("Outcome:", lhsWidth)} Conversion interrupted. Conversion ran into a problem.");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Message:", lhsWidthPlus1)} {ex.Message}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conversion duration:", lhsWidthLess5)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Results computed:", lhsWidthLess4)} none");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidth)} Failure. Please read the conversion report for more information.");

            var answer2 = new PublisherOutputItem
            {
                LabelOfEventAsIdentifier = publisherInputItem?.EventLabelAsEventIdentifier,
                RanToCompletionMessage = ranToCompletionMsgSb.ToString(),
                ConversionReport = conversionReportSb.ToString(),
                ConversionDidFail = true,
                ComputedResults = Array.Empty<ResultItem>()
            };
            return await Task.FromResult(answer2);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }


        #endregion
    }

    #region const

    private const string sourceFileXAttribute = "sourcefile";

    private const string IdentifierOfResultsFromAccessFromAJ = "FileFromSimonHoldenAsXml"; // the identifier here originates from the publisher profile file. Must be kept in sync

    #endregion

    #region fields

    private readonly AzureStorageServiceMethodsHelper _storage = new(new AzureStorageAccessor());

    #endregion

    #region field name mappings

    private static class FieldNames
    {
        public const string WeirdKelsoClockTime = "WeirdKelsoClockTime";
        public const string WeirdKelsoClockTimeOffset = "WeirdKelsoClockTimeOffset";
        public const string TrophyPoints = "TrohyPoints";

    }

    private static Dictionary<string, string> XElementNameMappingDictionary => new()
    {
        {"plate", ResultDto.XeBib},
        {"first_name", ResultDto.XeFirst},
        {"last_name", ResultDto.XeLast},
        {"gender", ResultDto.XeSex},
        {"Class_Race", ResultDto.XeRace},
        {"age_category", ResultDto.XeAgeGroup},
        {"Time_Text", FieldNames.WeirdKelsoClockTime},
        {"TmAdjustment", FieldNames.WeirdKelsoClockTimeOffset},
        {"Notes", ResultDto.XeDnxString},
        {"isseries", ResultDto.XeIsSeries},
        {"city", ResultDto.XeCity}
    };

    private static Dictionary<string, string> XElementValueMappingDictionary => new()
    {
        {"", ""}
    };

    private static List<string> XElementsThatCanBeDeletedAsTheFinalStepAfterConversionIsFinished => new()
    {
        "event",
        "checked_in",
        "Time_Text",
        "TmAdjustment",
        FieldNames.WeirdKelsoClockTime,
        FieldNames.WeirdKelsoClockTimeOffset
    };

    #endregion

    #region helpers

    // ReSharper disable once UnusedMember.Local
    private static string MapXElementValues(string xmlFileContentsAsPlainText,
        Dictionary<string, string> mappingDictionary)
    {
        var failure = "Unable to map XElement names in accordance with mapping dictionary.";
        const string locus = "[MapXElementNames]";


        var provisionalCulpritOldValue = "";
        var provisionalCulpritNewValue = "";

        var scratchPadText = xmlFileContentsAsPlainText;

        try
        {
            if (string.IsNullOrWhiteSpace(xmlFileContentsAsPlainText))
                throw new ArgumentNullException(nameof(xmlFileContentsAsPlainText));

            foreach (var entry in mappingDictionary.Where(z => !string.IsNullOrWhiteSpace(z.Key)))
            {
                provisionalCulpritOldValue = entry.Key;
                provisionalCulpritNewValue = entry.Value;
                scratchPadText = scratchPadText.Replace($">{entry.Key}<", $">{entry.Value}<");
            }

            return scratchPadText;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro = JghString.ConcatAsSentences(
                "Unable to substitute fields with their specified equivalents.",
                $"Problem arose whilst replacing occurrences of {provisionalCulpritOldValue} with {provisionalCulpritNewValue}.",
                "Please inspect the data. There could be a programming error here.",
                "A typical programming error is that a specified equivalent contains a forbidden character.");

            failure = JghString.ConcatAsSentences(failure, intro);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static string MapXElementNames(string xmlFileContentsAsPlainText,
        Dictionary<string, string> mappingDictionary)
    {
        var failure = "Unable to map XElement names in accordance with mapping dictionary.";
        const string locus = "[MapXElementNames]";


        var provisionalCulpritOldValue = "";
        var provisionalCulpritNewValue = "";

        var scratchPadText = xmlFileContentsAsPlainText;

        try
        {
            if (string.IsNullOrWhiteSpace(xmlFileContentsAsPlainText))
                throw new ArgumentNullException(nameof(xmlFileContentsAsPlainText));

            foreach (var entry in mappingDictionary.Where(z => !string.IsNullOrWhiteSpace(z.Key)))
            {
                provisionalCulpritOldValue = entry.Key;
                provisionalCulpritNewValue = entry.Value;
                scratchPadText = scratchPadText.Replace($"<{entry.Key}>", $"<{entry.Value}>");
                scratchPadText = scratchPadText.Replace($"</{entry.Key}>", $"</{entry.Value}>");
            }

            return scratchPadText;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro = JghString.ConcatAsSentences(
                "Unable to substitute field names with their specified equivalents.",
                $"Problem arose whilst replacing occurrences of the field name {provisionalCulpritOldValue} with {provisionalCulpritNewValue}.",
                "Please inspect the data. There could be a programming error here.",
                "A typical programming error is that a specified equivalent contains a forbidden character.  Angle-brackets, for example, are prohibited in XML field names.");

            failure = JghString.ConcatAsSentences(failure, intro);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static XElement[] ExtractListOfIndividualResults(XContainer parentXContainer,
        string nameOfRepeatingChildElement)
    {
        var failure = "Unable to extract child elements from parent Xml document.";
        const string locus = "[ExtractListOfIndividualResults]";

        try
        {
            if (parentXContainer == null)
                throw new ArgumentNullException(nameof(parentXContainer));

            if (string.IsNullOrWhiteSpace(nameOfRepeatingChildElement))
                throw new ArgumentException(
                    $"Fatal error. Parameter <{nameof(nameOfRepeatingChildElement)}> is empty.");

            var repeaters = nameOfRepeatingChildElement.Contains("*")
                ? parentXContainer.Elements().ToArray()
                : parentXContainer.Elements(nameOfRepeatingChildElement).ToArray();

            if (!repeaters.Any())
                throw new Exception("No rows of data found.");

            return repeaters;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro = JghString.ConcatAsSentences(
                "Unable to see or retrieve multiple repeating records in the data. This might be because there aren\'t any. It might be because the items are invisible.",
                "If the data is exported from Access or Excel, be aware that row titles are generated automatically by the export wizard.",
                "The wizard takes them from the name of their containing worksheet or table or table query output as the case may be.",
                $"The problematic row title seems to be <{nameOfRepeatingChildElement}>.",
                "This needs to be the same as the title in the dataset.");

            failure = JghString.ConcatAsSentences(failure, intro);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static XElement[] GiveChildrenTheCorrectNameForRezultzApi(XContainer parentXContainer)
    {
        var failure = "Unable to change name of repeating child elements to comply with Rezultz API.";
        const string locus = "[GiveChildrenTheCorrectNameForRezultzApi]";

        try
        {
            if (parentXContainer == null)
                throw new ArgumentNullException(nameof(parentXContainer));

            var repeaters = parentXContainer.Elements().ToArray();

            if (!repeaters.Any())
                throw new Exception("No rows of data found.");

            return repeaters.Select(repeater => new XElement(ResultDto.XeResult, repeater.Elements()))
                .ToArray();
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"(For reference, the compliant name we are seeking to introduce is <{ResultDto.XeResult})>.";

            failure = JghString.ConcatAsSentences(failure, intro);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static IEnumerable<XElement> TranslateIsSeriesField(IEnumerable<XElement> resultElements)
    {
        var failure = "Unable to determine if participant is registered for the series or not.";
        const string locus = "[TranslateIsSeriesField]";

        var provisionallyCorruptItemIndex = 1;

        var provisionallyCorruptElement = new XElement("blank", "blank");

        var arrayOfResultElements = resultElements.Where(z => z != null).ToArray();

        try
        {
            foreach (var resultElement in arrayOfResultElements)
            {
                provisionallyCorruptElement = resultElement;

                var xElement = resultElement.Element(ResultDto.XeIsSeries);
                if (xElement != null)
                    xElement.Value = xElement.Value == "MTBRSFULL" ? "Y" : "N";

                provisionallyCorruptItemIndex++;
            }

            return arrayOfResultElements;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"Unable to compute and insert element called <{ResultDto.XeIsSeries}>. Insertion process aborted at record count={provisionallyCorruptItemIndex}. Please inspect the following problematic element carefully. It might be visibly incomplete or inadequate or missing something.";

            var problematicElement = $"{provisionallyCorruptElement}";

            failure = JghString.ConcatAsParagraphs(JghString.ConcatAsSentences(failure, intro),
                problematicElement);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static IEnumerable<XElement> TranslateDnxField(IEnumerable<XElement> resultElements)
    {
        var failure = "Unable to determine if participant has a valid Dnx entry or not. (DNF, DNS, TBD";
        const string locus = "[TranslateDnxField]";

        var provisionallyCorruptItemIndex = 1;

        var provisionallyCorruptElement = new XElement("blank", "blank");

        try
        {
            if (resultElements == null)
                throw new ArgumentNullException(nameof(resultElements));

            var arrayOfresultElements = resultElements.Where(z => z != null).ToArray();

            foreach (var resultElement in arrayOfresultElements)
            {
                provisionallyCorruptElement = resultElement;

                #region leave valid Dnx's intact, erase the value if not a valid Dnx

                var xElement = resultElement.Element(ResultDto.XeDnxString);
                if (xElement != null)
                    xElement.Value = ElementValueIsDnxSymbol(xElement) ? xElement.Value : string.Empty;

                #endregion

                provisionallyCorruptItemIndex++;
            }

            return arrayOfresultElements;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"Unable to compute and insert element called <{ResultDto.XeDnxString}>. Insertion process aborted at record count={provisionallyCorruptItemIndex}. Please inspect the following problematic element carefully. It might be visibly incomplete or inadequate or missing something.";

            var problematicElement = $"{provisionallyCorruptElement}";

            failure = JghString.ConcatAsParagraphs(JghString.ConcatAsSentences(failure, intro),
                problematicElement);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static IEnumerable<XElement> CalculateChipField(IEnumerable<XElement> resultElements)
    {
        var failure =
            "Unable to determine a participant's chip time (derived from gun-time and start-time offset).";
        const string locus = "[CalculateChipField]";

        var provisionallyCorruptItemIndex = 1;

        var provisionallyCorruptElement = new XElement("blank", "blank");

        try
        {
            if (resultElements == null)
                throw new ArgumentNullException(nameof(resultElements));

            var arrayOfResultElements = resultElements.Where(z => z != null).ToArray();

            foreach (var resultElement in arrayOfResultElements)
            {
                provisionallyCorruptElement = resultElement;

                #region if this is a Dnx, do nothing. otherwise, add a new <Chip> element

                var chipElement = new XElement(ResultDto.XeT01,
                    "nullfornow");

                if (ElementValueIsDnxSymbol(resultElement.Element(ResultDto.XeDnxString)))
                {
                    // no need to generate a ResultDto.XeT01 element. this is a dnx for sure
                }
                else
                {
                    // validly formatted <Time_Text> and <TmAdjustment> might or might not exist. give it a go. try work out Chip duration. if that fails, throw an exception

                    var chipDuration = AdjustGunTimeForStartTimeOffset(
                        resultElement.Element(FieldNames.WeirdKelsoClockTime),
                        resultElement.Element(FieldNames.WeirdKelsoClockTimeOffset));

                    chipElement.Value = chipDuration.ToString("g");

                    resultElement.Add(chipElement);
                }

                #endregion

                provisionallyCorruptItemIndex++;
            }

            return arrayOfResultElements;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"Unable to compute and insert element called <{ResultDto.XeT01}>. Insertion process aborted at record count={provisionallyCorruptItemIndex}. Please inspect the following problematic element carefully. It might be visibly incomplete or inadequate or missing something.";

            var problematicElement = $"{provisionallyCorruptElement}";


            failure = JghString.ConcatAsParagraphs(JghString.ConcatAsSentences(failure, intro),
                problematicElement);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    /// <summary>
    /// Note: this method is deprecated. It was written in 2015 and has been replaced by a more sophisticated method that uses a points scale table.
    /// </summary>
    private static XElement[] InsertTrophyPtsField(IEnumerable<XElement> resultElements)
    {
        var failure = "Unable to insert the points allocted for a win in the participant's race.";
        const string locus = "[InsertTrophyPtsField]";

        var provisionallyCorruptItemIndex = 1;

        var provisionallyCorruptElement = new XElement("blank", "blank");

        try
        {
            if (resultElements == null)
                throw new ArgumentNullException(nameof(resultElements));

            var arrayOfResultElements = resultElements.Where(z => z != null).ToArray();

            foreach (var resultElement in arrayOfResultElements)
            {
                provisionallyCorruptElement = resultElement;

                #region add the TrophyPoints element

                var trophyPointsElement = new XElement(FieldNames.TrophyPoints, "");

                // trophy points for category in which the competitor belongs (Exp = 300, Sport = 200, Novice = 100, Beginner = 50)

                if (JghString.AreEqualIgnoreOrdinalCase("EXPERT",
                        JghXElementHelpers.AsTmlr(resultElement.Element(ResultDto.XeRace))))
                    trophyPointsElement.Value = 300.ToString(CultureInfo.InvariantCulture);
                else if (JghString.AreEqualIgnoreOrdinalCase("SPORT",
                             JghXElementHelpers.AsTmlr(resultElement.Element(ResultDto.XeRace))))
                    trophyPointsElement.Value = 200.ToString(CultureInfo.InvariantCulture);
                else if (JghString.AreEqualIgnoreOrdinalCase("NOVICE",
                             JghXElementHelpers.AsTmlr(resultElement.Element(ResultDto.XeRace))))
                    trophyPointsElement.Value = 100.ToString(CultureInfo.InvariantCulture);

                else if (JghString.AreEqualIgnoreOrdinalCase("BEGINNER",
                             JghXElementHelpers.AsTmlr(resultElement.Element(ResultDto.XeRace))))
                    trophyPointsElement.Value = 50.ToString(CultureInfo.InvariantCulture);
                else
                    trophyPointsElement.Value = 0.ToString(CultureInfo.InvariantCulture);

                resultElement.Add(trophyPointsElement);

                #endregion

                provisionallyCorruptItemIndex++;
            }

            return arrayOfResultElements;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"Unable to compute and insert element called <{FieldNames.TrophyPoints}>. Insertion process aborted at record count={provisionallyCorruptItemIndex}. Please inspect the following problematic element carefully. It might be visibly incomplete or inadequate or missing something.";

            var problematicElement = $"{provisionallyCorruptElement}";

            failure = JghString.ConcatAsParagraphs(JghString.ConcatAsSentences(failure, intro),
                problematicElement);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static XElement[] RemoveSuperfluousFields(IEnumerable<XElement> resultElements,
        List<string> namesOfSuperfluousElements)
    {
        var failure =
            "Unable to remove superfluous fields in the inputEntity data i.e. those not required by the Rezultz API.";
        const string locus = "[RemoveSuperfluousFields]";

        var provisionallyCorruptItemIndex = 1;

        var provisionallyCorruptElement = new XElement("blank", "blank");

        try
        {
            if (resultElements == null)
                throw new ArgumentNullException(nameof(resultElements));

            if (namesOfSuperfluousElements == null)
                throw new ArgumentNullException(nameof(namesOfSuperfluousElements));

            var arrayOfResultElements = resultElements.Where(z => z != null).ToArray();

            foreach (var resultElement in arrayOfResultElements)
            {
                provisionallyCorruptElement = resultElement;

                foreach (var fieldName in namesOfSuperfluousElements)
                {
                    var xE = resultElement.Element(fieldName);
                    xE?.Remove();
                }

                provisionallyCorruptItemIndex++;
            }

            return arrayOfResultElements;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"Conversion process aborted at record count={provisionallyCorruptItemIndex}. Please inspect the following problematic record carefully. It might be visibly incomplete or inadequate or missing something.";

            var problematicElement = $"{provisionallyCorruptElement}";

            failure = JghString.ConcatAsSentences(failure, intro, problematicElement);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static XElement ParsePlainTextIntoXml(string inputText)
    {
        var failure = "Unable to parse text into xml.";
        const string locus = "[ParsePlainTextIntoXml]";

        try
        {
            if (inputText == null)
                throw new ArgumentNullException(nameof(inputText));

            return XElement.Parse(inputText); // automatically throws if invalid
        }

        #region try-catch

        catch (Exception ex)
        {
            failure = JghString.ConcatAsSentences(
                $"{failure} Text is not 100% correctly formatted.",
                "Even the tiniest error in format, syntax and/or content causes disqualification.");

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static bool IsSuperficiallyValidKelsoTimeText(string text)
    {
        var failure =
            "Unable to confirm if the Kelso field is a valid time field or not in terms of its own standard format.";
        const string locus = "[IsSuperficiallyValidKelsoTimeText]";

        try
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.Length >= 8 && int.TryParse(text, out var dummySuccessfullyParsedDigits);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    private static bool ElementValueIsDnxSymbol(XElement dnxElement)
    {
        const string failure = " Unable to determine if value of element is a valid Dnx symbol.";
        const string locus = "[ElementValueIsDnxSymbol]";

        if (dnxElement == null) return false;


        if (string.IsNullOrWhiteSpace(dnxElement.Value))
            return false;

        try
        {
            var candidateDnxString = dnxElement.Value;

            return JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(candidateDnxString, Symbols.SymbolDnf)
                   | JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(candidateDnxString, Symbols.SymbolDns)
                   | JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(candidateDnxString, Symbols.SymbolDq)
                   | JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(candidateDnxString, Symbols.SymbolTbd);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    private static TimeSpan AdjustGunTimeForStartTimeOffset(XElement gunTimeElement,
        XElement startTimeOffSetElement)
    {
        var failure = "Unable to adjust gun time for start time offset.";
        const string locus = "[AdjustGunTimeForStartTimeOffset]";

        try
        {
            if (gunTimeElement == null)
                throw new ArgumentNullException(nameof(gunTimeElement));

            if (startTimeOffSetElement == null)
                throw new ArgumentNullException(nameof(startTimeOffSetElement));

            var gunTimespan = ConvertElementValueFromKelsoTimeTextToTimespan(gunTimeElement);

            var offsetTimespan = ConvertElementValueFromKelsoTimeTextToTimespan(startTimeOffSetElement);

            return gunTimespan - offsetTimespan;
        }

        #region try-catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static TimeSpan ConvertElementValueFromKelsoTimeTextToTimespan(XElement timeTextXElement)
    {
        var failure = "Unable to convert text purporting to be a valid time to typeof C# Timespan.";
        const string locus = "[ConvertElementValueFromKelsoTimeTextToTimespan]";

        try
        {
            #region null checks

            if (timeTextXElement == null) throw new ArgumentNullException(nameof(timeTextXElement));

            if (string.IsNullOrWhiteSpace(timeTextXElement.Value))
                throw new ArgumentException(
                    $"Null argument. The parameter <{nameof(timeTextXElement)}> in the method <ConvertElementValueFromKelsoTimeTextToTimespan> is null.");

            var text = timeTextXElement.Value;

            text = string.Concat("00000000", text);
            // create a string of eight leading zeroes at the absolute minimum

            if (!IsSuperficiallyValidKelsoTimeText(text))
                throw new ArgumentException(
                    $"Format error. The value of element <{timeTextXElement.Name}> is formatted incorrectly. The value should be in hhmmssff format whereas the value is <{timeTextXElement.Value}>.");

            #endregion

            var lengthOfText = text.Length;

            var hhText = "00";
            var mmText = "00";
            var ssText = "00";
            var ffText = "00";

            // parse the string from the RHS

            if (lengthOfText >= 8)
                hhText = text.Substring(lengthOfText - 8, 2);

            if (lengthOfText >= 6)
                mmText = text.Substring(lengthOfText - 6, 2);

            if (lengthOfText >= 4)
                ssText = text.Substring(lengthOfText - 4, 2);

            if (lengthOfText >= 2)
                ffText = text.Substring(lengthOfText - 2, 2);

            var hours = int.Parse(hhText);
            var minutes = int.Parse(mmText);
            var seconds = int.Parse(ssText);
            var milliseconds = int.Parse(ffText) * 10;

            var answer = new TimeSpan(0, hours, minutes, seconds, milliseconds);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    private ResultItem[] ConvertArrayOfXElementsToArrayOfResultItemDataTransferObjects(XElement[] arrayOfIndividualResultXes)
    {
        List<ResultItem> answer = new();

        foreach (var element in arrayOfIndividualResultXes)
        {
            ResultItem resultItem = new()
            {
                Bib = element.Element(ResultDto.XeBib)?.Value,
                FirstName = element.Element(ResultDto.XeFirst)?.Value,
                LastName = element.Element(ResultDto.XeLast)?.Value,
                Gender = element.Element(ResultDto.XeSex)?.Value,
                RaceGroup = element.Element(ResultDto.XeRace)?.Value,
                AgeGroup = element.Element(ResultDto.XeAgeGroup)?.Value,
                City = element.Element(ResultDto.XeCity)?.Value,
                Team = element.Element(ResultDto.XeTeam)?.Value,
                IsSeries = JghConvert.ToBool(element.Element(ResultDto.XeIsSeries)?.Value),
                DnxString = element.Element(ResultDto.XeDnxString)?.Value,
                T01 = element.Element(ResultDto.XeT01)?.Value
            };
            answer.Add(resultItem);
        }

        return answer.ToArray();
    }

    private static string WriteOneLineReport(int index, ResultItem dtoItem, string inputDuration)
    {
        string answer;

        if (string.IsNullOrWhiteSpace(inputDuration))
            answer = $"{index,-3}  {dtoItem.Bib,-3}  {dtoItem.FirstName,-15} {dtoItem.LastName,-15}  {dtoItem.T01,-15}  {dtoItem.DnxString,-3}";
        else
            answer = $"{index,-3}  {dtoItem.Bib,-3}  {dtoItem.FirstName,-15} {dtoItem.LastName,-15}  {dtoItem.T01,-15}  {dtoItem.DnxString,-3}  ({inputDuration,-15})";

        return answer;
    }

    #endregion
}