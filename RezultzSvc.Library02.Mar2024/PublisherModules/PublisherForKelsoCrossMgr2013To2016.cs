using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Xml.July2018;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using RezultzSvc.Library02.Mar2024.SvcHelpers;

// ReSharper disable InconsistentNaming

namespace RezultzSvc.Library02.Mar2024.PublisherModules;

/// <summary>
///     Interpret and translate raw CX inputEntity data from Simon Holden from CrossMgr software exported in USAC format.
///     Requires a separate participant list file and a CX points scale file from Simon.
///     Does extensive analysis, arithmetic, reformatting and renaming to comply with Rezultz 2018 inputEntity API
///     Specified in
///     https://systemrezultzlevel1.blob.core.windows.net/publishingmoduleprofiles/publisherprofile-13cx.xml
///     At time of writing, the conversion module specification file specifies
///     three PublisherButtonProfileItem. The three DatasetIdentifier are
///     "CustomParticipantListAsXml" and "CrossMgrTimingDataAsXml" and "PointsScaleAsXml".
///     These are the buttons that the user clicks to browse the hard drive and import the required xml files
///     and these are the datasets expected here.
/// </summary>
public class PublisherForKelsoCrossMgr2013To2016 : PublisherBase
{
    private const string Locus2 = nameof(PublisherForKelsoCrossMgr2013To2016);
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
        const int lhsWidthLess2 = lhsWidth - 2;
        //const int lhsWidthLess3 = lhsWidth - 3;
        const int lhsWidthLess4 = lhsWidth - 4;
        const int lhsWidthLess5 = lhsWidth - 5;
        //const int lhsWidthLess6 = lhsWidth - 6;

        var conversionReportSb = new JghStringBuilder();
        var ranToCompletionMsgSb = new JghStringBuilder();
        var startDateTime = DateTime.UtcNow;

        conversionReportSb.AppendLineFollowedByOne("Conversion report:");

