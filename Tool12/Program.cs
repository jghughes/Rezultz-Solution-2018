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
        console.WriteLine($"{JghString.LeftAlign("Series points folder:", LhsWidth)} {FolderContainingKelsoSeriesPointsFilesFromAndrewAsXml}");
        console.WriteLine($"{JghString.LeftAlign("Portal participants file:", LhsWidth)} {FilenameOfParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Portal participants folder:", LhsWidth)} {FolderContainingParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Diagnostic report filename:", LhsWidth)} {FilenameOfDiagnosticReport}");
        console.WriteLine($"{JghString.LeftAlign("MustProcessXmlNotCsvData:", LhsWidth)} ={MustProcessXmlNotCsvData}");
        console.WriteLine($"{JghString.LeftAlign("MustProcessXmlUsingSystemXmlSerializerNotJghMapper:", LhsWidth)} ={MustProcessXmlUsingSystemXmlSerializerNotJghMapper}");

        console.WriteLinePrecededByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingKelsoSeriesPointsFilesFromAndrewAsXml);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingKelsoSeriesPointsFilesFromAndrewAsXml);
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

            #region confirm existence of participantHubItems .json file from portal

            // NB: be sure that this file is totally up-to-date. changes are being made daily!

            var path = Path.Combine(FolderContainingParticipantMasterListExportedFromRezultzPortal, FilenameOfParticipantMasterListExportedFromRezultzPortal);

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

                PrintSummaryOfFileEntries(portalParticipantFileInfo, participantHubItems);

                foreach (var participantHubItem in participantHubItems)
                    if (!string.IsNullOrWhiteSpace(participantHubItem.Bib))
                        dictionaryOfParticipantsInPortalMasterListKeyedByBib.Add(participantHubItem.Bib, participantHubItem);
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            console.WriteLine();

            #endregion

            #region locate all the files in the designated folder for series points files from Andrew (can choose to designate either xml folder or csv folder)

            var di = new DirectoryInfo(MustProcessXmlNotCsvData ? FolderContainingKelsoSeriesPointsFilesFromAndrewAsXml : FolderContainingKelsoSeriesPointsFilesFromAndrewAsCsv); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLine($"No files found in designated folder: {di.FullName}");
                return;
            }

            #endregion

            #region read all the files in the designated folder for series points files from Andrew - whether xml or csv

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

                    List<ParticipantWithSeriesPoints> participantsWithSeriesPointsInKelsoSpreadsheet;

                    if (MustProcessXmlNotCsvData)
                        participantsWithSeriesPointsInKelsoSpreadsheet = await ProcessXmlFile(fullInputPath, fileItem);
                    else
                        participantsWithSeriesPointsInKelsoSpreadsheet = await ProcessCsvFile(fullInputPath, fileItem);

                    PrintSummaryOfFileEntries(fileInfo, participantsWithSeriesPointsInKelsoSpreadsheet);

                    foreach (var participantWithPoints in participantsWithSeriesPointsInKelsoSpreadsheet)
                        dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib.Add(participantWithPoints.Bib, participantWithPoints);


                    totalPeopleWithPoints += participantsWithSeriesPointsInKelsoSpreadsheet.Count;
                }
                catch (Exception e)
                {
                    var msg = e.Message;

                    var innerMsg = e.InnerException;

                    var message = $"Failed to successfully obtain people with points participant info. {msg} {innerMsg}";

                    console.WriteLine(message);

                    console.WriteLine("");

                    throw new Exception(message);

                    return;
                }

            console.WriteLine($"Total entries with points: {totalPeopleWithPoints}");

            #endregion

            #region print one-off participants entered in the Portal - works perfectly - commented out for now

            //var oneOffParticipantsInPortal = participantsInPortalKeyedByBibDictionary.FindAllValues(z => !z.IsSeries).ToArray();
            //var oneOffParticipantsWithPoints = participantsWithPointsKeyedByBibDictionary.FindAllValues(z => !z.IsSeries).ToArray(); // there shouldn't be any by definition

            //console.WriteLinePrecededByOne($"One-off participants in Portal: {oneOffParticipantsInPortal.Length}");
            //PrintPeople(oneOffParticipantsInPortal.OrderBy(z => z.RaceGroupBeforeTransition).ThenBy(z => z.Bib));

            //console.WriteLinePrecededByOne($"One-off participants in Points: {oneOffParticipantsWithPoints.Length}");
            //PrintPeople(oneOffParticipantsWithPoints.OrderBy(z => z.RaceGroup).ThenBy(z => z.Bib));

            //console.WriteLine();

            #endregion

            #region print defective entries in participants with points

            var defectiveEntriesInPoints = dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib.FindAllValues(z => z.IsDefective).OrderBy(z => z.RaceGroup).ThenBy(z => z.Bib).ToArray();

            console.WriteLinePrecededByOne($"DEFECTS in SERIESPOINTS spreadsheets: {defectiveEntriesInPoints.Length}");

            PrintPeople(defectiveEntriesInPoints);

            #endregion

            #region analyse bibs allocated more than once

            // portal

            var bibsAllocatedMoreThanOnceInPortal = dictionaryOfParticipantsInPortalMasterListKeyedByBib
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();


            console.WriteLinePrecededByOne($"BIBS in PORTAL in more than one category (regrades, or possible errors): {bibsAllocatedMoreThanOnceInPortal.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPortal)
            {
                var people = dictionaryOfParticipantsInPortalMasterListKeyedByBib[bib].Where(z => z is not null).ToArray();

                foreach (var person in people)
                    console.Write($"{JghString.LeftAlign(person.Bib, LhsWidthTiny)} {person.FirstName} {person.LastName} {person.BirthYear} ({person.RaceGroupBeforeTransition}->{person.RaceGroupAfterTransition})");
                console.WriteLine();
            }

            // points

            var bibsAllocatedMoreThanOnceInPoints = dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"BIBS in SERIESPOINTS spreadsheets in more than one category (regrades, or possible errors): {bibsAllocatedMoreThanOnceInPoints.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPoints)
            {
                var people = dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib[bib].Where(z => z is not null).ToArray();

                var j = 0;

                foreach (var person in people)
                {
                    switch (j)
                    {
                        case 0:
                            console.Write($"{JghString.LeftAlign(person.Bib, LhsWidthTiny)} {person.FullName} {person.Age} {person.RaceGroup}");
                            break;
                        case > 0:
                            console.Write($"/{person.RaceGroup}");
                            break;
                    }

                    j++;
                }

                console.WriteLine();
            }

            #endregion

            #region analyse names allocated more than once

            // portal

            JghListDictionary<string, ParticipantHubItem> participantsInPortalKeyedByNameDictionary = [];

            foreach (var person in dictionaryOfParticipantsInPortalMasterListKeyedByBib.FindAllValues(z => z.IsSeries))
            {
                var key = JghString.TmLr(JghString.Remove(' ', JghString.Concat(person.FirstName, person.LastName)));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsInPortalKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPortal = participantsInPortalKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"NAMES in PORTAL in more than one category (errors): {namesAllocatedMoreThanOnceInPortal.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPortal) PrintPeople(participantsInPortalKeyedByNameDictionary.FindAllValuesByKey(z => z == key));

            //points

            JghListDictionary<string, ParticipantWithSeriesPoints> participantsWithPointsKeyedByNameDictionary = [];

            foreach (var person in dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib.FindAllValues(z => z.IsSeries))
            {
                var key = JghString.TmLr(JghString.Remove(' ', person.FullName));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsWithPointsKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPoints = participantsWithPointsKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"NAMES in SERIESPOINTS spreadsheets in more than one category (regrades, or possible errors): {namesAllocatedMoreThanOnceInPoints.Length}");

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

            console.WriteLinePrecededByOne($"BIBS CONFLICTS between PORTAL and POINTS: {i}");
            console.WriteLine(sb.ToString());

            #endregion


            #region analyse names in both portal and points, but only those who have conflicting RaceGroups

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

            console.WriteLinePrecededByOne($"BIBS CONFLICTS between PORTAL and POINTS: {i}");
            console.WriteLine(sb.ToString());

            #endregion



            #region analyse 'missing' persons

            var seriesEntriesInPortal = dictionaryOfParticipantsInPortalMasterListKeyedByBib.FindAllValues(z => z.IsSeries).ToArray();
            var seriesEntriesWithPoints = dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib.FindAllValues(z => z.IsSeries).ToArray(); // tedious long way round. ha ha

            var allSeriesBibsInPortal = dictionaryOfParticipantsInPortalMasterListKeyedByBib.Keys.ToArray();
            var allSeriesBibsInPoints = dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib.Keys.ToArray();

            var missingBibsInPortal = allSeriesBibsInPoints.Except(allSeriesBibsInPortal).Except(conflictedBibsInPoints).ToList();
            var missingBibsInPoints = allSeriesBibsInPortal.Except(allSeriesBibsInPoints).Except(conflictedBibsInPortal).ToList();

            var peopleWithPointsWhoAreMissingInPortal = seriesEntriesWithPoints.Where(z => missingBibsInPortal.Contains(z.Bib)).Where(z => !z.IsDefective).ToArray();
            console.WriteLinePrecededByOne($"PORTAL MISSING people: {peopleWithPointsWhoAreMissingInPortal.Length}");
            PrintPeople(peopleWithPointsWhoAreMissingInPortal);

            var seriesParticipantsInPortalWhoAreMissingInPoints = seriesEntriesInPortal.Where(z => missingBibsInPoints.Contains(z.Bib)).ToArray();
            console.WriteLinePrecededByOne($"SERIESPOINTS spreadsheets MISSING people: {seriesParticipantsInPortalWhoAreMissingInPoints.Length}");
            PrintPeople(seriesParticipantsInPortalWhoAreMissingInPoints);

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
            var msg = ex.Message;

            var innerMsg = ex.InnerException;


            console.WriteLineFollowedByOne($"{msg} {innerMsg}");
            console.ReadLine();
        }
    }

    private static async Task<List<ParticipantWithSeriesPoints>> ProcessXmlFile(string fullInputPath, FileItem fileItem)
    {
        try
        {
            var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);

            fileItem.FileContentsAsText = rawInputAsText;

            try
            {
                var rawInputAsXElement = XElement.Parse(rawInputAsText);

                fileItem.FileContentsAsXElement = rawInputAsXElement;
            }
            catch (Exception ex)
            {
                var msg = $"{fileItem.FileInfo.Name} failed to parse successfully. A valid XML document is expected. Is this a XML document? ({ex.Message})";

                console.WriteLine(msg);

                throw new Exception(ex.Message);
            }
        }
        catch (Exception e)
        {
            console.WriteLine(e.Message);
            throw new Exception(e.InnerException?.Message);
        }

        var arrayOfRepeatingXe = fileItem.FileContentsAsXElement.Elements().ToArray();

        if (arrayOfRepeatingXe.Length == 0)
            throw new Exception($"Found not even a single repeating child XElement in file <{fileItem.FileInfo.Name}>.");

        List<ParticipantWithSeriesPoints> seriesParticipants = [];

        foreach (var repeatXe in arrayOfRepeatingXe)
        {
            ParticipantWithSeriesPoints item;

            item = MustProcessXmlUsingSystemXmlSerializerNotJghMapper
                ? DeserialiseXmlToParticipantUsingSystemXmlSerializer(repeatXe) // try out system deserialiser - not recommended
                : DeserialiseXmlToParticipantUsingJghMapper(repeatXe); // use custom mapper - strongly recommended because is more versatile and robust and less easily flummoxed

            seriesParticipants.Add(item);
        }

        return seriesParticipants;
    }

    private static async Task<List<ParticipantWithSeriesPoints>> ProcessCsvFile(string fullInputPath, FileItem fileItem)
    {
        var rawInputAsRows = await File.ReadAllLinesAsync(fullInputPath);

        fileItem.FileContentsAsText = string.Join(Environment.NewLine, rawInputAsRows);
        fileItem.FileContentsAsXElement = new XElement("dummy");

        var headerRow = rawInputAsRows[0];

        List<string> rowsOfRepeatingCsvData = [];

        for (var i = 1; i < rawInputAsRows.Length; i++) rowsOfRepeatingCsvData.Add(rawInputAsRows[i]);


        List<ParticipantWithSeriesPoints> seriesParticipants = [];

        foreach (var row in rowsOfRepeatingCsvData)
        {
            var xx = new JghMapper(headerRow, row, NewNameDirtyNameMapForJghMapper); // use our custom mapper for xml/csv

            var newLineItem = new ParticipantWithSeriesPointsDto
            {
                Position = xx.GetAsString(ParticipantWithSeriesPointsDto.XePos),
                FullName = xx.GetAsString(ParticipantWithSeriesPointsDto.XeFullName),
                Product = xx.GetAsString(ParticipantWithSeriesPointsDto.XeProduct),
                Plate = xx.GetAsString(ParticipantWithSeriesPointsDto.XePlate),
                BibTag = xx.GetAsString(ParticipantWithSeriesPointsDto.XeBibTag),
                DateOfBirthAsString = xx.GetAsString(ParticipantWithSeriesPointsDto.XeDateOfBirth),
                Age = xx.GetAsString(ParticipantWithSeriesPointsDto.XeAge),
                Sex = xx.GetAsString(ParticipantWithSeriesPointsDto.XeSex),
                Category = xx.GetAsString(ParticipantWithSeriesPointsDto.XeCategory),
                PointsTopNine = xx.GetAsString(ParticipantWithSeriesPointsDto.XePointsTopN),
                PointsOverall = xx.GetAsString(ParticipantWithSeriesPointsDto.XePointsOverall),
                R1 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
                R2 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR2),
                R3 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR3),
                R4 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR4),
                R5 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR5),
                R6 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR6),
                R7 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR7),
                R8 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR8),
                R9 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR9),
                R10 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR10),
                R11 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR11),
                R12 = xx.GetAsString(ParticipantWithSeriesPointsDto.XeR12),
                Comment = string.Empty
            };

            var zz = ParticipantWithSeriesPoints.FromDataTransferObject(newLineItem);

            seriesParticipants.Add(zz);
        }


        return seriesParticipants;
    }

    #endregion

    #region variables

    private static readonly JghConsoleHelperV2 console = new();

    private static readonly JghListDictionary<string, ParticipantHubItem> dictionaryOfParticipantsInPortalMasterListKeyedByBib = [];

    private static readonly JghListDictionary<string, ParticipantWithSeriesPoints> dictionaryOfParticipantsInKelsoSeriesPointsSpreadsheetsKeyedByBib = [];

    #endregion

    #region parameters

    private const int LhsWidthTiny = 5;
    private const int LhsWidthSmall = 13;
    private const int LhsWidth = 30;

    private const bool MustProcessXmlNotCsvData = true;

    private const bool MustProcessXmlUsingSystemXmlSerializerNotJghMapper = true;

    private const string FilenameOfDiagnosticReport = @"ParticipantMasterListDiagnosticReport.txt";

    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\";

    private const string FolderContainingKelsoSeriesPointsFilesFromAndrewAsCsv = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\ExportedAsCsv\"; // for csv data

    private const string FolderContainingKelsoSeriesPointsFilesFromAndrewAsXml = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\ExportedToAccessAndThenToXml\"; // for xml data

    private const string FilenameOfParticipantMasterListExportedFromRezultzPortal = @"2024-06-16T12-33-30+Participants.json";

    private const string FolderContainingParticipantMasterListExportedFromRezultzPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";

    #endregion

    #region mappings from dirty source XElement Names or csv column headers to specified new names

    private static Dictionary<string, string> DirtyNameNewNameMapForSystemSerializer => new()
    {
        {
            "Participant", ParticipantWithSeriesPointsDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Expert", ParticipantWithSeriesPointsDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Intermediate", ParticipantWithSeriesPointsDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Novice", ParticipantWithSeriesPointsDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Sport", ParticipantWithSeriesPointsDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.

        { "POS", ParticipantWithSeriesPointsDto.XePos },
        { "Name", ParticipantWithSeriesPointsDto.XeFullName },
        { "Product", ParticipantWithSeriesPointsDto.XeProduct },
        { "PLATE", ParticipantWithSeriesPointsDto.XePlate },
        { "BIBTAG", ParticipantWithSeriesPointsDto.XeBibTag },
        { "Date_x0020_of_x0020_Birth", ParticipantWithSeriesPointsDto.XeDateOfBirth },
        { "Age", ParticipantWithSeriesPointsDto.XeAge },
        { "Sex", ParticipantWithSeriesPointsDto.XeSex },
        { "Category", ParticipantWithSeriesPointsDto.XeCategory },
        { "Top_x0020_9_x0020_Points", ParticipantWithSeriesPointsDto.XePointsTopN },
        { "Total_x0020_Points", ParticipantWithSeriesPointsDto.XePointsOverall },
        { "R1", ParticipantWithSeriesPointsDto.XeR1 },
        { "R2", ParticipantWithSeriesPointsDto.XeR2 },
        { "R3", ParticipantWithSeriesPointsDto.XeR3 },
        { "R4", ParticipantWithSeriesPointsDto.XeR4 },
        { "R5", ParticipantWithSeriesPointsDto.XeR5 },
        { "R6", ParticipantWithSeriesPointsDto.XeR6 },
        { "R7", ParticipantWithSeriesPointsDto.XeR7 },
        { "R8", ParticipantWithSeriesPointsDto.XeR8 },
        { "R9", ParticipantWithSeriesPointsDto.XeR9 },
        { "R10", ParticipantWithSeriesPointsDto.XeR10 },
        { "R11", ParticipantWithSeriesPointsDto.XeR11 },
        { "R12", ParticipantWithSeriesPointsDto.XeR12 }
    };

    private static KeyValuePair<string, string>[] NewNameDirtyNameMapForJghMapper => new[]
    {
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeParticipant,
            "Participant"), // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything.
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeParticipant, "Expert"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeParticipant, "Intermediate"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeParticipant, "Novice"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeParticipant, "Sport"), // ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePos, "POS"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeFullName, "Name"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeFullName,
            "Visitor Name"), // note the duplicate XeFullName! a nice example of accommodating different worksheets from Andrew with different/inconsistent column headings
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeFullName, "Visitor_x0020_Name"), //ditto
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeProduct, "Product"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePlate, "PLATE"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePlate, "BIB"), // note the duplicate XePlate! a nice example of accommodating different worksheets from Andrew with different/inconsistent column headings
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeBibTag, "BIBTAG"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeDateOfBirth, "Date of Birth"), // Excel export version (csv heading). Caution: presence of the blanks will detonate system xml serializer but not JghMapper
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeDateOfBirth, "Date_x0020_of_x0020_Birth"), // Access export version (XElement name)
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeAge, "Age"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeSex, "Sex"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeCategory, "Category"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePointsTopN, "Top 9 Points"), // Excel export version (csv heading). Caution: presence of the blanks will detonate system xml serializer but not JghMapper
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePointsTopN, "Top_x0020_9_x0020_Points"), // Access export version (XElement name)
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePointsOverall, "Total Points"), // Excel export version (csv heading). Caution: presence of the blank will detonate system xml serializer but not JghMapper
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XePointsOverall, "Total_x0020_Points"), // Access export version (XElement name)
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR1, "R1"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR2, "R2"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR3, "R3"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR4, "R4"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR5, "R5"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR6, "R6"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR7, "R7"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR8, "R8"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR9, "R9"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR10, "R10"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR11, "R11"),
        new KeyValuePair<string, string>(ParticipantWithSeriesPointsDto.XeR12, "R12")
    };

    private static Dictionary<string, string> SymbolAndEnumMap => new()
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

        var countOfSeriesParticipants = array.Count(z => z.IsSeries);

        var countOfOneOffParticipants = array.Count(z => !z.IsSeries);
        //var countOfSeriesParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => z.IsSeries);
        //var countOfOneOffParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => !z.IsSeries);

        console.WriteLine($"Loaded file: <{portalParticipantFileInfo.Name}>");
        console.WriteLine($"Series entries: {countOfSeriesParticipants}");
        console.WriteLine($"OneOff entries: {countOfOneOffParticipants}");
        //console.WriteLine($"Total entries: {countOfSeriesParticipants + countOfOneOffParticipants}");
    }

    private static void PrintSummaryOfFileEntries(FileInfo portalParticipantFileInfo, IEnumerable<ParticipantWithSeriesPoints> participantHubItems)
    {
        var array = participantHubItems.ToArray();

        var countOfSeriesParticipants = array.Count(z => z.IsSeries);

        var countOfOneOffParticipants = array.Count(z => !z.IsSeries);

        //var countOfSeriesParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => z.IsSeries);

        //var countOfOneOffParticipants = array.Where(z => !string.IsNullOrWhiteSpace(z.Bib)).Count(z => !z.IsSeries);

        console.WriteLine($"Loaded file: <{portalParticipantFileInfo.Name}>");
        console.WriteLine($"Series entries: {countOfSeriesParticipants}");
        console.WriteLine($"OneOff entries: {countOfOneOffParticipants}");
        //console.WriteLine($"Total entries: {countOfSeriesParticipants + countOfOneOffParticipants}");
    }

    private static void PrintPeople(ParticipantWithSeriesPoints person)
    {
        console.WriteLine($"{JghString.LeftAlign(person.Bib, LhsWidthTiny)} {JghString.LeftAlign(person.RaceGroup, LhsWidthSmall)} {person.FullName} {person.Age} {person.Comment}");
    }

    private static void PrintPeople(IEnumerable<ParticipantWithSeriesPoints> people)
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

    private static ParticipantWithSeriesPoints DeserialiseXmlToParticipantUsingSystemXmlSerializer(XElement repeatXe)
    {
        string ReformatXElementNames(string xmlFileContentsAsPlainText, Dictionary<string, string> mappingDictionary)
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

        #region prettify all specified XElement values by cunning means of editing the document as plain text

        var xElementAsStringUndergoingMappingOfSubStrings = ReformatXmlSymbolsAndEnums(repeatXe.ToString(), SymbolAndEnumMap);

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

        xElementAsStringUndergoingMappingOfSubStrings = ReformatXElementNames(xElementAsStringUndergoingMappingOfSubStrings, DirtyNameNewNameMapForSystemSerializer);

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

        var dto = JghSerialisation.ToObjectFromXml<ParticipantWithSeriesPointsDto>(remappedRepeatXe, [typeof(ParticipantWithSeriesPointsDto)]);

        var item = ParticipantWithSeriesPoints.FromDataTransferObject(dto);

        #endregion

        return item;
    }

    private static ParticipantWithSeriesPoints DeserialiseXmlToParticipantUsingJghMapper(XElement repeatXe)
    {
        #region prettify all specified XElement symbols and enums by cunning means of editing the document as plain text

        var xElementAsStringUndergoingMappingOfSubStrings = ReformatXmlSymbolsAndEnums(repeatXe.ToString(), SymbolAndEnumMap);

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

        #region to map all specified XElement names in source, instantiate my beautiful manual mapper

        var mapper = new JghMapper(xElementUndergoingMappingOfSubStrings, NewNameDirtyNameMapForJghMapper);

        #endregion

        #region deserialise

        ParticipantWithSeriesPointsDto dto = new()
        {
            Position = mapper.GetAsString(ParticipantWithSeriesPointsDto.XePos),
            FullName = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeFullName),
            Plate = mapper.GetAsString(ParticipantWithSeriesPointsDto.XePlate),
            BibTag = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeBibTag),
            DateOfBirthAsString = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeDateOfBirth),
            Age = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeAge),
            Sex = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeSex),
            Category = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeCategory),
            PointsTopNine = mapper.GetAsString(ParticipantWithSeriesPointsDto.XePointsTopN),
            PointsOverall = mapper.GetAsString(ParticipantWithSeriesPointsDto.XePointsOverall),
            R1 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R2 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R3 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R4 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R5 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R6 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R7 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R8 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R9 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R10 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R11 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            R12 = mapper.GetAsString(ParticipantWithSeriesPointsDto.XeR1),
            Comment = string.Empty
        };
        var item = ParticipantWithSeriesPoints.FromDataTransferObject(dto);

        #endregion

        return item;
    }


    private static string ReformatXmlSymbolsAndEnums(string fileContentsAsPlainText, Dictionary<string, string> mappingDictionary)
    {
        var failure = "Unable to map XElement values in accordance with mapping dictionary.";
        const string locus = "[MapXElementSymbolsAndEnums]";


        var provisionalCulpritOldValue = "";
        var provisionalCulpritNewValue = "";

        var scratchPadText = fileContentsAsPlainText;

        try
        {
            if (string.IsNullOrWhiteSpace(fileContentsAsPlainText))
                throw new ArgumentNullException(nameof(fileContentsAsPlainText));

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