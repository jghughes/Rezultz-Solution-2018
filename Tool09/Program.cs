using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;
using System;
using System.Reflection;
using Tool12;

namespace Tool09;

internal class Program
{
    #region variables

    private static readonly JghConsoleHelperV2 console = new();

    private static readonly JghListDictionary<string, ParticipantHubItem> DictionaryOfParticipantsInPortalMasterListKeyedByBib = [];

    private static readonly PortalTimingSystemFileBeingAnalysedItem ResultsPortalTimingSystemFileBeingAnalysed = new();

    #endregion

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Input filename for Portal participants file:", LhsWidth)} {NameOfParticipantMasterListFromPortalFile}");
        console.WriteLine($"{JghString.LeftAlign("Input folder for Portal participants file:", LhsWidth)} {InputFolderOfParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Input folder for Portal timing module split intervals", LhsWidth)} : {InputFolderOfSplitIntervalsFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Input folder for MyLaps data from Andrew", LhsWidth)} : {InputFolderFromMyLaps}");
        console.WriteLine($"{JghString.LeftAlign("Output folder for diagnostic report document", LhsWidth)} : {InputFolderFromMyLaps}");
        console.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of input and output folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolderOfSplitIntervalsFromPortal);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + InputFolderOfSplitIntervalsFromPortal);
                return;
            }
            

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolderOfParticipantMasterListFromPortal);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + InputFolderOfParticipantMasterListFromPortal);
                return;
            }


            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolderFromMyLaps);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + InputFolderFromMyLaps);
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

            var path = Path.Combine(InputFolderOfParticipantMasterListFromPortal, NameOfParticipantMasterListFromPortalFile);

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

            var resultsFileInfo = new FileInfo(InputFolderOfSplitIntervalsFromPortal + NameOfPortalRezultzFile);

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

            var di = new DirectoryInfo(InputFolderFromMyLaps); // Create a reference to the input directory.

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

            console.WriteLineWrappedByOne("MYLAPS SPREADSHEETS MISSING finishers:");

            foreach (var kvp in dictionaryOfPortalTimingSystemResultsKeyedByBib)
            {
                if (kvp.Value.Any(z => !string.IsNullOrWhiteSpace(z.DnxString)))
                    continue; // skip people who have a DNF, DNS, etc.

                if (dictionaryOfMyLapsTimingSystemResultsKeyedByBib.ContainsKey(kvp.Key)) continue; //todo: check for DNF, DNS, etc. (currently the data is not available in the MyLaps files)

                var person = kvp.Value.FirstOrDefault();

                if (person is null) continue;

                console.WriteLine($"<{kvp.Key}  {person.First}  {person.Last}  {person.RaceGroup}  {person.T01}>");
            }

            #endregion

            #region report Bibs missing in Portal i.e. that are listed in Results from Portal but not in MyLaps

            console.WriteLineWrappedByOne("PORTAL HUB MISSING finishers:");

            foreach (var kvp in dictionaryOfMyLapsTimingSystemResultsKeyedByBib)
            {
                if (dictionaryOfPortalTimingSystemResultsKeyedByBib.ContainsKey(kvp.Key)) continue;

                var person = kvp.Value.FirstOrDefault();

                if (person is null) continue;

                console.WriteLine($"<{kvp.Key}  {person.FullName}  {person.RaceGroup}  {person.DurationAsString}>");
            }

            console.WriteLinePrecededByOne("The End");

            #endregion


            #region convert lists of results to dictionaries keyed by FullName for further analysis

            JghListDictionary<string, ResultDto> dictionaryOfPortalTimingSystemResultsKeyedByName = new();

            foreach (var resultDto in ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileContentsAsResultsDataTransferObjects)
            {
                var key = JghString.TmLr(JghString.Remove(' ', JghString.Concat(resultDto.First, resultDto.Last)));

                dictionaryOfPortalTimingSystemResultsKeyedByName.Add(key, resultDto);
            }

            JghListDictionary<string, MyLapsResultItem> dictionaryOfMyLapsTimingSystemResultsKeyedByName = new();

            foreach (var myLapsFile in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            foreach (var myLapsResult in myLapsFile.MyLapsFileContentsAsMyLapsResultObjects)
            {
                var key = JghString.TmLr(JghString.Remove(' ', myLapsResult.FullName));

                dictionaryOfMyLapsTimingSystemResultsKeyedByName.Add(key, myLapsResult);
            }

            #endregion


            #region report Names who raced in a different RaceGroup compared to RaceGroup they are registered in for the series 


            console.WriteLineWrappedByOne("MYLAPS DISCREPANCIES between registered series category (in Portal master list) and race-day category in MyLaps:");

            #region analyse names in both portal and points, but only those who have conflicting categories

            var allNameKeysInPortal = dictionaryOfPortalTimingSystemResultsKeyedByName.Keys.ToList();
            var allNameKeysInPoints = dictionaryOfMyLapsTimingSystemResultsKeyedByName.Keys.ToList();

            var distinctNameKeysInBothPortalAndPoints = allNameKeysInPortal.Intersect(allNameKeysInPoints).Distinct().ToArray();

            List<string> conflictedNamesInPortal = [];
            List<string> conflictedNamesInPoints = [];

            JghStringBuilder sb2 = new();

            var ii = 0;

            foreach (var key in distinctNameKeysInBothPortalAndPoints)
            {
                var personInPortal = dictionaryOfPortalTimingSystemResultsKeyedByName[key].FirstOrDefault();

                if (personInPortal is not null)
                {
                    var registeredPersonInPortal = DictionaryOfParticipantsInPortalMasterListKeyedByBib[personInPortal.Bib].FirstOrDefault();

                    var personInPoints = dictionaryOfMyLapsTimingSystemResultsKeyedByName[key].FirstOrDefault();

                    var registeredRaceGroupInPortal = FigureOutRaceGroup(ParticipantHubItem.ToDataTransferObject(registeredPersonInPortal), DateOfThisEvent);

                    if (registeredPersonInPortal is not null && personInPoints is not null && registeredRaceGroupInPortal != personInPoints.RaceGroup)
                    {
                        sb2.AppendLine(
                            $"Bib: RegisteredForSeriesInPortal/MyLaps={JghString.LeftAlign($"{registeredPersonInPortal.Bib}-{personInPoints.Bib}", LhsWidthSmall)} {personInPoints.FullName}  RaceGroup: RegisteredForSeriesInPortal/MyLaps=({registeredRaceGroupInPortal})/{personInPoints.RaceGroup}");

                        conflictedNamesInPortal.Add(registeredPersonInPortal.Bib);
                        conflictedNamesInPoints.Add(personInPoints.Bib);
                        ii += 1;
                    }


                }
            }

            console.WriteLinePrecededByOne($"CATEGORY CONFLICTS between registered series category (in Portal master list) and Andrew's series points lists based on MyLaps: {ii}");
            console.WriteLine(sb2.ToString());

            #endregion

            //if (string.IsNullOrWhiteSpace(inputDuration))
            //    answer = $"{index,-3}  {resultItem.Bib,-3}  {resultItem.FirstName,-15} {resultItem.LastName,-15}  {resultItem.T01,-15}  {resultItem.DnxString,-3}";
            //else
            //    answer = $"{index,-3}  {resultItem.Bib,-3}  {resultItem.FirstName,-15} {resultItem.LastName,-15}  {resultItem.T01,-15}  {resultItem.DnxString,-3}  ({inputDuration,-15})";
            //return answer;

            #endregion

            #region wrap up

            console.WriteLinePrecededByOne("Summary:");
            var prettyFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FilenameOfDiagnosticReport);
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

        Console.WriteLine($"{JghString.LeftAlign("File saved:", LhsWidth)} {outPutFilename}");
        Console.WriteLine($"{JghString.LeftAlign("Folder:", LhsWidth)} {outPutFolder}");
    }

    #endregion

    #region parameters

    private const string Description = "This console program (Tool09) reads files of MyLaps timing data and compares the electronic data from the timing mat " +
                                       "to a (potentially totally different) set of timing data recorded in the timing tent using the Portal timing system. " +
                                       "The purpose of doing this is to search for gaps in the data by means of comparison. Based on empirical experience, there are " +
                                       "typically about ten anomalies in a Kelso event because the electronic mat misses people. The Portal timing team also misses " +
                                       "people, but generallly fewer. The pipeline for this tool is that data is exported from MyLaps in Excel format, then exported " +
                                       "from Excel in .csv format, and then finally imported into this tool as .csv and deserialised and analysed. Portal data is exported " +
                                       "effortlessly by clicking the export button on the Portal Timing Tools screen.";

    private const int LhsWidthTiny = 5;
    private const int LhsWidthSmall = 13;
    private const int LhsWidth = 53;

    private const string RequiredInputFileFormat = "xml";

    private static readonly DateTime DateOfThisEvent = new(2024, 6, 18); // R5 June 18


    private const string FilenameOfDiagnosticReport = @"MyLapsVersusPortalTimingDataDiagnosticReport.txt";

    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R5 June 18\DiagnosticReport\";


    private const string InputFolderFromMyLaps = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R5 June 18\ExportedAsCsv\";

    private const string NameOfPortalRezultzFile = @"2024-06-20T14-25-11+DraftResultsForLeaderboard.xml";
    private const string InputFolderOfSplitIntervalsFromPortal = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R5 June 18\DiagnosticReport\";

    private const string NameOfParticipantMasterListFromPortalFile = @"2024-06-20T14-25-11+DraftResultsForLeaderboard.xml";
    private const string InputFolderOfParticipantMasterListFromPortal = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R5 June 18\DiagnosticReport\";


    #endregion
}