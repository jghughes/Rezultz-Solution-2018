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

        _console.WriteLineFollowedByOne("Welcome.");
        _console.WriteLineFollowedByOne(Description);
        _console.WriteLine($"{JghString.LeftAlign("Filename for SeriesProfile file:", LhsWidth)} {FileNameOfSeriesProfileDto}");
        _console.WriteLine($"{JghString.LeftAlign("Folder for SeriesProfile file:", LhsWidth)} {FolderForSeriesProfile}");
        _console.WriteLine($"{JghString.LeftAlign("Filename for Portal participants file:", LhsWidth)} {FileNameOfParticipantMasterListFromPortal}");
        _console.WriteLine($"{JghString.LeftAlign("Folder for Portal participants file:", LhsWidth)} {FolderForParticipantMasterListFromPortal}");
        _console.WriteLine($"{JghString.LeftAlign("Folder for series results:", LhsWidth)} {FolderForInputData}");
        _console.WriteLine($"{JghString.LeftAlign("Folder for diagnostic report documents:", LhsWidth)} {FolderForDiagnosticReport}");
        _console.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        _console.ReadLine();

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
                _console.WriteLine("Directory not found: " + FolderForInputData);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForParticipantMasterListFromPortal);
            }
            catch (DirectoryNotFoundException)
            {
                _console.WriteLine("Directory not found: " + FolderForParticipantMasterListFromPortal);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForDiagnosticReport);
            }
            catch (DirectoryNotFoundException)
            {
                _console.WriteLine("Directory not found: " + FolderForDiagnosticReport);
                return;
            }

            #endregion

            #region confirm existence of seriesprofile .json file

            // NB: be sure that this file is totally up-to-date. changes are being made daily!

            var path = Path.Combine(FolderForSeriesProfile, FileNameOfSeriesProfileDto);

            var fi = new FileInfo(path);

            if (!fi.Exists)
            {
                _console.WriteLine($"Failed to locate designated series profile file. <{fi.Name}>");

                return;
            }

            #endregion

            #region confirm existence of participantHubItems .json file from portal

            // NB: be sure that this file is totally up-to-date. changes are being made daily!

            var path2 = Path.Combine(FolderForParticipantMasterListFromPortal, FileNameOfParticipantMasterListFromPortal);

            var fi2 = new FileInfo(path2);

            if (!fi2.Exists)
            {
                _console.WriteLine($"Failed to locate designated participant file. <{fi2.Name}>");

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
                _console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            _console.WriteLine();

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
                _console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            _console.WriteLine();

            #endregion

            #region confirm existence of .xml files of published results for all events in the series to date

            var di = new DirectoryInfo(FolderForInputData); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                _console.WriteLinePrecededByOne($"{JghString.LeftAlign($"Problem: No files found in published results folder. [{FolderForInputData}]", LhsWidth)}");

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

                _console.WriteLine($"Loaded {inputFileItem.FileInfo.Name}: {inputFileItem.ContentsAsText.Length} characters");
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
                    _console.WriteLine("Deserialization failure. Results from file exported from Portal not obtained: " + ex.Message);
                    return;
                }

                _console.WriteLine($"Successfully(?) extracted .xml contents of {inputFileItem.FileInfo.Name}: {inputFileItem.ContentsAsCollectionOfPossiblySuspiciousFinishes.Count} line items");

                #endregion
            }

            #endregion

            #region convert lists of results in all files to dictionary of ResultDto keyed by Bib for easy analysis - exclude Dnx outcomes and non-series riders

            JghListDictionary<string, PossiblySuspiciousFinish> dictionaryOfPublishedResultsKeyedByBib = new();

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

            _console.WriteLinePrecededByOne($"People who finished in more than one category during the series so far: {dictionaryOfCulpritsKeyedByBib.Count}");

            foreach (var culprit in dictionaryOfCulpritsKeyedByBib)
            {
                var person = culprit.Value.FirstOrDefault();

                if (person is null)
                    continue;

                var xx = culprit.Value.Distinct(new PossiblySuspiciousFinishEqualityComparer()).ToArray();

                var xxList = string.Join(", ", xx.Select(z => z.ResultDto.RaceGroup));

                _console.WriteLine($"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-25} {"Categories:",-10} {xx.Count(),3}  {xxList}");
            }

            #endregion

            #region report people who raced in a different RaceGroup in MyLaps compared to RaceGroup they are registered for in the Portal for the series

            _console.WriteLinePrecededByOne("Finishes in more than one category during the series so far:");

            foreach (var culprit in dictionaryOfCulpritsKeyedByBib)
            {
                var person = culprit.Value.FirstOrDefault();

                if (person is null)
                    continue;

                foreach (var finish in culprit.Value)
                {
                    if (finish is null) continue;

                    var participantHubItemDto = DictionaryOfParticipantsInPortalMasterListKeyedByBib[finish.ResultDto.Bib].FirstOrDefault();

                    if (participantHubItemDto is null)
                    {
                        _console.WriteLine(
                            $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} {"Series: unknown",-20} {finish.ResultDto.T01,12}");

                        continue;
                    };

                    if (!DateTime.TryParse(finish.EventProfileDto.AdvertisedDateAsString, out var dateOfEvent))
                    {
                        _console.WriteLine(
                            $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} {"Series: unknown",-20} {finish.ResultDto.T01,12}");
   
                        continue;

                    }
                    var officialRaceGroup = FigureOutRaceGroup(participantHubItemDto, dateOfEvent);

                    if (officialRaceGroup == finish.ResultDto.RaceGroup)
                    {
                        _console.WriteLine(
                            $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} Series: {officialRaceGroup,-13} {finish.ResultDto.T01,12}");

                    }
                    else
                    {
                        _console.WriteLine(
                            $"Bib: {$"{person.ResultDto.Bib}",4} {$"{person.ResultDto.First} {person.ResultDto.Last}",-22} {finish.EventProfileDto.Label,-23} {finish.EventProfileDto.AdvertisedDateAsString,-11} Raced: {finish.ResultDto.RaceGroup,-13} Series: {officialRaceGroup,-13} {finish.ResultDto.T01,12} Not in series category");

                    }
                }

                _console.WriteLine();
            }


            #endregion

            #region wrap up

            _console.WriteLinePrecededByOne("Everything complete. No further action required.");
            _console.WriteLineWrappedByOne("ooo0 - Goodbye - 0ooo");

            SaveWorkToHardDrive(_console.ToString(),
                FolderForDiagnosticReport,
                JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FileNameOfDiagnosticReport));

            _console.ReadLine();

            #endregion
        }
        catch (Exception ex)
        {
            _console.WriteLineFollowedByOne(ex.ToString());
            _console.ReadLine();
        }
    }

    #endregion

    #region variables

    private static readonly JghConsoleHelperV2 _console = new();

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

        _console.WriteLine($"{JghString.LeftAlign("File saved:", 15)} {outPutFilename}");
        _console.WriteLine($"{JghString.LeftAlign("Folder:", 15)} {outPutFolder}");
    }

    #endregion

    #region parameters

    private const string Description = "This console program (Tool13) reads files of the publsihed results for all events in the series to date " +
                                       "and then determines which participants have raced on more than one category because they upgraded or " +
                                       "downgraded, or were mis-categorised, or just rode as a bandit in an illegitimate category. ";

    private const int LhsWidth = 40;

    private const string FileNameOfDiagnosticReport = @"DiagnosticReport-Out-of-Category-Rides.txt";

    private const string FileNameOfSeriesProfileDto = @"!seriesprofile-Kelso2024-mtb.json";
    private const string FolderForSeriesProfile = @"C:\Users\johng\holding pen\StuffByJohn\2024-SeriesProfile\";
    private const string FileNameOfParticipantMasterListFromPortal = @"2024-07-09T14-53-23+Participants.json";
    private const string FolderForParticipantMasterListFromPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";
    private const string FolderForInputData = @"C:\Users\johng\holding pen\StuffByJohn\2024-Published-Results-For-All-Events\";
    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffByJohn\2024-DiagnosticReport-OutOfCategoryRides\";

    #endregion
}