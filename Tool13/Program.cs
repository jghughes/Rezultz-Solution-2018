using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;

namespace Tool13;

internal class Program
{
    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Filename for SeriesProfile file:", LhsWidth)} {FileNameOfSeriesProfileDto}");
        console.WriteLine($"{JghString.LeftAlign("Folder for SeriesProfile file:", LhsWidth)} {FolderForSeriesProfile}");
        console.WriteLine($"{JghString.LeftAlign("Filename for Portal participants file:", LhsWidth)} {FileNameOfParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for Portal participants file:", LhsWidth)} {FolderForParticipantMasterListFromPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for series results:", LhsWidth)} {FolderForInputData}");
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
                Directory.SetCurrentDirectory(FolderForInputData);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForInputData);
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
                Directory.SetCurrentDirectory(FolderForDiagnosticReport);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForDiagnosticReport);
                return;
            }

            #endregion

            #region confirm existence of seriesprofile .json file

            // NB: be sure that this file is totally up-to-date. changes are being made daily!

            var path = Path.Combine(FolderForSeriesProfile, FileNameOfSeriesProfileDto);

            var fi = new FileInfo(path);

            if (!fi.Exists)
            {
                console.WriteLine($"Failed to locate designated series profile file. <{fi.Name}>");

                return;
            }

            #endregion

            #region confirm existence of participantHubItems .json file from portal

            // NB: be sure that this file is totally up-to-date. changes are being made daily!

            var path2 = Path.Combine(FolderForParticipantMasterListFromPortal, FileNameOfParticipantMasterListFromPortal);

            var fi2 = new FileInfo(path2);

            if (!fi2.Exists)
            {
                console.WriteLine($"Failed to locate designated participant file. <{fi2.Name}>");

                return;
            }

            #endregion

            #region deserialise seriesprofile from .json file

            try
            {
                var contents = await File.ReadAllTextAsync(fi.FullName);

                _seriesProfileDto = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(contents);
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            console.WriteLine();

            #endregion

            #region deserialise participantHubItems from .json file from Portal

            try
            {
                var rawInputAsText = await File.ReadAllTextAsync(fi2.FullName);

                var participantHubItemDtoArray = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto[]>(rawInputAsText);

                //ParticipantHubItem[] participantHubItems = ParticipantHubItem.FromDataTransferObject(participantHubItemDtoArray);

                foreach (var participantHubItemDto in participantHubItemDtoArray)
                    if (!string.IsNullOrWhiteSpace(participantHubItemDto.Bib))
                        DictionaryOfParticipantsInPortalMasterListKeyedByBib.Add(participantHubItemDto.Bib, participantHubItemDto);
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            console.WriteLine();

            #endregion

            #region confirm existence of .xml files of published results for all events in the series to date

            var di = new DirectoryInfo(FolderForInputData); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLinePrecededByOne($"{JghString.LeftAlign($"Problem: No files found in published results folder. [{FolderForInputData}]", LhsWidth)}");

                return;
            }

            List<InputFileItem> allInputFilesItems = [];

            foreach (var publishedFileInfo in arrayOfInputFileInfo.Where(z => z.FullName.EndsWith(".xml")))
            {
                var thisInputFileItem = new InputFileItem
                {
                    FileInfo = publishedFileInfo
                };

                allInputFilesItems.Add(thisInputFileItem);
            }

            #endregion

            #region read contents of all files as plain text

            foreach (var inputFileItem in allInputFilesItems)
            {
                var fileContentsAsText = await File.ReadAllTextAsync(inputFileItem.FileInfo.FullName);

                inputFileItem.ContentsAsText = fileContentsAsText;

                console.WriteLine($"Loaded {inputFileItem.FileInfo.Name}: {inputFileItem.ContentsAsText.Length} characters");
            }

            #endregion

            #region deserialise contents of all files as ResultDto[]

            foreach (var inputFileItem in allInputFilesItems)
            {
                var thisEventProfileDto = _seriesProfileDto.EventProfileCollection.Where(z => z.XmlFileNamesForPublishedResults.Contains(inputFileItem.FileInfo.Name)).FirstOrDefault() ?? new EventProfileDto();

                var conversionReport = new JghStringBuilder();

                #region try deserialise split intervals (ResultDto[])

                ResultDto[]? resultDtos;

                try
                {
                    var rawInputAsText = inputFileItem.ContentsAsText;

                    resultDtos = JghSerialisation.ToObjectFromXml<ResultDto[]>(rawInputAsText, [typeof(ResultDto)]);

                    var xx = resultDtos.Select(z => new PossiblySuspiciousFinish { EventProfileDto = thisEventProfileDto, ResultDto = z });

                    inputFileItem.ContentsAsCollectionOfPossiblySuspiciousFinishes.AddRange(xx);
                }
                catch (Exception ex)
                {
                    console.WriteLine("Deserialization failure. Results from file exported from Portal not obtained: " + ex.Message);
                    return;
                }

                console.WriteLine($"Successfully(?) extracted .xml contents of {inputFileItem.FileInfo.Name}: {inputFileItem.ContentsAsCollectionOfPossiblySuspiciousFinishes.Count} line items");

                #endregion
            }

            #endregion

            #region convert lists of results in all files to dictionary of ResultDto keyed by Bib for easy analysis - exclude Dnx outcomes and non-series riders

            JghListDictionary<string, PossiblySuspiciousFinish> dictionaryOfPublishedResultsKeyedByBib = [];

            foreach (var inputFileItem in allInputFilesItems)
            foreach (var finish in inputFileItem.ContentsAsCollectionOfPossiblySuspiciousFinishes)
            {
                if (!finish.ResultDto.IsSeries)
                    continue;

                if (!string.IsNullOrWhiteSpace(finish.ResultDto.DnxString))
                    continue;

                dictionaryOfPublishedResultsKeyedByBib.Add(finish.ResultDto.Bib, finish);
            }

            #endregion

            #region reduce to shortlist of series culprits who finished in more than one category during the series

            JghListDictionary<string, PossiblySuspiciousFinish> dictionaryOfCulpritsKeyedByBib = [];

            foreach (var kvp in dictionaryOfPublishedResultsKeyedByBib.OrderBy(z => z.Key))
            {
                var xx = kvp.Value.Distinct(new PossiblySuspiciousFinishEqualityComparer());

                if (xx.Count() <= 1)
                    continue;

                foreach (var resultDto in kvp.Value) dictionaryOfCulpritsKeyedByBib.Add(kvp.Key, resultDto);
            }

            #endregion

            #region report people who raced in a more than one category during the series

            console.WriteLinePrecededByOne($"People who finished in more than one category during the series so far: {dictionaryOfCulpritsKeyedByBib.Count}");

            foreach (var culprit in dictionaryOfCulpritsKeyedByBib)
            {
                var person = culprit.Value.FirstOrDefault();

                if (person is null)
                    continue;

                var xx = culprit.Value.Distinct(new PossiblySuspiciousFinishEqualityComparer()).ToArray();

                var xxList = string.Join(", ", xx.Select(z => z.ResultDto.RaceGroup));

                console.WriteLine($"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-25} {"Categories:",-10} {xx.Count(),3}  {xxList}");
            }

            #endregion

            #region report more detail on people above

            console.WriteLinePrecededByOne("Finishes in more than one category during the series so far:");

            foreach (var culprit in dictionaryOfCulpritsKeyedByBib)
            {
                PrintReportOnPerson(culprit);

                console.WriteLine();
            }

            #endregion

            #region report more detail on anyone/everyone who recorded a finish out of series category during the series

            console.WriteLinePrecededByOne("All finishes not in series category so far:");

            foreach (var culprit in dictionaryOfPublishedResultsKeyedByBib)
                if (IsFinisherOutOfCategory(culprit))
                {
                    PrintReportOnPerson(culprit);

                    console.WriteLine();
                }

            #endregion

            #region report more detail on arbitrary list of people selected for closer inspection for the series

            if (ListOfBibsForCloserInspection.Count > 0)
            {
                console.WriteLinePrecededByOne("People selected for closer inspection:");

                foreach (var bib in ListOfBibsForCloserInspection)
                {
                    PrintReportOnPerson(new KeyValuePair<string, IList<PossiblySuspiciousFinish>>(bib, dictionaryOfPublishedResultsKeyedByBib[bib]));

                    console.WriteLine();
                }

                #endregion

                #region wrap up

                console.WriteLinePrecededByOne("Everything complete. No further action required.");
                console.WriteLineWrappedByOne("ooo0 - Goodbye - 0ooo");

                SaveWorkToHardDrive(console.ToString(),
                    FolderForDiagnosticReport,
                    JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FileNameOfDiagnosticReport));

                console.ReadLine();

                #endregion
            }
        }
        catch (Exception ex)
        {
            console.WriteLineFollowedByOne(ex.ToString());
            console.ReadLine();
        }
    }

    private static bool IsFinisherOutOfCategory(KeyValuePair<string, IList<PossiblySuspiciousFinish>> culprit)
    {
        var person = culprit.Value.FirstOrDefault();

        if (person is null)
            return false;


        foreach (var finish in culprit.Value)
        {
            var participantHubItemDto = DictionaryOfParticipantsInPortalMasterListKeyedByBib[finish.ResultDto.Bib].FirstOrDefault();

            if (participantHubItemDto is null)
                //console.WriteLine(
                //    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} {"Series: unknown",-20} {finish.ResultDto.T01,12}");
                return false;

            if (!DateTime.TryParse(finish.EventProfileDto.AdvertisedDateAsString, out var dateOfEvent))
                //console.WriteLine(
                //    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} {"Series: unknown",-20} {finish.ResultDto.T01,12}");
                continue;

            var officialRaceGroup = FigureOutRaceGroup(participantHubItemDto, dateOfEvent);

            if (officialRaceGroup == finish.ResultDto.RaceGroup)
                continue;
            //console.WriteLine(
            //    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} Series: {officialRaceGroup,-13} {finish.ResultDto.T01,12}");
            return true;
            //console.WriteLine(
            //    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} Series: {officialRaceGroup,-13} {finish.ResultDto.T01,12} Not in series category");
        }

        return false;
    }

    private static void PrintReportOnPerson(KeyValuePair<string, IList<PossiblySuspiciousFinish>> culprit)
    {
        var person = culprit.Value.FirstOrDefault();

        if (person is null)
            return;


        foreach (var finish in culprit.Value)
        {
            var participantHubItemDto = DictionaryOfParticipantsInPortalMasterListKeyedByBib[finish.ResultDto.Bib].FirstOrDefault();

            if (participantHubItemDto is null)
            {
                console.WriteLine(
                    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} {"Series: unknown",-20} {finish.ResultDto.T01,12}");

                continue;
            }

            if (!DateTime.TryParse(finish.EventProfileDto.AdvertisedDateAsString, out var dateOfEvent))
            {
                console.WriteLine(
                    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} {"Series: unknown",-20} {finish.ResultDto.T01,12}");

                continue;
            }

            var officialRaceGroup = FigureOutRaceGroup(participantHubItemDto, dateOfEvent);

            if (officialRaceGroup == finish.ResultDto.RaceGroup)
                console.WriteLine(
                    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} Series: {officialRaceGroup,-13} {finish.ResultDto.T01,12}");
            else
                console.WriteLine(
                    $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} Series: {officialRaceGroup,-13} {finish.ResultDto.T01,12} Not in series category");
        }
    }

    #endregion

    #region variables

    private static readonly JghConsoleHelperV2 console = new();

    private static SeriesProfileDto _seriesProfileDto = new();

    private static readonly JghListDictionary<string, ParticipantHubItemDto> DictionaryOfParticipantsInPortalMasterListKeyedByBib = [];

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

    private const string Description = "This console program (Tool13) reads files of the published results for all events in the series to date " +
                                       "and then determines which participants have raced on more than one category because they upgraded or " +
                                       "downgraded, or were mis-categorised, or just rode as a bandit in an illegitimate category. If asked, it can " +
                                       "also inspect an arbitrary list of bibs.";

    private const int LhsWidth = 40;

    private const string FileNameOfDiagnosticReport = @"DiagnosticReport-NotInCategoryRides.txt";

    private const string FileNameOfSeriesProfileDto = @"!seriesprofile-Kelso2024-mtb.json";
    private const string FolderForSeriesProfile = @"C:\Users\johng\holding pen\StuffByJohn\2024-SeriesProfile\";
    private const string FileNameOfParticipantMasterListFromPortal = @"2024-07-09T14-53-23+Participants.json";
    private const string FolderForParticipantMasterListFromPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";
    private const string FolderForInputData = @"C:\Users\johng\holding pen\StuffByJohn\2024-Published-Results-For-All-Events\";
    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffByJohn\2024-DiagnosticReport-NotInCategoryRides\";

    private static readonly List<string> ListOfBibsForCloserInspection = ["7", "8","218", "195", "34", "288", "271", "230", "109", "150", "160", "43", "230", "291", "210","294", "54", "148", "85", "16", "15", "200", "271", "188", "249", "181", "62", "198", "218", "113", "114", "48", "343", "160", "121", "85", "293", "371"];

    #endregion
}