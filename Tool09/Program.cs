﻿using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using RezultzSvc.Library02.Mar2024.PublisherModuleHelpers;

namespace Tool09;

internal class Program
{
    #region variables

    private static readonly PortalTimingSystemFileBeingAnalysedItem ResultsPortalTimingSystemFileBeingAnalysed = new();

    #endregion

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        JghConsoleHelper.WriteLineFollowedByOne("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for Portal timing module split intervals", LhsWidth)} : {InputFolderOfSplitIntervalsFromPortal}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for MyLaps data from Andrew", LhsWidth)} : {InputFolderFromMyLaps}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for diagnostic report document", LhsWidth)} : {InputFolderFromMyLaps}");
        JghConsoleHelper.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        JghConsoleHelper.ReadLine();

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
                JghConsoleHelper.WriteLine("Directory not found: " + InputFolderOfSplitIntervalsFromPortal);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolderFromMyLaps);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + InputFolderFromMyLaps);
                return;
            }

            #endregion

            #region confirm existence of portal timing system file of split intervals

            var resultsFileInfo = new FileInfo(InputFolderOfSplitIntervalsFromPortal + NameOfPortalRezultzFile);

            if (!resultsFileInfo.Exists)
            {
                JghConsoleHelper.WriteLine("File not found: " + resultsFileInfo.FullName);

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
                JghConsoleHelper.WriteLine("Deserialisation failure. Results from file exported from Portal not obtained: " + ex.Message);
                return;
            }

            ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileContentsAsResultsDataTransferObjects = provisionalResultsFromPortal.ToList();

            JghConsoleHelper.WriteLine($"Loaded {resultsFileInfo.Name}: {provisionalResultsFromPortal.Length} results");

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
                var fileContentsAsText = await File.ReadAllTextAsync(myLapsFileObject.MyLapsFileInfo.Name);

                myLapsFileObject.MyLapsFileContentsAsText = fileContentsAsText;

                JghConsoleHelper.WriteLine($"Loaded {myLapsFileObject.MyLapsFileInfo.Name}: {myLapsFileObject.MyLapsFileContentsAsText.Length} characters");
            }

            #endregion

            #region extract key info from .csv files

            foreach (var fileItem in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            {
                JghConsoleHelper.WriteLinePrecededByOne($"Processing .csv contents: {fileItem.MyLapsFileInfo.Name}");

                var conversionReport = new JghStringBuilder();

                var myLapsFile = new MyLapsFile(fileItem.MyLapsFileInfo.Name, fileItem.MyLapsFileInfo.FullName, fileItem.MyLapsFileContentsAsText);

                var listOfResultItems = MyLaps2024HelperCsv.GenerateResultItemArrayFromMyLapsFile(myLapsFile, null, null, DateTime.Today, conversionReport, 30).ToList();

                var xx = listOfResultItems.Select(z => new MyLapsResultItem(z.Bib, z.FullName, z.T01, z.RaceGroup)).ToList();

                fileItem.MyLapsFileContentsAsMyLapsResultObjects.AddRange(xx);

                JghConsoleHelper.WriteLine(conversionReport.ToString());

                JghConsoleHelper.WriteLine($"Successfully(?) extracted .csv contents of {fileItem.MyLapsFileInfo.Name}: {fileItem.MyLapsFileContentsAsMyLapsResultObjects.Count} line items");
            }

            #endregion

            #region convert lists of results to dictionaries for easy analysis

            JghListDictionary<string, ResultDto> dictionaryOfPortalTimingSystemResults = new();

            foreach (var resultDto in ResultsPortalTimingSystemFileBeingAnalysed.SingleRezultzFileContentsAsResultsDataTransferObjects) dictionaryOfPortalTimingSystemResults.Add(resultDto.Bib, resultDto);

            JghListDictionary<string, MyLapsResultItem> dictionaryOfMyLapsTimingSystemResults = new();

            foreach (var myLapsFile in ResultsPortalTimingSystemFileBeingAnalysed.ListOfMyLapsFileObjects)
            foreach (var myLapsResult in myLapsFile.MyLapsFileContentsAsMyLapsResultObjects)
                dictionaryOfMyLapsTimingSystemResults.Add(myLapsResult.Bib, myLapsResult);

            #endregion

            #region report people that are listed in Results from Portal but not in MyLaps

            JghConsoleHelper.WriteLineWrappedByOne("MYLAPS SPREADSHEETS MISSING finishers:");

            foreach (var kvp in dictionaryOfPortalTimingSystemResults)
            {
                if (kvp.Value.Any(z => !string.IsNullOrWhiteSpace(z.DnxString)))
                    continue; // skip people who have a DNF, DNS, etc.

                if (dictionaryOfMyLapsTimingSystemResults.ContainsKey(kvp.Key)) continue; //todo: check for DNF, DNS, etc. (currently the data is not available in the MyLaps files)

                var person = kvp.Value.FirstOrDefault();

                if (person is null) continue;

                JghConsoleHelper.WriteLine($"<{kvp.Key}  {person.First}  {person.Last}  {person.RaceGroup}  {person.T01}>");
            }

            #endregion
            
            #region report people that are listed in Results from Portal but not in MyLaps

            JghConsoleHelper.WriteLineWrappedByOne("PORTAL HUB MISSING finishers:");

            foreach (var kvp in dictionaryOfMyLapsTimingSystemResults)
            {
                if (dictionaryOfPortalTimingSystemResults.ContainsKey(kvp.Key)) continue;

                var person = kvp.Value.FirstOrDefault();

                if (person is null) continue;

                JghConsoleHelper.WriteLine($"<{kvp.Key}  {person.FullName}  {person.RaceGroup}  {person.DurationAsString}>");
            }

            JghConsoleHelper.WriteLinePrecededByOne("The End");

            #endregion
        }
        catch (Exception ex)
        {
            JghConsoleHelper.WriteLineFollowedByOne(ex.ToString());
            JghConsoleHelper.ReadLine();
        }
    }

    #endregion

    #region helper methods

    private static void SaveWorkToHardDriveAsXml(string xmlAsText, string outPutFolder, string outPutFilename, int numberOfItems)
    {
        var pathOfFile = Path.Combine(outPutFolder, outPutFilename);

        File.WriteAllText(pathOfFile, xmlAsText);

        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Folder", 20)} : {outPutFolder}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("FileName", 20)} : {outPutFilename}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Items saved", 20)} : {numberOfItems}");
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

    private const int LhsWidth = 53;

    private const string RequiredInputFileFormat = "xml";

    private const string InputFolderFromMyLaps = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R5 June 18\ExportedAsCsv\";

    private const string InputFolderOfSplitIntervalsFromPortal = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbFromMyLaps\2024-Timing-Compendium\R5 June 18\";

    private const string NameOfPortalRezultzFile = @"2024-06-20T14-25-11+DraftResultsForLeaderboard.xml";

    #endregion
}