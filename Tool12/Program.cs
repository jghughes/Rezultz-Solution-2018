using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.SimpleIntervalRecorder.July2018;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

// ReSharper disable InconsistentNaming

namespace Tool12;

internal class Program
{
    private const string Description =
        "This console program (Tool12) is intended to indentify discrepencies between the Kelso SeriesPoints-participant lists in Andrew's four-worksheet " +
        "Excel spreadsheet and the ParticipantHubItems in the portal. To obtain the Kelso list, it reads (XML) files exported from Andrew's SERIES-POINTS " +
        "spreadsheet (exported by JGH into four worksheets in Access named Expert, Intermediate, Novice, and Sport and then exported as XML). To obtain an " +
        "up-to-date master list of the participants registered in the Portal, JGH exports the master list from the Particpant registration module as a Json file.";

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Date of determination of series category:", LhsWidth)} {DateOfDeterminationOfSeriesCategory:D}");
        console.WriteLine($"{JghString.LeftAlign("File of exported Portal ParticipantHubItems:", LhsWidth)} {FilenameOfParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("File of DiagnosticReport:", LhsWidth)} {FilenameOfDiagnosticReport}");
        console.WriteLine($"{JghString.LeftAlign("Folder for DiagnosticReport:", LhsWidth)} {FolderContainingDiagnosticReportDocument}");
        console.WriteLine($"{JghString.LeftAlign("Folder for exported Portal ParticipantHubItems:", LhsWidth)} {FolderContainingParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Folder for SeriesPoints files from Andrew:", LhsWidth)} {FolderContainingSeriesPointsFilesFromAndrewAsXml}");

        console.WriteLine($"{JghString.LeftAlign("MustProcessXmlNotCsvData:", LhsWidth)} {MustProcessXmlNotCsvData}");
        console.WriteLine($"{JghString.LeftAlign("UseSystemXmlSerializerNotJghDeserializer:", LhsWidth)} {AndrewsSeriesPointsWorkSheetDeserialiser.MustProcessXmlUsingSystemXmlSerializerNotJghDeserialiser}");

        console.WriteLinePrecededByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingSeriesPointsFilesFromAndrewAsXml);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingSeriesPointsFilesFromAndrewAsXml);
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
                Directory.SetCurrentDirectory(FolderContainingDiagnosticReportDocument);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingDiagnosticReportDocument);
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

            //console.WriteLine();

            #endregion

            #region locate all the files in the designated folder for SeriesPoints files from Andrew (can choose to designate either xml folder or csv folder)

            var di = new DirectoryInfo(MustProcessXmlNotCsvData ? FolderContainingSeriesPointsFilesFromAndrewAsXml : FolderContainingSeriesPointsFilesFromAndrewAsCsv); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLine($"No files found in designated folder: {di.FullName}");
                return;
            }

            #endregion

            #region deserialise the contents of the SeriesPoints files

            var totalPeopleWithPoints = 0;

            foreach (var fi in arrayOfInputFileInfo)
                try
                {
                    var fileItem = new FileItem
                    {
                        FileInfo = fi,
                        FileContentsAsText = "",
                        FileContentsAsXElement = new XElement("dummy"),
                        OutputSubFolderName = string.Empty // not used
                    };

                    var fullInputPath = fileItem.FileInfo.FullName;

                    string clockSummaryReport = string.Empty;

                    List<ParticipantOnAndrewsPointsSpreadsheet> participantsWithSeriesPointsIAndrewsSpreadsheet;

                    if (MustProcessXmlNotCsvData)
                    {
                        var clock = new IntervalsRecorder();

                        clock.BeginInterval("xml - pre-instantiate 3");

                        participantsWithSeriesPointsIAndrewsSpreadsheet = await AndrewsSeriesPointsWorkSheetDeserialiser.ProcessXmlFile(fullInputPath, fileItem);

                        clock.EndInterval();

                        clockSummaryReport = clock.SummaryOfResults;

                        clock.Reset();
                    }
                    else
                    {
                        var clock = new IntervalsRecorder();

                        clock.BeginInterval("csv - base case 3");

                        participantsWithSeriesPointsIAndrewsSpreadsheet = await AndrewsSeriesPointsWorkSheetDeserialiser.ProcessCsvFile(fullInputPath, fileItem);

                        clock.EndInterval();

                        clockSummaryReport = clock.SummaryOfResults;

                        clock.Reset();

                    }

                    PrintSummaryOfFileEntries(fi, participantsWithSeriesPointsIAndrewsSpreadsheet);

                    console.WriteLinePrecededByOne(clockSummaryReport);

                    foreach (var participantWithPoints in participantsWithSeriesPointsIAndrewsSpreadsheet)
                        dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib.Add(participantWithPoints.Bib, participantWithPoints);


                    totalPeopleWithPoints += participantsWithSeriesPointsIAndrewsSpreadsheet.Count;
                }
                catch (Exception e)
                {
                    var msg = e.Message;

                    var innerMsg = e.InnerException;

                    var message = $"Failed to successfully obtain people with points participant info. {msg} {innerMsg}";

                    console.WriteLine(message);

                    console.WriteLine("");

                    throw new Exception(message);
                }

            console.WriteLine($"{"Total rows:",78} {totalPeopleWithPoints}");

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

            var defectiveEntriesInPoints = dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib.FindAllValues(z => z.IsDefective).OrderBy(z => z.RaceGroup).ThenBy(z => z.Bib).ToArray();

            console.WriteLinePrecededByOne($"DEFECTS in SERIES-POINTS spreadsheets: {defectiveEntriesInPoints.Length}");

            PrintPeople(defectiveEntriesInPoints);

            #endregion

            #region analyse bibs allocated more than once

            // portal

            var bibsAllocatedMoreThanOnceInPortal = dictionaryOfParticipantsInPortalMasterListKeyedByBib
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();


            console.WriteLinePrecededByOne($"BIBS in PORTAL in more than one race category (regrades, or possible errors): {bibsAllocatedMoreThanOnceInPortal.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPortal)
            {
                var people = dictionaryOfParticipantsInPortalMasterListKeyedByBib[bib].Where(z => z is not null).ToArray();

                foreach (var person in people)
                    console.WriteLine($"Bib: {$"{person.Bib}",4} {$"{person.FirstName} {person.LastName}",-25} {"Year of birth:",-14} {person.BirthYear,4}  {person.RaceGroupBeforeTransition}=>{person.RaceGroupAfterTransition}");

                console.WriteLine();
            }

            // points

            var bibsAllocatedMoreThanOnceInPoints = dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"BIBS in SERIES-POINTS spreadsheets in more than one race category (regrades, or possible errors): {bibsAllocatedMoreThanOnceInPoints.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPoints)
            {
                var people = dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib[bib].Where(z => z is not null).ToArray();

                var j = 0;

                foreach (var person in people)
                {
                    switch (j)
                    {
                        case 0:
                            console.Write($"Bib: {$"{person.Bib}",4} {$"{person.FullName}",-25} {"Age:",-4} {person.Age,3} {person.RaceGroup}");
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

            JghListDictionary<string, ParticipantHubItem> participantsInPortalMaterListKeyedByNameDictionary = [];

            foreach (var person in dictionaryOfParticipantsInPortalMasterListKeyedByBib.FindAllValues(z => z.IsSeries))
            {
                var key = JghString.TmLr(JghString.Remove(' ', JghString.Concat(person.FirstName, person.LastName)));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsInPortalMaterListKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPortal = participantsInPortalMaterListKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"NAMES in PORTAL in more than one category (errors): {namesAllocatedMoreThanOnceInPortal.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPortal) PrintPeople(participantsInPortalMaterListKeyedByNameDictionary.FindAllValuesByKey(z => z == key));

            //points

            JghListDictionary<string, ParticipantOnAndrewsPointsSpreadsheet> participantsInKelsoSeriesPointsKeyedByNameDictionary = [];

            foreach (var person in dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib.FindAllValues(z => z.IsSeries))
            {
                var key = JghString.TmLr(JghString.Remove(' ', person.FullName));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsInKelsoSeriesPointsKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPoints = participantsInKelsoSeriesPointsKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"NAMES in SERIES-POINTS spreadsheets in more than one category (regrades, or possible errors): {namesAllocatedMoreThanOnceInPoints.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPoints) PrintPeople(participantsInPortalMaterListKeyedByNameDictionary.FindAllValuesByKey(z => z == key));

            #endregion

            #region analyse names in both portal and series-points, but only those who have conflicting bibs

            var allNameKeysInPortal = participantsInPortalMaterListKeyedByNameDictionary.Keys.ToList();
            var allNameKeysInPoints = participantsInKelsoSeriesPointsKeyedByNameDictionary.Keys.ToList();

            var distinctNameKeysInBothPortalAndPoints = allNameKeysInPortal.Intersect(allNameKeysInPoints).Distinct().ToArray();

            JghStringBuilder sb = new();

            var i = 0;

            foreach (var key in distinctNameKeysInBothPortalAndPoints)
            {
                var personInPortal = participantsInPortalMaterListKeyedByNameDictionary[key].FirstOrDefault();
                var personInPoints = participantsInKelsoSeriesPointsKeyedByNameDictionary[key].FirstOrDefault();

                if (personInPortal is not null && personInPoints is not null && personInPortal.Bib != personInPoints.Bib)
                {
                    sb.AppendLine(
                        $"Bib: Portal/Points {$"{personInPortal.Bib}/{personInPoints.Bib}",9}  {$"{personInPoints.FullName}",-25} {"Age:",-4} {personInPoints.Age,3} RaceGroup: Portal/Points ({personInPortal.RaceGroupBeforeTransition}=>{personInPortal.RaceGroupAfterTransition})/{personInPoints.RaceGroup}");
                    i += 1;
                }
            }

            console.WriteLinePrecededByOne($"BIBS CONFLICTS between PORTAL and SERIES-POINTS: {i}");
            console.WriteLine(sb.ToString());

            #endregion

            #region analyse names in both portal and series-points, but only those who have conflicting categories

            List<string> conflictedNamesInPortal = [];
            List<string> conflictedNamesInPoints = [];

            JghStringBuilder sb2 = new();

            var i2 = 0;

            foreach (var key in distinctNameKeysInBothPortalAndPoints)
            {
                var personInPortal = participantsInPortalMaterListKeyedByNameDictionary[key].FirstOrDefault();
                var personInPoints = participantsInKelsoSeriesPointsKeyedByNameDictionary[key].FirstOrDefault();

                var registeredRaceGroupInPortal = AndrewsSeriesPointsWorkSheetDeserialiser.FigureOutRaceGroup(ParticipantHubItem.ToDataTransferObject(personInPortal), DateOfDeterminationOfSeriesCategory);

                if (personInPortal is not null && personInPoints is not null && registeredRaceGroupInPortal != personInPoints.RaceGroup)
                {
                    sb2.AppendLine(
                        $"Bib: Portal/Points {$"{personInPortal.Bib}/{personInPoints.Bib}",9}  {$"{personInPoints.FullName}",-25} {"Age:",-4} {personInPoints.Age,3} RaceGroup: Portal/Points {registeredRaceGroupInPortal}/{personInPoints.RaceGroup}");

                    //sb2.AppendLine(
                    //    $"Bib: Portal/Points={JghString.LeftAlign($"{personInPortal.Bib}-{personInPoints.Bib}", LhsWidthSmall)} {personInPoints.FullName}  RaceGroup: RegisteredForSeriesInPortal/MyLaps=({registeredRaceGroupInPortal})/{personInPoints.RaceGroup}");

                    conflictedNamesInPortal.Add(personInPortal.Bib);
                    conflictedNamesInPoints.Add(personInPoints.Bib);
                    i2 += 1;
                }
            }

            console.WriteLinePrecededByOne($"CATEGORY CONFLICTS between registered series category (in Portal master list) and Andrew's series-points lists based on MyLaps: {i2}");
            console.WriteLine(sb2.ToString());

            #endregion

            #region analyse 'missing' persons

            var seriesEntriesInPortal = dictionaryOfParticipantsInPortalMasterListKeyedByBib.FindAllValues(z => z.IsSeries).ToArray();
            var seriesEntriesWithPoints = dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib.FindAllValues(z => z.IsSeries).ToArray(); // tedious long way round. ha ha

            var allSeriesBibsInPortal = dictionaryOfParticipantsInPortalMasterListKeyedByBib.Keys.ToArray();
            var allSeriesBibsInPoints = dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib.Keys.ToArray();

            var missingBibsInPortal = allSeriesBibsInPoints.Except(allSeriesBibsInPortal).Except(conflictedNamesInPoints).ToList();
            var missingBibsInPoints = allSeriesBibsInPortal.Except(allSeriesBibsInPoints).Except(conflictedNamesInPortal).ToList();

            var peopleWithPointsWhoAreMissingInPortal = seriesEntriesWithPoints.Where(z => missingBibsInPortal.Contains(z.Bib)).Where(z => !z.IsDefective).ToArray();
            console.WriteLinePrecededByOne($"PORTAL MISSING people: {peopleWithPointsWhoAreMissingInPortal.Length}");
            PrintPeople(peopleWithPointsWhoAreMissingInPortal);

            var seriesParticipantsInPortalWhoAreMissingInPoints = seriesEntriesInPortal.Where(z => missingBibsInPoints.Contains(z.Bib)).ToArray();
            console.WriteLinePrecededByOne($"SERIES-POINTS spreadsheets MISSING people: {seriesParticipantsInPortalWhoAreMissingInPoints.Length}");
            PrintPeople(seriesParticipantsInPortalWhoAreMissingInPoints);

            #endregion

            #region wrap up

            console.WriteLinePrecededByOne("Diagnostic report saved:");

            var prettyFileName = JghFilePathValidator.MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(FilenameOfDiagnosticReport);

            SaveWorkToHardDrive(console.ToString(), FolderContainingDiagnosticReportDocument, prettyFileName);

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

    #endregion

    #region variables

    private static readonly JghConsoleHelperV2 console = new();

    private static readonly JghListDictionary<string, ParticipantHubItem> dictionaryOfParticipantsInPortalMasterListKeyedByBib = [];

    private static readonly JghListDictionary<string, ParticipantOnAndrewsPointsSpreadsheet> dictionaryOfParticipantsInAndrewsSeriesPointsSpreadsheetKeyedByBib = [];

    #endregion

    #region parameters

    private const int LhsWidth = 50;

    private const bool MustProcessXmlNotCsvData = false;

    private static readonly DateTime DateOfDeterminationOfSeriesCategory = new(2024, 6, 18); // R5 June 18

    private const string FolderForCommonStuff = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\DiagnosticReport\";
    private const string FolderContainingDiagnosticReportDocument = FolderForCommonStuff;
    private const string FolderContainingParticipantMasterListExportedFromRezultzPortal = FolderForCommonStuff;

    private const string FilenameOfParticipantMasterListExportedFromRezultzPortal = @"2024-06-26T16-57-55+Participants.json";
    private const string FilenameOfDiagnosticReport = @"ParticipantMasterListDiagnosticReport.txt";

    private const string FolderContainingSeriesPointsFilesFromAndrewAsCsv = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\ExportedAsCsv\"; // for csv data
    private const string FolderContainingSeriesPointsFilesFromAndrewAsXml = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbSeriesPoints\ExportedToAccessAndThenToXml\"; // for xml data

    #endregion

    #region helper methods
    
    private static void PrintSummaryOfFileEntries(FileInfo fileInfo, IEnumerable<ParticipantHubItem> participants)
    {
        var array = participants.ToArray();

        var countOfSeriesParticipants = array.Count(z => z.IsSeries);

        var countOfOneOffParticipants = array.Count(z => !z.IsSeries);

        console.WriteLine(
            $"{"Loaded file:",-13} {fileInfo.Name,-40}   {"Series participants:",-21} {countOfSeriesParticipants,3}  {"One-off participants:",-15} {countOfOneOffParticipants,3}  {"Total participants:",-17} {countOfSeriesParticipants + countOfOneOffParticipants,3}");
    }

    private static void PrintSummaryOfFileEntries(FileInfo fileInfo, IEnumerable<ParticipantOnAndrewsPointsSpreadsheet> participants)
    {
        var array = participants.ToArray();

        var countOfSeriesParticipants = array.Count(z => z.IsSeries);

        var countOfOneOffParticipants = array.Count(z => !z.IsSeries);

        console.WriteLine($"{"Loaded file:",-13} {fileInfo.Name,-40}   {"Rows of participants:",-20} {countOfSeriesParticipants,3}");
        //console.WriteLine($"{"Loaded file:",-13} {fileInfo.Name,-40}   {"Series participants:",-19} {countOfSeriesParticipants,3} {"One-off participants:",-19} {countOfOneOffParticipants,3} {"Total participants:",-19} {countOfSeriesParticipants + countOfOneOffParticipants,3}");
    }

    private static void PrintPeople(ParticipantOnAndrewsPointsSpreadsheet person)
    {
        console.WriteLine($"Bib: {$"{person.Bib}",4}  {$"{person.FullName}",-25} {"Age:",-4} {person.Age,3}  {$"{person.RaceGroup}",-12} {"Comment:",-7} {person.Comment,-20}");
    }

    private static void PrintPeople(IEnumerable<ParticipantOnAndrewsPointsSpreadsheet> people)
    {
        foreach (var person in people) PrintPeople(person);
    }

    private static void PrintPeople(ParticipantHubItem person)
    {
        console.WriteLine(person.RaceGroupBeforeTransition != person.RaceGroupAfterTransition
            ? $"Bib: {$"{person.Bib}",4}  {$"{person.FirstName} {person.LastName}",-25} {"Birth:",-6} {person.BirthYear,4} {$"{person.RaceGroupBeforeTransition}/{person.RaceGroupAfterTransition}",-24} {"Comment:",-7} {person.Comment,-20}"
            : $"Bib: {$"{person.Bib}",4}  {$"{person.FirstName} {person.LastName}",-25} {"Birth:",-6} {person.BirthYear,4} {$"{person.RaceGroupBeforeTransition}",-12} {"Comment:",-7} {person.Comment,-20}");
    }

    private static void PrintPeople(IEnumerable<ParticipantHubItem> people)
    {
        foreach (var person in people) PrintPeople(person);
    }

    private static void SaveWorkToHardDrive(string text, string outPutFolder, string outPutFilename)
    {
        var pathOfFile = Path.Combine(outPutFolder, outPutFilename);

        File.WriteAllText(pathOfFile, text);

        Console.WriteLine($"{JghString.LeftAlign("File:", 8)} {outPutFilename}");
        Console.WriteLine($"{JghString.LeftAlign("Folder:", 8)} {outPutFolder}");
    }


    #endregion
}