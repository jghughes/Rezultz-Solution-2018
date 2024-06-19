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
    private const string Description = "This console program (Tool10) reads the Kelso Master List of people registered before the start of the season for the series. It generates equivalent ParticipantHubItems for upload to the hub so that the Portal is ready to go from Day 1.";

    #region the MEAT

    private static async Task Main()
    {
        #region intro

        console.WriteLineFollowedByOne("Welcome.");
        console.WriteLineFollowedByOne(Description);
        console.WriteLine($"{JghString.LeftAlign("SeasonProfileID", LhsWidth)} : {DesiredSeasonProfileID}");
        console.WriteLine($"{JghString.LeftAlign("Input folder for Andrew's participant master list/s", LhsWidth)} : {FolderContainingMasterListFromAndrew}");
        console.WriteLine($"{JghString.LeftAlign("Output folder for parsed participants", LhsWidth)} : {FolderForDeserialisedMasterList}");
        console.WriteLine($"{JghString.LeftAlign("Output folder for ParticipantHubItems-Originating", LhsWidth)} : {FolderForOriginatingParticipantHubItems}");
        console.WriteLine($"{JghString.LeftAlign("Output folder for ParticipantHubItems-Modified", LhsWidth)} : {FolderForModifiedParticipantHubItems}");
        console.WriteLineWrappedByOne("Press enter to go. When you see FINISH you're done.");
        console.ReadLine();

        #endregion

        try
        {
            #region download SeriesProfileItem from Azure to obtain certain required info for our task

            try
            {
                console.WriteLine($"Please wait. Fetching SeasonProfile({DesiredSeasonProfileID})");

                currentSeasonProfileItem = await LeaderboardResultsSvcAgent.GetSeasonProfileAsync(DesiredSeasonProfileID);
                currentSeriesProfileItem = currentSeasonProfileItem?.SeriesProfiles.LastOrDefault();
                currentEventProfileItem = currentSeriesProfileItem?.EventProfileItems.LastOrDefault();

                console.WriteLine($"Season obtained = <{currentSeasonProfileItem?.Title}>");
                console.WriteLine($"Series obtained = <{currentSeriesProfileItem?.Title}>");
            }
            catch (Exception e)
            {
                console.WriteLine($"Oops. Failed to locate (or de-serialise) the designated season profile file. {e.Message}");

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
                console.WriteLine("Directory not found: " + FolderContainingMasterListFromAndrew);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForDeserialisedMasterList);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForDeserialisedMasterList);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForOriginatingParticipantHubItems);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForOriginatingParticipantHubItems);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(FolderForModifiedParticipantHubItems);
            }
            catch (DirectoryNotFoundException)
            {
                console.WriteLine("Directory not found: " + FolderForModifiedParticipantHubItems);
                return;
            }

            #endregion

            #region confirm existence of input master list file (.xml)

            var participantFileInfo = new FileInfo(FolderContainingMasterListFromAndrew + FilenameForMasterListFromAndrew);

            if (!participantFileInfo.Exists)
            {
                console.WriteLine($"Failed to locate designated participant file. <{participantFileInfo.Name}>");

                return;
            }

            #endregion

            #region instantiate a FileItem to represent the master list and parse the parent XElement in the contents

            console.WriteLineWrappedByOne("Reading master list file.");

            FileItem participantFileItem;

            try
            {
                participantFileItem = new FileItem
                {
                    FileInfo = participantFileInfo,
                    FileContentsAsText = "",
                    FileContentsAsXElement = new XElement(BabyParticipantDto.XeDataRootForContainerOfSimpleStandAloneArray),
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

                    console.WriteLineFollowedByOne($"Successfully processed: {participantFileItem.FileInfo.Name}");
                }
                catch (Exception e)
                {
                    console.WriteLine(e.Message);
                    throw new Exception(e.InnerException?.Message);
                }
            }
            catch (Exception e)
            {
                console.WriteLine($"Failed to locate and/or parse XElement in designated participant file. {e.Message}");
                console.WriteLine("");
                return;
            }

            #endregion

            #region deserialise the XElement into a list of BabyParticipantDto

            List<BabyParticipantDto> babyParticipants = [];

            var arrayOfRepeatingXe = participantFileItem.FileContentsAsXElement.Elements(NameOfRepeatingChildXElement).ToArray();

            if (arrayOfRepeatingXe.Length == 0)
                throw new Exception($"Found not even a single repeating child XElement named <{NameOfRepeatingChildXElement}> in file <{participantFileItem.FileInfo.Name}>.");

            foreach (var repeatXe in arrayOfRepeatingXe)
            {
                var baby = CreateBaby(repeatXe);

                if (baby is null)
                    continue;

                if (OneOrMoreEntriesAreInvalid(baby, out var errorMessage))
                    throw new JghAlertMessageException(errorMessage);

                babyParticipants.Add(baby);
            }

            #endregion

            #region save the list of babies for future reference

            var arrayOfBabiesAsXmlText = JghSerialisation.ToXmlFromObject(babyParticipants.ToArray(), [typeof(BabyParticipantDto)]);

            SaveWorkToHardDriveAsXml(arrayOfBabiesAsXmlText, FolderForDeserialisedMasterList, FilenameForBabyParticipants, babyParticipants.Count);

            #endregion

            #region translate each BabyParticipantDto into a matching pair of ParticpantHubItems - "originating" and "modified" - and add them all into RepositoryOfHubStyleEntries

            List<ParticipantHubItem> originatingParticipantHubItems = [];

            List<ParticipantHubItem> modifiedParticipantHubItems = [];

            var i = 0;

            foreach (var babyParticipant in babyParticipants)
            {
                var originatingBib = babyParticipant.Bib;

                if (string.IsNullOrWhiteSpace(originatingBib))
                    originatingBib = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());

                if (!JghString.IsOnlyLettersOrDigitsOrHyphen(originatingBib))
                {
                    originatingBib = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());

                    console.WriteLine(
                        $"Error: Bib <{originatingBib}> is malformed for <{babyParticipant.FirstName} {babyParticipant.LastName}> Applied fallback instead <{originatingBib}>. (Bib must consist of letters, digits, or hyphens, or be blank))");
                }

                var originatingHubItem = ParticipantHubItem.Create(i, babyParticipant.Bib, babyParticipant.Rfid, EnumStrings.KindOfEntryIsParticipantEntry, "jgh");

                originatingParticipantHubItems.Add(originatingHubItem);

                var didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(originatingHubItem, out var errorMessage1);

                if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage1);

                var modification = MergeEditsBackIntoItemBeingModified(originatingHubItem, babyParticipant, "jgh");

                modifiedParticipantHubItems.Add(modification);

                didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(modification, out var errorMessage2);

                if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage2);

                i++;
            }

            #endregion

            #region save save the list of "originating" HubItems for future reference

            var originatingParticipantHubItemAsXmlText = JghSerialisation.ToXmlFromObject(ParticipantHubItem.ToDataTransferObject(originatingParticipantHubItems.ToArray()), [typeof(ParticipantHubItem)]);

            SaveWorkToHardDriveAsXml(originatingParticipantHubItemAsXmlText, FolderForOriginatingParticipantHubItems, FilenameForOriginatingParticipantHubItems, originatingParticipantHubItems.Count);

            #endregion

            #region save save the list of "modified" HubItems for future reference

            var modifiedParticipantHubItemAsXmlText = JghSerialisation.ToXmlFromObject(ParticipantHubItem.ToDataTransferObject(modifiedParticipantHubItems.ToArray()), [typeof(ParticipantHubItem)]);

            SaveWorkToHardDriveAsXml(modifiedParticipantHubItemAsXmlText, FolderForModifiedParticipantHubItems, FileNameForModifiedParticipantHubItems, modifiedParticipantHubItems.Count);

            #endregion

            #region decide if to proceed with upload of all HubItems to the Hub. If No, then bale

            console.WriteLineWrappedByOne("Do you wish to proceed and upload all the ParticipantHubItems? Press 'y' or anything else to quit.");

            var answer = Console.ReadLine();

            if (answer != "y")
            {
                console.WriteLine();
                console.WriteLine("Everything complete. No further action required. Goodbye.");
                console.WriteLine();
                console.WriteLine("ooo0 - Goodbye - 0ooo");
                Console.ReadLine();
                return;
            }

            #endregion

            #region if Yes, proceed with upload

            #region get parameters ready

            var cloudDataLocation = ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(currentSeriesProfileItem, currentEventProfileItem);

            #endregion

            #region obtain data from RepositoryOfHubStyleEntries

            var entriesInRepositoryNotPushedPreviously = RepositoryOfHubStyleEntries.GetAllEntriesAsRawData()
                .Where(z => z.IsStillToBePushed)
                .ToArray();

            if (!entriesInRepositoryNotPushedPreviously.Any())
                throw new JghAlertMessageException(JghString.ConcatAsLines(
                    "Nothing new to push to remote hub."));

            var numberOfEntriesNotPushedPreviously = entriesInRepositoryNotPushedPreviously.Count(z => z.IsStillToBePushed);

            var whenPushed = DateTime.Now.ToBinary();

            #endregion

            #region push all HubItems to the Hub

            var scratchPadOfClonesToBePushed = new List<ParticipantHubItem>();

            foreach (var item in entriesInRepositoryNotPushedPreviously)
            {
                var clone = item.ToShallowMemberwiseClone();
                clone.IsStillToBePushed = false;
                clone.WhenPushedBinaryFormat = whenPushed;
                scratchPadOfClonesToBePushed.Add(clone);
            }

            console.WriteLineWrappedByOne($"Please wait. Uploading <{entriesInRepositoryNotPushedPreviously.Length}> ParticipantHubItems.");

            var uploadReport = await ParticipantRegistrationSvcAgent.PostParticipantItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, scratchPadOfClonesToBePushed.ToArray());

            #region success? - report back

            foreach (var item in entriesInRepositoryNotPushedPreviously)
            {
                item.IsStillToBePushed = false;
                item.WhenPushedBinaryFormat = whenPushed;
            }

            var messageOk = numberOfEntriesNotPushedPreviously switch
            {
                0 => "Nothing pushed. Cache was empty.",
                1 => "Success. A copy of a single entry was pushed to the hub.",
                _ => "Success. Copies of multiple entries were pushed to the hub."
            };

            var reportRegardingItemsNeverPushedBefore = JghString.ConcatAsParagraphs(messageOk, uploadReport);

            console.WriteLine(reportRegardingItemsNeverPushedBefore);
            console.WriteLine($"Account: {cloudDataLocation.Item1}");

            #endregion

            #endregion

            #endregion

            #region wrap up

            console.WriteLinePrecededByOne("Everything complete. No further action required. Goodbye.");
            console.WriteLine("ooo0 - Goodbye - 0ooo");

            SaveReportToHardDrive(console.ToString(), FolderForReport, FilenameOfReport);

            Console.ReadLine();

            #endregion
        }
        catch (Exception ex)
        {
            console.WriteLineFollowedByOne(ex.ToString());
            console.ReadLine();
        }
    }

    #endregion

    #region constants

    private const int LhsWidth = 53;

    private const string DesiredSeasonProfileID = "998"; // kelso 2024 - real life
    //private const string DesiredSeasonProfileID = "999"; // my test profile

    private const string NameOfRepeatingChildXElement = "Master_x0020_List";

    private const string FolderContainingMasterListFromAndrew = @"C:\Users\johng\holding pen\participants-from-Andrew\";
    private const string FolderForDeserialisedMasterList = @"C:\Users\johng\holding pen\participants-BabyParticipants\";
    private const string FolderForOriginatingParticipantHubItems = @"C:\Users\johng\holding pen\participants-originating-ParticipantHubItems\";
    private const string FolderForModifiedParticipantHubItems = @"C:\Users\johng\holding pen\participants-modified-ParticipantHubItems\";

    private const string FilenameForMasterListFromAndrew = @"2024-MTB-Racer-List-2024-05-10-Friday.xml";
    private const string FilenameForBabyParticipants = @"BabyItems.xml";
    private const string FilenameForOriginatingParticipantHubItems = @"OriginatingHubItems.xml";
    private const string FileNameForModifiedParticipantHubItems = @"ModifiedHubItems.xml";

    private const string FolderForReport = FolderContainingMasterListFromAndrew;
    private const string FilenameOfReport = @"AgeGroupAnalysis.txt";

    #endregion

    #region variables

    private static SeasonProfileItem? currentSeasonProfileItem;

    private static SeriesProfileItem? currentSeriesProfileItem;

    private static EventProfileItem? currentEventProfileItem;

    public static readonly IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem> RepositoryOfHubStyleEntries = new RepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem>(new IsolatedStorageService());

    private static readonly JghConsoleHelperV2 console = new();

    #endregion

    #region remote svc's available for use

    // ReSharper disable once UnusedMember.Local
    private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientMvc());
    private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientMvc());
    private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientMvc());

    #endregion

    #region helper methods

    public static BabyParticipantDto? CreateBaby(XElement child)
    {
        #region Element names and symbols on the source Kelso masterlist in XML (src)

        const string SrcXeBib = "Plate_x0020__x0023_"; // the repeating element of the array
        const string SrcXeFirstName = "First_x0020_Name";
        const string SrcXeLastName = "Last_x0020_Name";
        const string SrcXeGender = "Sex";
        const string SrcXeBirthYear = "Date_x0020_of_x0020_Birth";
        const string SrcXeCity = "city";
        const string SrcXeCategory = "Category";
        const string SrcXeRfid = "Bibtag_x0020__x0023_";
        const string SrcXeReservation = "Reservation";
        const string SrcXeProduct = "Product";

        const string SrcValueExpert = "Expert";
        const string SrcValueSport = "Sport";
        const string SrcValueIntermediate = "Intermediate";
        const string SrcValueNovice = "Novice";
        const string SrcValueBeginner = "Beginner";
        const string SrcValueKids = "Kids";
        const string SrcValueMale = "M";
        const string SrcValueFemale = "F";
        const string SrcValueNonBinary = "X";
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
            Rfid = JghString.TmLr(child.Elements(SrcXeRfid).FirstOrDefault()?.Value),
            FirstName = JghString.TmLr(child.Elements(SrcXeFirstName).FirstOrDefault()?.Value),
            LastName = JghString.TmLr(child.Elements(SrcXeLastName).FirstOrDefault()?.Value),
            MiddleInitial = string.Empty,
            Gender = JghString.TmLr(child.Elements(SrcXeGender).FirstOrDefault()?.Value),
            BirthYear = JghString.TmLr(child.Elements(SrcXeBirthYear).FirstOrDefault()?.Value),
            City = JghString.TmLr(child.Elements(SrcXeCity).FirstOrDefault()?.Value),
            Team = string.Empty,
            Series = string.Empty,
            Reservation = JghString.TmLr(child.Elements(SrcXeReservation).FirstOrDefault()?.Value),
            Product = JghString.TmLr(child.Elements(SrcXeProduct).FirstOrDefault()?.Value)
        };

        #endregion

        #region fix Gender

        var candidateGenderSymbol = baby.Gender;

        if (candidateGenderSymbol == null || string.IsNullOrWhiteSpace(candidateGenderSymbol))
        {
            baby.Gender = Symbols.SymbolMale; // default
        }
        else
        {
            if (candidateGenderSymbol == JghString.TmLr(SrcValueMale))
                baby.Gender = Symbols.SymbolMale;
            else if (candidateGenderSymbol == JghString.TmLr(SrcValueFemale))
                baby.Gender = Symbols.SymbolFemale;
            else if (candidateGenderSymbol == JghString.TmLr(SrcValueNonBinary))
                baby.Gender = Symbols.SymbolNonBinary;
            else
                baby.Gender = Symbols.SymbolMale; // final default
        }

        #endregion

        #region fix BirthYear

        var defaultBirthYear = "1900";

        if (string.IsNullOrWhiteSpace(baby.BirthYear))
        {
            baby.BirthYear = defaultBirthYear; // default
        }
        else
        {
            var candidateBirthYear = JghString.Substring(0, 4, baby.BirthYear);

            if (string.IsNullOrWhiteSpace(candidateBirthYear))
                baby.BirthYear = defaultBirthYear; // default
            else
                baby.BirthYear = JghString.IsOnlyDigits(candidateBirthYear) ? candidateBirthYear : defaultBirthYear;
        }

        #endregion

        #region fix City - this is a kludge unfortunately

        var candidateCity = baby.City;

        if (candidateCity.Contains(','))
            baby.City = JghString.TmLr(candidateCity.Split(",").FirstOrDefault());
        else if (candidateCity.Contains("n/a"))
            baby.City = string.Empty;
        else if (candidateCity.Contains('/'))
            baby.City = JghString.TmLr(candidateCity.Split("/").FirstOrDefault());
        else if (candidateCity.Contains("77 john street milton")) baby.City = "milton";

        #endregion

        #region fix IsSeries

        baby.IsSeries = baby.Product.Contains(JghString.TmLr(SrcValueFullSeries));

        #endregion

        #region fix RaceGroups

        if (string.IsNullOrWhiteSpace(candidateRaceGroup))
        {
            baby.RaceGroupBeforeTransition = JghString.TmLr(SrcValueNovice); // default - shortest race
        }
        else
        {
            if (JghString.AreEqualIgnoreOrdinalCase(candidateRaceGroup, SrcValueExpert) ||
                JghString.AreEqualIgnoreOrdinalCase(candidateRaceGroup, SrcValueSport) ||
                JghString.AreEqualIgnoreOrdinalCase(candidateRaceGroup, SrcValueIntermediate) ||
                JghString.AreEqualIgnoreOrdinalCase(candidateRaceGroup, SrcValueNovice))
                baby.RaceGroupBeforeTransition = candidateRaceGroup;
            else
                baby.RaceGroupBeforeTransition = JghString.TmLr(SrcValueNovice); // default - shortest race
        }

        baby.RaceGroupAfterTransition = string.Empty;
        baby.DateOfRaceGroupTransitionAsString = string.Empty;

        #endregion

        return baby;
    }

    public static ParticipantHubItem MergeEditsBackIntoItemBeingModified(ParticipantHubItem itemBeingModified, BabyParticipantDto baby, string touchedBy)
    {
        #region Step 1. new up a pre-populated draft answer

        var answer = itemBeingModified.ToShallowMemberwiseClone();

        #endregion

        #region Step 2. insert editable fields into the "new" Modify item

        // non-editable fields in template - new plan is to deny Bib to be changed - if it was entered wrong then delete the item and re-enter it
        //if (string.IsNullOrWhiteSpace(Bib))
        //    answer.Bib = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, System.Guid.NewGuid().ToString());
        //else
        //    answer.Bib = JghString.TmLr(JghString.CleanAndConvertToLetterOrDigitOrHyphen(JghString.TmLr(Bib)));

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
        answer.Reservation = JghString.TmLr(baby.Reservation);
        answer.Rfid = answer.Rfid;


        answer.MustDitchOriginatingItem = false;
        answer.TouchedBy = string.IsNullOrWhiteSpace(touchedBy) ? "anonymous" : JghString.TmLr(touchedBy);

        answer.IsStillToBeBackedUp = true;
        answer.IsStillToBePushed = true;
        answer.DatabaseActionEnum = EnumStrings.DatabaseModify;
        answer.WhenTouchedBinaryFormat = DateTime.Now.ToBinary();

        answer.Guid = Guid.NewGuid().ToString(); // NB. Guid assigned here at moment of population by user (not in ctor). the ctor does not create the Guid fields. only in ParticipantHubItem.CreateItem() and ParticipantHubItemEditTemplateViewModel.MergeEditsBackIntoItemBeingModified()

        answer.Label = JghString.Concat(answer.Bib, answer.FirstName, answer.LastName);

        #endregion

        return answer;
    }

    public static bool OneOrMoreEntriesAreInvalid(BabyParticipantDto baby, out string errorMessage)
    {
        // NB> ensure this list identical to all the fields of ParticipantHubItem

        var sb = new StringBuilder();

        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(baby.FirstName) || baby.FirstName.Length < 2) sb.AppendLine($"First name must be two or more letters. May include a hyphen. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLetters(baby.MiddleInitial) || baby.MiddleInitial.Length > 1) sb.AppendLine($"Middle initial must be a single letter or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(baby.LastName) || baby.LastName.Length < 2)
            sb.AppendLine($"Last name must be two or more letters. May include a hyphen or apostrophe or a space. <{baby.FirstName} {baby.LastName}>");
        if (string.IsNullOrWhiteSpace(baby.Gender)) sb.AppendLine($"Gender must be specified. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphenOrSpace(baby.RaceGroupBeforeTransition)) sb.AppendLine($"Race must be specified. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphenOrSpace(baby.City)) sb.AppendLine($"City name must be letters or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphenOrSpace(baby.Team)) sb.AppendLine($"Team name must be letters and/or digits or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyDigits(baby.BirthYear) || baby.BirthYear.Length is < 4 or > 4) sb.AppendLine($"Year of birth must be four digits. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphenOrSpace(baby.Series)) sb.AppendLine($"SeriesIdentifier identifier must be letters or digits or blank. <{baby.FirstName} {baby.LastName}>");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphen(baby.Bib)) sb.AppendLine($"ID must consist of letters, digits, or hyphens (or be temporarily blank). <{baby.FirstName} {baby.LastName}>");

        if (sb.Length <= 0)
        {
            errorMessage = string.Empty;
            return false;
        }

        errorMessage = sb.ToString();

        return true;
    }

    private static void SaveWorkToHardDriveAsXml(string xmlAsText, string outPutFolder, string outPutFilename, int numberOfItems)
    {
        var pathOfFile = Path.Combine(outPutFolder, outPutFilename);

        File.WriteAllText(pathOfFile, xmlAsText);

        console.WriteLine($"{JghString.LeftAlign("Folder", 20)} : {outPutFolder}");
        console.WriteLine($"{JghString.LeftAlign("FileName", 20)} : {outPutFilename}");
        console.WriteLine($"{JghString.LeftAlign("Items saved", 20)} : {numberOfItems}");
    }

    private static void SaveReportToHardDrive(string text, string outPutFolder, string outPutFilename)
    {
        var pathOfFile = Path.Combine(outPutFolder, outPutFilename);
        File.WriteAllText(pathOfFile, text);

        Console.WriteLine($"{JghString.LeftAlign("File saved:", LhsWidth)} {outPutFilename}");
        Console.WriteLine($"{JghString.LeftAlign("Folder:", LhsWidth)} {outPutFolder}");
    }

    #endregion
}