using System.Text;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.OnBoardServices01.July2018.Persistence;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using RezultzSvc.Agents.Mar2024.SvcAgents;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService;
// ReSharper disable NotAccessedVariable

// ReSharper disable InconsistentNaming

namespace Tool10;

internal class Program
{
    private const string Description = "This console program (Tool09) reads Andrew's participant master list/s and generates ParticipantHubItems.";

    private static SeasonProfileItem? seasonProfileItem;

    private static SeriesProfileItem? seriesProfileItem;


    #region the MEAT

    private static async Task Main()
    {
        #region intro

        JghConsoleHelper.WriteLineFollowedByOne("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for Andrew's participant master list XML file/s", LhsWidth)} : {FolderContainingMasterListFromAndrew}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for BabyParticipants", LhsWidth)} : {FolderForBabyParticipants}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for OriginatingParticipantHubItems", LhsWidth)} : {FolderForOriginatingParticipantHubItems}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for ModifiedParticipantHubItems", LhsWidth)} : {FolderForModifiedParticipantHubItems}");
        JghConsoleHelper.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineFollowedByOne("Working. Please wait...");

        #endregion

        try
        {
            #region first things first - test if our svc agents are working - comment/uncomment out those you do or don't want at any point

            //JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - AzureStorageSvcAgent...");
            //var answer1 = await AzureStorageSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer1}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - ParticipantRegistrationSvcAgent...");
            //var answer2 = await ParticipantRegistrationSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer2}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();


            //JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - LeaderboardResultsSvcAgent...");
            //var answer3 = await LeaderboardResultsSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer3}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();


            //JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - TimeKeepingSvcAgent...");
            //var answer4 = await TimeKeepingSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer4}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - RaceResultsPublishingSvcAgent...");
            //var answer5 = await RaceResultsPublishingSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer5}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            #endregion

            #region download SeriesProfileItem from Azure to obtain relevant info

            string seriesLabel;
            string eventLabel;
            string accountName;
            string containerName;

            try
            {
                JghConsoleHelper.WriteLineFollowedByOne("Please wait. Fetching SeasonProfile('999')");

                seasonProfileItem = await LeaderboardResultsSvcAgent.GetSeasonProfileAsync("999");
                JghConsoleHelper.WriteLineFollowedByOne($"Profile obtained = <{seasonProfileItem.Label}>");

                seriesProfileItem = seasonProfileItem.SeriesProfiles.LastOrDefault();

                seriesLabel = seriesProfileItem?.Label ?? string.Empty;
                eventLabel = seriesProfileItem?.EventProfileItems.Last()?.Label ?? string.Empty; // i.e. the earliest 
                accountName = seriesProfileItem?.ContainerForParticipantHubItemData?.AccountName ?? string.Empty;
                containerName = seriesProfileItem?.ContainerForParticipantHubItemData?.ContainerName ?? string.Empty;
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLineFollowedByOne($"Oops. Failed to locate (or de-serialise) the designated season profile file. {e.Message}");

                return;
            }

            #endregion

            #region confirm existence of input and output folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderContainingMasterListFromAndrew);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + FolderContainingMasterListFromAndrew);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForBabyParticipants);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + FolderForBabyParticipants);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForOriginatingParticipantHubItems);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + FolderForOriginatingParticipantHubItems);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForModifiedParticipantHubItems);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + FolderForModifiedParticipantHubItems);
                return;
            }

            #endregion

            #region find all files that exist in input folder

            var di = new DirectoryInfo(FolderContainingMasterListFromAndrew); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            #endregion

            #region select XML files only, read them, and load their contents into one or more populated FileItems

            JghConsoleHelper.WriteLineWrappedByOne("Please wait. Translating file/s into XDocument/s...");

            try
            {
                foreach (FileInfo fi in arrayOfInputFileInfo)
                {
                    if (!Path.GetFileName(fi.FullName).EndsWith($".{RequiredInputFileFormat}"))
                    {
                        JghConsoleHelper.WriteLineFollowedByOne($"Skipping <{fi.Name}> because it's not a <{RequiredInputFileFormat}> file.");
                        continue;
                    }

                    JghConsoleHelper.WriteLine($"Found an XML file called <{fi.Name}>. Initialising a FileItem for this file.");

                    var fileItem = new FileItem
                    {
                        FileInfo = fi,
                        FileContentsAsText = "",
                        FileContentsAsXElement = new XElement(BabyParticipantDto.XeDataRootForContainerOfSimpleStandAloneArray),
                        OutputSubFolderName = string.Empty // not used
                    };

                    FilesOfParticipantListsImportedFromAndrew.Add(fileItem);

                    JghConsoleHelper.WriteLineFollowedByOne($"FileItem successfully initialised.");

                }

                JghConsoleHelper.WriteLine();

                try
                {
                    foreach (var fileItem in FilesOfParticipantListsImportedFromAndrew)
                    {
                        JghConsoleHelper.WriteLine($"Parsing <{fileItem.FileInfo.Name}> into XML....");

                        var fullInputPath = fileItem.FileInfo.FullName;

                        var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);
                        var rawInputAsXElement = XElement.Parse(rawInputAsText);

                        fileItem.FileContentsAsText = rawInputAsText;
                        fileItem.FileContentsAsXElement = rawInputAsXElement;

                        JghConsoleHelper.WriteLine($"Successfully parsed and loaded <{fileItem.FileInfo.Name}>");
                    }
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLine(e.Message);
                    throw new Exception(e.InnerException?.Message);
                }

                if (FilesOfParticipantListsImportedFromAndrew.Count == 0)
                    throw new Exception("Found not even a single participant data file from Andrew.");
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine($"Failed to locate designated participant file. {e.Message}");
                JghConsoleHelper.WriteLine("");
                return;
            }

            JghConsoleHelper.WriteLineFollowedByOne($"{FilesOfParticipantListsImportedFromAndrew.Count} parent XElement/s successfully created after reading participant data file/s from Andrew.");

            #endregion

            #region foreach FileItem, convert the xml contents into a list of BabyParticipantDto, consolidate the lists from all FileItems

            JghConsoleHelper.WriteLinePrecededByOne("Please wait. Translating XElement/s into consolidated master list of all BabyParticipants..");

            List<BabyParticipantDto> babyParticipants = new();

            foreach (var fileItem in FilesOfParticipantListsImportedFromAndrew)
            {
                var arrayOfRepeatingXe = fileItem.FileContentsAsXElement.Elements(NameOfRepeatingChildXElement).ToArray();

                if (arrayOfRepeatingXe.Length == 0)
                    throw new Exception($"Found not even a single repeating child XElement named <{NameOfRepeatingChildXElement}> in file <{fileItem.FileInfo.Name}>.");

                foreach (var repeatXe in arrayOfRepeatingXe)
                {
                    var baby = CreateBaby(repeatXe);

                    if (baby is null)
                        continue;

                    if (OneOrMoreEntriesAreInvalid(baby, out var errorMessage))
                        throw new JghAlertMessageException(errorMessage);

                    babyParticipants.Add(baby);
                }
            }

            JghConsoleHelper.WriteLine($"{babyParticipants.Count} BabyParticipants created.");

            #endregion

            #region save the consolidated file of all babies

            var arrayOfBabiesAsXmlText = JghSerialisation.ToXmlFromObject(babyParticipants.ToArray(), new[] {typeof(BabyParticipantDto)});

            SaveWorkToHardDriveAsXml(arrayOfBabiesAsXmlText, FolderForBabyParticipants, FilenameForBabyParticipants);

            JghConsoleHelper.WriteLine($"{JghString.LeftAlign("BabyParticipants successfully saved: ", LhsWidth)} : {FolderForBabyParticipants} {FilenameForBabyParticipants} {babyParticipants.Count} babies saved.");

            #endregion

            #region translate all BabyParticipantDto into originating ParticpantHubItem and modified ParticpantHubItem

            JghConsoleHelper.WriteLinePrecededByOne("Please wait. Translating Babies into hubItems...");

            List<ParticipantHubItem> originatingParticipantHubItems = new();

            List<ParticipantHubItem> modifiedParticipantHubItems = new();

            var i = 0;

            foreach (var babyParticipant in babyParticipants)
            {
                var originatingIdentifier = babyParticipant.Identifier;

                if (string.IsNullOrWhiteSpace(originatingIdentifier))
                    originatingIdentifier = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());

                if (!JghString.IsOnlyLettersOrDigitsOrHyphen(originatingIdentifier))
                {
                    originatingIdentifier = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());

                    JghConsoleHelper.WriteLine(
                        $"Error: Identifier <{originatingIdentifier}> is malformed for <{babyParticipant.FirstName} {babyParticipant.LastName}> Applied default instead <{originatingIdentifier}>. (ID must consist of letters, digits, or hyphens, or be blank))");
                }

                var originatingHubItem = ParticipantHubItem.Create(i, babyParticipant.Identifier, EnumStrings.KindOfEntryIsParticipantEntry, "jgh");

                originatingParticipantHubItems.Add(originatingHubItem);

                var didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(originatingHubItem, out var errorMessage1);

                if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage1);

                var modification = MergeEditsBackIntoItemBeingModified(originatingHubItem, babyParticipant, "jgh");

                modifiedParticipantHubItems.Add(modification);

                didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(modification, out var errorMessage2);

                if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage2);

                i++;
            }

            JghConsoleHelper.WriteLine("HubItems created.");

            #endregion

            #region save consolidated files of both

            var originatingParticipantHubItemAsXmlText = JghSerialisation.ToXmlFromObject(originatingParticipantHubItems.ToArray(), new[] {typeof(ParticipantHubItem)});

            SaveWorkToHardDriveAsXml(originatingParticipantHubItemAsXmlText, FolderForOriginatingParticipantHubItems, FilenameForOriginatingParticipantHubItems);

            JghConsoleHelper.WriteLine(
                $"{JghString.LeftAlign("Originating hub items successfully saved: ", LhsWidth)} : {FolderForOriginatingParticipantHubItems} {FilenameForOriginatingParticipantHubItems} {originatingParticipantHubItems.Count} items saved.");

            var modifiedParticipantHubItemAsXmlText = JghSerialisation.ToXmlFromObject(modifiedParticipantHubItems.ToArray(), new[] {typeof(ParticipantHubItem)});

            SaveWorkToHardDriveAsXml(modifiedParticipantHubItemAsXmlText, FolderForModifiedParticipantHubItems, FileNameForModifiedParticipantHubItems);

            JghConsoleHelper.WriteLine(
                $"{JghString.LeftAlign("Modified hub items successfully saved: ", LhsWidth)} : {FolderForModifiedParticipantHubItems} {FileNameForModifiedParticipantHubItems} {modifiedParticipantHubItems.Count} items saved.");

            #endregion

            #region wrap up

            JghConsoleHelper.WriteLineWrappedByTwo("Deletions complete. No further action required. Goodbye.");
            JghConsoleHelper.WriteLine("ooo0 - Goodbye - 0ooo");
            Console.ReadLine();

            #endregion
        }
        catch (Exception ex)
        {
            JghConsoleHelper.WriteLineFollowedByOne(ex.ToString());
            JghConsoleHelper.ReadLine();
        }
    }

    #endregion

    #region constants

    private const int LhsWidth = 50;
    private const string RequiredInputFileFormat = "xml";
    private const string NameOfRepeatingChildXElement = "Master_x0020_List";

    private const string FolderContainingMasterListFromAndrew = @"C:\Users\johng\holding pen\participants-from-Andrew\";
    private const string FolderForBabyParticipants = @"C:\Users\johng\holding pen\participants-BabyParticipants\";
    private const string FolderForOriginatingParticipantHubItems = @"C:\Users\johng\holding pen\participants-originating-ParticipantHubItems\";
    private const string FolderForModifiedParticipantHubItems = @"C:\Users\johng\holding pen\participants-modified-ParticipantHubItems\";

    private const string FilenameForBabyParticipants = @"Babies.xml";
    private const string FilenameForOriginatingParticipantHubItems = @"OriginatingParticipantHubItems.xml";
    private const string FileNameForModifiedParticipantHubItems = @"ModifiedParticipantHubItems.xml";

    #endregion

    #region variables

    private static readonly List<FileItem> FilesOfParticipantListsImportedFromAndrew = new();

    public static readonly IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem> RepositoryOfHubStyleEntries = new RepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem>(new IsolatedStorageService());

    #endregion

    #region svc's available for use

    // ReSharper disable once UnusedMember.Local
    private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientMvc());
    private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientMvc());
    private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientMvc());
    //private static readonly TimeKeepingSvcAgent TimeKeepingSvcAgent = new(new TimeKeepingServiceClientMvc());
    //private static readonly RaceResultsPublishingSvcAgent RaceResultsPublishingSvcAgent = new(new RaceResultsPublishingServiceClientMvc());

    //private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientWcf());
    //private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientWcf());
    //private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientWcf());
    //private static readonly TimeKeepingSvcAgent TimeKeepingSvcAgent = new(new TimeKeepingServiceClientWcf());
    //private static readonly RaceResultsPublishingSvcAgent RaceResultsPublishingSvcAgent = new(new RaceResultsPublishingServiceClientWcf());

    #endregion

    #region helper methods

    public static BabyParticipantDto? CreateBaby(XElement child)
    {
        const string XeIdentifier = "Plate_x0020__x0023_"; // the repeating element of the array
        const string XeFirstName = "First_x0020_Name";
        const string XeLastName = "Last_x0020_Name";
        const string XeGender = "Sex";
        const string XeBirthYear = "Date_x0020_of_x0020_Birth";
        const string XeCity = "city";
        const string XeRaceGroupBeforeTransition = "Category";

        var candidateIdentifier = JghString.TmLr(child.Elements(XeIdentifier).First().Value);

        if (string.IsNullOrWhiteSpace(candidateIdentifier))
            return null;

        var baby = new BabyParticipantDto
        {
            Identifier = candidateIdentifier,
            FirstName = JghString.TmLr(child.Elements(XeFirstName).FirstOrDefault()?.Value),
            LastName = JghString.TmLr(child.Elements(XeLastName).FirstOrDefault()?.Value),
            MiddleInitial = string.Empty,
            Gender = JghString.TmLr(child.Elements(XeGender).FirstOrDefault()?.Value),
            BirthYear = JghString.TmLr(child.Elements(XeBirthYear).FirstOrDefault()?.Value),
            City = JghString.TmLr(child.Elements(XeCity).FirstOrDefault()?.Value),
            Team = string.Empty,
            RaceGroupBeforeTransition = JghString.TmLr(child.Elements(XeRaceGroupBeforeTransition).FirstOrDefault()?.Value),
            RaceGroupAfterTransition = string.Empty,
            DateOfRaceGroupTransitionAsString = string.Empty,
            IsSeries = true, // default
            Series = string.Empty
        };


        #region fix Gender

        var candidateGenderSymbol = baby.Gender;

        if (candidateGenderSymbol == null || string.IsNullOrWhiteSpace(candidateGenderSymbol))
        {
            baby.Gender = Symbols.SymbolMale; // default
        }
        else
        {
            if (candidateGenderSymbol == "M")
            {
                baby.Gender = Symbols.SymbolMale;
            }
            else if (candidateGenderSymbol == "F")
            {
                baby.Gender = Symbols.SymbolFemale;
            }
            else if (candidateGenderSymbol == "X")
            {
                baby.Gender = Symbols.SymbolNonBinary;
            }
            else
            {
                baby.Gender = Symbols.SymbolMale; // final default
            }
        }

        #endregion

        #region fix BirthYear

        if (string.IsNullOrWhiteSpace(baby.BirthYear))
        {
            baby.BirthYear = "1900"; // default
        }
        else
        {
            var candidateBirthYear = JghString.Substring(0,4, baby.BirthYear);

            if (candidateBirthYear == null)
            {
                baby.BirthYear = "1900"; // default
            }
            else
            {
                baby.BirthYear = JghString.IsOnlyDigits(candidateBirthYear) ? candidateBirthYear : "1900"; // default
            }
        }

        #endregion

        #region fix City

        var candidateCity = baby.City;

        if (candidateCity.Contains(','))
        {
            baby.City = JghString.TmLr(candidateCity.Split(",").FirstOrDefault());
        }
        else if (candidateCity.Contains("n/a"))
        {
            baby.City = Symbols.SymbolQuestionMark;
        }
        else if (candidateCity.Contains('/'))
        {
            baby.City = JghString.TmLr(candidateCity.Split(",").FirstOrDefault());
        }
        else if (candidateCity.Contains(" "))
        {
            baby.City = JghString.TmLr(candidateCity.Split(" ").LastOrDefault());
        }
        #endregion

        #region fix RaceGroup

        const string XvalueExpert = "Expert";
        const string XvalueSport = "Sport";
        const string XvalueIntermediate = "Intermediate";
        const string XvalueNovice = "Novice";

        var candidateRaceGroup = baby.RaceGroupBeforeTransition;

        if (candidateRaceGroup == null || string.IsNullOrWhiteSpace(candidateRaceGroup))
        {
            baby.RaceGroupBeforeTransition = JghString.TmLr(XvalueNovice); // default - shortest race
        }
        else
        {
            if (candidateRaceGroup is XvalueExpert or XvalueSport or XvalueIntermediate or XvalueNovice)
            {
                baby.RaceGroupBeforeTransition = JghString.TmLr(candidateRaceGroup);
            }
            else
            {
                baby.RaceGroupBeforeTransition = JghString.TmLr(XvalueNovice); // default - shortest race
            }
        }

        #endregion

        return baby;
    }

    public static ParticipantHubItem MergeEditsBackIntoItemBeingModified(ParticipantHubItem itemBeingModified, BabyParticipantDto baby, string touchedBy)
    {
        #region Step 1. new up a pre-populated draft answer

        var answer = itemBeingModified.ToShallowMemberwiseClone();

        #endregion

        #region Step 2. insert editable fields into the "new" Modify item

        // non-editable fields in template - new plan is to deny Identifier to be changed - if it was entered wrong then delete the item and re-enter it
        //if (string.IsNullOrWhiteSpace(Identifier))
        //    answer.Identifier = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, System.Guid.NewGuid().ToString());
        //else
        //    answer.Identifier = JghString.TmLr(JghString.CleanAndConvertToLetterOrDigitOrHyphen(JghString.TmLr(Identifier)));

        // editable fields in template
        answer.FirstName = JghString.TmLr(baby.FirstName);
        answer.MiddleInitial = JghString.TmLr(baby.MiddleInitial);
        answer.LastName = JghString.TmLr(baby.LastName);
        answer.Gender = JghString.TmLr(baby.Gender); 
        JghConvert.TryConvertToInt32(baby.BirthYear, out var yearAsInt, out _);
        answer.BirthYear = yearAsInt;

        answer.City = JghString.TmLr(baby.City);
        answer.Team = JghString.TmLr(baby.Team);
        answer.RaceGroupBeforeTransition = baby.RaceGroupBeforeTransition;
        answer.RaceGroupAfterTransition = string.Empty;
        answer.DateOfRaceGroupTransition = DateTime.MaxValue; // this is a dummy placeholder. will be ignored when converting to ParticipantHubItemDto as desired

        answer.IsSeries = baby.IsSeries;
        answer.Series = string.Empty;
        answer.EventIdentifiers = string.Empty;
        answer.MustDitchOriginatingItem = false;
        answer.TouchedBy = string.IsNullOrWhiteSpace(touchedBy) ? "anonymous" : JghString.TmLr(touchedBy);

        answer.IsStillToBeBackedUp = true;
        answer.IsStillToBePushed = true;
        answer.DatabaseActionEnum = EnumStrings.DatabaseModify;
        answer.WhenTouchedBinaryFormat = DateTime.Now.ToBinary();

        answer.Guid = Guid.NewGuid().ToString(); // NB. Guid assigned here at moment of population by user (not in ctor). the ctor does not create the Guid fields. only in ParticipantHubItem.CreateItem() and ParticipantHubItemEditTemplateViewModel.MergeEditsBackIntoItemBeingModified()

        answer.Label = JghString.Concat(answer.Identifier, answer.FirstName, answer.LastName);

        #endregion

        return answer;
    }

    public static bool OneOrMoreEntriesAreInvalid(BabyParticipantDto baby, out string errorMessage)
    {
        // NB> ensure this list identical to all the fields of ParticipantHubItem

        var sb = new StringBuilder();

        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(baby.FirstName) || baby.FirstName.Length < 2) sb.AppendLine($"First name must be two or more letters. May include a hyphen. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLetters(baby.MiddleInitial) || baby.MiddleInitial.Length > 1) sb.AppendLine($"Middle initial must be a single letter or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(baby.LastName) || baby.LastName.Length < 2) sb.AppendLine($"Last name must be two or more letters. May include a hyphen or apostrophe or a space. <{baby.FirstName} {baby.LastName}>");
        if (string.IsNullOrWhiteSpace(baby.Gender)) sb.AppendLine($"Gender must be specified. <{baby.FirstName} {baby.LastName}>");
        if (string.IsNullOrWhiteSpace(baby.RaceGroupBeforeTransition)) sb.AppendLine($"Race must be specified. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLetters(baby.City)) sb.AppendLine($"City name must be letters or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigits(baby.Team)) sb.AppendLine($"Team name must be letters and/or digits or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyDigits(baby.BirthYear) || baby.BirthYear.Length is < 4 or > 4) sb.AppendLine($"Year of birth must be four digits. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigits(baby.Series)) sb.AppendLine($"SeriesIdentifier identifier must be letters or digits or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphen(baby.Identifier)) sb.AppendLine($"ID must consist of letters, digits, or hyphens (or be temporarily blank). <{baby.FirstName} {baby.LastName}>");

        if (sb.Length <= 0)
        {
            errorMessage = string.Empty;
            return false;
        }

        errorMessage = sb.ToString();

        return true;
    }

    private static void SaveWorkToHardDriveAsXml(string xmlAsText, string outPutFolder, string outPutFilename)
    {
        var pathOfXmlFile = outPutFolder + @"\" + outPutFilename;

        File.WriteAllText(pathOfXmlFile, xmlAsText);

        JghConsoleHelper.WriteLineFollowedByOne("The extracted data has been saved for perusal at your leisure.");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Folder", 20)} : {outPutFolder}");
        JghConsoleHelper.WriteLineFollowedByOne($"{JghString.LeftAlign("FileName", 20)} : {outPutFilename}");
    }

    #endregion
}