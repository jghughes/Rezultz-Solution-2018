using System.Xml.Linq;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

// ReSharper disable InconsistentNaming

namespace Tool12;

internal class Program
{
    private const string Description =
        "This console program (Tool12) reads four series points tallies files from Kelso and compares the consolidated list of participants in them with the series participants on the Rezultz portal.";

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Points tally files folder:", LhsWidth)} {FolderContainingPointsTallyFilesFromAndrew}");
        console.WriteLine($"{JghString.LeftAlign("Participant file from Portal:", LhsWidth)} {FilenameOfParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Participant file folder:", LhsWidth)} {FolderContainingParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Diagnostic Report filename:", LhsWidth)} {FilenameOfDiagnosticReport}");
        console.WriteLine($"{JghString.LeftAlign("Diagnostic Report folder:", LhsWidth)} {FolderForDiagnosticReport}");


        console.WriteLinePrecededByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingPointsTallyFilesFromAndrew);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingPointsTallyFilesFromAndrew);
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

            #region find all (four) files that contain participants with points tallies from Andrew

            var di = new DirectoryInfo(FolderContainingPointsTallyFilesFromAndrew); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLine($"No files found in designated folder: {di.FullName}");
                return;
            }

            #endregion

            #region read all the files into a dictionary of participants with points

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

                    try
                    {
                        var fullInputPath = fileItem.FileInfo.FullName;

                        var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);
                        var rawInputAsXElement = XElement.Parse(rawInputAsText);

                        fileItem.FileContentsAsText = rawInputAsText;
                        fileItem.FileContentsAsXElement = rawInputAsXElement;
                    }
                    catch (Exception e)
                    {
                        console.WriteLine(e.Message);
                        throw new Exception(e.InnerException?.Message);
                    }

                    var arrayOfRepeatingXe = fileItem.FileContentsAsXElement.Elements(NameOfRepeatingChildXElement).ToArray();

                    if (arrayOfRepeatingXe.Length == 0)
                        throw new Exception($"Found not even a single repeating child XElement named <{NameOfRepeatingChildXElement}> in file <{fileItem.FileInfo.Name}>.");

                    List<ParticipantWithSeriesPointsTally> seriesParticipantsWithPoints = [];

                    foreach (var repeatXe in arrayOfRepeatingXe)
                    {
                        var dto = JghSerialisation.ToObjectFromXml<ParticipantWithSeriesPointsTallyDto>(repeatXe, [typeof(ParticipantWithSeriesPointsTallyDto)]);

                        var item = ParticipantWithSeriesPointsTally.FromDataTransferObject(dto);

                        if (!string.IsNullOrWhiteSpace(item.Bib))
                            seriesParticipantsWithPoints.Add(item);
                    }

                    PrintSummaryOfFileEntries(fileInfo, seriesParticipantsWithPoints);

                    foreach (var participantWithPoints in seriesParticipantsWithPoints)
                        participantsWithPointsKeyedByBibDictionary.Add(participantWithPoints.Bib, participantWithPoints);


                    totalPeopleWithPoints += seriesParticipantsWithPoints.Count;
                }
                catch (Exception e)
                {
                    console.WriteLine($"Failed to successfully obtain people with points participant info. {e.Message}");
                    console.WriteLine("");
                }

            console.WriteLine($"Total entries with points: {totalPeopleWithPoints}");

            #endregion

            #region print one-off participants - works perfectly - commented out for now

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
                var people = participantsInPortalKeyedByBibDictionary[bib].Where(z => z != null).ToArray();

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
                var people = participantsWithPointsKeyedByBibDictionary[bib].Where(z => z != null).ToArray();
                var firstPerson = people.First();

                console.Write($"{JghString.LeftAlign(firstPerson.Bib, LhsWidthTiny)} {firstPerson.FullName} {firstPerson.Age} ");
                foreach (var person in people) console.Write($"/{person.RaceGroup}");

                console.WriteLine();
            }

            #endregion

            #region analyse names allocated more than once

            // portal

            JghListDictionary<string, ParticipantHubItem> participantsInPortalKeyedByNameDictionary = [];

            foreach (var person in participantsInPortalKeyedByBibDictionary.FindAllValues(z=>z.IsSeries))
            {
                string key = JghString.TmLr(JghString.Remove(' ', JghString.Concat(person.FirstName, person.LastName)));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsInPortalKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPortal = participantsInPortalKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"Names mistakenly appearing in multiple locations in the Portal: {namesAllocatedMoreThanOnceInPortal.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPortal)
            {
                PrintPeople(participantsInPortalKeyedByNameDictionary.FindAllValuesByKey(z=> z == key));
            }

            //points

            JghListDictionary<string, ParticipantWithSeriesPointsTally> participantsWithPointsKeyedByNameDictionary = [];

            foreach (var person in participantsWithPointsKeyedByBibDictionary.FindAllValues(z => z.IsSeries))
            {
                string key = JghString.TmLr(JghString.Remove(' ', person.FullName));

                if (!string.IsNullOrWhiteSpace(key))
                    participantsWithPointsKeyedByNameDictionary.Add(key, person);
            }

            var namesAllocatedMoreThanOnceInPoints = participantsWithPointsKeyedByNameDictionary
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            console.WriteLinePrecededByOne($"Names mistakenly appearing in multiple locations in Points - possibly across multiple categories (see above): {namesAllocatedMoreThanOnceInPoints.Length}");

            foreach (var key in namesAllocatedMoreThanOnceInPoints)
            {
                PrintPeople(participantsInPortalKeyedByNameDictionary.FindAllValuesByKey(z => z == key));
            }


            #endregion

            #region analyse names in both portal and points, but only those who have conflicting bibs


            var allNameKeysInPortal = participantsInPortalKeyedByNameDictionary.Keys.ToList();
            var allNameKeysInPoints = participantsWithPointsKeyedByNameDictionary.Keys.ToList();

            var distinctNameKeysInBothPortalAndPoints = allNameKeysInPortal.Intersect(allNameKeysInPoints).Distinct().ToArray();

            List<string> conflictedBibsInPortal = [];
            List<string> conflictedBibsInPoints = [];

            JghStringBuilder sb = new();

            int i = 0;

            foreach (var key in distinctNameKeysInBothPortalAndPoints)
            {
                var personInPortal = participantsInPortalKeyedByNameDictionary[key].FirstOrDefault();
                var personInPoints = participantsWithPointsKeyedByNameDictionary[key].FirstOrDefault();

                if ((personInPortal != null && personInPoints != null) && (personInPortal.Bib != personInPoints.Bib))
                {
                    sb.AppendLine($"Bib: Portal-Points={JghString.LeftAlign($"{personInPortal.Bib}-{personInPoints.Bib}", LhsWidthSmall)} {personInPoints.FullName}  RaceGroup: Portal/Points=({personInPortal.RaceGroupBeforeTransition}->{personInPortal.RaceGroupAfterTransition})/{personInPoints.RaceGroup}");

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

    private const int LhsWidth = 15;

    private const string NameOfRepeatingChildXElement = "Participant";

    private const string FilenameOfDiagnosticReport = @"ParticipantMasterListDiagnosticReport.txt";
    private const string FolderForDiagnosticReport = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbPointsTally\";


    private const string FilenameOfParticipantMasterListExportedFromRezultzPortal = @"2024-06-12T14-43-39+Participants.json";
    private const string FolderContainingParticipantMasterListExportedFromRezultzPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";

    private const string FolderContainingPointsTallyFilesFromAndrew = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbPointsTally\ExportedToAccessAndThenToXml\";

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

    #endregion
}