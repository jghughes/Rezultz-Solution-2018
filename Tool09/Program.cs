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
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Presumed date of event:", LhsWidth)} {DateOfThisEvent:D}");
        console.WriteLine($"{JghString.LeftAlign("Filename for Portal participants file:", LhsWidth)} {FileOfParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for Portal participants file:", LhsWidth)} {FolderForParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for Portal split intervals:", LhsWidth)} {FolderForSplitIntervalsFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for MyLaps data from Andrew:", LhsWidth)} {FolderForMyLapsTimingDataFiles}");
        console.WriteLine($"{JghString.LeftAlign("Folder for diagnostic report document:", LhsWidth)} {FolderForDiagnosticReport}");
        console.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of input and output folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForSplitIntervalsFromPortal);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForSplitIntervalsFromPortal);
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
                Directory.SetCurrentDirectory(FolderForMyLapsTimingDataFiles);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForMyLapsTimingDataFiles);
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

            var path = Path.Combine(FolderForParticipantMasterListFromPortal, FileOfParticipantMasterListFromPortal);

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

            #region confirm existence of portal timing system file of split intervals

            var resultsFileInfo = new FileInfo(FolderForSplitIntervalsFromPortal + FileOfTmeStampsConsolidatedIntoProvisionalResultsFromPortal);

            if (!resultsFileInfo.Exists)
            {
                console.WriteLine("File not found: " + resultsFileInfo.FullName);

                return;
            }

            ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileInfo = resultsFileInfo;

            #endregion

            #region try deserialise Results from portal timing system file of split durations (ResultDto[])

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

            #region find all files that exist in myLaps timing system results folders

            var di = new DirectoryInfo(FolderForMyLapsTimingDataFiles); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            #endregion

            #region find all .csv files exported from Excel that exist in myLaps timing system results folder

            foreach (var thisMyLapsFileInfo in arrayOfInputFileInfo.Where(z => z.FullName.EndsWith(".csv")))
            {
                var thisMyLapsFileItem = new MyLapsFileItem
                {
                    MyLapsFileInfo = thisMyLapsFileInfo
                };

                ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects.Add(thisMyLapsFileItem);
            }

            #endregion

            #region read text contents of files

            foreach (var myLapsFileObject in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            {
                var fileContentsAsText = await File.ReadAllTextAsync(myLapsFileObject.MyLapsFileInfo.FullName);

                myLapsFileObject.MyLapsFileContentsAsText = fileContentsAsText;

                console.WriteLine($"Loaded {myLapsFileObject.MyLapsFileInfo.Name}: {myLapsFileObject.MyLapsFileContentsAsText.Length} characters");
            }

            #endregion

            #region extract key info from .csv files

            foreach (var fileItem in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            {
                console.WriteLinePrecededByOne($"Processing .csv contents: {fileItem.MyLapsFileInfo.Name}");

                var conversionReport = new JghStringBuilder();

                var myLapsFile = new MyLapsFile(fileItem.MyLapsFileInfo.Name, fileItem.MyLapsFileInfo.FullName, fileItem.MyLapsFileContentsAsText);

                var listOfMyLapsResultItems = MyLaps2024HelperCsv.GenerateResultItemArrayFromMyLapsFile(myLapsFile, null, null, DateTime.Today, conversionReport, 30).ToList();

                var xx = listOfMyLapsResultItems
                    .Select(z => new MyLapsResultItem(z.Bib, z.FullName, z.T01, z.RaceGroup)).ToList();

                fileItem.MyLapsFileContentsAsMyLapsResultObjects.AddRange(xx);

                console.WriteLine(conversionReport.ToString());

                console.WriteLine($"Successfully(?) extracted .csv contents of {fileItem.MyLapsFileInfo.Name}: {fileItem.MyLapsFileContentsAsMyLapsResultObjects.Count} line items");
            }

            #endregion

            #region convert lists of results to dictionaries keyed by Bib for easy analysis

            JghListDictionary<string, ResultDto> dictionaryOfPortalTimingSystemResultsKeyedByBib = new();

            foreach (var resultDto in ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileContentsAsResultsDataTransferObjects) dictionaryOfPortalTimingSystemResultsKeyedByBib.Add(resultDto.Bib, resultDto);

            JghListDictionary<string, MyLapsResultItem> dictionaryOfMyLapsTimingSystemResultsKeyedByBib = new();

            foreach (var myLapsFile in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            foreach (var myLapsResult in myLapsFile.MyLapsFileContentsAsMyLapsResultObjects)
                dictionaryOfMyLapsTimingSystemResultsKeyedByBib.Add(myLapsResult.Bib, myLapsResult);

            #endregion

            #region report Bibs missing in Mylaps i.e. that are listed in Results from Portal but not in MyLaps

            JghStringBuilder sb = new();

            var i = 0;

            foreach (var kvp in dictionaryOfPortalTimingSystemResultsKeyedByBib)
            {
                if (kvp.Value.Any(z => (!string.IsNullOrWhiteSpace(z.DnxString))))
                    continue; // skip people who have a DNF, DNS, etc.

                if (dictionaryOfMyLapsTimingSystemResultsKeyedByBib.ContainsKey(kvp.Key)) continue; //todo: check for DNF, DNS, etc. (currently the data is not available in the MyLaps files)

                var person = kvp.Value.FirstOrDefault();

                if (person is null) continue;

                sb.AppendLine($"Bib: Series={$"{kvp.Key}",4}  {$"{person.First} {person.Last}",-25}  {$"{person.T01}", 15}  IsSeries: {$"{person.IsSeries}", 5}  SeriesGroup: {person.RaceGroup}");

                i += 1;

            }

            console.WriteLineWrappedByOne($"MYLAPS TIMING SYSTEM: MISSING finishers: {i} (excluding Dnx outcomes in Portal)");

            console.WriteLine(sb.ToString());

            #endregion

            #region report Bibs missing in Portal i.e. that are listed in Results from Portal but not in MyLaps

            JghStringBuilder sb1 = new();

            var j = 0;

            foreach (var kvp in dictionaryOfMyLapsTimingSystemResultsKeyedByBib)
            {
               
                if (dictionaryOfPortalTimingSystemResultsKeyedByBib.ContainsKey(kvp.Key)) continue;

                var person = kvp.Value.FirstOrDefault();

                if (person is null) continue;

                sb1.AppendLine($"Bib: Series={$"{kvp.Key}",4}  {$"{person.FullName}",-25} {$"{person.DurationAsString}",15}  RaceDayGroup: {person.RaceGroup}");

                j += 1;

            }

            console.WriteLineWrappedByOne($"PORTAL TIMING SYSTEM: MISSING finishers: {j}");

            console.WriteLine(sb1.ToString());

            #endregion

            #region report people who raced in a different RaceGroup in MyLaps compared to RaceGroup they are registered for in the Portal for the series


            JghStringBuilder sb2 = new();

            var ii = 0;

            foreach (var listKeyedByBib in dictionaryOfMyLapsTimingSystemResultsKeyedByBib)
            {
                var personInMyLapsTimingSystem = listKeyedByBib.Value.FirstOrDefault();

                if (personInMyLapsTimingSystem is null) continue;

                var registeredSeriesParticipant = DictionaryOfParticipantsInPortalMasterListKeyedByBib[personInMyLapsTimingSystem.Bib].FirstOrDefault(z => z.IsSeries == true);

                if (registeredSeriesParticipant is null) continue;

                var registeredRaceGroupOfSeriesParticipantInPortal = FigureOutRaceGroup(ParticipantHubItem.ToDataTransferObject(registeredSeriesParticipant), DateOfThisEvent);

                if (registeredRaceGroupOfSeriesParticipantInPortal == personInMyLapsTimingSystem.RaceGroup) continue;

                sb2.AppendLine($"Bib: Series/RaceDay= {$"{registeredSeriesParticipant.Bib}-{personInMyLapsTimingSystem.Bib}", 8} {$"{personInMyLapsTimingSystem.FullName}",-25} Group: Series/RaceDay= {registeredRaceGroupOfSeriesParticipantInPortal}/{personInMyLapsTimingSystem.RaceGroup}"); 
                
                ii += 1;
            }

            console.WriteLineWrappedByOne($"CATEGORY CONFLICTS between SeriesGroup and RaceDayGroup: {ii}");

            console.WriteLine(sb2.ToString());

            #endregion

            #region wrap up

            console.WriteLineWrappedByOne("Diagnostic report:");
            var prettyFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FileOfDiagnosticReport);
            SaveWorkToHardDrive(console.ToString(), FolderForDiagnosticReport, prettyFileName);

            console.WriteLinePrecededByOne("Everything complete. No further action required. Goodbye.");
            console.WriteLine("ooo0 - Goodbye - 0ooo");
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

        Console.WriteLine($"{JghString.LeftAlign("File saved:", 15)} {outPutFilename}");
        Console.WriteLine($"{JghString.LeftAlign("Folder:", 15)} {outPutFolder}");
    }

    #endregion

    #region parameters

    private const string Description = "This console program (Tool09) reads files of MyLaps timing data and compares the electronic " +
                                       "data from the timing mat to a (potentially totally different) set of timing data recorded in " +
                                       "the timing tent using the Portal timing system. The purpose of doing this is to search for gaps " +
                                       "in the data by means of comparison. Based on empirical experience, there are typically about ten " +
                                       "anomalies in a Kelso event because the electronic mat misses people. The Portal timing team also misses " +
                                       "people, but generallly fewer. The pipeline for this tool is that data is exported from MyLaps in Excel " +
                                       "format, then exported from Excel in .csv format, and then finally imported into this tool as .csv and " +
                                       "deserialised and analysed. Portal data is exported effortlessly by clicking the export button on the " +
                                       "Portal Timing Tools screen and exporting the data as timestamps consolidated into provisional results.";

    private const int LhsWidth = 40;

    private static readonly DateTime DateOfThisEvent = new(2024, 5,14);

    private const string FolderForCommonStuff = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R1 May 14\DiagnosticReport\";
    private const string FolderForMyLapsTimingDataFiles = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R1 May 14\ExportedAsCsv\";

    private const string FolderForParticipantMasterListFromPortal = FolderForCommonStuff;
    private const string FolderForSplitIntervalsFromPortal = FolderForCommonStuff;
    private const string FolderForDiagnosticReport = FolderForCommonStuff;

    private const string FileOfDiagnosticReport = @"MyLapsVersusPortalTimingDataDiagnosticReport.txt";
    private const string FileOfParticipantMasterListFromPortal = @"2024-06-22T11-32-49+Participants.json";
    private const string FileOfTmeStampsConsolidatedIntoProvisionalResultsFromPortal = @"2024-06-23T12-27-14+DraftResultsForLeaderboard.xml";

    #endregion
}