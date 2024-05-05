using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using RezultzSvc.Library02.Mar2024.SvcHelpers;

namespace RezultzSvc.Library02.Mar2024.PublisherModules;

/// <summary>
///     Validates a single file of results synthesised inside RezultzPortal.Uwp from hub
///     timing data and participant data. Results from the native timing system require
///     no further processing. This module merely validates the xml format of the data
///     by deserialising the xml file and ensuring it doesn't blow up.
///     The xml profile associated with this module is
///     https://systemrezultzlevel1.blob.core.windows.net/publishingmoduleprofiles/publisherprofile-21portal.xml
///     The value of DatasetIdentifier in the one and only GuiButtonProfile in the xml file MUST be identical
///     to Jgh.SymbolsStringsConstants.Mar2022.EnumsForPublisherModule.ResultItemsAsXmlFromPortalNativeTimingSystem.
///     If it is not, this module fails outright. This enum is how we tie datasets together.
/// </summary>
public class PublisherForRezultzPortalTimingSystem2021 : PublisherBase
{
    private const string Locus2 = nameof(PublisherForRezultzPortalTimingSystem2021);
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

            #region results data from Rezultz Portal

            // Note: if you go look see in the xml profile, you will see that in this module we are only looking for the contents of a single file - EnumForResultItemsFromSystemHub

            conversionReportSb.AppendLine("Doing null checks to confirm that we have a file of timestamps and participants from the Rezultz hub consolidated into results");

            var targetFile = publisherInputItem.DeduceStorageLocation(IdentifierOfResultItemsFromRezultzHub) ??
                             throw new JghAlertMessageException("Results file not identified. Please import a file");

            if (!await _storage.GetIfBlobExistsAsync(targetFile.AccountName, targetFile.ContainerName, targetFile.EntityName))
                throw new JghAlertMessageException($"Error. Missing file. Expected Results file not found. <{targetFile.EntityName}>");

            var contentsOfDatasetAsString = JghConvert.ToStringFromUtf8Bytes(
                await _storage.DownloadBlockBlobAsBytesAsync(targetFile.AccountName, targetFile.ContainerName, targetFile.EntityName));

            if (string.IsNullOrWhiteSpace(contentsOfDatasetAsString))
                throw new JghAlertMessageException($"Results file is empty. <{targetFile.EntityName}>");

            #endregion

            #region just to confirm this is plausably a file of ResultDto[] as XMl, deserialise it and convert to ResultItems. Hopefully will blow up if alien data

            conversionReportSb.AppendLinePrecededByOne($"Processing results file emanating for Rezultz Portal app <{targetFile.EntityName}>");

            conversionReportSb.AppendLine($"Double-checking that deserialisation of array of ResultDto objects is successful");

            List<ResultItem> allComputedResults;
            try
            {


                var dummy1 = XDocument.Parse(contentsOfDatasetAsString);

                var dummy2 = dummy1.Element(ResultDto.XeRootForContainerOfSimpleStandAloneArray);

                if (dummy2 == null) throw new JghAlertMessageException($"The root of this file is wrongly named. The obligatory name is <{ResultDto.XeRootForContainerOfSimpleStandAloneArray}>. Please investigate the file.");

                var resultsFromSystemHub = JghSerialisation.ToObjectFromXml<ResultDto[]>(contentsOfDatasetAsString, new[] {typeof(ResultDto[])});

                allComputedResults = ResultItem.FromDataTransferObject(resultsFromSystemHub).OrderBy(z => z.RaceGroup).ThenBy(z => z.DnxString).ThenBy(z => z.T01).ToList();

                // todo: do some additional checks to inspect the xml to see if it is plausible, escalating in cleverness
            }
            catch (Exception e)
            {
                var errorMsg = JghString.ConcatAsParagraphs("Failure.", "Format and/or content of file is not what publisher is coded to expect.",
                    "A valid array of ResultDto objects in XML format is expected from timestamps and participants pulled from the hub.", $"({e.Message})");

                throw new JghAlertMessageException(errorMsg);
            }

            #endregion

            #region do all computations and calculations

            // this data requires no computations or calculations. If the null checks succeeded, and the deserialisation checks succeeded, the data is likely
            // in the correct format for Rezultz 2018 import API. Simply return what the file contains. i.e. the results that were in the file in the first place

            conversionReportSb.AppendLineWrappedByOne($"{JghString.LeftAlign("Results synthesised from this file", lhsWidth)} : {allComputedResults.Count}");

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
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Source dataset:", lhsWidthLess2)} <{targetFile.EntityName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{targetFile.ContainerName}>");
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

    private const string IdentifierOfResultItemsFromRezultzHub = EnumsForPublisherModule.ResultItemsAsXmlFromPortalNativeTimingSystem;
    
    #endregion

    #region fields

    private readonly AzureStorageServiceMethodsHelper _storage = new(new AzureStorageAccessor());

    #endregion

    #region helpers

    private static string WriteOneLineReport(int index, ResultItem resultItem, string inputDuration)
    {
        string answer;

        if (string.IsNullOrWhiteSpace(inputDuration))
            answer = $"{index,-3}  {resultItem.Bib,-3}  {resultItem.FirstName,-15} {resultItem.LastName,-20}  {resultItem.RaceGroup,-15}  {resultItem.T01,-15}  {resultItem.DnxString,-3}";
        else
            answer = $"{index,-3}  {resultItem.Bib,-3}  {resultItem.FirstName,-15} {resultItem.LastName,-20}  {resultItem.RaceGroup,-15}  {resultItem.T01,-15}  {resultItem.DnxString,-3}  <{inputDuration,-15}>";

        return answer;
    }

    #endregion
}