using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;
using Tool12;

namespace Tool09;

internal class Program
{
    #region the MEAT

    private static async Task Main()
    {
        const string description = "This program (Tool09) reads files of MyLaps timing data and compares the electronic " +
                                   "data from the timing mat to a (potentially totally different) set of timing data recorded in " +
                                   "the timing tent using the Portal timing system. The purpose of doing this is to search for gaps " +
                                   "in the data by means of comparison. Based on empirical experience, there are typically about ten " +
                                   "anomalies each week because the electronic mat misses people. With a double-mat for redundancy, the anomalies are reduced " +
                                   "The Portal timing team also misses people, but generally fewer. The pipeline for this tool is that data is exported from MyLaps in Excel " +
                                   "format, then exported from Excel in .csv format, and then finally imported into this tool as .csv and " +
                                   "deserialized and analysed. Portal data is exported effortlessly by clicking the export button on the " +
                                   "Portal Timing Tools screen and exporting the data as timestamps consolidated into provisional results.";


        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(description);
        console.WriteLine($"{JghString.LeftAlign("Presumed date of event:", LhsWidth)} {DateOfThisEvent:D}");
        console.WriteLine($"{JghString.LeftAlign("Filename for Portal participants file:", LhsWidth)} {FileNameOfParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for MyLaps data from Andrew:", LhsWidth)} {FolderForMyLapsTimingDataFilesFromAndrew}");
        console.WriteLine($"{JghString.LeftAlign("Folder for Portal participants file:", LhsWidth)} {FolderForParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for Portal split intervals:", LhsWidth)} {FolderForInputDataFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for diagnostic report documents:", LhsWidth)} {FolderForDiagnosticReport}");
        console.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of input and output folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForInputDataFromRezultzPortal);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForInputDataFromRezultzPortal);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForParticipantMasterListFromPortal);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForParticipantMasterListFromPortal);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForMyLapsTimingDataFilesFromAndrew);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForMyLapsTimingDataFilesFromAndrew);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForDiagnosticReport);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForDiagnosticReport);
                return;
            }

            #endregion

            #region confirm existence of participantHubItems .json file from portal

            // NB: be sure that this file is totally up-to-date. changes are being made daily!

            var path = Path.Combine(FolderForParticipantMasterListFromPortal, FileNameOfParticipantMasterListFromPortal);

            var portalParticipantFileInfo = new FileInfo(path);

            if (!portalParticipantFileInfo.Exists)
            {
                console.WriteLine($"Failed to locate designated participant file. <{portalParticipantFileInfo.Name}>");

                return;
            }

            #endregion

            #region deserialise participantHubItems from .json file from Portal

            try
            {
                var rawInputAsText = await File.ReadAllTextAsync(portalParticipantFileInfo.FullName);

                var participantHubItemDtoArray = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto[]>(rawInputAsText);

                ParticipantHubItem[] participantHubItems = ParticipantHubItem.FromDataTransferObject(participantHubItemDtoArray);

                foreach (var participantHubItem in participantHubItems)
                    if (!string.IsNullOrWhiteSpace(participantHubItem.Bib))
                        DictionaryOfParticipantsInPortalMasterListKeyedByBib.Add(participantHubItem.Bib, participantHubItem);
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            console.WriteLine();

            #endregion

            #region confirm existence of .xml file of consolidated split intervals exported manually from Portal timing system

            var resultsFileInfo = new FileInfo(FolderForInputDataFromRezultzPortal + FileNameOfTmeStampsConsolidatedIntoSplitIntervalProvisionalResultsFromPortal);

            if (!resultsFileInfo.Exists)
            {
                console.WriteLine("File not found: " + resultsFileInfo.FullName);

                return;
            }

            ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileInfo = resultsFileInfo;

            #endregion

            #region try deserialise split intervals (ResultDto[])

            ResultDto[]? provisionalResultsFromPortal;

            try
            {
                var rawInputAsText = await File.ReadAllTextAsync(resultsFileInfo.FullName);

                provisionalResultsFromPortal = JghSerialisation.ToObjectFromXml<ResultDto[]>(rawInputAsText, [typeof(ResultDto)]);
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialisation failure. Results from file exported from Portal not obtained: " + ex.Message);
                return;
            }

            ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileContentsAsResultsDataTransferObjects = provisionalResultsFromPortal.ToList();

            console.WriteLine($"Loaded {resultsFileInfo.Name}: {provisionalResultsFromPortal.Length} results");

            #endregion

            #region confirm existence of .csv files exported manually from Excel files from Andrew

            var di = new DirectoryInfo(FolderForMyLapsTimingDataFilesFromAndrew); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLinePrecededByOne($"{JghString.LeftAlign($"Problem: No files found in MyLaps timing data folder. [{FolderForMyLapsTimingDataFilesFromAndrew}]", LhsWidth)}");

                return;
            }

            foreach (var thisMyLapsFileInfo in arrayOfInputFileInfo.Where(z => z.FullName.EndsWith(".csv")))
            {
                var thisMyLapsFileItem = new MyLapsFileItem
                {
                    MyLapsFileInfo = thisMyLapsFileInfo
                };

                ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects.Add(thisMyLapsFileItem);
            }

            #endregion

            #region read text contents of .csv files

            foreach (var myLapsFileObject in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            {
                var fileContentsAsText = await File.ReadAllTextAsync(myLapsFileObject.MyLapsFileInfo.FullName);

                myLapsFileObject.MyLapsFileContentsAsText = fileContentsAsText;

                console.WriteLine($"Loaded {myLapsFileObject.MyLapsFileInfo.Name}: {myLapsFileObject.MyLapsFileContentsAsText.Length} characters");
            }

            #endregion

            #region extract info from .csv files using freeform text parsing

            foreach (var myLapsFileItem in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            {
                console.WriteLinePrecededByOne($"Processing .csv contents: {myLapsFileItem.MyLapsFileInfo.Name}");

                var conversionReport = new JghStringBuilder();

                var myLapsFile = new RezultzSvc.Library02.Mar2024.PublisherModuleHelpers.MyLapsFileItem(myLapsFileItem.MyLapsFileInfo.Name, myLapsFileItem.MyLapsFileInfo.FullName, myLapsFileItem.MyLapsFileContentsAsText);

                var listOfMyLapsResultItems = MyLaps2024HelperCsv.GenerateResultItemArrayFromMyLapsFile(myLapsFile, null, null, DateTime.Today, conversionReport, 30).ToList();

                var xx = listOfMyLapsResultItems
                    .Select(z => new MyLapsResultItem(z.Bib, z.FullName, z.T01, z.RaceGroup, z.Comment)).ToList();

                myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects.AddRange(xx);

                console.WriteLine(conversionReport.ToString());

                console.WriteLine($"Successfully(?) extracted .csv contents of {myLapsFileItem.MyLapsFileInfo.Name}: {myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects.Count} line items");
            }

            #endregion

            #region convert lists of Portal and MyLaps results to dictionaries keyed by Bib for easy analysis

            JghListDictionary<string, ResultDto> dictionaryOfPortalTimingSystemResultsKeyedByBib = [];

            foreach (var resultDto in ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileContentsAsResultsDataTransferObjects) dictionaryOfPortalTimingSystemResultsKeyedByBib.Add(resultDto.Bib, resultDto);

            JghListDictionary<string, MyLapsResultItem> dictionaryOfMyLapsTimingSystemResultsKeyedByBib = new();

            foreach (var myLapsFile in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            foreach (var myLapsResult in myLapsFile.MyLapsFileContentsAsMyLapsResultObjects)
                dictionaryOfMyLapsTimingSystemResultsKeyedByBib.Add(myLapsResult.Bib, myLapsResult);

            #endregion


            #region report top finishers in Mylaps

            JghListDictionary<string, MyLapsResultItem> dictionaryOfTopFinishersKeyedByRaceGroup = [];

            foreach (var kvp in dictionaryOfMyLapsTimingSystemResultsKeyedByBib)
            {
                var myLapsResult = kvp.Value.FirstOrDefault();

                if (myLapsResult is null) continue;

                dictionaryOfTopFinishersKeyedByRaceGroup.Add(myLapsResult.RaceGroup, myLapsResult);
            }

            console.WriteLineWrappedByOne("MYLAPS TIMING SYSTEM: Top finishers: ");

            foreach (var kvp in dictionaryOfTopFinishersKeyedByRaceGroup)
            {
                var myLapsResults = kvp.Value.OrderBy(z => z.DurationAsString).Take(5);

                foreach (var myLapsResult in myLapsResults) console.WriteLine($"Bib:{$"{myLapsResult.Bib}",4} {$"{myLapsResult.FullName}",-25} {$"{myLapsResult.RaceGroup}",-13} {$"{myLapsResult.DurationAsString}",15} ");

                console.WriteLine();
            }

            #endregion


            #region report results missing in Mylaps i.e. that are listed in Results from Portal but not in MyLaps

            JghStringBuilder sb = new();

            List<ResultDto> resultsForInclusionInMyLaps = [];

            var i = 0;

            foreach (var kvp in dictionaryOfPortalTimingSystemResultsKeyedByBib)
            {
                if (kvp.Value.Any(z => !string.IsNullOrWhiteSpace(z.DnxString)))
                    continue; // skip people who have a DNF, DNS, etc.

                if (dictionaryOfMyLapsTimingSystemResultsKeyedByBib.ContainsKey(kvp.Key)) continue;

                var portalResult = kvp.Value.FirstOrDefault();

                if (portalResult is null) continue;

                resultsForInclusionInMyLaps.Add(portalResult);

                sb.AppendLine($"Bib:{$"{kvp.Key}",4} {$"{portalResult.First} {portalResult.Last}",-25} IsSeries={$"{portalResult.IsSeries}",-5} {$"{portalResult.RaceGroup}",-13} {$"{portalResult.T01}",15} ");

                i += 1;
            }

            console.WriteLineWrappedByOne($"MYLAPS TIMING SYSTEM: MISSING finishers: {i} (excluding Dnx outcomes in Portal)");

            console.WriteLine(sb.ToString());

            var resultsForInclusionInMyLapsAsXml = JghSerialisation.ToXElementFromObject(resultsForInclusionInMyLaps, new[] { typeof(ResultDto) });

            SaveWorkToHardDrive(resultsForInclusionInMyLapsAsXml.ToString(),
                FolderForDiagnosticReport,
                JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FileNameOfResultsForAdditionToMyLaps));

            #endregion

            #region report results missing in Portal i.e. that are listed in MyLaps but not in Results from Portal

            JghStringBuilder sb1 = new();

            var j = 0;

            foreach (var kvp in dictionaryOfMyLapsTimingSystemResultsKeyedByBib)
            {
                if (dictionaryOfPortalTimingSystemResultsKeyedByBib.ContainsKey(kvp.Key)) continue;

                var myLapsResult = kvp.Value.FirstOrDefault();

                if (myLapsResult is null) continue;

                sb1.AppendLine($"Bib:{$"{kvp.Key}",4} {$"{myLapsResult.FullName}",-25} {$"{myLapsResult.RaceGroup}",-13} {$"{myLapsResult.DurationAsString}",15}");

                //sb1.AppendLine($"Bib: Series={$"{kvp.Key}",4}  {$"{myLapsResult.FullName}",-25} {$"{myLapsResult.DurationAsString}",15}  RaceDayGroup: {myLapsResult.RaceGroup}");

                j += 1;
            }

            console.WriteLineWrappedByOne($"PORTAL TIMING SYSTEM: MISSING finishers: {j}");

            console.WriteLine(sb1.ToString());

            #endregion

            #region report people who raced in a different RaceGroup in MyLaps compared to RaceGroup they are registered for in the Portal for the series

            JghStringBuilder sb2 = new();

            //List<MyLapsResultItem> resultsForExclusionFromAllSeriesPointsCalculationsForSpecialReasons = [];
            List<ResultDto> forExclusionFromAllSeriesPointsCalculationsForSpecialReasons = [];

            var ii = 0;

            foreach (var listKeyedByBib in dictionaryOfMyLapsTimingSystemResultsKeyedByBib)
            {
                var myLapsResult = listKeyedByBib.Value.FirstOrDefault();

                if (myLapsResult is null) continue;

                var registeredSeriesParticipant = DictionaryOfParticipantsInPortalMasterListKeyedByBib[myLapsResult.Bib].FirstOrDefault(z => z.IsSeries);

                if (registeredSeriesParticipant is null) continue;

                var registeredRaceGroupOfSeriesParticipantInPortal = FigureOutRaceGroup(ParticipantHubItem.ToDataTransferObject(registeredSeriesParticipant), DateOfThisEvent);

                if (registeredRaceGroupOfSeriesParticipantInPortal == JghString.TmLr(myLapsResult.RaceGroup)) continue;

                sb2.AppendLine(
                    $"Bib: Raced/Series={$"{registeredSeriesParticipant.Bib}-{myLapsResult.Bib}",8} {$"{myLapsResult.FullName}",-25} Raced: {myLapsResult.RaceGroup,-13} Series: {registeredRaceGroupOfSeriesParticipantInPortal,-13} {myLapsResult.DurationAsString,12} Not in series category");
                //sb2.AppendLine($"Bib: Series/RaceDay= {$"{registeredSeriesParticipant.Bib}-{myLapsResult.Bib}", 8} {$"{myLapsResult.FullName}",-25} Group: Series/RaceDay= {registeredRaceGroupOfSeriesParticipantInPortal}/{myLapsResult.RaceGroup}");
                forExclusionFromAllSeriesPointsCalculationsForSpecialReasons.Add(new ResultDto { Bib = myLapsResult.Bib, Last = myLapsResult.FullName, IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons = true });
                ii += 1;
            }

            console.WriteLineWrappedByOne($"CATEGORY CONFLICTS between SeriesGroup and RaceDayGroup: {ii}");

            console.WriteLine(sb2.ToString());

            var resultsForExclusionFromAllSeriesPointsCalculationsForSpecialReasonsAsXml = JghSerialisation.ToXElementFromObject(forExclusionFromAllSeriesPointsCalculationsForSpecialReasons, new[] { typeof(ResultDto) });

            SaveWorkToHardDrive(resultsForExclusionFromAllSeriesPointsCalculationsForSpecialReasonsAsXml.ToString(),
                FolderForDiagnosticReport,
                JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FileNameOfMyLapsResultsForExclusionFromAllSeriesPointsCalculationsForSpecialReasons));

            #endregion

            #region wrap up

            console.WriteLinePrecededByOne("Everything complete. No further action required. Goodbye.");
            console.WriteLineWrappedByOne("ooo0 - Goodbye - 0ooo");

            SaveWorkToHardDrive(console.ToString(),
                FolderForDiagnosticReport,
                JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FileNameOfDiagnosticReport));

            console.ReadLine();

            #endregion
        }
        catch (Exception ex)
        {
            console.WriteLineFollowedByOne(ex.ToString());
            console.ReadLine();
        }
    }

    #endregion

    #region variables

    private static readonly JghConsoleHelperV2 console = new();

    private static readonly JghListDictionary<string, ParticipantHubItem> DictionaryOfParticipantsInPortalMasterListKeyedByBib = [];

    private static readonly PortalTimingSystemFileBeingAnalysedItem ResultsPortalTimingSystemFileBeingAnalysed = new();

    #endregion

    #region helper methods

    public static string FigureOutRaceGroup(ParticipantHubItemDto participantItem, DateTime dateOfEvent)
    {
        var isTransitionalParticipant = participantItem.RaceGroupBeforeTransition != participantItem.RaceGroupAfterTransition;

        string answerAsRaceGroup;

        if (isTransitionalParticipant)
        {
            if (DateTime.TryParse(participantItem.DateOfRaceGroupTransitionAsString, out var dateOfTransition))
            {
                var eventIsBeforeRaceGroupTransition = dateOfEvent < dateOfTransition;

                answerAsRaceGroup = eventIsBeforeRaceGroupTransition ? participantItem.RaceGroupBeforeTransition : participantItem.RaceGroupAfterTransition;

                return answerAsRaceGroup;
            }

            answerAsRaceGroup = participantItem.RaceGroupBeforeTransition;
        }
        else
        {
            answerAsRaceGroup = participantItem.RaceGroupBeforeTransition;
        }

        return answerAsRaceGroup;
    }

    private static void SaveWorkToHardDrive(string text, string outPutFolder, string outPutFilename)
    {
        var pathOfFile = Path.Combine(outPutFolder, outPutFilename);

        File.WriteAllText(pathOfFile, text);

        console.WriteLine($"{JghString.LeftAlign("File saved:", 15)} {outPutFilename}");
        console.WriteLine($"{JghString.LeftAlign("Folder:", 15)} {outPutFolder}");
    }

    #endregion

    #region parameters

    private const int LhsWidth = 40;

    private const string Event = "R6-July-02";
    private static readonly DateTime DateOfThisEvent = new(2024, 7, 02);
    private const string FileNameOfTmeStampsConsolidatedIntoSplitIntervalProvisionalResultsFromPortal = @"2024-07-17T14-11-13+SplitIntervalsFromRezultzPortalTimingSystem.xml";

    private const string FileNameOfDiagnosticReport = @"MyLapsVersusPortalTimingDataDiagnosticReport-" + Event + ".txt";
    private const string FileNameOfResultsForAdditionToMyLaps = @"SplitIntervalsFromPortalTimingSystemForAdditionToMyLaps-" + Event + ".xml";
    private const string FileNameOfMyLapsResultsForExclusionFromAllSeriesPointsCalculationsForSpecialReasons = @"MyLapsResultsForExclusionFromAllSeriesPointsCalculationsForSpecialReasons-" + Event + ".xml";
    private const string FolderForMyLapsTimingDataFilesFromAndrew = @"C:\Users\johng\holding pen\StuffFromAndrew\2024-MyLaps-Timing-Data\" + Event + @"\ExportedDirectlyAsCsv\";
    private const string FolderForInputDataFromRezultzPortal = @"C:\Users\johng\holding pen\StuffByJohn\2024-MyLaps-Timing-Analysis\" + Event + @"\InputFromRezultzPortal\";
    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffByJohn\2024-MyLaps-Timing-Analysis\" + Event + @"\MyLapsVersusPortalComparison\";

    private const string FileNameOfParticipantMasterListFromPortal = @"2024-07-17T14-01-53+Participants.json";
    private const string FolderForParticipantMasterListFromPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";

    #endregion
}