        try
        {
            #region null checks

            //throw new ArgumentNullException(nameof(publisherInputItem), "This is a showstopping exception thrown solely for the purpose of testing and debugging. Be sure to delete it when testing is finished.");

            if (publisherInputItem == null)
                throw new ArgumentNullException(nameof(publisherInputItem), "Remote publishing service received an input object that was null.");

            if (!string.IsNullOrWhiteSpace(publisherInputItem.NullChecksFailureMessage))
                throw new ArgumentException(publisherInputItem.NullChecksFailureMessage); // Note: might look odd, but isn't. The pre-formatted message is the exception message

            #endregion

            #region fetch participant master list

            conversionReportSb.AppendLine("Doing null checks to confirm that we have a file of the master list of participants ...");

            var participantFileTarget = publisherInputItem.DeduceStorageLocation(IdentifierOfParticipantList) ??
                                        throw new JghAlertMessageException("Participant file not identified. Please import a participant file");

            if (!await _storage.GetIfBlobExistsAsync(participantFileTarget.AccountName, participantFileTarget.ContainerName, participantFileTarget.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected Participant file not found. <{participantFileTarget.EntityName}> ");

            var participantListAsXml = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(participantFileTarget.AccountName, participantFileTarget.ContainerName, participantFileTarget.EntityName));

            if (string.IsNullOrWhiteSpace(participantListAsXml))
                throw new JghAlertMessageException($"Participant file is empty. <{participantFileTarget.EntityName}>");

            #endregion

            #region fetch Points scale

            conversionReportSb.AppendLine("Doing null checks to confirm that we have a file of the points scale ...");

            var pointsScaleFileTarget = publisherInputItem.DeduceStorageLocation(IdentifierOfPointsScale) ??
                                        throw new JghAlertMessageException("PointsScale file not identified. Please import a participant file");

            if (!await _storage.GetIfBlobExistsAsync(pointsScaleFileTarget.AccountName, pointsScaleFileTarget.ContainerName, pointsScaleFileTarget.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected Participant file not found. <{pointsScaleFileTarget.EntityName}> ");

            var pointsScaleAsXml = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(pointsScaleFileTarget.AccountName, pointsScaleFileTarget.ContainerName, pointsScaleFileTarget.EntityName));

            if (string.IsNullOrWhiteSpace(pointsScaleAsXml))
                throw new JghAlertMessageException($"Participant file is empty. <{pointsScaleFileTarget.EntityName}>");

            #endregion

            #region fetch CrossMgr timing data

            conversionReportSb.AppendLine("Doing null checks to confirm that we have a file of timing data originating from CrossMgr ...");

            var crossMgrFileTarget = publisherInputItem.DeduceStorageLocation(IdentifierOfCrossMgrTimingData) ??
                                     throw new JghAlertMessageException("CrossMgr file not identified. Please import a CrossMgr file");

            if (!await _storage.GetIfBlobExistsAsync(crossMgrFileTarget.AccountName, crossMgrFileTarget.ContainerName, crossMgrFileTarget.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected CrossMgr file not found. <{crossMgrFileTarget.EntityName}>");

            var crossMgrCxDataAsXml = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(crossMgrFileTarget.AccountName, crossMgrFileTarget.ContainerName, crossMgrFileTarget.EntityName));

            if (string.IsNullOrWhiteSpace(crossMgrCxDataAsXml))
                throw new JghAlertMessageException($"CrossMgr file is empty. <{crossMgrFileTarget.EntityName}>");

            #endregion

            #region do all computations and calculations

            #region before starting, parse cxTimingDataAsXml to Xml - this will blow up if not well formed xml

            conversionReportSb.AppendLine("checking if Xml data is well formed ....");

            var dummyDoc = ParsePlainTextIntoXml(crossMgrCxDataAsXml);

            #endregion

            #region before going any further, extract child elements from cxTimingDataAsXml. this will blow up if no repeating elements are found with the specified given name or "*" as the case may be

            conversionReportSb.AppendLine("checking if row titles match up ....");

            // ReSharper disable once UnusedVariable
            var dummyRows = ExtractListOfIndividualResults(dummyDoc, "*"); // blow up?

            #endregion

            #region rename cxTimingDataAsXml xelement names by cunning means of editing the document as plain text

            conversionReportSb.AppendLine("renaming xElement names ....");

            var cxTimingDataAsXmlUndergoingMappingOfSubStrings =
                MapXElementNames(crossMgrCxDataAsXml, XElementNameMappingDictionary);

            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            conversionReportSb.AppendLine("verifying format after mapping XElement names ....");

            try
            {
                // ReSharper disable once UnusedVariable
                var crashDummyXe = ParsePlainTextIntoXml(cxTimingDataAsXmlUndergoingMappingOfSubStrings); // blow up?
            }
            catch (Exception)
            {
                throw new Exception(
                    "Very sorry. In the process of renaming XElements, the format of your data was corrupted. It no longer parses back into Xml.");
            }

            #endregion

            #region rename all specified XElement values by cunning means of editing the document as plain text

            conversionReportSb.AppendLine("renaming xElement values ....");

            cxTimingDataAsXmlUndergoingMappingOfSubStrings = MapXElementValues(cxTimingDataAsXmlUndergoingMappingOfSubStrings,
                XElementValueMappingDictionary);

            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            conversionReportSb.AppendLine("verifying format after renaming ....");

            XElement cxTimingDataWithAllSubstringsMappedToRezultzApiAsXe;

            try
            {
                cxTimingDataWithAllSubstringsMappedToRezultzApiAsXe =
                    ParsePlainTextIntoXml(cxTimingDataAsXmlUndergoingMappingOfSubStrings);
            }
            catch (Exception)
            {
                throw new Exception(
                    "Very sorry. In the process of renaming fields, the format of your data was corrupted. It no longer parses back into Xml.");
            }

            #endregion

            #region create list of individual results from renamed doc

            conversionReportSb.AppendLine(
                $"searching for repeating rows of xml entitled <{ResultDto.XeResult}>  ....");

            var arrayOfIndividualResultXes =
                GiveChildrenTheCorrectNameForRezultzApi(cxTimingDataWithAllSubstringsMappedToRezultzApiAsXe);

            #endregion

            #region remove superfluous child elements

            arrayOfIndividualResultXes = RemoveSuperfluousFields(arrayOfIndividualResultXes,
                XElementsThatCanBeDeletedAsTheFinalStepAfterConversionIsFinished);

            #endregion

            #region add particulars of particpants

            var participantListParentXe = XElement.Parse(participantListAsXml);

            var participantMasterListRowXes = participantListParentXe.Elements().ToList();

            arrayOfIndividualResultXes = AddParticipantParticulars(arrayOfIndividualResultXes.ToList(),
                participantMasterListRowXes.ToList(), out var errorMessage);

            if (arrayOfIndividualResultXes == null)
                throw new Exception(errorMessage);

            #endregion

            #region add points scale

            var pointsScaleParentXe = XElement.Parse(pointsScaleAsXml);

            var pointsScaleLineItems = pointsScaleParentXe.Elements().ToList();

            arrayOfIndividualResultXes = AddPointsAwarded(arrayOfIndividualResultXes.ToList(), pointsScaleLineItems,
                out errorMessage);

            if (arrayOfIndividualResultXes == null)
                throw new Exception(errorMessage);

            #endregion

            #region create answer as XML document as text. add XAttribute to document to provide the name of the inputEntity file as the primarysource - the start of a paper trail

            var answerDocument =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent("root", arrayOfIndividualResultXes);

            answerDocument.Add(new XAttribute(sourceFileXAttribute, crossMgrFileTarget.EntityName));

            conversionReportSb.AppendLine("Done. Calculations and translations complete.");

            // ReSharper disable once UnusedVariable
            var answerAsString = answerDocument.ToString(SaveOptions.None);

            #endregion

            #region in the original version of this preprocessor we merely returned this pretty XML document. But in this 2023 update, we go further and return a PreprocessingOutputItem containing an array of ResultItemsDTo as a string

            var arrayOfResultItemDataTransferObject = ConvertArrayOfXElementsToArrayOfResultItemDataTransferObjects(arrayOfIndividualResultXes);

            var allComputedResults = ResultItem.FromDataTransferObject(arrayOfResultItemDataTransferObject.OrderBy(z => z.T01).ToArray()).ToList();

            #endregion

            #endregion

            #region report progress and wrap up

            var j = 1;

            foreach (var resultItem in allComputedResults)
            {
                conversionReportSb.AppendLine(WriteOneLineReport(j, resultItem, string.Empty));

                j += 1;
            }

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Computations ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (participants):", lhsWidth)} <{participantFileTarget.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{participantFileTarget.ContainerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (points scale):", lhsWidthLess2)} <{pointsScaleFileTarget.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{pointsScaleFileTarget.ContainerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset (CrossMgr):", lhsWidthLess2)} <{crossMgrFileTarget.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{crossMgrFileTarget.ContainerName}>");
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
    private const string IdentifierOfCrossMgrTimingData = "CrossMgrTimingDataAsXml";
    private const string IdentifierOfParticipantList = "CustomParticipantListAsXml";

    private const string IdentifierOfPointsScale = "PointsScaleAsXml";
    // the identifiers above are defined in the XML publishing profile file. Must be kept in sync

    private const string XePlaceString = "place";
    private const string XePointsAwardedString = "points-awarded"; // dummy placeholder - deprecated since 2013 when this publisher was written
    #endregion

    #region fields

    private readonly AzureStorageServiceMethodsHelper _storage = new(new AzureStorageAccessor());

    #endregion

    #region field name mappings

    private static Dictionary<string, string> XElementNameMappingDictionary => new()
    {
        // for now 8 laps seems to be enough

        {"Race_x0020_Date", RaceSpecificationDto.XeAdvertisedStartDateTime},
        {"Race_x0020_Discipline", RaceSpecificationDto.XeDiscipline},
        {"Rider_x0020_Bib_x0020__x0023_", UbiquitousFieldNames.XeBib},
        {"Rider_x0020_Place", XePlaceString},
        {"Race_x0020_Category", ResultDto.XeRace},
        {"Rider_x0020_Time", "dummy"},
        {"Race_x0020_Distance", RaceSpecificationDto.XeDistanceOfCourseKm},
        //{"Race_x0020_Distance_x0020_Type", RaceSpecificationItemSerialiserNames.DistanceUnitsOfMeasureEnum},
        {"Rider_x0020_Lap_x0020_1", ResultDto.XeT01},
        {"Rider_x0020_Lap_x0020_2", ResultDto.XeT02},
        {"Rider_x0020_Lap_x0020_3", ResultDto.XeT03},
        {"Rider_x0020_Lap_x0020_4", ResultDto.XeT04},
        {"Rider_x0020_Lap_x0020_5", ResultDto.XeT05},
        {"Rider_x0020_Lap_x0020_6", ResultDto.XeT06},
        {"Rider_x0020_Lap_x0020_7", ResultDto.XeT07},
        {"Rider_x0020_Lap_x0020_8", ResultDto.XeT08}
    };

    private static Dictionary<string, string> XElementValueMappingDictionary => new()
    {
        {"DNP", Symbols.SymbolDq}
    };

    private static List<string> XElementsThatCanBeDeletedAsTheFinalStepAfterConversionIsFinished => new()
    {
        "ID",
        "Race_x0020_Gender"
    };

    private static class ParticipantMasterListXeNames
    {
        // gender not required because it's in the master list of participants

        public const string Bib = "Bib"; // primary key
        public const string First = "first";
        public const string Last = "last";
        public const string Gender = "gender";
        public const string AgeCategory = "age_category";
        public const string Series = "Series";
        public const string City = "city";
    }

    private static class PointsScaleXeNames
    {
        // ReSharper disable once UnusedMember.Local
        public const string PointsScaleChildren = "PointsScaleLineItem";

        public const string Points = "Points";
        public const string Place = "Place"; // primary key
        public const string Race = "Race";
    }

    #endregion

    #region helpers

    private static string MapXElementValues(string xmlFileContentsAsPlainText, Dictionary<string, string> mappingDictionary)
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
                throw new ArgumentException("Error. Null or blank argument. [nameOfRepeatingChildElement]");

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
                "Unable to see or retrieve multiple repeating records in your data. This might be because there aren\'t any. It might be because the items are invisible.",
                "If your data is exported from Access or Excel, be aware that row titles are generated automatically by the export wizard.",
                "The wizard takes them from the name of their containing worksheet or table or table query output as the case may be.",
                $"The problematic row title seems to be <{nameOfRepeatingChildElement}>.",
                "This needs to be the same as the title in your dataset.");

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

            if (!repeaters.Any()) throw new Exception("No rows of data found.");

            return
                repeaters.Select(repeater => new XElement(ResultDto.XeResult, repeater.Elements()))
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

            var arrayOfresultElements = resultElements.Where(z => z != null).ToArray();

            foreach (var resultElement in arrayOfresultElements)
            {
                provisionallyCorruptElement = resultElement;

                foreach (var fieldName in namesOfSuperfluousElements)
                {
                    var xE = resultElement.Element(fieldName);
                    xE?.Remove();
                }

                provisionallyCorruptItemIndex++;
            }

            return arrayOfresultElements;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro =
                $"Conversion process aborted at record count={provisionallyCorruptItemIndex}. Please inspect the following record carefully. Can you spot the problem with it? Can you see why it is incomplete or inadequate or missing something?";

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

            return XElement.Parse(inputText);
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

    private static XElement[] AddParticipantParticulars(List<XElement> listOfIndividualResultsXe,
        List<XElement> participantMasterListLineItems, out string errorMessage)
    {
        #region example of individual result Xe

        #endregion

        #region null value error handling

        if ((listOfIndividualResultsXe == null) | (participantMasterListLineItems == null))
        {
            errorMessage = "Null argument. One or both XDocuments are null. Null parent XElement.";
            return null;
        }

        #endregion

        #region setup variables to aid debugging of erroneous data

        var debugSb = new StringBuilder();

        debugSb.AppendLine("Weaving of timing-tent results, participant-master-list and points-table failed.");

        #endregion

        try
        {
            var answer = new List<XElement>();

            var i = 1; // for debugging

            foreach (var individualResultXe in
                     listOfIndividualResultsXe.Where(item => item is {HasElements: true}))
            {
                debugSb.AppendLine($"Processing result # {i}");

                debugSb.AppendLine(string.Format(individualResultXe.ToString()));

                var bibXe = individualResultXe.Elements(UbiquitousFieldNames.XeBib).First();

                var bibCode = bibXe.Value;

                debugSb.AppendLine($"Looking up bib # {bibCode} on Participant master list ....");

                var participantXe =
                    participantMasterListLineItems.FirstOrDefault(z => IsMatchingBibCode(z, bibCode));

                if (participantXe != null)
                {
                    #region add the particulars to individualResultXe

                    debugSb.AppendLine(string.Format(participantXe.ToString()));

                    debugSb.AppendLine("adding fields sourced from master Participant list to Result ....");

                    debugSb.AppendLine("dealing with xElement named first ....");

                    if (participantXe.Elements(ParticipantMasterListXeNames.First).Any())
                    {
                        individualResultXe.Elements(UbiquitousFieldNames.FirstName).Remove();
                        individualResultXe.Add(new XElement(UbiquitousFieldNames.FirstName,
                            participantXe.Elements(ParticipantMasterListXeNames.First).First().Value));
                    }

                    debugSb.AppendLine("dealing with xElement named Last ....");

                    if (participantXe.Elements(ParticipantMasterListXeNames.Last).Any())
                    {
                        individualResultXe.Elements(UbiquitousFieldNames.LastName).Remove();
                        individualResultXe.Add(new XElement(UbiquitousFieldNames.LastName,
                            participantXe.Elements(ParticipantMasterListXeNames.Last).First().Value));
                    }

                    debugSb.AppendLine("dealing with xElement named gender ....");

                    if (participantXe.Elements(ParticipantMasterListXeNames.Gender).Any())
                    {
                        individualResultXe.Elements(ResultDto.XeSex).Remove();
                        individualResultXe.Add(new XElement(ResultDto.XeSex,
                            participantXe.Elements(ParticipantMasterListXeNames.Gender).First().Value));
                        // see above. also note shift
                    }

                    debugSb.AppendLine("dealing with xElement named age_category ....");

                    if (participantXe.Elements(ParticipantMasterListXeNames.AgeCategory).Any())
                    {
                        individualResultXe.Elements(ResultDto.XeAgeGroup).Remove();
                        individualResultXe.Add(
                            new XElement(ResultDto.XeAgeGroup,
                                participantXe.Elements(ParticipantMasterListXeNames.AgeCategory).First()
                                    .Value));
                    }

                    debugSb.AppendLine("dealing with xElement named Series ....");

                    if (participantXe.Elements(ParticipantMasterListXeNames.Series).Any())
                    {
                        individualResultXe.Elements(ResultDto.XeIsSeries).Remove();
                        individualResultXe.Add(new XElement(ResultDto.XeIsSeries,
                            participantXe.Elements(ParticipantMasterListXeNames.Series).First().Value));
                        // NB note shift
                    }

                    debugSb.AppendLine("dealing with xElement named city ....");

                    if (participantXe.Elements(ParticipantMasterListXeNames.City).Any())
                    {
                        individualResultXe.Elements(ResultDto.XeCity).Remove();
                        individualResultXe.Add(new XElement(ResultDto.XeCity,
                            participantXe.Elements(ParticipantMasterListXeNames.City).First().Value));
                    }

                    #endregion
                }

                answer.Add(individualResultXe); // out

                i++;
            }

            errorMessage = null; // out

            return answer.ToArray();
        }

        #region try-catch

        catch (Exception ex)
        {
            debugSb.AppendLine("");

            debugSb.AppendLine(ex.Message);

            errorMessage = debugSb.ToString();

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Note: this method is deprecated. It was written in 2013 and has been replaced by a more sophisticated method that uses a points scale table.
    /// </summary>
    /// <param name="listOfIndividualResultsXe"></param>
    /// <param name="pointsScaleLineItems"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static XElement[] AddPointsAwarded(List<XElement> listOfIndividualResultsXe,
        List<XElement> pointsScaleLineItems, out string errorMessage)
    {
        #region example of individual result Xe

        //<Result>
        //    <AdvertisedStartDateTime>09/09/2014</AdvertisedStartDateTime>
        //    <Sex>All</Sex>
        //    <Field3>Cyclo-cross</Field3>
        //    <AgeGroup>Beginner</AgeGroup>
        //    <Bib>63</Bib>
        //    <Place>1</Place>
        //    <Chip>00:36:26</Chip>
        //    <Distance>8.17884625129582</Distance>
        //    <DistanceUnits>km</DistanceUnits>
        //    <T01>12:21</T01>
        //    <T02>12:06</T02>
        //    <T03>11:57</T03>
        //    <Race>Beginner</Race>
        //</Result>

        #endregion

        #region null value error handling

        if ((listOfIndividualResultsXe == null) | (pointsScaleLineItems == null))
        {
            errorMessage = "Null argument. One or both XDocuments are null. Null parent XElement.";
            return null;
        }

        #endregion

        #region setup variables to aid debugging of erroneous data

        var debugSb = new StringBuilder();

        debugSb.AppendLine("Weaving of timing-software results, participant-master-list and points-table failed.");

        #endregion

        try
        {
            var answer = new List<XElement>();

            var i = 1; // for debugging

            foreach (var individualResultXe in listOfIndividualResultsXe.Where(z => z != null && z.HasElements))
            {
                if (!individualResultXe.Elements(ResultDto.XeRace).Any() ||
                    !individualResultXe.Elements(XePlaceString).Any()) continue;

                debugSb.AppendLine($"Processing result # {i}");

                debugSb.AppendLine(string.Format(individualResultXe.ToString()));

                debugSb.AppendLine("doing points table lookup ....");

                var raceXe = individualResultXe.Elements(ResultDto.XeRace).First();

                var placeXe = individualResultXe.Elements(XePlaceString).First();

                var raceCode = raceXe.Value;

                var placeCode = placeXe.Value;

                var matchingPointsTableEntryXe =
                    pointsScaleLineItems.FirstOrDefault(z =>
                        IsMatchingRaceCodeAndPlaceCode(z, raceCode, placeCode));

                if (matchingPointsTableEntryXe != null)
                {
                    debugSb.AppendLine(matchingPointsTableEntryXe.ToString());

                    debugSb.AppendLine("dealing with xElement named Points ....");

                    var pointsAwardedXe = matchingPointsTableEntryXe.Elements(PointsScaleXeNames.Points).First();

                    var pointsAwarded = pointsAwardedXe.Value;

                    individualResultXe.Elements(XePointsAwardedString).Remove();

                    individualResultXe.Add(new XElement(XePointsAwardedString, pointsAwarded));
                }

                answer.Add(individualResultXe); // out
                i++;
            }

            errorMessage = null; // out

            return answer.ToArray();
        }

        #region try-catch

        catch (Exception ex)
        {
            debugSb.AppendLine("");

            debugSb.AppendLine(ex.Message);

            errorMessage = debugSb.ToString();

            return null;
        }

        #endregion
    }

    private static bool IsMatchingRaceCodeAndPlaceCode(XContainer pointScaleLineItemXe, string raceCode,
        string placeCode)
    {
        #region example of points scale Xe

        //<PointsScaleLineItem>
        //    <ID>1</ID>
        //    <Points>200</Points>
        //    <Place>1</Place>
        //    <Race>Beginner</Race>
        //</PointsScaleLineItem>

        #endregion

        if (pointScaleLineItemXe == null) throw new ArgumentNullException(nameof(pointScaleLineItemXe));
        if (raceCode == null) throw new ArgumentNullException(nameof(raceCode));
        if (placeCode == null) throw new ArgumentNullException(nameof(placeCode));

        var raceXe = pointScaleLineItemXe.Element(PointsScaleXeNames.Race);
        if (raceXe == null)
            return false;

        if (JghString.AreNotEqualIgnoreOrdinalCase(raceXe.Value, raceCode))
            return false;

        var placeXe = pointScaleLineItemXe.Element(PointsScaleXeNames.Place);
        if (placeXe == null)
            return false;

        return JghString.AreEqualIgnoreOrdinalCase(placeXe.Value, placeCode);
    }

    private static bool IsMatchingBibCode(XContainer participantXe, string bibCode)
    {
        #region example of participant Xe

        //<Participant>
        //    <Bib>65</Bib>
        //    <First>GREG</First>
        //    <Last>SCOTT</Last>
        //    <Gender>M</Gender>
        //    <Age>53</Age>
        //    <Race>NOVICE</Race>
        //    <Series>YES</Series>
        //    <City>MISSISSAUGA</City>
        //</Participant>

        #endregion

        if (participantXe == null) throw new ArgumentNullException(nameof(participantXe));
        if (bibCode == null) throw new ArgumentNullException(nameof(bibCode));

        var participantBibXe = participantXe.Element(ParticipantMasterListXeNames.Bib);

        if (participantBibXe == null) return false;

        var bibCodesDoMatch = JghString.AreEqualIgnoreOrdinalCase(participantBibXe.Value, bibCode);

        return bibCodesDoMatch;
    }

    private ResultDto[] ConvertArrayOfXElementsToArrayOfResultItemDataTransferObjects(XElement[] arrayOfIndividualResultXes)
    {
        List<ResultDto> answer = new();

        foreach (var element in arrayOfIndividualResultXes)
        {
            ResultDto resultItem = new()
            {
                Bib = element.Element(UbiquitousFieldNames.XeBib)?.Value,
                First = element.Element(ResultDto.XeFirst)?.Value,
                Last = element.Element(ResultDto.XeLast)?.Value,
                Sex = element.Element(ResultDto.XeSex)?.Value,
                RaceGroup = element.Element(ResultDto.XeRace)?.Value,
                AgeGroup = element.Element(ResultDto.XeAgeGroup)?.Value,
                City = element.Element(ResultDto.XeCity)?.Value,
                Team = element.Element(ResultDto.XeTeam)?.Value,
                IsSeries = JghConvert.ToBool(element.Element(ResultDto.XeIsSeries)?.Value),
                DnxString = element.Element(ResultDto.XeDnxString)?.Value,
                T01 = element.Element(ResultDto.XeT01)?.Value,
                T02 = element.Element(ResultDto.XeT02)?.Value,
                T03 = element.Element(ResultDto.XeT03)?.Value,
                T04 = element.Element(ResultDto.XeT04)?.Value,
                T05 = element.Element(ResultDto.XeT05)?.Value,
                T06 = element.Element(ResultDto.XeT06)?.Value,
                T07 = element.Element(ResultDto.XeT07)?.Value,
                T08 = element.Element(ResultDto.XeT08)?.Value
            };
            answer.Add(resultItem);
        }

        return answer.ToArray();
    }

    private static string WriteOneLineReport(int index, ResultItem result, string inputDuration)
    {
        string answer;

        if (string.IsNullOrWhiteSpace(inputDuration))
            answer = $"{index,-3}  {result.Bib,-3}  {result.FirstName,-15} {result.LastName,-15}  {result.T01,-15}  {result.DnxString,-3}";
        else
            answer = $"{index,-3}  {result.Bib,-3}  {result.FirstName,-15} {result.LastName,-15}  {result.T01,-15}  {result.DnxString,-3}  ({inputDuration,-15})";

        return answer;
    }

    #endregion
}