using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

// ReSharper disable InconsistentNaming

namespace Tool12;

internal class Program
{
    private const string Description =
        "This console program (Tool12) is intended to indentify discrepencies between the Kelso series-participant list and the portal participant master list." +
        "To obtain the Kelso list, it reads (XML) files exported from Andrew's SERIES POINTS spreadsheet (exported by JGH into four worksheets in Access named Expert, Intermediate, Novice, and Sport and then exported as XML). " +
        "To obtain an up to date master list of the participants recorded in the Portal, JGH exports the master list from the Particpant registration module.";

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Series points folder:", LhsWidth)} {FolderKelsoSeriesPointsFilesFromAndrew}");
        console.WriteLine($"{JghString.LeftAlign("Portal participants file:", LhsWidth)} {FilenameOfParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Portal participants folder:", LhsWidth)} {FolderContainingParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Diagnostic report filename:", LhsWidth)} {FilenameOfDiagnosticReport}");
        console.WriteLine($"{JghString.LeftAlign("Diagnostic report folder:", LhsWidth)} {FolderForDiagnosticReport}");


        console.WriteLinePrecededByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderKelsoSeriesPointsFilesFromAndrew);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderKelsoSeriesPointsFilesFromAndrew);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingParticipantMasterListExportedFromRezultzPortal);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingParticipantMasterListExportedFromRezultzPortal);
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

            #region confirm existence of participantHubItems file from portal

            var path = Path.Combine(FolderContainingParticipantMasterListExportedFromRezultzPortal, FilenameOfParticipantMasterListExportedFromRezultzPortal);

            var portalParticipantFileInfo = new FileInfo(path);

            if (!portalParticipantFileInfo.Exists)
            {
                console.WriteLine($"Failed to locate designated participant file. <{portalParticipantFileInfo.Name}>");

                return;
            }

            #endregion

            #region deserialise participantHubItems into a Dictionary

            try
            {
                var rawInputAsText = await File.ReadAllTextAsync(portalParticipantFileInfo.FullName);

                var participantHubItemDtoArray = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto[]>(rawInputAsText);

                ParticipantHubItem[] participantHubItems = ParticipantHubItem.FromDataTransferObject(participantHubItemDtoArray);

                PrintSummaryOfFileEntries(portalParticipantFileInfo, participantHubItems);

                foreach (var participantHubItem in participantHubItems)
                    if (!string.IsNullOrWhiteSpace(participantHubItem.Bib))
                        participantsInPortalKeyedByBibDictionary.Add(participantHubItem.Bib, participantHubItem);
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            console.WriteLine();

            #endregion

            #region find all (four) XML files that contain Kelso series-participants

            var di = new DirectoryInfo(FolderKelsoSeriesPointsFilesFromAndrew); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLine($"No files found in designated folder: {di.FullName}");
                return;
            }

            #endregion

            #region read all the files into a dictionary of participants with series points

            var totalPeopleWithPoints = 0;

            foreach (var fileInfo in arrayOfInputFileInfo)
                try
                {
                    var fileItem = new FileItem
                    {
                        FileInfo = fileInfo,
                        FileContentsAsText = "",
                        FileContentsAsXElement = new XElement("dummy"),
                        OutputSubFolderName = string.Empty // not used
                    };

                    var fullInputPath = fileItem.FileInfo.FullName;


                    var seriesParticipantsWithPoints = await ProcessXmlData(fullInputPath, fileItem);

                    PrintSummaryOfFileEntries(fileInfo, seriesParticipantsWithPoints);

                    foreach (var participantWithPoints in seriesParticipantsWithPoints)
                        participantsWithPointsKeyedByBibDictionary.Add(participantWithPoints.Bib, participantWithPoints);


                    totalPeopleWithPoints += seriesParticipantsWithPoints.Count;
                }
                catch (Exception e)
                {
                    console.WriteLine($"Failed to successfully obtain people with points participant info. {e.Message} {e.InnerException!.Message}");
                    console.WriteLine("");

                    return;
                }

            console.WriteLine($"Total entries with points: {totalPeopleWithPoints}");

            #endregion

            #region print one-off participants enetered in the Portal - works perfectly - commented out for now

            //var oneOffParticipantsInPortal = participantsInPortalKeyedByBibDictionary.FindAllValues(z => !z.IsSeries).ToArray();
            //var oneOffParticipantsWithPoints = participantsWithPointsKeyedByBibDictionary.FindAllValues(z => !z.IsSeries).ToArray(); // there shouldn't be any by definition

            //console.WriteLinePrecededByOne($"One-off participants in Portal: {oneOffParticipantsInPortal.Length}");
            //PrintPeople(oneOffParticipantsInPortal.OrderBy(z => z.RaceGroupBeforeTransition).ThenBy(z => z.Bib));

            //console.WriteLinePrecededByOne($"One-off participants in Points: {oneOffParticipantsWithPoints.Length}");
            //PrintPeople(oneOffParticipantsWithPoints.OrderBy(z => z.RaceGroup).ThenBy(z => z.Bib));

            //console.WriteLine();

            #endregion

            #region print defective entries in participants with points

            var defectiveEntriesInPoints = participantsWithPointsKeyedByBibDictionary.FindAllValues(z => z.IsDefective).OrderBy(z => z.RaceGroup).ThenBy(z => z.Bib).ToArray();

            console.WriteLine($"Defective entries in Points: {defectiveEntriesInPoints.Length}");
            PrintPeople(defectiveEntriesInPoints);

            #endregion

            #region analyse bibs allocated more than once

            // portal

            var bibsAllocatedMoreThanOnceInPortal = participantsInPortalKeyedByBibDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();


            console.WriteLinePrecededByOne($"Bibs allocated by mistake in multiple categories in the Portal: {bibsAllocatedMoreThanOnceInPortal.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPortal)
            {
                var people = participantsInPortalKeyedByBibDictionary[bib].Where(z => z is not null).ToArray();

                var firstPerson = people.First();
                console.Write($"{JghString.LeftAlign(firstPerson.Bib, LhsWidthTiny)} {firstPerson.FirstName} {firstPerson.FirstName} {firstPerson.BirthYear} ");
                foreach (var person in people) console.Write($"/({person.RaceGroupBeforeTransition} {person.RaceGroupAfterTransition})");

                console.WriteLine();
            }

            // points

            var bibsAllocatedMoreThanOnceInPoints = participantsWithPointsKeyedByBibDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"Bibs allocated by mistake in multiple categories in Points: {bibsAllocatedMoreThanOnceInPoints.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPoints.Where(z => !string.IsNullOrWhiteSpace(z)))
            {
                var people = participantsWithPointsKeyedByBibDictionary[bib].Where(z => z is not null).ToArray();
                var firstPerson = people.First();

                console.Write($"{JghString.LeftAlign(firstPerson.Bib, LhsWidthTiny)} {firstPerson.FullName} {firstPerson.Age} ");
                foreach (var person in people) console.Write($"/{person.RaceGroup}");

                console.WriteLine();
            }

            #endregion

            #region analyse names allocated more than once

            // portal

            JghListDictionary<string, ParticipantHubItem> participantsInPortalKeyedByNameDictionary = [];

            foreach (var person in participantsInPortalKeyedByBibDictionary.FindAllValues(z => z.IsSeries))
            {
                var key = JghString.TmLr(JghString.Remove(' ', JghString.Concat(person.FirstName, person.LastName)));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsInPortalKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPortal = participantsInPortalKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"Names mistakenly appearing in multiple locations in the Portal: {namesAllocatedMoreThanOnceInPortal.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPortal) PrintPeople(participantsInPortalKeyedByNameDictionary.FindAllValuesByKey(z => z == key));

            //points

            JghListDictionary<string, ParticipantWithSeriesPointsTally> participantsWithPointsKeyedByNameDictionary = [];

            foreach (var person in participantsWithPointsKeyedByBibDictionary.FindAllValues(z => z.IsSeries))
            {
                var key = JghString.TmLr(JghString.Remove(' ', person.FullName));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsWithPointsKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPoints = participantsWithPointsKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"Names mistakenly appearing in multiple locations in Points - possibly across multiple categories (see above): {namesAllocatedMoreThanOnceInPoints.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPoints) PrintPeople(participantsInPortalKeyedByNameDictionary.FindAllValuesByKey(z => z == key));

            #endregion

            #region analyse names in both portal and points, but only those who have conflicting bibs

            var allNameKeysInPortal = participantsInPortalKeyedByNameDictionary.Keys.ToList();
            var allNameKeysInPoints = participantsWithPointsKeyedByNameDictionary.Keys.ToList();

            var distinctNameKeysInBothPortalAndPoints = allNameKeysInPortal.Intersect(allNameKeysInPoints).Distinct().ToArray();

            List<string> conflictedBibsInPortal = [];
            List<string> conflictedBibsInPoints = [];

            JghStringBuilder sb = new();

            var i = 0;

            foreach (var key in distinctNameKeysInBothPortalAndPoints)
            {
                var personInPortal = participantsInPortalKeyedByNameDictionary[key].FirstOrDefault();
                var personInPoints = participantsWithPointsKeyedByNameDictionary[key].FirstOrDefault();

                if (personInPortal is not null && personInPoints is not null && personInPortal.Bib != personInPoints.Bib)
                {
                    sb.AppendLine(
                        $"Bib: Portal-Points={JghString.LeftAlign($"{personInPortal.Bib}-{personInPoints.Bib}", LhsWidthSmall)} {personInPoints.FullName}  RaceGroup: Portal/Points=({personInPortal.RaceGroupBeforeTransition}->{personInPortal.RaceGroupAfterTransition})/{personInPoints.RaceGroup}");

                    conflictedBibsInPortal.Add(personInPortal.Bib);
                    conflictedBibsInPoints.Add(personInPoints.Bib);
                    i += 1;
                }
            }

            console.WriteLinePrecededByOne($"Names common to Portal and Points with conflicting Bibs: {i}");
            console.WriteLine(sb.ToString());

            #endregion

            #region list of 'missing' persons (comparatively speaking)

            var seriesEntriesInPortal = participantsInPortalKeyedByBibDictionary.FindAllValues(z => z.IsSeries).ToArray();
            var seriesEntriesWithPoints = participantsWithPointsKeyedByBibDictionary.FindAllValues(z => z.IsSeries).ToArray(); // tedious long way round. ha ha

            var allSeriesBibsInPortal = participantsInPortalKeyedByBibDictionary.Keys.ToArray();
            var allSeriesBibsInPoints = participantsWithPointsKeyedByBibDictionary.Keys.ToArray();

            //console.WriteLine();

            //console.WriteLine($"Participants in the portal (series only): {seriesEntriesInPortal.Count()}");
            //console.WriteLine($"Participants in the points master lists: {seriesEntriesWithPoints.Count()}");

            var missingBibsInPortal = allSeriesBibsInPoints.Except(allSeriesBibsInPortal).Except(conflictedBibsInPoints).ToList();
            var missingBibsInPoints = allSeriesBibsInPortal.Except(allSeriesBibsInPoints).Except(conflictedBibsInPortal).ToList();

            var peopleWithPointsWhoAreMissingInPortal = seriesEntriesWithPoints.Where(z => missingBibsInPortal.Contains(z.Bib)).Where(z => !z.IsDefective).ToArray();
            console.WriteLinePrecededByOne($"There are people in the points master lists who need to be added to the portal: {peopleWithPointsWhoAreMissingInPortal.Length}");
            PrintPeople(peopleWithPointsWhoAreMissingInPortal);

            var seriesParticipantsInPortalWhoAreMissingInPoints = seriesEntriesInPortal.Where(z => missingBibsInPoints.Contains(z.Bib)).ToArray();
            console.WriteLinePrecededByOne($"There are people in the Portal who need to be added to the points master lists: {seriesParticipantsInPortalWhoAreMissingInPoints.Length}");
            PrintPeople(seriesParticipantsInPortalWhoAreMissingInPoints);

            #endregion

            #region wrap up

            console.WriteLinePrecededByOne("Summary:");
            //console.WriteLinePrecededByOne($"{JghString.LeftAlign("Line items in participant master list:", LhsWidth)} {arrayOfRepeatingXe.Length}");
            //console.WriteLine($"{JghString.LeftAlign("Total birthdays worthy of analysis:", LhsWidth)} {seriesParticipantsWithPoints.Count}");
            //console.WriteLine($"{JghString.LeftAlign("Birthdays before series:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsPreSeriesBirthday: true })}");
            //console.WriteLine($"{JghString.LeftAlign("Birthdays after series:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsPostSeriesBirthday: true })}");
            //console.WriteLine($"{JghString.LeftAlign("Birthdays during series:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsMidSeriesBirthday: true })}");
            //console.WriteLine($"{JghString.LeftAlign(" - unchanged age group:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsMidSeriesBirthday: true } and { DoesSwitchAgeGroup: false })}");
            //console.WriteLine($"{JghString.LeftAlign(" - changed age group:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsMidSeriesBirthday: true } and { DoesSwitchAgeGroup: true })}");

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

    private static async Task<List<ParticipantWithSeriesPointsTally>> ProcessXmlData(string fullInputPath, FileItem fileItem)
    {
        try
        {
            var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);

            XElement rawInputAsXElement;

            try
            {
                rawInputAsXElement = XElement.Parse(rawInputAsText);
            }
            catch (Exception ex)
            {
                var msg = $"{fileItem.FileInfo.Name} failed to parse successfully. A valid XML document is expected. Is this a XML document? ({ex.Message})";

                console.WriteLine(msg);

                throw new Exception(ex.Message);
            }

            fileItem.FileContentsAsText = rawInputAsText;
            fileItem.FileContentsAsXElement = rawInputAsXElement;
        }
        catch (Exception e)
        {
            console.WriteLine(e.Message);
            throw new Exception(e.InnerException?.Message);
        }

        var arrayOfRepeatingXe = fileItem.FileContentsAsXElement.Elements().ToArray();

        if (arrayOfRepeatingXe.Length == 0)
            throw new Exception($"Found not even a single repeating child XElement in file <{fileItem.FileInfo.Name}>.");

        List<ParticipantWithSeriesPointsTally> seriesParticipantsWithPoints = [];

        foreach (var repeatXe in arrayOfRepeatingXe)
        {
            var item = DeserialiseLineItemToParticipantWithSeriesPointsTallyV3(repeatXe);
            //var item = DeserialiseLineItemToParticipantWithSeriesPointsTallyV2(repeatXe);

            if (!string.IsNullOrWhiteSpace(item.Bib))
                seriesParticipantsWithPoints.Add(item);
        }

        return seriesParticipantsWithPoints;
    }

    private static async Task<List<ParticipantWithSeriesPointsTally>> ProcessCsvData(string fullInputPath, FileItem fileItem)
    {
        var rawInputAsRows = await File.ReadAllLinesAsync(fullInputPath);

        var headerRow = rawInputAsRows[0];

        List<string> rowsOfRepeatingCsvData = [];

        for (var i = 1; i < rawInputAsRows.Length; i++)
        {
            rowsOfRepeatingCsvData.Add(rawInputAsRows[i]);
        }


        List<ParticipantWithSeriesPointsTally> seriesParticipantsWithPoints = [];

        foreach (var row in rowsOfRepeatingCsvData)
        {
            var xx = new JghMapper(headerRow, row, CsvCellNameMappingList);

            var newLineItem = new ParticipantWithSeriesPointsTallyDto
            {
                Position = xx.GetAsString(ParticipantWithSeriesPointsTallyDto.XePos),
                FullName = xx.GetAsString(ParticipantWithSeriesPointsTallyDto.XeFullName),
                Product = null,
                Plate = null,
                BibTag = null,
                DateOfBirthAsString = null,
                Age = null,
                Sex = null,
                Category = null,
                PointsTopNine = null,
                PointsOverall = null,
                R1 = null,
                R2 = null,
                R3 = null,
                R4 = null,
                R5 = null,
                R6 = null,
                R7 = null,
                R8 = null,
                R9 = null,
                R10 = null,
                R11 = null,
                R12 = null,
                Comment = null
            };

            var zz = ParticipantWithSeriesPointsTally.FromDataTransferObject(newLineItem);

            seriesParticipantsWithPoints.Add(zz);

        }

        fileItem.FileContentsAsText = string.Join(Environment.NewLine, rawInputAsRows);
        fileItem.FileContentsAsXElement = new XElement("dummy");

        return seriesParticipantsWithPoints;
    }

    #endregion

    #region variables

//private static XElement[] arrayOfRepeatingXe = [];

    private static readonly JghConsoleHelperV2 console = new();

    private static readonly JghListDictionary<string, ParticipantHubItem> participantsInPortalKeyedByBibDictionary = [];

    private static readonly JghListDictionary<string, ParticipantWithSeriesPointsTally> participantsWithPointsKeyedByBibDictionary = [];

    #endregion

    #region constants

    private const int LhsWidthTiny = 5;
    private const int LhsWidthSmall = 13;
    private const int LhsWidth = 30;

    private const string FilenameOfDiagnosticReport = @"ParticipantMasterListDiagnosticReport.txt";
    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\";

    //private const string FolderKelsoSeriesPointsFilesFromAndrew = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\ExportedAsCsv\";
    private const string FolderKelsoSeriesPointsFilesFromAndrew = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\ExportedToAccessAndThenToXml\";

    private const string FilenameOfParticipantMasterListExportedFromRezultzPortal = @"2024-06-16T12-33-30+Participants.json";

    //private const string FilenameOfParticipantMasterListExportedFromRezultzPortal = @"2024-06-12T14-43-39+Participants.json";
    private const string FolderContainingParticipantMasterListExportedFromRezultzPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";

    #endregion

    #region mappings from dirty source XElement Names to ParticipantWithSeriesPointsTallyDto DataMember names

    private static Dictionary<string, string> XElementNameMappingDictionary => new()
    {
        {
            "Participant", ParticipantWithSeriesPointsTallyDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Expert", ParticipantWithSeriesPointsTallyDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Intermediate", ParticipantWithSeriesPointsTallyDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Novice", ParticipantWithSeriesPointsTallyDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Sport", ParticipantWithSeriesPointsTallyDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.

        { "POS", ParticipantWithSeriesPointsTallyDto.XePos },
        { "Name", ParticipantWithSeriesPointsTallyDto.XeFullName },
        { "Product", ParticipantWithSeriesPointsTallyDto.XeProduct },
        { "PLATE", ParticipantWithSeriesPointsTallyDto.XePlate },
        { "BIBTAG", ParticipantWithSeriesPointsTallyDto.XeBibTag },
        { "Date_x0020_of_x0020_Birth", ParticipantWithSeriesPointsTallyDto.XeDateOfBirth },
        { "Age", ParticipantWithSeriesPointsTallyDto.XeAge },
        { "Sex", ParticipantWithSeriesPointsTallyDto.XeSex },
        { "Category", ParticipantWithSeriesPointsTallyDto.XeCategory },
        { "Top_x0020_9_x0020_Points", ParticipantWithSeriesPointsTallyDto.XePointsTopN },
        { "Total_x0020_Points", ParticipantWithSeriesPointsTallyDto.XePointsOverall },
        { "R1", ParticipantWithSeriesPointsTallyDto.XeR1 },
        { "R2", ParticipantWithSeriesPointsTallyDto.XeR2 },
        { "R3", ParticipantWithSeriesPointsTallyDto.XeR3 },
        { "R4", ParticipantWithSeriesPointsTallyDto.XeR4 },
        { "R5", ParticipantWithSeriesPointsTallyDto.XeR5 },
        { "R6", ParticipantWithSeriesPointsTallyDto.XeR6 },
        { "R7", ParticipantWithSeriesPointsTallyDto.XeR7 },
        { "R8", ParticipantWithSeriesPointsTallyDto.XeR8 },
        { "R9", ParticipantWithSeriesPointsTallyDto.XeR9 },
        { "R10", ParticipantWithSeriesPointsTallyDto.XeR10 },
        { "R11", ParticipantWithSeriesPointsTallyDto.XeR11 },
        { "R12", ParticipantWithSeriesPointsTallyDto.XeR12 }
    };

    private static KeyValuePair<string, string>[] XElementNameMappingList => new[]
    {
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant,
            "Participant"), // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything.
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Expert"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Intermediate"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Novice"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Sport"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePos, "POS"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeFullName, "Name"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeProduct, "Product"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePlate, "PLATE"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeBibTag, "BIBTAG"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeDateOfBirth, "Date_x0020_of_x0020_Birth"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeAge, "Age"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeSex, "Sex"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeCategory, "Category"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePointsTopN, "Top_x0020_9_x0020_Points"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePointsOverall, "Total_x0020_Points"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR1, "R1"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR2, "R2"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR3, "R3"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR4, "R4"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR5, "R5"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR6, "R6"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR7, "R7"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR8, "R8"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR9, "R9"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR10, "R10"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR11, "R11"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR12, "R12")
    };
    private static KeyValuePair<string, string>[] CsvCellNameMappingList => new[]
    {
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant,
            "Participant"), // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything.
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Expert"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Intermediate"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Novice"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeParticipant, "Sport"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePos, "POS"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeFullName, "Name"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeProduct, "Product"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePlate, "PLATE"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeBibTag, "BIBTAG"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeDateOfBirth, "Date_x0020_of_x0020_Birth"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeAge, "Age"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeSex, "Sex"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeCategory, "Category"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePointsTopN, "Top_x0020_9_x0020_Points"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XePointsOverall, "Total_x0020_Points"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR1, "R1"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR2, "R2"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR3, "R3"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR4, "R4"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR5, "R5"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR6, "R6"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR7, "R7"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR8, "R8"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR9, "R9"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR10, "R10"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR11, "R11"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsTallyDto.XeR12, "R12")
    };

    private static Dictionary<string, string> ValueMappingDictionary => new()
    {
        { "M", "m" },
        { "F", "f" },
        { "Expert", "expert" },
        { "Intermediate", "intermediate" },
        { "Novice", "novice" },
        { "Sport", "sport" }
    };

    #endregion

    #region helper methods

    private static void PrintSummaryOfFileEntries(FileInfo portalParticipantFileInfo, IEnumerable<ParticipantHubItem> participantHubItems)
    {
        var array = participantHubItems.ToArray();

        var countOfSeriesParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => z.IsSeries);

        var countOfOneOffParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => !z.IsSeries);

        console.WriteLine($"Loaded file: <{portalParticipantFileInfo.Name}>");
        console.WriteLine($"Series entries: {countOfSeriesParticipants}");
        console.WriteLine($"OneOff entries: {countOfOneOffParticipants}");
        //console.WriteLine($"Total entries: {countOfSeriesParticipants + countOfOneOffParticipants}");
    }

    private static void PrintSummaryOfFileEntries(FileInfo portalParticipantFileInfo, IEnumerable<ParticipantWithSeriesPointsTally> participantHubItems)
    {
        var array = participantHubItems.ToArray();

        var countOfSeriesParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => z.IsSeries);

        var countOfOneOffParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => !z.IsSeries);

        console.WriteLine($"Loaded file: <{portalParticipantFileInfo.Name}>");
        console.WriteLine($"Series entries: {countOfSeriesParticipants}");
        console.WriteLine($"OneOff entries: {countOfOneOffParticipants}");
        //console.WriteLine($"Total entries: {countOfSeriesParticipants + countOfOneOffParticipants}");
    }

    private static void PrintPeople(ParticipantWithSeriesPointsTally person)
    {
        console.WriteLine($"{JghString.LeftAlign(person.Bib, LhsWidthTiny)} {JghString.LeftAlign(person.RaceGroup, LhsWidthSmall)} {person.FullName} {person.Age} {person.Comment}");
    }

    private static void PrintPeople(IEnumerable<ParticipantWithSeriesPointsTally> people)
    {
        foreach (var person in people) PrintPeople(person);
    }

    private static void PrintPeople(ParticipantHubItem person)
    {
        console.WriteLine(person.RaceGroupBeforeTransition != person.RaceGroupAfterTransition
            ? $"{JghString.LeftAlign(person.Bib, LhsWidthTiny)} {JghString.LeftAlign(person.RaceGroupBeforeTransition, LhsWidthSmall)} {JghString.LeftAlign(person.RaceGroupAfterTransition, LhsWidthTiny)} {person.FirstName} {person.LastName} {person.BirthYear} {person.Comment}"
            : $"{JghString.LeftAlign(person.Bib, LhsWidthTiny)} {JghString.LeftAlign(person.RaceGroupBeforeTransition, LhsWidthSmall)} {person.FirstName} {person.LastName} {person.BirthYear} {person.Comment}");
    }

    private static void PrintPeople(IEnumerable<ParticipantHubItem> people)
    {
        foreach (var person in people) PrintPeople(person);
    }

    private static void SaveWorkToHardDrive(string text, string outPutFolder, string outPutFilename)
    {
        var pathOfFile = Path.Combine(outPutFolder, outPutFilename);

        File.WriteAllText(pathOfFile, text);

        Console.WriteLine($"{JghString.LeftAlign("File saved:", LhsWidth)} {outPutFilename}");
        Console.WriteLine($"{JghString.LeftAlign("Folder:", LhsWidth)} {outPutFolder}");
    }

    private static ParticipantWithSeriesPointsTally DeserialiseLineItemToParticipantWithSeriesPointsTallyV2(XElement repeatXe)
    {
        #region prettify all specified XElement values by cunning means of editing the document as plain text

        var xElementAsStringUndergoingMappingOfSubStrings = MapXElementValues(repeatXe.ToString(), ValueMappingDictionary);

        try
        {
            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            ParsePlainTextIntoXml(xElementAsStringUndergoingMappingOfSubStrings);
        }
        catch (Exception)
        {
            throw new Exception("Error. In the process of renaming XElement values, the format of the data was corrupted. One or more elements no longer parse back into Xml. This is a program error. Please contact the administrator.");
        }

        #endregion

        #region rename/map all specified XElement names in source by cunning means of editing the document as plain text

        xElementAsStringUndergoingMappingOfSubStrings = MapXElementNames(xElementAsStringUndergoingMappingOfSubStrings, XElementNameMappingDictionary);

        try
        {
            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            ParsePlainTextIntoXml(xElementAsStringUndergoingMappingOfSubStrings); // blow up?
        }
        catch (Exception)
        {
            throw new Exception("Error. In the process of reformatting XElement values, the format of the data was corrupted. One or more elements no longer parse back into Xml. This is a program error. Please contact the administrator.");
        }

        #endregion

        #region deserialise

        var remappedRepeatXe = ParsePlainTextIntoXml(xElementAsStringUndergoingMappingOfSubStrings);

        var dto = JghSerialisation.ToObjectFromXml<ParticipantWithSeriesPointsTallyDto>(remappedRepeatXe, [typeof(ParticipantWithSeriesPointsTallyDto)]);

        var item = ParticipantWithSeriesPointsTally.FromDataTransferObject(dto);

        #endregion

        return item;
    }

    private static ParticipantWithSeriesPointsTally DeserialiseLineItemToParticipantWithSeriesPointsTallyV3(XElement repeatXe)
    {
        #region prettify all specified XElement values by cunning means of editing the document as plain text

        var xElementAsStringUndergoingMappingOfSubStrings = MapXElementValues(repeatXe.ToString(), ValueMappingDictionary);

        XElement xElementUndergoingMappingOfSubStrings;

        try
        {
            // parse plain text document back into xml to verify the (potentially error-prone) renaming process

            xElementUndergoingMappingOfSubStrings = ParsePlainTextIntoXml(xElementAsStringUndergoingMappingOfSubStrings);
        }
        catch (Exception)
        {
            throw new Exception("Error. In the process of reformatting XElement values, the format of the data was corrupted. One or more elements no longer parse back into Xml. This is a program error. Please contact the administrator.");
        }

        #endregion

        #region to map all specified XElement names in source, instantiate my beautiful manual deserialiser

        var deserialiser = new JghMapper(xElementUndergoingMappingOfSubStrings, XElementNameMappingList);

        #endregion

        #region deserialise

        ParticipantWithSeriesPointsTallyDto dto = new()
        {
            Position = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XePos),
            FullName = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeFullName),
            Plate = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XePlate),
            BibTag = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeBibTag),
            DateOfBirthAsString = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeDateOfBirth),
            Age = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeAge),
            Sex = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeSex),
            Category = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeCategory),
            PointsTopNine = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XePointsTopN),
            PointsOverall = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XePointsOverall),
            R1 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R2 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R3 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R4 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R5 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R6 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R7 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R8 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R9 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R10 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R11 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            R12 = deserialiser.GetAsString(ParticipantWithSeriesPointsTallyDto.XeR1),
            Comment = string.Empty
        };
        var item = ParticipantWithSeriesPointsTally.FromDataTransferObject(dto);

        #endregion

        return item;
    }

    // ReSharper disable once UnusedMember.Local
    private static string MapXElementValues(string xmlFileContentsAsPlainText, Dictionary<string, string> mappingDictionary)
    {
        var failure = "Unable to map XElement values in accordance with mapping dictionary.";
        const string locus = "[MapXElementValues]";


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

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    private static string MapXElementNames(string xmlFileContentsAsPlainText, Dictionary<string, string> mappingDictionary)
    {
        var failure = "Unable to map XElement names from source data in accordance with mapping dictionary.";
        const string locus = "[MapXElementNames]";


        var provisionalCulpritOldXeName = "";
        var provisionalCulpritNewXeName = "";

        var scratchPadText = xmlFileContentsAsPlainText;

        try
        {
            if (string.IsNullOrWhiteSpace(xmlFileContentsAsPlainText))
                throw new ArgumentNullException(nameof(xmlFileContentsAsPlainText));

            foreach (var entry in mappingDictionary.Where(z => !string.IsNullOrWhiteSpace(z.Key)))
            {
                provisionalCulpritOldXeName = entry.Key;
                provisionalCulpritNewXeName = entry.Value;
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
                $"Problem arose whilst replacing occurrences of the field name {provisionalCulpritOldXeName} with {provisionalCulpritNewXeName}.",
                "Please inspect the data. There could be a programming error here.",
                "A typical programming error is that a specified equivalent contains a forbidden character.  Angle-brackets, for example, are prohibited in XML field names.");

            failure = JghString.ConcatAsSentences(failure, intro);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    private static XElement ParsePlainTextIntoXml(string inputText)
    {
        var failure = "Unable to parse text into xml.";
        const string locus = "[ParsePlainTextIntoXml]";

        try
        {
            if (inputText is null)
                throw new ArgumentNullException(nameof(inputText));

            return XElement.Parse(inputText); // automatically throws if invalid
        }

        #region try-catch

        catch (Exception ex)
        {
            failure = JghString.ConcatAsSentences(
                $"{failure} Text is not 100% correctly formatted.",
                "Even the tiniest error in format, syntax and/or content causes disqualification.");

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    #endregion
}