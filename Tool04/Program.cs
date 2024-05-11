using System.Text;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace Tool04;

internal class Program
{
    private const string Description =
        "This program reads one or more XML files of published results data from the Portal." +
        " For every ResultItemDto in each of those files, it overwrites the T01 field" +
        " with the corresponding field obtained from the three associated MyLaps data files (in CSV format)." +
        " Then it exports tidied up file/s of XML.";

    private static Task Main()
    {
        JghConsoleHelper.WriteLineWrappedInTwo("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for Rezultz files", LhsWidth)} : {InputFolderFromRezultz}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for MyLaps files", LhsWidth)} : {InputFolderFromMyLaps}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for resultant processed Results", LhsWidth)} : {OutputFolderForXml}");
        JghConsoleHelper.WriteLineWrappedInOne("Press enter to go. When you see FINISH you're done.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

        //Console.WriteLine($"{url,-60} {content.Length,10:#,#}")

        try
        {
            #region get ready

            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolderFromMyLaps);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + InputFolderFromMyLaps);
                return Task.CompletedTask;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolderFromRezultz);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + InputFolderFromRezultz);
                return Task.CompletedTask;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(OutputFolderForXml);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + OutputFolderForXml);
                return Task.CompletedTask;
            }

            #endregion

            #region confirm existence of files of input data

            foreach (var fileKvp in DictionaryOfPreprocessedRezultzFiles)
            {
                var rezultzFileInfo = new FileInfo(InputFolderFromRezultz + fileKvp.Key);

                if (!rezultzFileInfo.Exists)
                {
                    JghConsoleHelper.WriteLine($"ResultItemDataTransferObject[] file not found : {rezultzFileInfo.Name}");
                    return Task.CompletedTask;
                }

                foreach (var myLapsFileName in fileKvp.Value)
                {
                    var myLapsFileInfo = new FileInfo(InputFolderFromMyLaps + myLapsFileName);

                    if (!myLapsFileInfo.Exists)
                    {
                        JghConsoleHelper.WriteLine($"MyLaps timing data file not found: {myLapsFileInfo.Name}");
                        return Task.CompletedTask;
                    }
                }
            }

            #endregion

            #region confirm existence of directories and files for output data

            try
            {
                foreach (var item in DictionaryOfPreprocessedRezultzFiles)
                {
                    var pathOfXmlFile = OutputFolderForXml + @"\" + Path.GetFileNameWithoutExtension(item.Key) + "-v2" + "." + StandardFileTypeSuffix.Xml;

                    File.WriteAllTextAsync(pathOfXmlFile, "<element>dummy</element>");
                }
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine($"Failed to access a designated output file. {e.Message}");

                return Task.CompletedTask;
            }

            #endregion

            #region create list of file items to be loaded

            foreach (var kvp in DictionaryOfPreprocessedRezultzFiles)
            {
                var thisRezultzFileItem = new RezultzFileItem
                {
                    RezultzFileInfo = new FileInfo(InputFolderFromRezultz + kvp.Key)
                };

                foreach (var myLapsFileName in kvp.Value)
                {
                    var myLapsFileInfo = new FileInfo(InputFolderFromMyLaps + myLapsFileName);

                    var thisMyLapsFileItem = new MyLapsFileObject
                    {
                        MyLapsFileInfo = myLapsFileInfo
                    };

                    thisRezultzFileItem.ListOfMyLapsFileItems.Add(thisMyLapsFileItem);
                }

                ListOfRezultzFileItemsBeingProcessed.Add(thisRezultzFileItem);
            }

            #endregion

            #region Load all the timing data files

            try
            {
                foreach (var rezultzFileItem in ListOfRezultzFileItemsBeingProcessed)
                {
                    if (!rezultzFileItem.RezultzFileInfo.Name.EndsWith($".{RequiredRezultzFileFormat}"))
                    {
                        JghConsoleHelper.WriteLine($"Skipping {rezultzFileItem.RezultzFileInfo.Name} because it's not a {RequiredRezultzFileFormat} file as you appear to have specified. Does this make sense?");
                        continue;
                    }

                    var fullRezultzFileInputPath = rezultzFileItem.RezultzFileInfo.FullName;

                    JghConsoleHelper.WriteLine($"Looking for {fullRezultzFileInputPath}");

                    rezultzFileItem.RezultzFileContentsAsText = File.ReadAllText(fullRezultzFileInputPath);

                    JghConsoleHelper.WriteLine($"Loaded {rezultzFileItem.RezultzFileContentsAsText.Length} characters");

                    foreach (var myLapsFileItem in rezultzFileItem.ListOfMyLapsFileItems)
                    {
                        if (!myLapsFileItem.MyLapsFileInfo.Name.EndsWith($".{RequiredMyLapsFileFormat}"))
                        {
                            JghConsoleHelper.WriteLine($"Skipping {myLapsFileItem.MyLapsFileInfo.Name} because it's not a {RequiredMyLapsFileFormat} file as you appear to have specified. Does this make sense?");
                            continue;
                        }

                        var fullMyLapsFileInputPath = myLapsFileItem.MyLapsFileInfo.FullName;

                        JghConsoleHelper.WriteLine($"Looking for {fullMyLapsFileInputPath}.");

                        myLapsFileItem.MyLapsFileContentsAsText = File.ReadAllText(fullMyLapsFileInputPath);

                        JghConsoleHelper.WriteLine($"Loaded {myLapsFileItem.MyLapsFileContentsAsText.Length} characters");
                    }
                }

                JghConsoleHelper.WriteLine();
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLineFollowedByOne($"Failed to locate designated Rezultz timing data file or possibly MyLaps timing data file. {e.Message}");
                return Task.CompletedTask;
            }

            JghConsoleHelper.WriteLine();

            #endregion

            #endregion

            Main01();
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"{e.Message}");
        }

        return Task.CompletedTask;
    }

    private static void Main01()
    {
        #region step 1 populate the list of RezultzFileItems with resultDto file contents

        try
        {
            foreach (var rezultzFileItem in ListOfRezultzFileItemsBeingProcessed)
            {
                if (!rezultzFileItem.RezultzFileInfo.Name.EndsWith($".{RequiredRezultzFileFormat}"))
                {
                    JghConsoleHelper.WriteLine($"Skipping {rezultzFileItem.RezultzFileInfo.Name} because it's not a {RequiredRezultzFileFormat} file as you appear to have specified. Does this make sense?");
                    continue;
                }

                JghConsoleHelper.WriteLine($"Deserializing text to array of ResultItemDataTransferObject for {rezultzFileItem.RezultzFileInfo.Name}");

                var xx = JghSerialisation.ToObjectFromXml<ResultDto[]>(rezultzFileItem.RezultzFileContentsAsText, new[] {typeof(ResultDto)});

                rezultzFileItem.RezultzFileContentsAsResultsDataTransferObjects = xx.ToList();

                JghConsoleHelper.WriteLine($"Loaded {rezultzFileItem.RezultzFileContentsAsResultsDataTransferObjects.Count} ResultItemDataTransferObjects");
            }

            JghConsoleHelper.WriteLine();
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Exception thrown: failed to successfully deserialize [rezultzFileItem.RezultzFileContentsAsText] to [rezultzFileItem.RezultzFileContentsAsResultsDataTransferObjects ]. {e.Message}");
            JghConsoleHelper.WriteLine("");
            return;
        }

        #endregion

        #region step for each RezultzFileItem and all its MyLaps files verify if the mylaps csv records contain valid TimeSpans

        try
        {
            foreach (var rezultzFileItem in ListOfRezultzFileItemsBeingProcessed)
            {
                JghConsoleHelper.WriteLine($"Verifying Gun Times in MyLaps files for {rezultzFileItem.RezultzFileInfo.Name}");

                foreach (var myLapsFileItem in rezultzFileItem.ListOfMyLapsFileItems)
                {
                    JghConsoleHelper.WriteLine($"Verifying Gun Times in {myLapsFileItem.MyLapsFileInfo.Name} for {rezultzFileItem.RezultzFileInfo.Name}");

                    VerifyMLapsCsvGunDurations(myLapsFileItem, NumberOfRowsPrecedingRowOfFieldNames);

                    JghConsoleHelper.WriteLine($"Verification ran to completion in {myLapsFileItem.MyLapsFileInfo.Name} for {rezultzFileItem.RezultzFileInfo.Name}");
                    JghConsoleHelper.WriteLine();
                }
            }

            JghConsoleHelper.WriteLine();
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Exception thrown: failed to successfully MYLaps file contents to [myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects]. {e.Message}");
            JghConsoleHelper.WriteLine("");
        }

        #endregion

        #region step 2 populate each RezultzFileItem with all the instantiated MyLapsResultObjects from each of the associated MyLapsFileObjects

        try
        {
            foreach (var rezultzFileItem in ListOfRezultzFileItemsBeingProcessed)
            {
                JghConsoleHelper.WriteLine($"Populating multiple MyLapsResultObjects associated with {rezultzFileItem.RezultzFileInfo.Name}");

                foreach (var myLapsFileItem in rezultzFileItem.ListOfMyLapsFileItems)
                {
                    JghConsoleHelper.WriteLine($"Deserializing MyLapsResultObjects from {myLapsFileItem.MyLapsFileInfo.Name}");

                    myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects = TranslateMyLapsCsvFileContentsToMyLapsResultObjects(myLapsFileItem);

                    JghConsoleHelper.WriteLine($"Successfully instantiated {myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects.Count} MyLapsFileObjects");
                }
            }

            JghConsoleHelper.WriteLine();
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Exception thrown: failed to successfully MYLaps file contents to [myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects]. {e.Message}");
            JghConsoleHelper.WriteLine("");
        }

        #endregion

        #region step 3 overwrite the T01 field with the MyLaps Gun Time field for every Rezultz finisher

        var summaryReportSb = new StringBuilder();

        try
        {
            foreach (var rezultzFileItem in ListOfRezultzFileItemsBeingProcessed)
            {
                #region make an archive JghListDictionary of the ResultItemDataTransferObjects inputted from Rezultz keyed by RaceGroup

                var inputListDict = new JghListDictionary<string, ResultDto>();

                foreach (var resultDto in rezultzFileItem.RezultzFileContentsAsResultsDataTransferObjects)
                    inputListDict.Add(resultDto.RaceGroup, resultDto);

                #endregion

                #region make a working JghListDictionary of ResultItems undergoing modification keyed by RaceGroup

                var beingModifiedListDictionary = new JghListDictionary<string, ResultItem>();

                foreach (KeyValuePair<string, IList<ResultDto>> kvp in inputListDict)
                foreach (var dto in kvp.Value)
                {
                    var result = ResultItem.FromDataTransferObject(dto);
                    beingModifiedListDictionary.Add(kvp.Key, result);
                }

                #endregion

                var message = $"Analysing T01 fields in {rezultzFileItem.RezultzFileInfo.Name}. if shown, difference is in seconds, left column is T01 in Rezultz, right column is GunTime in MyLaps";

                JghConsoleHelper.WriteLine(message);
                summaryReportSb.AppendLine(message);


                List<Tuple<ResultItem, double>> allDeltas = new();
                List<MyLapsResultObject?> allMyLapsResultObjectForThisEvent = new();


                foreach (var myLapsFileItem in rezultzFileItem.ListOfMyLapsFileItems)
                    allMyLapsResultObjectForThisEvent.AddRange(myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects);

                var countOfT01Overwrites = 0;

                var countOfNotSeenInMyLaps = 0;

                foreach (var kvp in beingModifiedListDictionary)
                foreach (var resultItemBeingModified in kvp.Value)
                {
                    #region debug stuff

                    //var rubbish = JghString.TmLr(result.Bib);

                    //if (rubbish == "58")
                    //{
                    //    var testRider = allMyLapsResultObjectForThisEvent.FirstOrDefault(z =>
                    //        z?.Bib == result.Bib);

                    //    if (testRider != null)
                    //    {
                    //        var fullName = testRider.FullName;

                    //        var names = testRider.FullName.Split(' ');

                    //        var first = result.FirstName;
                    //        var last = result.LastName;

                    //        var rubbish3 = JghString.TmLr(testRider.FullName).Contains(JghString.TmLr(result.FirstName));

                    //        var rubbish4 = JghString.TmLr(testRider.FullName).Contains(JghString.TmLr(result.LastName));
                    //    }
                    //}

                    #endregion

                    if (DetermineIfIsNothingNecessaryToDoKindOfMatch(resultItemBeingModified))
                        // do nothing to this resultsItem. nothing required
                        continue;

                    var conversionReport = FigureOutReasonIfMatchDoesNotExist(resultItemBeingModified, allMyLapsResultObjectForThisEvent, out var matchingMyLapsResultObject);

                    if (string.IsNullOrWhiteSpace(conversionReport))
                    {
                        #region we have a matching record - but the Duration is suspiciously low. bale

                        TimeSpan suspiciouslyLowTimeForNovice = new(0, 0, 20, 0);
                        TimeSpan suspiciouslyLowTimeForSport = new(0, 0, 50, 0);
                        TimeSpan suspiciouslyLowTimeForExpert = new(0, 0, 60, 0);

                        if (
                            (resultItemBeingModified.RaceGroup == "novice" && matchingMyLapsResultObject.Duration < suspiciouslyLowTimeForNovice)
                            ||
                            (resultItemBeingModified.RaceGroup == "sport" && matchingMyLapsResultObject.Duration < suspiciouslyLowTimeForSport)
                            ||
                            (resultItemBeingModified.RaceGroup == "expert" && matchingMyLapsResultObject.Duration < suspiciouslyLowTimeForExpert)
                        )
                        {
                            countOfNotSeenInMyLaps++;

                            message =
                                $"{resultItemBeingModified.Bib,-3}   {resultItemBeingModified.FirstName,-15}   {resultItemBeingModified.LastName,-20}   {resultItemBeingModified.RaceGroup,-7}    {resultItemBeingModified.T01,-17}    Faulty MyLaps csv data. GunTime meaningless [{matchingMyLapsResultObject.Duration.ToString("G")}]";

                            JghConsoleHelper.WriteLine(message);

                            summaryReportSb.AppendLine(message);

                            continue;
                        }

                        #endregion

                        #region success at last: we have a plausible matching record - overwrite the T01 field with the MyLaps Gun Time field

                        var deltaSeconds = (matchingMyLapsResultObject.Duration - TimeSpan.Parse(resultItemBeingModified.T01)).TotalSeconds;


                        if (deltaSeconds > 2)
                        {
                            message =
                                $"{resultItemBeingModified.Bib,-3}   {resultItemBeingModified.FirstName,-15}   {resultItemBeingModified.LastName,-20}   {resultItemBeingModified.RaceGroup,-7}    {resultItemBeingModified.T01,-17}     {matchingMyLapsResultObject.Duration.ToString("G"),-17}    Delta: {deltaSeconds.ToString("00.0")}";
                        }
                        else
                        {
                            var deltaThreshold = "Less than two seconds";

                            message =
                                $"{resultItemBeingModified.Bib,-3}   {resultItemBeingModified.FirstName,-15}   {resultItemBeingModified.LastName,-20}   {resultItemBeingModified.RaceGroup,-7}    {resultItemBeingModified.T01,-17}     {deltaThreshold,-17}";
                        }


                        JghConsoleHelper.WriteLine(message);

                        summaryReportSb.AppendLine(message);


                        resultItemBeingModified.T01 =
                            matchingMyLapsResultObject.Duration
                                .ToString("G"); // the nub. overwrite the T01 field with the MyLaps Gun Time field. NB: we are only doing this because we have already screened the cases where the Gun Time field is valid (when often it isn't)

                        allDeltas.Add(new Tuple<ResultItem, double>(resultItemBeingModified, deltaSeconds));

                        countOfT01Overwrites++;

                        #endregion
                    }
                    else
                    {
                        #region failure. no match found

                        countOfNotSeenInMyLaps++;

                        message =
                            $"{resultItemBeingModified.Bib,-3}   {resultItemBeingModified.FirstName,-15}   {resultItemBeingModified.LastName,-20}   {resultItemBeingModified.RaceGroup,-7}    {resultItemBeingModified.T01,-17}    {conversionReport}";

                        JghConsoleHelper.WriteLine(message);

                        summaryReportSb.AppendLine(message);

                        #endregion
                    }
                }

                #region pretty print all the results - those not not requiring modification, those modified, and those not modified alike

                foreach (var kvp in beingModifiedListDictionary)
                {
                    var beingModifiedItemsInThisRaceGroup = kvp.Value.OrderBy(z => z.T01).ToList(); // essential

                    var j = 1;

                    foreach (var item in beingModifiedItemsInThisRaceGroup)
                    {
                        message = $"{j,-3} {item.Bib,-3} {item.FirstName,-15} {item.LastName,-20} {item.RaceGroup,-7}  {item.T01,-17}";
                        JghConsoleHelper.WriteLine(message);

                        j++;
                    }
                }

                #endregion

                #region pretty print all the deltas

                message = $"These are the deltas in {rezultzFileItem.RezultzFileInfo.Name}. Delta is in seconds, left column is T01 in Rezultz, right column is GunTime in MyLaps";

                JghConsoleHelper.WriteLine(message);

                //summaryReportSb.AppendLine(message);

                var i = 1;

                foreach (var resultItemWithDelta in allDeltas.OrderByDescending(z => Math.Abs(z.Item2)))
                {
                    var item = resultItemWithDelta.Item1;

                    var seconds = resultItemWithDelta.Item2;

                    if (seconds > 2)
                    {
                        message = $"{i,-3} {item.Bib,-3} {item.FirstName,-15} {item.LastName,-20} {item.RaceGroup,-7}  {item.T01,-17}  Delta: {seconds.ToString("00.0"),-17}";
                    }
                    else
                    {
                        var deltaThreshold = "Less than two seconds";

                        message = $"{i,-3} {item.Bib,-3} {item.FirstName,-15} {item.LastName,-20} {item.RaceGroup,-7}  {item.T01,-17}  Delta: {deltaThreshold,-17}";
                    }

                    JghConsoleHelper.WriteLine(message);
                    //summaryReportSb.AppendLine(message);

                    i++;
                }

                #endregion

                message = $"Task completed for {rezultzFileItem.RezultzFileInfo.Name} : {countOfT01Overwrites} over-writable T01 fields. {countOfNotSeenInMyLaps} MyLaps finishers were missing or invalid or meaningless csv data.";

                JghConsoleHelper.WriteLine(message);
                JghConsoleHelper.WriteLine();

                summaryReportSb.AppendLine(message);

                SaveCulminationOfWorkDestinedForRezultzToHardDriveAsXml(rezultzFileItem.RezultzFileContentsAsResultsDataTransferObjects, rezultzFileItem); // save the work to hard drive as XML - the final step!
            }

            JghConsoleHelper.WriteLine();
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Exception thrown: failed to successfully MYLaps file contents to [myLapsFileItem.MyLapsFileContentsAsMyLapsResultObjects]. {e.Message}");
            JghConsoleHelper.WriteLine("");
        }

        #endregion

        #region step 4 write summary report to file

        var pathOfXmlFile = OutputFolderForXml + @"\" + "SummaryReport.txt";

        File.WriteAllText(pathOfXmlFile, summaryReportSb.ToString());

        JghConsoleHelper.WriteLine($"{summaryReportSb.Length} records saved in {pathOfXmlFile}");

        #endregion
    }

    #region constants

    private const int LhsWidth = 50;

    private const string InputFolderFromRezultz = @"C:\Users\johng\holding pen\StuffFromRezultzAzure\PublishedPreprocessedResults\";
    private const string InputFolderFromMyLaps = @"C:\Users\johng\holding pen\StuffFromAndrew\Event03-04-06-08FromMyLaps\";
    private const string OutputFolderForXml = @"C:\Users\johng\holding pen\StuffByJohn\Event03-04-06-08FromRezultzEditedWithTimesFromMyLaps\";
    private const string OutputFolderForUploadToRezultz = @"C:\Users\johng\holding pen\StuffByJohn\Event03-04-06-08FromRezultzEditedWithTimesFromMyLaps\ForUploadToRezultz\";


    private const string RequiredMyLapsFileFormat = "csv"; // "csv"
    private const string RequiredRezultzFileFormat = "xml"; // "xml"

    private static readonly JghListDictionary<string, string> DictionaryOfPreprocessedRezultzFiles = new()
    {
        {"Kelso2023mtb-results-03.xml", "Expert03.csv"},
        {"Kelso2023mtb-results-03.xml", "Novice03.csv"},
        {"Kelso2023mtb-results-03.xml", "Sport03.csv"},
        {"Kelso2023mtb-results-04.xml", "Expert04.csv"},
        {"Kelso2023mtb-results-04.xml", "Novice04.csv"},
        {"Kelso2023mtb-results-04.xml", "Sport04.csv"},
        {"Kelso2023mtb-results-06.xml", "Expert06.csv"},
        {"Kelso2023mtb-results-06.xml", "Novice06.csv"},
        {"Kelso2023mtb-results-06.xml", "Sport06.csv"},
        {"Kelso2023mtb-results-08.xml", "Expert08.csv"},
        {"Kelso2023mtb-results-08.xml", "Novice08.csv"},
        {"Kelso2023mtb-results-08.xml", "Sport08.csv"}
    };

    private const int NumberOfRowsPrecedingRowOfFieldNames = 1; // Oddity when the csv file is exported directly from MyLaps. they have some sort of title row before the field names row.
    //private const int NumberOfRowsPrecedingRowOfFieldNames = 0; // 0 is normal for csv files exported from Excel. 1 for csv files exported directly from MyLaps. they have some sort of title row before the field names row.


    #endregion

    #region variables

    private static readonly List<RezultzFileItem> ListOfRezultzFileItemsBeingProcessed = new();

    #endregion

    #region helpers

    private static void VerifyMLapsCsvGunDurations(MyLapsFileObject myLapsFileItem, int numberOfRowsPrecedingRowOfFieldNames)
    {
        #region step 1 interpret the MyLaps file format if we can

        var myLapsTimingDataInCsvFormatAsString = myLapsFileItem.MyLapsFileContentsAsText;

        var allRowsOfCsvText = myLapsTimingDataInCsvFormatAsString.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None).ToList(); // remove carriage returns and line breaks

        if (string.IsNullOrWhiteSpace(myLapsTimingDataInCsvFormatAsString) && !allRowsOfCsvText.Any())
        {
            JghConsoleHelper.WriteLine($"Unable to read this file. Contents can't processed as plain text. {myLapsFileItem.MyLapsFileInfo.Name}");

            return;
        }

        var relevantRowsOfCsvText =
            allRowsOfCsvText.Where(z => !string.IsNullOrWhiteSpace(z)).Where(z => z.Contains(','))
                .ToList(); // eliminate blank lines and lines that are non-data lines in the MyLaps files - for starters there is a pair at the bottom of the file

        List<string> relevantRowsWithoutEscapeLiterals = new();

        foreach (var rowOfCsv in relevantRowsOfCsvText)
        {
            var thisRowOfCsv = rowOfCsv;
            thisRowOfCsv = thisRowOfCsv.Replace(@"\", string.Empty);
            thisRowOfCsv = thisRowOfCsv.Replace(@"""", string.Empty);

            if (!string.IsNullOrWhiteSpace(thisRowOfCsv))
                relevantRowsWithoutEscapeLiterals.Add(thisRowOfCsv);
        }

        if (relevantRowsWithoutEscapeLiterals.Count() < numberOfRowsPrecedingRowOfFieldNames + 2)
        {
            JghConsoleHelper.WriteLine($"unable to read this csv file. we don't have at least a heading row and one data row. {myLapsFileItem.MyLapsFileInfo.Name}");
            return;
        }

        #endregion

        #region step 2 process the headings row (which might orr might not be the first row) - locate the column headings

        var arrayOfColumnHeadings = relevantRowsWithoutEscapeLiterals.Skip(numberOfRowsPrecedingRowOfFieldNames).FirstOrDefault()?.Split(',');

        if (arrayOfColumnHeadings == null)
        {
            JghConsoleHelper.WriteLine($"unable to read this csv file. We don't have any column headings in the header row. {myLapsFileItem.MyLapsFileInfo.Name}");
            return;
        }

        // example of headings from MyLaps : "Pos","Bib#","Athlete","Gun Time","Race","Age","Gender"
        var indexOfBibColumn = Array.IndexOf(arrayOfColumnHeadings, "Bib#");
        var indexOfTotalDuration = Array.IndexOf(arrayOfColumnHeadings, "Gun Time");
        var indexOfFullName = Array.IndexOf(arrayOfColumnHeadings, "Athlete");
        var indexOfRaceGroup = Array.IndexOf(arrayOfColumnHeadings, "Race");
        var indexOfAge = Array.IndexOf(arrayOfColumnHeadings, "Age");
        var indexOfGender = Array.IndexOf(arrayOfColumnHeadings, "Gender");

        if (indexOfBibColumn == -1 && indexOfTotalDuration == -1 && indexOfFullName == -1 && indexOfRaceGroup == -1 && indexOfAge == -1 && indexOfGender == -1)
        {
            JghConsoleHelper.WriteLine($"Unable to find any of the columns required for our purposes in the csv file. {myLapsFileItem.MyLapsFileInfo.Name}");

            return;
        }

        #endregion

        #region step 3 process all the subsequent rows in the csv file

        var rowsOfData = relevantRowsWithoutEscapeLiterals.Skip(numberOfRowsPrecedingRowOfFieldNames + 1).ToList(); // skip first line i.e. the header row we just processed in Step 2

        if (!rowsOfData.Any())
        {
            JghConsoleHelper.WriteLine($"Unable to do anything. File contains no rows of data. {myLapsFileItem.MyLapsFileInfo.Name}");

            return;
        }

        var failureCount = 1;
        var successCount = 1;
        var totalCount = 1;

        foreach (var row in rowsOfData)
        {
            var arrayOfDataInRow = row.Split(',');

            #region figure out Duration

            var durationAsString = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfTotalDuration);

            if (!TryConvertTextToTimespan(durationAsString, out var _, out var conversionFailureReport))
            {
                failureCount++;
                JghConsoleHelper.WriteLine($"{totalCount,-4}  Failure. Gun Time is not valid format. {conversionFailureReport}");
            }
            else
            {
                //if (
                //    (resultItemBeingModified.RaceGroup == "novice" && matchingMyLapsResultObject.Duration < suspiciouslyLowTimeForNovice)
                //    ||
                //    (resultItemBeingModified.RaceGroup == "sport" && matchingMyLapsResultObject.Duration < suspiciouslyLowTimeForSport)
                //    ||
                //    (resultItemBeingModified.RaceGroup == "expert" && matchingMyLapsResultObject.Duration < suspiciouslyLowTimeForExpert)
                //)
                //{
                //    failureCount++;
                //    JghConsoleHelper.WriteLine($"{totalCount,-4}  Failure. Gun Time is suspiciously brief. {conversionFailureReport} [{durationAsString}]");
                //}
                //else
                //{
                successCount++;
                JghConsoleHelper.WriteLine($"{totalCount,-4} Possible success. Gun Time is in plausibly valid format [{durationAsString}]");
                //}
            }

            #endregion

            totalCount += 1;
        }

        #endregion

        JghConsoleHelper.WriteLine($"Verifications ran to completion. Out of a total of {totalCount} records, {successCount} Gun Times were valid and {failureCount} were not.");
    }

    private static List<MyLapsResultObject> TranslateMyLapsCsvFileContentsToMyLapsResultObjects(MyLapsFileObject myLapsFileItem)
    {
        var candidateAnswer = new List<MyLapsResultObject>();

        #region step 1 interpret the MyLaps file format if we can

        var myLapsTimingDataInCsvFormatAsString = myLapsFileItem.MyLapsFileContentsAsText;

        var allRowsOfCsvText = myLapsTimingDataInCsvFormatAsString.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None).ToList(); // remove carriage returns and line breaks


        if (string.IsNullOrWhiteSpace(myLapsTimingDataInCsvFormatAsString) && !allRowsOfCsvText.Any())
        {
            JghConsoleHelper.WriteLine($"Unable to read this file. Contents can't processed as plain text. {myLapsFileItem.MyLapsFileInfo.Name}");

            return candidateAnswer;
        }

        var relevantRowsOfCsvText =
            allRowsOfCsvText.Where(z => !string.IsNullOrWhiteSpace(z)).Where(z => z.Contains(','))
                .ToList(); // eliminate blank lines and lines that are non-data lines in the MyLaps files - for starters there is a pair at the bottom of the file

        List<string> relevantRowsWithoutEscapeLiterals = new();

        foreach (var rowOfCsv in relevantRowsOfCsvText)
        {
            var thisRowOfCsv = rowOfCsv;
            thisRowOfCsv = thisRowOfCsv.Replace(@"\", string.Empty);
            thisRowOfCsv = thisRowOfCsv.Replace(@"""", string.Empty);

            if (!string.IsNullOrWhiteSpace(thisRowOfCsv))
                relevantRowsWithoutEscapeLiterals.Add(thisRowOfCsv);
        }

        if (relevantRowsWithoutEscapeLiterals.Count() < NumberOfRowsPrecedingRowOfFieldNames + 2)
        {
            // bale if we don't have at least a heading row and one data row
            SaveFileOfPopulatedMyLapsResultItemsToHardDriveAsXml(candidateAnswer, myLapsFileItem);

            return candidateAnswer;
        }

        #endregion

        #region step 2 process the headings row (which might orr might not be the first row) - locate the column headings

        var arrayOfColumnHeadings = relevantRowsWithoutEscapeLiterals.Skip(NumberOfRowsPrecedingRowOfFieldNames).FirstOrDefault()?.Split(',');

        if (arrayOfColumnHeadings == null)
        {
            // bale if we don't have the headings row
            SaveFileOfPopulatedMyLapsResultItemsToHardDriveAsXml(candidateAnswer, myLapsFileItem);
            return candidateAnswer;
        }

        // example of headings from MyLaps : "Pos","Bib#","Athlete","Gun Time","Race","Age","Gender"
        var indexOfBibColumn = Array.IndexOf(arrayOfColumnHeadings, "Bib#");
        var indexOfTotalDuration = Array.IndexOf(arrayOfColumnHeadings, "Gun Time");
        var indexOfFullName = Array.IndexOf(arrayOfColumnHeadings, "Athlete");
        var indexOfRaceGroup = Array.IndexOf(arrayOfColumnHeadings, "Race");
        var indexOfAge = Array.IndexOf(arrayOfColumnHeadings, "Age");
        var indexOfGender = Array.IndexOf(arrayOfColumnHeadings, "Gender");

        if (indexOfBibColumn == -1 && indexOfTotalDuration == -1 && indexOfFullName == -1 && indexOfRaceGroup == -1 && indexOfAge == -1 && indexOfGender == -1)
        {
            JghConsoleHelper.WriteLine($"Unable to find any of the required columns in the file {myLapsFileItem.MyLapsFileInfo.Name}");
            SaveFileOfPopulatedMyLapsResultItemsToHardDriveAsXml(candidateAnswer, myLapsFileItem);
            return candidateAnswer;
        }

        #endregion

        #region step 3 process all the subsequent rows in the csv file

        var rowsOfData = relevantRowsWithoutEscapeLiterals.Skip(NumberOfRowsPrecedingRowOfFieldNames + 1).ToList(); // skip first line i.e. the header row we just processed in Step 2

        if (!rowsOfData.Any())
        {
            // bale if we don't have any data rows
            SaveFileOfPopulatedMyLapsResultItemsToHardDriveAsXml(candidateAnswer, myLapsFileItem);
            return candidateAnswer;
        }

        var scratchPadFileName = JghString.TmLr(myLapsFileItem.MyLapsFileInfo.Name);

        var i = 1;

        foreach (var row in rowsOfData)
        {
            var arrayOfDataInRow = row.Split(',');

            #region instantiate MyLapsResultObject for this row and populate with particulars from participant master list iff Bib is found in master list

            var candidateLapsResultItem = new MyLapsResultObject
            {
                Bib = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfBibColumn),
                FullName = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfFullName)
            };

            #endregion

            #region figure out Duration

            var mustSkipThisRowBecauseGunTimeIsNonsense = false; // initial default

            var durationAsString = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfTotalDuration);

            if (TryConvertTextToTimespan(durationAsString, out var myTimeSpan, out var _))
                candidateLapsResultItem.Duration = myTimeSpan;
            else
                mustSkipThisRowBecauseGunTimeIsNonsense = true;

            #endregion

            #region add RaceGroup

            if (scratchPadFileName.Contains("expert"))
                candidateLapsResultItem.RaceGroup = "expert";
            else if (scratchPadFileName.Contains("novice"))
                candidateLapsResultItem.RaceGroup = "novice";
            else if (scratchPadFileName.Contains("sport")) candidateLapsResultItem.RaceGroup = "sport";

            #endregion

            ConsoleWriteOneLineMyLapsResultItemConversionReport(i, candidateLapsResultItem, myTimeSpan);

            i += 1;

            if (mustSkipThisRowBecauseGunTimeIsNonsense)
                continue;

            candidateAnswer.Add(candidateLapsResultItem);
        }

        #endregion

        SaveFileOfPopulatedMyLapsResultItemsToHardDriveAsXml(candidateAnswer, myLapsFileItem);

        return candidateAnswer;
    }

    private static bool DetermineIfIsNothingNecessaryToDoKindOfMatch(ResultItem resultItem)
    {
        if (!string.IsNullOrWhiteSpace(resultItem.DnxString)) return true;

        if (resultItem.RaceGroup == "uncategorised") return true;

        return false;
    }

    private static string FigureOutReasonIfMatchDoesNotExist(ResultItem resultItem, IReadOnlyCollection<MyLapsResultObject?> myLapsResultObjects, out MyLapsResultObject matchingMyLapsResultObject)
    {
        #region debug

        //var rubbish = JghString.TmLr(resultItem.Bib);

        //if (rubbish == "62")
        //{
        //    var testRider = myLapsResultObjects.FirstOrDefault(z =>
        //        z?.Bib == resultItem.Bib);

        //    if (testRider != null)
        //    {
        //        var fullName = testRider.FullName;

        //        var names = testRider.FullName.Split(' ');

        //        var first = resultItem.FirstName;
        //        var last = resultItem.LastName;

        //        var rubbish3 = JghString.TmLr(testRider.FullName).Contains(JghString.TmLr(resultItem.FirstName));

        //        var rubbish4 = JghString.TmLr(testRider.FullName).Contains(JghString.TmLr(resultItem.LastName));
        //    }
        //}

        #endregion

        if (myLapsResultObjects.Count(z => z?.Bib == resultItem.Bib) > 1)
        {
            matchingMyLapsResultObject = new MyLapsResultObject();

            return "More than one MyLaps finisher has this Bib";
        }

        if (myLapsResultObjects.Count(z => z?.Bib == resultItem.Bib) == 0)
        {
            matchingMyLapsResultObject = new MyLapsResultObject();

            return "Missing Bib in MyLaps file.";
        }

        var myLapsCandidateMatch = myLapsResultObjects.FirstOrDefault(z => z?.Bib == resultItem.Bib);

        if (myLapsCandidateMatch == null)
        {
            matchingMyLapsResultObject = new MyLapsResultObject();

            return "Missing Bib in MyLaps file.";
        }

        if (myLapsCandidateMatch.RaceGroup != resultItem.RaceGroup)
        {
            matchingMyLapsResultObject = new MyLapsResultObject();

            return $"Wrong RaceGroup in MyLaps. Wrong={myLapsCandidateMatch.RaceGroup} Required={resultItem.RaceGroup}";
        }

        if (!JghString.TmLr(myLapsCandidateMatch.FullName).Contains(JghString.TmLr(resultItem.FirstName)))
        {
            matchingMyLapsResultObject = new MyLapsResultObject();
            return $"First name : possible mismatch : MyLaps=[{myLapsCandidateMatch.FullName}] Portal=[{resultItem.FirstName}]]";
        }

        if (!JghString.TmLr(myLapsCandidateMatch.FullName).Contains(JghString.TmLr(resultItem.LastName)))
        {
            matchingMyLapsResultObject = new MyLapsResultObject();
            return $"Last name : possible mismatch : MyLaps=[{myLapsCandidateMatch.FullName}] Portal=[{resultItem.LastName}]]";
        }

        var successfulMatch = myLapsResultObjects.FirstOrDefault(z => z?.Bib == resultItem.Bib &&
                                                                      z.RaceGroup == resultItem.RaceGroup &&
                                                                      JghString.TmLr(z.FullName).Contains(JghString.TmLr(resultItem.FirstName)) &&
                                                                      JghString.TmLr(z.FullName).Contains(JghString.TmLr(resultItem.LastName)));
        if (successfulMatch != null)
        {
            // success. we have a match
            matchingMyLapsResultObject = successfulMatch;
            return string.Empty;
        }

        // if we arrive here, we have a problem

        matchingMyLapsResultObject = new MyLapsResultObject();

        return "Problem. Correctly matching record not seen in MyLapsFile for an unexpected reason. Please investigate.";
    }

    private static void SaveFileOfPopulatedMyLapsResultItemsToHardDriveAsXml(List<MyLapsResultObject> myLapsLineItem, MyLapsFileObject fileItem)
    {
        var myLapsLineObjectDTos = new List<MyLapsResultObjectDto>();

        foreach (var outputItem in myLapsLineItem)
            myLapsLineObjectDTos.Add(new MyLapsResultObjectDto {BibNumber = outputItem.Bib, FullName = outputItem.FullName, RaceGroup = outputItem.RaceGroup, Duration = outputItem.Duration.ToString("G")});

        var xmlFileContents = JghSerialisation.ToXmlFromObject(myLapsLineObjectDTos.ToArray(), new[] {typeof(List<MyLapsResultObjectDto>)});

        var pathOfXmlFile = OutputFolderForXml + @"\" + Path.GetFileNameWithoutExtension(fileItem.MyLapsFileInfo.Name) + "- MyLapsResultItems" + "." + StandardFileTypeSuffix.Xml;

        File.WriteAllText(pathOfXmlFile, xmlFileContents);

        JghConsoleHelper.WriteLine($"{myLapsLineObjectDTos.Count} records saved in {pathOfXmlFile}");
    }

    private static string GetTextItemFromArrayByIndexOrStringEmpty(string[] arrayOfText, int indexOfDataItem)
    {
        var textItem = JghArrayHelpers.SelectItemFromArrayByArrayIndex(arrayOfText, indexOfDataItem);

        return JghString.TmLr(textItem ?? string.Empty);
    }

    private static bool TryConvertTextToTimespan(string purportedTimeSpanAsText, out TimeSpan answer, out string conversionReport)
    {
        //if (purportedTimeSpanAsText == "59:26.8")
        //{
        //    var rubbish2 = purportedTimeSpanAsText;
        //    // stop 
        //}

        static bool TryGetFrontComponentAsInteger(string[] subStrings, out int firstValue)
        {
            if (!subStrings.Any())
            {
                firstValue = 0;
                return true;
            }

            if (int.TryParse(subStrings[0], out firstValue))
                return true;

            firstValue = 0;
            return false;
        }

        const string failure = "Conversion anomaly.";
        const string locus = "[TryConvertTextToTimespan]";

        conversionReport = string.Empty;

        var defaultConversionReport = $"{failure} Format is [{purportedTimeSpanAsText}] whereas [d.hh:mm:ss.ff] is required.";

        int days = 0, hours = 0, minutes = 0, seconds = 0, decimalSeconds = 0;

        try
        {
            // Examples:
            //12:1:07:23.47
            // 0:1:07:23.47
            //   1:07:23.47
            //   0:07:23.47
            //      7:23.47
            //      0:23.47
            //        23.47
            //         0.47
            //          .47

            #region null and format checks

            if (string.IsNullOrWhiteSpace(purportedTimeSpanAsText))
            {
                answer = TimeSpan.MinValue;

                conversionReport = $"{failure} The text is null or whitespace.";

                return false;
            }

            #endregion

            #region get the fractional seconds  - special case

            string[] components;

            if (purportedTimeSpanAsText.Contains('.')) // there is a fractional seconds component
            {
                var oneOrTwoComponents = purportedTimeSpanAsText.Split(new[] {'.'}).Reverse().ToArray();

                if (!TryGetFrontComponentAsInteger(oneOrTwoComponents, out decimalSeconds))
                {
                    conversionReport = defaultConversionReport + " Fractional seconds look wrong.";
                    decimalSeconds = 0;
                }

                components = purportedTimeSpanAsText.Split(':', '.').Reverse().Skip(1).ToArray();
            }
            else
            {
                components = purportedTimeSpanAsText.Split(new[] {':'}).Reverse().ToArray();
            }

            #endregion

            #region get the rest of the time span components

            for (var i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out seconds))
                        {
                            conversionReport = defaultConversionReport + " Seconds look wrong.";
                            seconds = 0;
                        }

                        break;
                    }
                    case 1:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out minutes))
                        {
                            conversionReport = defaultConversionReport + " Minutes look wrong.";
                            minutes = 0;
                        }

                        break;
                    }
                    case 2:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out hours))
                        {
                            conversionReport = defaultConversionReport + " Hours look wrong.";
                            hours = 0;
                        }

                        break;
                    }
                    case 3:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out days))
                        {
                            conversionReport = defaultConversionReport + " Days look wrong.";
                            days = 0;
                        }

                        break;
                    }
                }

                components = components.Skip(1).ToArray();
            }

            #endregion

            double.TryParse($"0.{decimalSeconds}", out var fractionalSeconds); // the special case

            try
            {
                answer = new TimeSpan(days, hours, minutes, seconds) + TimeSpan.FromSeconds(fractionalSeconds);
            }
            catch (Exception)
            {
                answer = TimeSpan.MinValue;

                conversionReport = $"{failure} One or more of the number of hours, minutes, seconds, and/or fractional seconds exceeds the logical maximum. (23,59, 59, and .999)";

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }
    }

    private static void ConsoleWriteOneLineMyLapsResultItemConversionReport(int index, MyLapsResultObject dtoItem, TimeSpan calculatedDuration)
    {
        if (calculatedDuration != TimeSpan.MinValue)
            JghConsoleHelper.WriteLine($"{index,-3}  {dtoItem.Bib,-3}  {dtoItem.FullName,-25} {dtoItem.RaceGroup,-10} Calculated T01: {calculatedDuration,-15:G}");
    }

    private static void SaveCulminationOfWorkDestinedForRezultzToHardDriveAsXml(List<ResultDto> listOfOutputDto, RezultzFileItem fileItem)
    {
        var xmlFileContents = JghSerialisation.ToXmlFromObject(listOfOutputDto.ToArray(), new[] {typeof(ResultDto[])});

        var pathOfXmlFile = OutputFolderForUploadToRezultz + @"\" + Path.GetFileNameWithoutExtension(fileItem.RezultzFileInfo.Name) + "-vMyLapsGunTimes" + "." + StandardFileTypeSuffix.Xml;

        File.WriteAllText(pathOfXmlFile, xmlFileContents);

        JghConsoleHelper.WriteLine($"Records saved to {pathOfXmlFile}");
    }

    #endregion
}