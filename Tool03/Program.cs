using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repositories;

namespace Tool03;

internal class Program
{
    private const string Description = "This program reads one or more files of MyLaps data (in problematic CSV format)." +
                                       " It tediously converts the data into ResultDto items, relying on the participant master list" +
                                       " from RezultzHub and parameters for the specified event from the SeriesProfile to plug gaps in the (incomplete) MyLaps data." +
                                       " Finallly, it exports the manicured file/s of publishable XML results for the event for upload to Azure.";

    private static void Main()
    {
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for Rezultz stuff", LhsWidth)} : {InputFolderFromRezultz}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder from MyLaps timing data", LhsWidth)} : {InputFolderFromMyLaps}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for publishable results", LhsWidth)} : {OutputFolderForXml}");
        JghConsoleHelper.WriteLineWrappedInOne("Press enter to go. When you see FINISH you're done.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

        #region confirm existence of folders

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

        try
        {
            // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
            Directory.SetCurrentDirectory(InputFolderFromRezultz);
        }
        catch (DirectoryNotFoundException)
        {
            JghConsoleHelper.WriteLine("Directory not found: " + InputFolderFromRezultz);
            return;
        }

        try
        {
            // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
            Directory.SetCurrentDirectory(OutputFolderForXml);
        }
        catch (DirectoryNotFoundException)
        {
            JghConsoleHelper.WriteLine("Directory not found: " + OutputFolderForXml);
            return;
        }

        #endregion

        #region confirm existence of files of input data

        var participantMasterListFileInfo = new FileInfo(InputFolderFromRezultz + ParticipantMasterFileNameFromRezultz);

        if (!participantMasterListFileInfo.Exists)
        {
            JghConsoleHelper.WriteLine($"Participant file not found: {participantMasterListFileInfo.Name}");
            return;
        }

        var seriesMetadataFileFromRezultzFileInfo = new FileInfo(InputFolderFromRezultz + SeriesMetadataFileFromRezultz);

        if (!seriesMetadataFileFromRezultzFileInfo.Exists)
        {
            JghConsoleHelper.WriteLine($"Series metadata file not found: {seriesMetadataFileFromRezultzFileInfo.Name}");
            return;
        }

        foreach (var timingDataFileTuple in ArrayOfTimingDataFileNameTuples)
        {
            var myFileInfo = new FileInfo(InputFolderFromMyLaps + timingDataFileTuple.Item1);

            if (!myFileInfo.Exists)
            {
                JghConsoleHelper.WriteLine($"Timing data file not found: {myFileInfo.Name}");
                return;
            }
        }

        #endregion

        #region confirm existence of directories and files for output data

        try
        {
            foreach (var item in FileItemsFromMyLaps)
            {
                var pathOfXmlFile = OutputFolderForXml + @"\" + Path.GetFileNameWithoutExtension(item.OutputFileName) + "." + StandardFileTypeSuffix.Xml;

                File.WriteAllText(pathOfXmlFile, "<element>dummy</element>");
            }
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Failed to access a designated output file. {e.Message}");

            return;
        }

        #endregion

        #region Step 1: Load the MyLaps timing data files

        try
        {
            foreach (var tuple in ArrayOfTimingDataFileNameTuples)
            {
                if (!Path.GetFileName(tuple.Item1).EndsWith($".{RequiredMyLapsFileFormat}"))
                {
                    JghConsoleHelper.WriteLine($"Skipping {tuple.Item1} because it's not a {RequiredMyLapsFileFormat} file as you appear to have specified. Does this make sense?");
                    continue;
                }

                var item = new FileItem
                {
                    FileInfo = new FileInfo(InputFolderFromMyLaps + tuple.Item1),
                    FileContentsAsLines = new List<string>(),
                    OutputFileName = tuple.Item2
                };

                FileItemsFromMyLaps.Add(item);
                JghConsoleHelper.WriteLine($"Will look for {item.FileInfo.Name}.");
            }

            JghConsoleHelper.WriteLine();


            try
            {
                foreach (var item in FileItemsFromMyLaps)
                {
                    JghConsoleHelper.WriteLine($"Looking for {item.FileInfo.Name}.");

                    var fullInputPath = item.FileInfo.FullName;

                    var rawInputAsText = File.ReadAllText(fullInputPath);
                    var rawInputAsLines = File.ReadAllLines(fullInputPath);

                    item.FileContentsAsText = rawInputAsText;
                    item.FileContentsAsLines = rawInputAsLines.ToList();

                    JghConsoleHelper.WriteLine($"Found {item.FileInfo.Name}.");
                }
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine(e.Message);
                throw new Exception(e.InnerException?.Message);
            }

            if (FileItemsFromMyLaps.Count == 0)
                throw new Exception("Found not even a single MyLaps timing data file.");
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Failed to locate designated MyLaps timing data. {e.Message}");
            JghConsoleHelper.WriteLine("");
            return;
        }

        JghConsoleHelper.WriteLine();

        #endregion

        #region Get ready : load the participant master list and series metadata from Rezultz

        SeriesProfileDto seriesProfileDto;

        List<ParticipantHubItemDto> masterListOfParticipantHubItemDTo;

        try
        {
            var seriesMetadataObjectDtoSerialisedAsStringOfJson = File.ReadAllText(InputFolderFromRezultz + SeriesMetadataFileFromRezultz);

            seriesProfileDto = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(seriesMetadataObjectDtoSerialisedAsStringOfJson);

            var participantMasterListSerialisedAsStringOfJson = File.ReadAllText(InputFolderFromRezultz + ParticipantMasterFileNameFromRezultz);

            masterListOfParticipantHubItemDTo = JghSerialisation.ToObjectFromJson<List<ParticipantHubItemDto>>(participantMasterListSerialisedAsStringOfJson);
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"Oops. Failed to locate (or de-serialise) the designated input file. {e.Message}");

            return;
        }

        #endregion

        try
        {
            Main01(FileItemsFromMyLaps, DateTime.Parse(AdvertisedDateOfEvent06), masterListOfParticipantHubItemDTo, seriesProfileDto, NumberOfRowsPrecedingRowOfFieldNames);
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLine($"{e.Message}");
        }
    }

    #region main methods

    private static void Main01(List<FileItem> fileItems, DateTime dateOfEvent, List<ParticipantHubItemDto> masterListOfParticipantHubItemDTo, SeriesProfileDto seriesMetadataItemDto,
        int numberOfRowsPreceedingRowOfFieldNames)
    {
        var ageGroupSpecificationItems = GetAgeGroupSpecificationItems(seriesMetadataItemDto);

        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of timing data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                List<ResultDto> listOfComputedResultItemDataTransferObjects = new();

                #region step 1 interpret myLapsTimingDataInCsvFormatAsString as csv data if we can

                var myLapsTimingDataInCsvFormatAsString = fileItem.FileContentsAsText;

                var allRowsOfCsvText = myLapsTimingDataInCsvFormatAsString.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None).ToList(); // remove carriage returns and line breaks

                //List<string> allRowsOfCsvText = fileItem.FileContentsAsLines;

                if (string.IsNullOrWhiteSpace(myLapsTimingDataInCsvFormatAsString) && !allRowsOfCsvText.Any())
                {
                    JghConsoleHelper.WriteLine($"Unable to read this file. Contents can't processed as plain text. {fileItem.FileInfo.Name}");
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

                if (relevantRowsWithoutEscapeLiterals.Count() < numberOfRowsPreceedingRowOfFieldNames + 2)
                {
                    // bale if we don't have at least a heading row and one data row
                    SaveWorkToHardDriveAsXml(listOfComputedResultItemDataTransferObjects, fileItem); // the list will be default empty if we are here
                    continue;
                }

                #endregion

                #region step 2 process the headings row (which might orr might not be the first row) - locate the column headings

                var arrayOfColumnHeadings = relevantRowsWithoutEscapeLiterals.Skip(numberOfRowsPreceedingRowOfFieldNames).FirstOrDefault()?.Split(',');

                if (arrayOfColumnHeadings == null)
                {
                    // bale if we don't have the headings row
                    SaveWorkToHardDriveAsXml(listOfComputedResultItemDataTransferObjects, fileItem);
                    continue;
                }

                // example of headings from MyLaps : "Pos","Bib#","Athlete","Gun Time","Race","Age","Gender"
                var indexOfBibColumn = Array.IndexOf(arrayOfColumnHeadings, "Bib#");
                var indexOfTotalDuration = Array.IndexOf(arrayOfColumnHeadings, "Finish Time");
                var indexOfFullName = Array.IndexOf(arrayOfColumnHeadings, "Athlete");
                var indexOfRaceGroup = Array.IndexOf(arrayOfColumnHeadings, "Race");
                var indexOfAge = Array.IndexOf(arrayOfColumnHeadings, "Age");
                var indexOfGender = Array.IndexOf(arrayOfColumnHeadings, "Gender");

                if (indexOfBibColumn == -1 && indexOfTotalDuration == -1 && indexOfFullName == -1 && indexOfRaceGroup == -1 && indexOfAge == -1 && indexOfGender == -1)
                {
                    JghConsoleHelper.WriteLine($"Unable to find any of the required columns in the file {fileItem.FileInfo.Name}");
                    SaveWorkToHardDriveAsXml(listOfComputedResultItemDataTransferObjects, fileItem);
                    continue;
                }

                #endregion

                #region step 3 process all the subsequent rows (which should be legit rows of data by now)

                var rowsOfData = relevantRowsWithoutEscapeLiterals.Skip(numberOfRowsPreceedingRowOfFieldNames + 1).ToList(); // skip first line i.e. the header row we just processed in Step 2

                if (!rowsOfData.Any()) break; // bale if we don't have any data rows

                var i = 1;

                foreach (var row in rowsOfData)
                {
                    var arrayOfDataInRow = row.Split(',');

                    #region instantiate ResultItemDataTransferObject for this row and populate with particulars from participant master list iff identifier is found in master list

                    var identifierOfParticipant = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfBibColumn);

                    if (string.IsNullOrWhiteSpace(identifierOfParticipant)) continue;

                    var participantItem = masterListOfParticipantHubItemDTo.FirstOrDefault(z => z.Identifier == identifierOfParticipant);

                    ResultDto thisResultItemDto;

                    if (participantItem == null)
                    {
                        // "Pos","Bib#","Athlete","Finish Time","Race","Age","Gender"

                        thisResultItemDto = new ResultDto
                        {
                            Bib = identifierOfParticipant,
                            Last = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfFullName),
                            Sex = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfGender),
                            Age = JghConvert.ToInt32(GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfAge)),
                            RaceGroup = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfRaceGroup),
                            IsSeries = false
                        };

                        JghConsoleHelper.WriteLine($"Warning! Participant master list fails to have an identifier for {thisResultItemDto.Bib} {thisResultItemDto.Last} {thisResultItemDto.RaceGroup}");
                    }
                    else
                    {
                        thisResultItemDto = new ResultDto
                        {
                            Bib = participantItem.Identifier,
                            First = participantItem.FirstName,
                            Last = participantItem.LastName,
                            MiddleInitial = participantItem.MiddleInitial,
                            Sex = participantItem.Gender,
                            Age = ParticipantDatabase.ToAgeFromBirthYear(participantItem.BirthYear),
                            AgeGroup = ParticipantDatabase.ToAgeCategoryDescriptionFromBirthYear(participantItem.BirthYear, ageGroupSpecificationItems),
                            City = participantItem.City,
                            Team = participantItem.Team,
                            IsSeries = participantItem.IsSeries
                        };

                        #region figure out RaceGroup

                        thisResultItemDto.RaceGroup = FigureOutRaceGroup(participantItem, dateOfEvent);

                        #endregion
                    }


                    #region figure out TO1 and Dnx regardless of whether participant is in master list

                    var mustSkipThisRowBecauseGunTimeIsNonsense = false; // initial default

                    var durationAsPossiblyNastyString = GetTextItemFromArrayByIndexOrStringEmpty(arrayOfDataInRow, indexOfTotalDuration);

                    if (TryConvertTextToTimespan(durationAsPossiblyNastyString, out var myTimeSpan, out var conversionReport01))
                    {
                        thisResultItemDto.T01 = myTimeSpan.ToString("G");
                        thisResultItemDto.DnxString = string.Empty;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dnf"))
                    {
                        thisResultItemDto.T01 = string.Empty;
                        thisResultItemDto.DnxString = Symbols.SymbolDnf;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dns"))
                    {
                        thisResultItemDto.T01 = string.Empty;
                        thisResultItemDto.DnxString = Symbols.SymbolDns;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("dq"))
                    {
                        thisResultItemDto.T01 = string.Empty;
                        thisResultItemDto.DnxString = Symbols.SymbolDq;
                    }
                    else if (JghString.TmLr(durationAsPossiblyNastyString).Contains("tbd"))
                    {
                        thisResultItemDto.T01 = string.Empty;
                        thisResultItemDto.DnxString = Symbols.SymbolTbd;
                    }
                    else
                    {
                        thisResultItemDto.T01 = $"Gun Time is nonsense. {conversionReport01}";
                        mustSkipThisRowBecauseGunTimeIsNonsense = true;
                    }

                    #endregion

                    WriteOneLineReport(i, thisResultItemDto, durationAsPossiblyNastyString);

                    i += 1;

                    if (mustSkipThisRowBecauseGunTimeIsNonsense)
                        continue;

                    listOfComputedResultItemDataTransferObjects.Add(thisResultItemDto);
                }

                #endregion

                #endregion

                #region step 4 write report to console for this file

                JghConsoleHelper.WriteLine("Resulting output :");

                var j = 1;

                foreach (var resultItemDataTransferObject in listOfComputedResultItemDataTransferObjects.OrderBy(z => z.T01))
                {
                    WriteOneLineReport(j, resultItemDataTransferObject, string.Empty);

                    j += 1;
                }

                JghConsoleHelper.WriteLine($"Total number of records read = {rowsOfData.Count}");
                JghConsoleHelper.WriteLine($"Total number of records produced = {listOfComputedResultItemDataTransferObjects.Count}");

                #endregion

                #region step 5 serialise to XML and save output file

                SaveWorkToHardDriveAsXml(listOfComputedResultItemDataTransferObjects, fileItem);

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    #endregion

    #region helper methods

    private static void SaveWorkToHardDriveAsXml(List<ResultDto> listOfOutputDto, FileItem fileItem)
    {
        var xmlFileContents = JghSerialisation.ToXmlFromObject(listOfOutputDto.ToArray(), new[] {typeof(ResultDto[])});

        var pathOfXmlFile = OutputFolderForXml + @"\" + Path.GetFileNameWithoutExtension(fileItem.OutputFileName) + "." + StandardFileTypeSuffix.Xml;

        File.WriteAllText(pathOfXmlFile, xmlFileContents);

        JghConsoleHelper.WriteLine($"Records saved to {pathOfXmlFile}");
    }

    private static string FigureOutRaceGroup(ParticipantHubItemDto participantItem, DateTime dateOfEvent)
    {
        var isTransitionalParticipant = participantItem.RaceGroupBeforeTransition != participantItem.RaceGroupAfterTransition;

        string answer;

        if (isTransitionalParticipant)
        {
            if (DateTime.TryParse(participantItem.DateOfRaceGroupTransitionAsString, out var dateOfTransition))
            {
                var eventIsBeforeRaceGroupTransition = dateOfEvent < dateOfTransition;

                answer = eventIsBeforeRaceGroupTransition ? participantItem.RaceGroupBeforeTransition : participantItem.RaceGroupAfterTransition;

                return answer;
            }

            answer = participantItem.RaceGroupBeforeTransition;
        }
        else
        {
            answer = participantItem.RaceGroupBeforeTransition;
        }

        return answer;
    }

    private static string GetTextItemFromArrayByIndexOrStringEmpty(string[] arrayOfText, int indexOfDataItem)
    {
        var textItem = JghArrayHelpers.SelectItemFromArrayByArrayIndex(arrayOfText, indexOfDataItem);

        return JghString.TmLr(textItem ?? string.Empty);
    }

    private static AgeGroupSpecificationItem[] GetAgeGroupSpecificationItems(SeriesProfileDto seriesMetaDataItemDto)
    {
        var arrayOfAgeGroupSpecificationItemDto = seriesMetaDataItemDto.DefaultSettingsForAllEvents.AgeGroupSpecifications;

        var ageGroupSpecificationItems = AgeGroupSpecificationItem.FromDataTransferObject(arrayOfAgeGroupSpecificationItemDto);

        return ageGroupSpecificationItems;
    }

    private static bool TryConvertTextToTimespan(string purportedTimeSpanAsText, out TimeSpan answer, out string conversionReport)
    {
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

        var failure = "Conversion anomaly.";
        const string locus = "[TryConvertTextToTimespan]";

        conversionReport = string.Empty;

        var defaultConversionReport = $"{failure} Purported timespan is [{purportedTimeSpanAsText}] whereas [d.hh:mm:ss.ff] is required.";

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
                    answer = TimeSpan.MinValue;
                    conversionReport = defaultConversionReport + " Fractional seconds look wrong.";
                    return false;
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
                            answer = TimeSpan.MinValue;
                            return false;
                        }

                        break;
                    }
                    case 1:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out minutes))
                        {
                            conversionReport = defaultConversionReport + " Minutes look wrong.";
                            answer = TimeSpan.MinValue;
                            return false;
                        }

                        break;
                    }
                    case 2:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out hours))
                        {
                            conversionReport = defaultConversionReport + " Hours look wrong.";
                            answer = TimeSpan.MinValue;
                            return false;
                        }

                        break;
                    }
                    case 3:
                    {
                        if (!TryGetFrontComponentAsInteger(components, out days))
                        {
                            conversionReport = defaultConversionReport + " Days look wrong.";
                            answer = TimeSpan.MinValue;
                            return false;
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

    private static void WriteOneLineReport(int index, ResultDto dtoItem, string inputDuration)
    {
        if (string.IsNullOrWhiteSpace(inputDuration))
            JghConsoleHelper.WriteLine($"{index,-3}  {dtoItem.Bib,-3}  {dtoItem.First,-15} {dtoItem.Last,-15}  {dtoItem.T01,-15}  {dtoItem.DnxString,-3}");
        else
            JghConsoleHelper.WriteLine($"{index,-3}  {dtoItem.Bib,-3}  {dtoItem.First,-15} {dtoItem.Last,-15}  {dtoItem.T01,-15}  {dtoItem.DnxString,-3}  [{inputDuration,-15}]");
    }

    #endregion

    #region constants

    private const int LhsWidth = 30;

    private const string AdvertisedDateOfEvent06 = "2023-07-04";

    private const string InputFolderFromRezultz = @"C:\Users\johng\holding pen\StuffFromRezultzAzure\";
    private const string InputFolderFromMyLaps = @"C:\Users\johng\holding pen\StuffFromAndrew\Event01FromMylaps\Current xls versions\Current xls versions exported to csv and then renamed to txt\";
    private const string OutputFolderForXml = @"C:\Users\johng\holding pen\StuffFromAndrew\Event01FromMylaps\Current xls versions\Current xls versions exported to csv and then renamed to txt\XmlVersions\";

    private const string RequiredMyLapsFileFormat = "txt"; // "txt" or "csv"
    private const string SeriesMetadataFileFromRezultz = @"series-Kelso2023-mtb.json";
    private const string ParticipantMasterFileNameFromRezultz = @"Participants+2023-07-18T12-16-20.json";
    private static readonly Tuple<string, string>[] ArrayOfTimingDataFileNameTuples =
    {
        new("Expert Results.txt", "Kelso2023mtb-results-01-expert.xml"),
        new("Sport Results.txt", "Kelso2023mtb-results-01-sport.xml"),
        new("Novice Results.txt", "Kelso2023mtb-results-01-novice.xml")
    };

    private const int
        NumberOfRowsPrecedingRowOfFieldNames =
            0; // 0 is normal for csv files exported manually by Jgh from the Excel exported from MyLaps. 1 for csv files exported directly from MyLaps. they have some sort of title row before the field names row.
    //int numberOfRowsPrecedingRowOfFieldNames = 1; // Oddity when the csv file is exported directly from MyLaps. they have some sort of title row before the field names row.


    #endregion

    #region variables

    private static readonly List<FileItem> FileItemsFromMyLaps = new();

    #endregion
}