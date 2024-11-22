using System.Xml.Linq;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

// ReSharper disable InconsistentNaming

namespace Tool11;

internal class Program
{
    private const string Description =
        "This mickey-mouse program (Tool11) reads the Kelso participant master list/s and determines which participants have birthdays mid-season that would trigger a change in age-group if age-group was variable and based on actual age on the day of an event.";

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("Participant filename:", LhsWidth)} {FilenameOfMasterList}");
        console.WriteLine($"{JghString.LeftAlign("Participant folder:", LhsWidth)} {FolderContainingMasterListFromAndrew}");
        console.WriteLine($"{JghString.LeftAlign("Age groups filename:", LhsWidth)} {FilenameOfAgeGroupSpecifications}");
        console.WriteLine($"{JghString.LeftAlign("Age groups folder:", LhsWidth)} {FolderContainingAgeGroupSpecifications}");
        console.WriteLine($"{JghString.LeftAlign("Report filename:", LhsWidth)} {FilenameOfReport}");
        console.WriteLine($"{JghString.LeftAlign("Report folder:", LhsWidth)} {FolderForReport}");
        console.WriteLine($"{JghString.LeftAlign("Start of series", LhsWidth)} {StartDateOfSeries}");
        console.WriteLine($"{JghString.LeftAlign("End of series:", LhsWidth)} {EndDateOfSeries}");


        console.WriteLinePrecededByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingMasterListFromAndrew);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingMasterListFromAndrew);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingAgeGroupSpecifications);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderContainingAgeGroupSpecifications);
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

            #region confirm existence of input files

            var participantFileInfo = new FileInfo(FolderContainingMasterListFromAndrew + FilenameOfMasterList);

            if (!participantFileInfo.Exists)
            {
                console.WriteLine($"Failed to locate designated participant file. <{participantFileInfo.Name}>");

                return;
            }

            var ageGroupSpecificationsFileInfo = new FileInfo(FolderContainingAgeGroupSpecifications + FilenameOfAgeGroupSpecifications);

            if (!participantFileInfo.Exists)
            {
                console.WriteLine($"Failed to locate designated file od age group specifications. <{ageGroupSpecificationsFileInfo.Name}>");

                return;
            }

            #endregion

            #region read XML file into a list of "baby" participants

            try
            {
                var participantFileItem = new FileItem
                {
                    FileInfo = participantFileInfo,
                    FileContentsAsText = "",
                    FileContentsAsXElement = new XElement("dummy"),
                    OutputSubFolderName = string.Empty // not used
                };

                console.WriteLine();

                try
                {
                    var fullInputPath = participantFileItem.FileInfo.FullName;

                    var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);
                    var rawInputAsXElement = XElement.Parse(rawInputAsText);

                    participantFileItem.FileContentsAsText = rawInputAsText;
                    participantFileItem.FileContentsAsXElement = rawInputAsXElement;
                }
                catch (Exception e)
                {
                    console.WriteLine(e.Message);
                    throw new Exception(e.InnerException?.Message);
                }

                arrayOfRepeatingXe = participantFileItem.FileContentsAsXElement.Elements(NameOfRepeatingChildXElement).ToArray();

                if (arrayOfRepeatingXe.Length == 0)
                    throw new Exception($"Found not even a single repeating child XElement named <{NameOfRepeatingChildXElement}> in file <{participantFileItem.FileInfo.Name}>.");

                foreach (var repeatXe in arrayOfRepeatingXe)
                {
                    var baby = CreateBaby(repeatXe);

                    if (baby is null)
                        continue;

                    babyParticipants.Add(baby);
                }
            }
            catch (Exception e)
            {
                console.WriteLine($"Failed to successfully obtain participant info. {e.Message}");
                console.WriteLine("");
            }

            #endregion

            #region add dummy babies for testing

            //AddDummyBabiesForTesting(babyParticipants); // uncomment this for testing

            #endregion

            #region read JSON file into a list of agegroups

            try
            {
                var ageGroupFileInfo = new FileInfo(FolderContainingAgeGroupSpecifications + FilenameOfAgeGroupSpecifications);

                console.WriteLine();

                var fullInputPath = ageGroupFileInfo.FullName;

                var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);

                officialAgeGroupSpecifications = JghSerialisation.ToObjectFromJson<AgeGroupSpecificationDto[]>(rawInputAsText).ToArray();
            }
            catch (Exception e)
            {
                console.WriteLine($"Failed to successfully obtain age group specifications. {e.Message}");
                console.WriteLine("");
            }

            #endregion

            #region analyse babies

            foreach (var baby in babyParticipants.OfType<BabyParticipantDto>()) // eliminate nulls
                ProcessBaby(baby);

            #endregion

            #region report analysis

            PrintReport(babyParticipants.OfType<BabyParticipantDto>().ToList());

            #endregion

            #region wrap up

            console.WriteLinePrecededByOne("Summary:");
            console.WriteLinePrecededByOne($"{JghString.LeftAlign("Line items in participant master list:", LhsWidth)} {arrayOfRepeatingXe.Length}");
            console.WriteLine($"{JghString.LeftAlign("Total birthdays worthy of analysis:", LhsWidth)} {babyParticipants.Count}");
            console.WriteLine($"{JghString.LeftAlign("Birthdays before series:", LhsWidth)} {babyParticipants.Count(z => z is {IsPreSeriesBirthday: true})}");
            console.WriteLine($"{JghString.LeftAlign("Birthdays after series:", LhsWidth)} {babyParticipants.Count(z => z is {IsPostSeriesBirthday: true})}");
            console.WriteLine($"{JghString.LeftAlign("Birthdays during series:", LhsWidth)} {babyParticipants.Count(z => z is {IsMidSeriesBirthday: true})}");
            console.WriteLine($"{JghString.LeftAlign(" - unchanged age group:", LhsWidth)} {babyParticipants.Count(z => z is {IsMidSeriesBirthday: true} and {DoesSwitchAgeGroup: false})}");
            console.WriteLine($"{JghString.LeftAlign(" - changed age group:", LhsWidth)} {babyParticipants.Count(z => z is {IsMidSeriesBirthday: true} and {DoesSwitchAgeGroup: true})}");


            console.WriteLinePrecededByOne("Everything complete. No further action required.");
            console.WriteLineWrappedByOne("ooo0 - Goodbye - 0ooo");

            SaveWorkToHardDrive(console.ToString(), FolderForReport, FilenameOfReport);

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

    private static readonly List<BabyParticipantDto?> babyParticipants = [];

    private static AgeGroupSpecificationDto[] officialAgeGroupSpecifications = [];

    private static readonly JghConsoleHelperV2 console = new();

    #endregion

    #region constants

    private const int LhsWidth = 40;

    private static readonly DateOnly StartDateOfSeries = new(2024, 05, 14);

    private static readonly DateOnly EndDateOfSeries = new(2024, 08, 27);

    private const string NameOfRepeatingChildXElement = "Master_x0020_List";

    private const string FolderContainingMasterListFromAndrew = @"C:\Users\johng\holding pen\StuffFromAndrew\2024mtbparticipants\";
    private const string FilenameOfMasterList = @"2024-MTB-Racer-List-2024-05-10-Friday.xml";

    private const string FolderContainingAgeGroupSpecifications = FolderContainingMasterListFromAndrew;
    private const string FilenameOfAgeGroupSpecifications = @"AgeGroupSpecifications.json";

    private const string FolderForReport = FolderContainingMasterListFromAndrew;
    private const string FilenameOfReport = @"AgeGroupAnalysis.txt";

    #endregion

    #region helper methods

    private static void AddDummyBabiesForTesting(List<BabyParticipantDto?> babies)
    {
        babies.Add(new BabyParticipantDto
        {
            Bib = "999",
            FirstName = "John",
            LastName = "Doe-Turns-11",
            DateOfBirthAsString = "2014-07-01",
            IsSeries = true
        });

        babies.Add(new BabyParticipantDto
        {
            Bib = "998",
            FirstName = "John",
            LastName = "Doe-Turns-19",
            DateOfBirthAsString = "2006-07-01",
            IsSeries = true
        });

    }

    public static BabyParticipantDto? CreateBaby(XElement child)
    {
        #region Element names and symbols on the source Kelso masterlist in XML (src)

        const string SrcXeBib = "Plate_x0020__x0023_"; // the repeating element of the array
        const string SrcXeFirstName = "First_x0020_Name";
        const string SrcXeLastName = "Last_x0020_Name";
        const string SrcXeDateOfBirth = "Date_x0020_of_x0020_Birth";
        const string SrcXeCategory = "Category";

        const string SrcXeProduct = "Product";
        const string SrcValueBeginner = "Beginner";
        const string SrcValueKids = "Kids";
        const string SrcValueFullSeries = "Full Series";

        #endregion

        #region skip the ones from Andrew's masterlist we don't want included

        var candidateBib = JghString.TmLr(child.Elements(SrcXeBib).FirstOrDefault()?.Value);

        if (string.IsNullOrWhiteSpace(candidateBib))
            return null;

        var candidateRaceGroup = JghString.TmLr(child.Elements(SrcXeCategory).FirstOrDefault()?.Value);

        if (string.IsNullOrWhiteSpace(candidateRaceGroup))
            return null;

        if (candidateRaceGroup == JghString.TmLr(SrcValueBeginner))
            return null;

        if (candidateRaceGroup == JghString.TmLr(SrcValueKids))
            return null;

        #endregion

        #region new up a baby

        var baby = new BabyParticipantDto
        {
            Bib = candidateBib,
            FirstName = JghString.TmLr(child.Elements(SrcXeFirstName).FirstOrDefault()?.Value),
            LastName = JghString.TmLr(child.Elements(SrcXeLastName).FirstOrDefault()?.Value),
            DateOfBirthAsString = JghString.TmLr(child.Elements(SrcXeDateOfBirth).FirstOrDefault()?.Value)
        };

        #endregion

        #region fix IsSeries

        var product = JghString.TmLr(child.Elements(SrcXeProduct).FirstOrDefault()?.Value);

        baby.IsSeries = product.Contains(JghString.TmLr(SrcValueFullSeries));

        #endregion

        return baby;
    }

    private static void ProcessBaby(BabyParticipantDto baby)
    {
        GetBirthDateAndAssignIfIsPreMidOrPostSeriesBirthday(baby, StartDateOfSeries, EndDateOfSeries);

        if (baby.LastName == "DoeTurns11" || baby.LastName == "DoeTurns20")
        {
            var rubbish = baby.LastName;

        }


        if (baby is {IsPreSeriesBirthday: false, IsMidSeriesBirthday: false, IsPostSeriesBirthday: false})
            return; // exit if DOB is dirty

        AssignAgeGroupLabelsAtStartAndEndOfSeriesAndIfDoesSwitchAgeGroups(baby);
    }

    private static void AssignAgeGroupLabelsAtStartAndEndOfSeriesAndIfDoesSwitchAgeGroups(BabyParticipantDto baby)
    {
        var yearsOfAgeOfPersonAtStartOfSeries = 0;
        var yearsOfAgeOfPersonAtEndOfSeries = 0;

        if (baby.IsPreSeriesBirthday)
        {
            yearsOfAgeOfPersonAtStartOfSeries = StartDateOfSeries.Year - baby.DateOfBirth.Year;

            yearsOfAgeOfPersonAtEndOfSeries = yearsOfAgeOfPersonAtStartOfSeries;
        }

        if (baby.IsMidSeriesBirthday)
        {
            yearsOfAgeOfPersonAtStartOfSeries = StartDateOfSeries.Year - baby.DateOfBirth.Year;

            yearsOfAgeOfPersonAtEndOfSeries = yearsOfAgeOfPersonAtStartOfSeries + 1;
        }

        if (baby.IsPostSeriesBirthday)
        {
            yearsOfAgeOfPersonAtEndOfSeries = EndDateOfSeries.Year - baby.DateOfBirth.Year - 1;

            yearsOfAgeOfPersonAtStartOfSeries = yearsOfAgeOfPersonAtEndOfSeries;
        }

        var labelOfAgeGroupAtEndOfSeries = ToAgeCategoryDescriptionFromYearsOfAge(yearsOfAgeOfPersonAtEndOfSeries, officialAgeGroupSpecifications);

        var labelOfAgeGroupAtStartOfSeries = ToAgeCategoryDescriptionFromYearsOfAge(yearsOfAgeOfPersonAtStartOfSeries, officialAgeGroupSpecifications);

        if (JghString.AreEqualAndNeitherIsNullOrWhiteSpace(labelOfAgeGroupAtEndOfSeries, labelOfAgeGroupAtStartOfSeries))
        {
            baby.DoesSwitchAgeGroup = false;
            baby.AgeGroupAtStartOfSeries = labelOfAgeGroupAtStartOfSeries;
            baby.AgeGroupAtEndOfSeries = labelOfAgeGroupAtStartOfSeries;

            return;
        }

        baby.DoesSwitchAgeGroup = true;
        baby.AgeGroupAtStartOfSeries = labelOfAgeGroupAtStartOfSeries;
        baby.AgeGroupAtEndOfSeries = labelOfAgeGroupAtEndOfSeries;
        baby.Comment = $"=> {baby.AgeGroupAtEndOfSeries}";
    }

    private static void GetBirthDateAndAssignIfIsPreMidOrPostSeriesBirthday(BabyParticipantDto baby, DateOnly startOfSeries, DateOnly endOfSeries)
    {
        var didParse = DateOnly.TryParse(baby.DateOfBirthAsString, out var dateOfBirthOfBaby);

        if (!didParse)
        {
            baby.Comment = "Warning! Date of birth invalid";
            baby.IsPreSeriesBirthday = false;
            baby.IsMidSeriesBirthday = false;
            baby.IsPostSeriesBirthday = false;
        }

        baby.IsPreSeriesBirthday = IsPreSeriesBirthday(dateOfBirthOfBaby, startOfSeries);

        baby.IsMidSeriesBirthday = IsMidSeriesBirthday(dateOfBirthOfBaby, startOfSeries, endOfSeries);

        baby.IsPostSeriesBirthday = IsPostSeriesBirthday(dateOfBirthOfBaby, endOfSeries);

        baby.DateOfBirthAsString = dateOfBirthOfBaby.ToString(); // overwrite DateTime from master list to tidier DateOnly

        baby.DateOfBirth = dateOfBirthOfBaby;
    }

    private static bool IsPreSeriesBirthday(DateOnly birthday, DateOnly startOfSeries)
    {
        if (birthday.Month < startOfSeries.Month) return true;

        if (birthday.Month == startOfSeries.Month)
            if (birthday.Day <= startOfSeries.Day)
                return true;

        return false;
    }

    private static bool IsMidSeriesBirthday(DateOnly birthday, DateOnly startOfSeries, DateOnly endOfSeries)
    {
        if (IsPreSeriesBirthday(birthday, startOfSeries) || IsPostSeriesBirthday(birthday, endOfSeries))
            return false;

        return true;
    }

    private static bool IsPostSeriesBirthday(DateOnly birthday, DateOnly endOfSeries)
    {
        if (birthday.Month > endOfSeries.Month) return true;

        if (birthday.Month == endOfSeries.Month)
            if (birthday.Day >= endOfSeries.Day)
                return true;

        return false;
    }

    private static void PrintReport(List<BabyParticipantDto> babies)
    {
        var orderedBabies = babies.OrderBy(z => z.DateOfBirthAsString).ToArray();

        console.WriteLinePrecededByOne("The following people have birthdays during the series and with date-of-event age grouping they happen to go up an age group:");

        foreach (var baby in orderedBabies.Where(z => z.IsMidSeriesBirthday).Where(z => z.DoesSwitchAgeGroup).OrderBy(z => z.DateOfBirth.Month).ThenBy(z => z.DateOfBirth.DayNumber)) WriteBaby(baby);

        console.WriteLinePrecededByOne("The following people had birthdays during the series and with date-of-event age grouping they get older but will not change age group:");

        foreach (var baby in orderedBabies.Where(z => z.IsMidSeriesBirthday).Where(z => !z.DoesSwitchAgeGroup).OrderBy(z => z.DateOfBirth.Month).ThenBy(z => z.DateOfBirth.DayNumber)) WriteBaby(baby);

        console.WriteLinePrecededByOne("The following people had birthdays before the series and therefore did not change age groups:");

        foreach (var baby in orderedBabies.Where(z => z.IsPreSeriesBirthday).Where(z => !z.DoesSwitchAgeGroup).OrderBy(z => z.DateOfBirth.Month).ThenBy(z => z.DateOfBirth.DayNumber)) WriteBaby(baby);

        console.WriteLinePrecededByOne("The following people had birthdays after the series and therefore did not change age groups:");

        foreach (var baby in orderedBabies.Where(z => z.IsPostSeriesBirthday).Where(z => !z.DoesSwitchAgeGroup).OrderBy(z => z.DateOfBirth.Month).ThenBy(z => z.DateOfBirth.DayNumber)) WriteBaby(baby);

        void WriteBaby(BabyParticipantDto baby)
        {
            console.WriteLine($"{baby.DateOfBirthAsString} {baby.AgeGroupAtStartOfSeries} {baby.Comment} {baby.FirstName} {baby.LastName}");
        }
    }

    private static string ToAgeCategoryDescriptionFromYearsOfAge(int age, AgeGroupSpecificationDto[] ageGroupSpecifications)
    {
        if (!ageGroupSpecifications.Any())
            return string.Empty;

        var ageGroupSpecification = ageGroupSpecifications
            .Where(z => age >= z.AgeLowerBound)
            .FirstOrDefault(z => age <= z.AgeUpperBound);

        if (ageGroupSpecification is null || string.IsNullOrWhiteSpace(ageGroupSpecification.Label))
            return string.Empty;

        var answer = ageGroupSpecification.Label;

        return answer;
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