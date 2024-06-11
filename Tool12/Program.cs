using System.Xml.Linq;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

// ReSharper disable InconsistentNaming

namespace Tool12;

internal class Program
{
    private const string Description =
        "This console program (Tool12) reads Andrew Haddow's four series points tallies files and compares the consolidated list of participants with series participants on the Rezultz portal.";

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Points tally files folder:", LhsWidth)} {FolderContainingPointsTallyFilesFromAndrew}");
        console.WriteLine($"{JghString.LeftAlign("Participant file from Portal:", LhsWidth)} {FilenameOfParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Participant file folder:", LhsWidth)} {FolderContainingParticipantMasterListExportedFromRezultzPortal}");
        console.WriteLine($"{JghString.LeftAlign("Report filename:", LhsWidth)} {FilenameOfReport}");
        console.WriteLine($"{JghString.LeftAlign("Report folder:", LhsWidth)} {FolderForReport}");


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
                Directory.SetCurrentDirectory(FolderForReport);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForReport);
                return;
            }

            #endregion

            #region confirm existence of participantHubItems file from portal

            var participantFileInfo = new FileInfo(Path.Combine(FolderContainingParticipantMasterListExportedFromRezultzPortal, FilenameOfParticipantMasterListExportedFromRezultzPortal));

            if (!participantFileInfo.Exists)
            {
                console.WriteLine($"Failed to locate designated participant file. <{participantFileInfo.Name}>");

                return;
            }

            #endregion

            #region deserialise participantHubItems into a Dictionary

            try
            {
                var rawInputAsText = await File.ReadAllTextAsync(participantFileInfo.FullName);

                var participantHubItemDtoArray = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto[]>(rawInputAsText);

                ParticipantHubItem[] participantHubItems = ParticipantHubItem.FromDataTransferObject(participantHubItemDtoArray);

                foreach (var participantHubItem in participantHubItems) participantsInPortalDict.Add(participantHubItem.Bib, participantHubItem);

                console.WriteLine($"Loaded ParticipantHubItems: {participantHubItems.Length} from {participantFileInfo.Name}");
            }
            catch (Exception ex)
            {
                console.WriteLine("Deserialization failure. ParticipantHubItems from portal hub not obtained: " + ex.Message);
                return;
            }

            #endregion

            #region find all (four) files that contain series participants with points tallies from Andrew

            var di = new DirectoryInfo(FolderContainingPointsTallyFilesFromAndrew); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            if (arrayOfInputFileInfo.Length == 0)
            {
                console.WriteLine($"No files found in designated folder: {di.FullName}");
                return;
            }

            #endregion

            #region read all the files into a dictionary of seriesParticipants

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

                    console.WriteLine();

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

                    arrayOfRepeatingXe = fileItem.FileContentsAsXElement.Elements(NameOfRepeatingChildXElement).ToArray();

                    if (arrayOfRepeatingXe.Length == 0)
                        throw new Exception($"Found not even a single repeating child XElement named <{NameOfRepeatingChildXElement}> in file <{fileItem.FileInfo.Name}>.");

                    List<ParticipantWithSeriesPointsTally> seriesParticipantsWithPoints = [];

                    foreach (var repeatXe in arrayOfRepeatingXe)
                    {
                        var dto = JghSerialisation.ToObjectFromXml<ParticipantWithSeriesPointsTallyDto>(repeatXe, [typeof(ParticipantWithSeriesPointsTallyDto)]);

                        var item = ParticipantWithSeriesPointsTally.FromDataTransferObject(dto);

                        seriesParticipantsWithPoints.Add(item);
                    }

                    foreach (var participantWithPoints in seriesParticipantsWithPoints) participantsWithPointsDict.Add(participantWithPoints.Bib, participantWithPoints);

                    console.WriteLine($"Loaded series participants from Andrew: {seriesParticipantsWithPoints.Count} participants from {arrayOfInputFileInfo.Length} files.");
                }
                catch (Exception e)
                {
                    console.WriteLine($"Failed to successfully obtain series participant info. {e.Message}");
                    console.WriteLine("");
                }

            #endregion

            #region print one-off participants

            var oneOffParticipantsInPortal = participantsInPortalDict.FindAllValues(z => !z.IsSeries).ToArray();
            var oneOffParticipantsWithPoints = participantsWithPointsDict.FindAllValues(z => !z.IsSeries).ToArray(); // there shouldn't be any by definition
            
            console.WriteLine($"One-off participants in Portal: {oneOffParticipantsInPortal.Length}");
            PrintParticipantHubItems(oneOffParticipantsInPortal.OrderBy(z => z.RaceGroupBeforeTransition).ThenBy(z => z.Bib));

            console.WriteLine($"One-off participants in Points: {oneOffParticipantsWithPoints.Length}");
            PrintParticipantsWithPoints(oneOffParticipantsWithPoints.OrderBy(z => z.RaceGroup).ThenBy(z => z.Bib));

            #endregion

            #region analyse bibs allocated more than once

            // portal

            var bibsAllocatedMoreThanOnceInPortal = participantsInPortalDict
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();

            var bibsAllocatedMoreThanOnceInPoints = participantsWithPointsDict
                .Where(z => z.Value.Count > 1)
                .Select(z => z.Key)
                .ToArray();


            console.WriteLine($"Bibs allocated more than once in the Portal: {bibsAllocatedMoreThanOnceInPortal.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPortal)
            {
                var people = participantsInPortalDict[bib].ToArray();
                PrintParticipantHubItems(people);
            }

            console.WriteLine($"Bibs allocated more than once in Points: {bibsAllocatedMoreThanOnceInPoints.Length}");

            foreach (var bib in bibsAllocatedMoreThanOnceInPoints)
            {
                var people = participantsWithPointsDict[bib].ToArray();
                PrintParticipantsWithPoints(people);
            }

            #endregion

            #region list of missing persons

            var seriesParticipantsInPortal = participantsInPortalDict.FindAllValues(z => z.IsSeries).ToArray();
            var participantsWithPoints = participantsWithPointsDict.FindAllValues(z => z != null).ToArray(); // tedious long way round. ha ha

            var allSeriesBibsInPortal = seriesParticipantsInPortal.Select(z=> z.Bib).Distinct().ToArray();
            var allBibsInPoints = participantsWithPointsDict.Keys.ToArray();

            console.WriteLine($"Participants in the portal (series only): {seriesParticipantsInPortal.Count()}");
            console.WriteLine($"Participants in the points master lists: {participantsWithPoints.Count()}");


            var missingBibsInPortal = allBibsInPoints.Except(allSeriesBibsInPortal).ToArray();
            var missingBibsInPoints = allSeriesBibsInPortal.Except(allBibsInPoints).ToArray();

            var peopleWithPointsWhoAreMissingInPortal = participantsWithPoints.Where(z => missingBibsInPortal.Contains(z.Bib)).ToArray();
            console.WriteLine($"Missing people in Portal: {peopleWithPointsWhoAreMissingInPortal.Length}");
            PrintParticipantsWithPoints(peopleWithPointsWhoAreMissingInPortal);

            var seriesParticipantsInPortalWhoAreMissingInPoints = seriesParticipantsInPortal.Where(z => missingBibsInPoints.Contains(z.Bib)).ToArray();
            console.WriteLine($"Missing people in Points: {seriesParticipantsInPortalWhoAreMissingInPoints.Length}");
            PrintParticipantHubItems(seriesParticipantsInPortalWhoAreMissingInPoints);

            #endregion

            #region wrap up

            //console.WriteLinePrecededByOne("Summary:");
            //console.WriteLinePrecededByOne($"{JghString.LeftAlign("Line items in participant master list:", LhsWidth)} {arrayOfRepeatingXe.Length}");
            //console.WriteLine($"{JghString.LeftAlign("Total birthdays worthy of analysis:", LhsWidth)} {seriesParticipantsWithPoints.Count}");
            //console.WriteLine($"{JghString.LeftAlign("Birthdays before series:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsPreSeriesBirthday: true })}");
            //console.WriteLine($"{JghString.LeftAlign("Birthdays after series:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsPostSeriesBirthday: true })}");
            //console.WriteLine($"{JghString.LeftAlign("Birthdays during series:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsMidSeriesBirthday: true })}");
            //console.WriteLine($"{JghString.LeftAlign(" - unchanged age group:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsMidSeriesBirthday: true } and { DoesSwitchAgeGroup: false })}");
            //console.WriteLine($"{JghString.LeftAlign(" - changed age group:", LhsWidth)} {seriesParticipantsWithPoints.Count(z => z is { IsMidSeriesBirthday: true } and { DoesSwitchAgeGroup: true })}");

            SaveWorkToHardDrive(console.ToString(), FolderForReport, FilenameOfReport);

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

    private static XElement[] arrayOfRepeatingXe = [];

    private static readonly JghListDictionary<string, ParticipantHubItem> participantsInPortalDict = [];

    private static readonly JghListDictionary<string, ParticipantWithSeriesPointsTally> participantsWithPointsDict = [];


    private static readonly JghConsoleHelperV2 console = new();

    #endregion

    #region constants

    private const int LhsWidth = 40;

    private const string NameOfRepeatingChildXElement = "Participant";

    private const string FolderContainingPointsTallyFilesFromAndrew = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbPointsTally\ImportedByAccessToXml\";

    private const string FolderContainingParticipantMasterListExportedFromRezultzPortal = @"C:\Users\johng\holding pen\StuffByJohn\ParticipantsFromPortal\";
    private const string FilenameOfParticipantMasterListExportedFromRezultzPortal = @"2024-06-09T10-11-10+ParticipantMasterListExportedFromRezultzPortal.json";

    private const string FolderForReport = @"C:\Users\johng\holding pen\StuffByJohn\";
    private const string FilenameOfReport = @"ParticipantMasterListAnalysis.txt";

    #endregion

    #region helper methods

    private static void PrintParticipantWithPoints(ParticipantWithSeriesPointsTally participantWithPoints)
    {
        console.WriteLine($"{participantWithPoints.Bib} {participantWithPoints.FullName} {participantWithPoints.RaceGroup}");
    }

    private static void PrintParticipantsWithPoints(IEnumerable<ParticipantWithSeriesPointsTally> participantsWithPoints)
    {
        foreach (var participantWithPoints in participantsWithPoints) PrintParticipantWithPoints(participantWithPoints);
    }

    private static void PrintParticipantHubItem(ParticipantHubItem person)
    {
        console.WriteLine($"{person.Bib} {person.FirstName} {person.LastName} {person.RaceGroupBeforeTransition} {person.RaceGroupAfterTransition}");
    }

    private static void PrintParticipantHubItems(IEnumerable<ParticipantHubItem> participantHubItems)
    {
        foreach (var participantHubItem in participantHubItems) PrintParticipantHubItem(participantHubItem);
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