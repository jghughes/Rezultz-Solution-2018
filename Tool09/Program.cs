using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.DataTypes.Mar2024;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.SvcAgents;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService;

namespace Tool09
{
    internal class Program
    {
        private const string Description = "This console program (Tool09) reads Andrew's participant list and translates them into ParticipantHubItems and optionally uploads them."

        private static async Task Main()
        {
            JghConsoleHelper.WriteLineFollowedByOne(Description);
            JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for Andrew's participant files", LhsWidth)} : {InputFolderContainingParticipantFiles}");
            JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for publishable results", LhsWidth)} : {OutputFolderForSavedCopiesOfParticipantHubItems}");
            JghConsoleHelper.WriteLineWrappedInOne("Press enter to go. When you see FINISH you're done.");
            JghConsoleHelper.ReadLine();
            JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

            #region first things first - test if all our svc connectors are working - comment/uncomment out those you do or don't want at any point

            //JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - AzureStorageSvcAgent...");
            //var answer1 = await AzureStorageSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer1}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - ParticipantRegistrationSvcAgent...");
            var answer2 = await ParticipantRegistrationSvcAgent.GetIfServiceIsAnsweringAsync();
            JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer2}");
            JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            JghConsoleHelper.ReadLine();


            JghConsoleHelper.WriteLineFollowedByOne("Please wait. Connecting to svc using - LeaderboardResultsSvcAgent...");
            var answer3 = await LeaderboardResultsSvcAgent.GetIfServiceIsAnsweringAsync();
            JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer3}");
            JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            JghConsoleHelper.ReadLine();


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

            SeasonProfileItem? seasonProfileItem;
            SeriesProfileItem seriesProfileItem;

            try
            {
                JghConsoleHelper.WriteLineFollowedByOne("Please wait. Fetching SeasonProfile('998')");

                seasonProfileItem = await LeaderboardResultsSvcAgent.GetSeasonProfileAsync("998");

                JghConsoleHelper.WriteLineFollowedByOne($"answer = [{seasonProfileItem.Label}]");

                seriesProfileItem = seasonProfileItem.SeriesProfiles.Last();

                seriesLabel = seriesProfileItem.Label;
                eventLabel = seriesProfileItem.EventProfileItems.Last().Label;

                accountName = seriesProfileItem.ContainerForParticipantHubItemData.AccountName;
                containerName = seriesProfileItem.ContainerForParticipantHubItemData.ContainerName;
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
                Directory.SetCurrentDirectory(InputFolderContainingParticipantFiles);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + InputFolderContainingParticipantFiles);
                return;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(OutputFolderForSavedCopiesOfParticipantHubItems);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + OutputFolderForSavedCopiesOfParticipantHubItems);
                return;
            }

            #endregion

            #region Locate all files in input folder

            var di = new DirectoryInfo(InputFolderContainingParticipantFiles); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing all the files in the current directory of all types.

            #endregion

            #region Load all candidate XML files from input folder

            try
            {
                foreach (var fi in arrayOfInputFileInfo)
                {
                    if (!Path.GetFileName(fi.FullName).EndsWith($".{RequiredInputFileFormat}"))
                    {
                        JghConsoleHelper.WriteLine($"Skipping {fi.Name} because it's not a {RequiredInputFileFormat} file as you appear to have specified. Does this make sense?");
                        continue;
                    }

                    JghConsoleHelper.WriteLine($"Found {fi.Name}.");

                    var fileItem = new FileItem
                    {
                        FileInfo = fi,
                        FileContentsAsText = "",
                        FileContentsAsXDocument = new XDocument("dummy"),
                        OutputFileName = fi.Name
                    };

                    FilesOfParticipantListsImportedFromAndrew.Add(fileItem);

                }

                JghConsoleHelper.WriteLine();

                try
                {
                    foreach (var file in FilesOfParticipantListsImportedFromAndrew)
                    {
                        JghConsoleHelper.WriteLine($"Parsing into XML {file.FileInfo.Name}....");

                        var fullInputPath = file.FileInfo.FullName;

                        var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);
                        var rawInputAsXDocument= XDocument.Parse(rawInputAsText);

                        file.FileContentsAsText = rawInputAsText;
                        file.FileContentsAsXDocument = rawInputAsXDocument;

                        JghConsoleHelper.WriteLine($"Successfully parsed and loaded {file.FileInfo.Name}");
                    }
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLine(e.Message);
                    throw new Exception(e.InnerException?.Message);
                }

                if (FilesOfParticipantListsImportedFromAndrew.Count == 0)
                    throw new Exception("Found not even a single participant data file.");
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine($"Failed to locate designated participant file. {e.Message}");
                JghConsoleHelper.WriteLine("");
                return;
            }

            JghConsoleHelper.WriteLine();

            #endregion












            #region upload timing data files required for preprocessing

            foreach (var fileItem in FilesOfParticipantListsImportedFromAndrew)
            {
                var prettyTimeNow1 = DateTime.Now.ToString(JghDateTime.SortablePattern);

                var fileName = fileItem.FileInfo.Name;

                var blobName3 = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', $"{prettyTimeNow1}____{fileName}");

                var didSucceed3 = await RaceResultsPublishingSvcAgent.UploadDatasetFileToBeProcessedSubsequentlyAsync(IdentifierOfResultsFromRezultzPortal, new EntityLocationItem(accountName, containerName, blobName3), fileItem.FileContentsAsText,
                    CancellationToken.None);

                if (didSucceed3) FilesToBeProcessedByPublisherModule.Add(new PublisherImportFileTargetItem(IdentifierOfResultsFromRezultzPortal, blobName3));
            }

            #endregion



        }

        private static async Task Main01(string seriesLabel, string eventLabel, SeriesProfileItem seriesProfileItem)
        {
            JghConsoleHelper.WriteLineFollowedByOne("RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync() ....");


            var publisherOutput = await RaceResultsPublishingSvcAgent.GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync("21portal", seriesLabel, eventLabel, seriesProfileItem, FilesToBeProcessedByPublisherModule.ToArray());

            if (publisherOutput == null)
            {
                throw new FileNotFoundException($"Failure. RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync() returned NULL. This should never really occur other than in explicit testing of what happens when a previously uploaded file mysteriously disappears.");
            }

            if (publisherOutput.ComputedResults == null)
            {
                JghConsoleHelper.WriteLine($"Failure. RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync() returned a NULL ComputedResults object array - signifying a handled failure (needing to be reported).");
                JghConsoleHelper.WriteLine("This is what the user sees in the conversion report TextBlock on the GUI when the svc method returns:");
                JghConsoleHelper.WriteLine("-----------------------------------------------------------------------------------------------------------------");
                JghConsoleHelper.WriteLine(publisherOutput.ConversionReport);
                JghConsoleHelper.WriteLine("-----------------------------------------------------------------------------------------------------------------");

                return;
            }


            JghConsoleHelper.WriteLineFollowedByOne("Answer as preprocessed results as XML file contents :");
            JghConsoleHelper.WriteLine($"{ConvertOutputToResultsDtoXmlFileContents(publisherOutput.ComputedResults)}");
            JghConsoleHelper.WriteLinePrecededByOne("This is what the user sees in the conversion report TextBlock on the GUI when the svc method returns:");
            JghConsoleHelper.WriteLine("-----------------------------------------------------------------------------------------------------------------");
            JghConsoleHelper.WriteLine(publisherOutput.ConversionReport);
            JghConsoleHelper.WriteLine("-----------------------------------------------------------------------------------------------------------------");
            JghConsoleHelper.WriteLinePrecededByOne("This is what the user sees in the MessageBox when the svc method returns:");
            JghConsoleHelper.WriteLine("-----------------------------------------------------------------------------------------------------------------");
            JghConsoleHelper.WriteLine(publisherOutput.RanToCompletionMessage);
            JghConsoleHelper.WriteLine("-----------------------------------------------------------------------------------------------------------------");

            SaveWorkToHardDriveAsXml(ConvertOutputToResultsDtoXmlFileContents(publisherOutput.ComputedResults));

        }

        #region helper methods

        private static string ConvertOutputToResultsDtoXmlFileContents(ResultItem[] resultItems)
        {
            var resultsDto = ResultItem.ToDataTransferObject(resultItems);

            return JghSerialisation.ToXmlFromObject(resultsDto, new[] { typeof(ResultDto) });
        }


        private static void SaveWorkToHardDriveAsXml(string resultsDtoAsXml)
        {
            var outPutFileName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', DateTime.Now.ToString(JghDateTime.SortablePattern)) + "______ResultsSynthesisedByPublishingSvc" + "." + StandardFileTypeSuffix.Xml;

            var pathOfXmlFile = OutputFolderForSavedCopiesOfParticipantHubItems + @"\" + outPutFileName;

            File.WriteAllText(pathOfXmlFile, resultsDtoAsXml);

            JghConsoleHelper.WriteLineFollowedByOne($"The publishable results have been saved for perusal at your leisure.");
            JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Folder", 30)} : {OutputFolderForSavedCopiesOfParticipantHubItems}");
            JghConsoleHelper.WriteLineFollowedByOne($"{JghString.LeftAlign("FileName", 30)} : {outPutFileName}");
        }

        #endregion

        #region constants

        private const string InputFolderContainingParticipantFiles = @"C:\Users\johng\holding pen\participants-from-Andrew\";

        private const string RequiredInputFileFormat = "xml";

        private const string OutputFolderForSavedCopiesOfParticipantHubItems = @"C:\Users\johng\holding pen\participants-ParticipantHubItems\";

        private const string IdentifierOfResultsFromRezultzPortal = EnumsForPublisherModule.ResultItemsAsXmlFromPortalNativeTimingSystem;

        private static readonly Tuple<string, string>[] ArrayOfRezultzPortalFileNameTuples =
        {
        new("Kelso2023mtb-results-04.xml", "Kelso2023mtb-results-04----testOutput.xml")
    };

        private const int LhsWidth = 70;

        #endregion

        #region svc's available for use

        private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientMvc());
        private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientMvc());
        private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientMvc());
        private static readonly TimeKeepingSvcAgent TimeKeepingSvcAgent = new(new TimeKeepingServiceClientMvc());
        private static readonly RaceResultsPublishingSvcAgent RaceResultsPublishingSvcAgent = new(new RaceResultsPublishingServiceClientMvc());

        //private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientWcf());
        //private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientWcf());
        //private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientWcf());
        //private static readonly TimeKeepingSvcAgent TimeKeepingSvcAgent = new(new TimeKeepingServiceClientWcf());
        //private static readonly RaceResultsPublishingSvcAgent RaceResultsPublishingSvcAgent = new(new RaceResultsPublishingServiceClientWcf());

        #endregion

        #region variables

        private static readonly List<FileItem> FilesOfParticipantListsImportedFromAndrew = new();

        private static readonly List<PublisherImportFileTargetItem> FilesToBeProcessedByPublisherModule = new();

        #endregion

    }
}
