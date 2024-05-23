using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.DataTypes.Mar2024;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.SvcAgents;
using RezultzSvc.Clients.Wcf.Mar2023.ServiceClients;

// ReSharper disable UnusedMember.Local

#pragma warning disable IDE0052

namespace Tool06;

internal class Program
{
    private const string Description = "This program is a scratchpad for testing RaceResultsPublishingSvcAgent ...." +
                                       " and the C# publisher module [PublisherForMyLapsElectronicTimingSystem2023]";

    #region constants

    //private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientMvc());
    //private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientMvc());
    //private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientMvc());
    //private static readonly TimeKeepingSvcAgent TimeKeepingSvcAgent = new(new TimeKeepingServiceClientMvc());
    //private static readonly RaceResultsPublishingSvcAgent RaceResultsPublishingSvcAgent = new(new RaceResultsPublishingServiceClientMvc());

    private static readonly AzureStorageSvcAgent AzureStorageSvcAgent = new(new AzureStorageServiceClientWcf());
    private static readonly ParticipantRegistrationSvcAgent ParticipantRegistrationSvcAgent = new(new ParticipantRegistrationServiceClientWcf());
    private static readonly LeaderboardResultsSvcAgent LeaderboardResultsSvcAgent = new(new LeaderboardResultsServiceClientWcf());
    private static readonly TimeKeepingSvcAgent TimeKeepingSvcAgent = new(new TimeKeepingServiceClientWcf());
    private static readonly RaceResultsPublishingSvcAgent RaceResultsPublishingSvcAgent = new(new RaceResultsPublishingServiceClientWcf());

    private const int LhsWidth = 70;
    private const string InputFolderFromRezultz = @"C:\Users\johng\holding pen\StuffFromRezultzAzure\";
    private const string FileNameOfParticipantMasterListFromRezultz = @"Participants+2023-07-18T12-16-20.json";
    private const string RequiredMyLapsFileFormat = "txt"; // "txt" or "csv"
    private const string InputFolderFromMyLaps = @"C:\Users\johng\holding pen\StuffFromAndrew\Event01FromMylaps\Current xls versions\Current xls versions exported to csv and then renamed to txt\";
    private const string OutputFolderForXmlResults = @"C:\Users\johng\holding pen\StuffFromAndrew\Event01FromMylaps\Current xls versions\Current xls versions exported to csv and then renamed to txt\XmlVersions\";

    private static readonly Tuple<string, string>[] ArrayOfTimingDataFileNameTuples =
    {
        new("Expert Results.txt", "Kelso2023mtb-results-01-expert.xml"),
        new("Sport Results.txt", "Kelso2023mtb-results-01-sport.xml"),
        new("Novice Results.txt", "Kelso2023mtb-results-01-novice.xml")
    };

    #endregion

    #region variables

    private static readonly List<FileItem> FileItemsFromMyLaps = new();

    private static readonly List<PublisherImportFileTargetItem> FilesForPublisherModuleToProcess = new();

    #endregion

    #region main

    private static async Task Main()
    {
        JghConsoleHelper.WriteLineFollowedByOne("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for test participant file sourced from Rezultz", LhsWidth)} : {InputFolderFromRezultz}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for test timing data files sourced from MyLaps", LhsWidth)} : {InputFolderFromMyLaps}");
        JghConsoleHelper.WriteLineFollowedByOne($"{JghString.LeftAlign("Output folder for publishable xml results", LhsWidth)} : {OutputFolderForXmlResults}");
        JghConsoleHelper.WriteLine("Are you ready to go? Press enter to continue.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLine("Working. Please wait...");

        try
        {
            #region get ready

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
                Directory.SetCurrentDirectory(OutputFolderForXmlResults);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + OutputFolderForXmlResults);
                return;
            }

            #endregion

            #region confirm existence of files of input data on hard drive

            var participantMasterListFileInfo = new FileInfo(InputFolderFromRezultz + FileNameOfParticipantMasterListFromRezultz);

            if (!participantMasterListFileInfo.Exists)
            {
                JghConsoleHelper.WriteLine($"Participant file not found: {participantMasterListFileInfo.Name}");
                return;
            }

            //var seriesMetadataFileFromRezultzFileInfo = new FileInfo(inputFolderFromRezultz + seriesMetadataFileFromRezultz);

            //if (!seriesMetadataFileFromRezultzFileInfo.Exists)
            //{
            //JghConsoleHelper.WriteLine($"Series metadata file not found: {seriesMetadataFileFromRezultzFileInfo.Name}");
            //return;
            //}

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

            #region confirm existence of directories and files for outcome of processing data on hard drive

            try
            {
                foreach (var item in FileItemsFromMyLaps)
                {
                    var pathOfXmlFile = OutputFolderForXmlResults + @"\" + Path.GetFileNameWithoutExtension(item.OutputFileName) + "." + StandardFileTypeSuffix.Xml;

                     File.WriteAllText(pathOfXmlFile, "<element>dummy</element>");
                }
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine($"Failed to access a designated output file. {e.Message}");

                return;
            }

            #endregion

            #region load the MyLaps timing data files on hard drive

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

            #region optional: test if all our svc connectors are working - comment/uncomment out those you do or don't want at any point

            //JghConsoleHelper.WriteLineFollowedByOne("AzureStorageSvcAgent...");
            //var answer1 = await AzureStorageSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer1}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("ParticipantRegistrationSvcAgent...");
            //var answer2 = await ParticipantRegistrationSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer2}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();


            //JghConsoleHelper.WriteLineFollowedByOne("LeaderboardResultsSvcAgent...");
            //var answer3 = await LeaderboardResultsSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer3}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();


            //JghConsoleHelper.WriteLineFollowedByOne("TimeKeepingSvcAgent...");
            //var answer4 = await TimeKeepingSvcAgent.GetIfServiceIsAnsweringAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer4}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            JghConsoleHelper.WriteLineFollowedByOne("RaceResultsPublishingSvcAgent...");
            var answer5 = await RaceResultsPublishingSvcAgent.GetIfServiceIsAnsweringAsync();
            JghConsoleHelper.WriteLineFollowedByOne($"GetIfServiceIsAnsweringAsync() = {answer5}");
            JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            JghConsoleHelper.ReadLine();

            #endregion

            #region download SeriesProfileItem from Azure to obtain relevant info

            SeasonProfileItem? seasonProfile;
            SeriesProfileItem seriesProfileItem;

            string seriesLabel;
            string eventLabel;

            string accountName;
            string containerName;

            try
            {
                JghConsoleHelper.WriteLineFollowedByOne("leaderboardResultsSvcAgent.GetSeasonProfileAsync('999')");

                seasonProfile = await LeaderboardResultsSvcAgent.GetSeasonProfileAsync("999");

                if (seasonProfile == null) throw new FileNotFoundException("LeaderboardResultsSvcAgent.GetSeasonProfile(...) returned null");

                JghConsoleHelper.WriteLineFollowedByOne($"answer = [{seasonProfile.Label}]");

                seriesProfileItem = seasonProfile.SeriesProfiles.Last();

                seriesLabel = seriesProfileItem.Label;
                eventLabel = seriesProfileItem.EventProfileItems.Last().Label;

                accountName = seriesProfileItem.ContainerForPublishingDatasetsToBeProcessed.AccountName;
                containerName = seriesProfileItem.ContainerForPublishingDatasetsToBeProcessed.ContainerName;
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLineFollowedByOne($"Oops. Failed to locate (or de-serialise) the designated input file. {e.Message}");

                return;
            }

            #endregion

            #region upload Participant file used for preprocessing in myLaps publisher module

            var identifierOfDataset = EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub; // dug out of publisher profile file, which in turn MUST be identical to this enum for hub item files

            var participantMasterListDtoSerialisedAsStringOfJson = File.ReadAllText(InputFolderFromRezultz + FileNameOfParticipantMasterListFromRezultz);

            var prettyTimeNow = DateTime.Now.ToString(JghDateTime.SortablePattern);

            var fileName = FileNameOfParticipantMasterListFromRezultz;

            var blobName1 = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', $"{prettyTimeNow}____{fileName}");

            var didSucceed = await RaceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(identifierOfDataset,
                new EntityLocationItem(accountName, containerName, blobName1),
                participantMasterListDtoSerialisedAsStringOfJson,
                CancellationToken.None);

            if (didSucceed)
            {
                FilesForPublisherModuleToProcess.Add(new PublisherImportFileTargetItem(identifierOfDataset, blobName1));
            }
            else
            {
                throw new FileNotFoundException("RaceResultsPublishingSvcAgent.SendFileOfRawDataToBeProcessedSubsequentlyAsync() returned FALSE");

            }

            #endregion

            #region upload MyLaps files required for preprocessing

            identifierOfDataset = "MyLaps2023AsCsv"; // dug out of publisher profile file

            foreach (var fileItem in FileItemsFromMyLaps)
            {
                prettyTimeNow = DateTime.Now.ToString(JghDateTime.SortablePattern);

                fileName = fileItem.FileInfo.Name;

                var blobName3 = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', $"{prettyTimeNow}____{fileName}");

                var didSucceed3 = await RaceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(identifierOfDataset, new EntityLocationItem(accountName, containerName, blobName3), fileItem.FileContentsAsText,
                    CancellationToken.None);

                if (didSucceed3)
                {
                    FilesForPublisherModuleToProcess.Add(new PublisherImportFileTargetItem(identifierOfDataset, blobName3));
                }
                else
                {
                    throw new FileNotFoundException($"Failure. RaceResultsPublishingSvcAgent.SendFileOfRawDataToBeProcessedSubsequentlyAsync([{identifierOfDataset}], [{blobName3}]) returned FALSE.");
                }
            }

            #endregion

            #endregion

            #region miscellaneous little validation tests - comment/uncomment out those you do or don't want at any point

            JghConsoleHelper.WriteLineFollowedByOne("GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync('21portal')");
            var isRecognised = await RaceResultsPublishingSvcAgent.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync("21portal");
            JghConsoleHelper.WriteLineFollowedByOne($"answer = {isRecognised}");
            JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            JghConsoleHelper.ReadLine();

            JghConsoleHelper.WriteLineFollowedByOne("GetFileNameFragmentsOfAllPublishingProfilesAsync()");
            var fileNamesArray = await RaceResultsPublishingSvcAgent.GetFileNameFragmentsOfAllPublishingProfilesAsync();
            JghConsoleHelper.WriteLineFollowedByOne("answer:");
            foreach (var fileName2 in fileNamesArray)
                JghConsoleHelper.WriteLine(fileName2);
            JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineWrappedInOne("GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(Timestamps+2023-11-12T13-37-48.json)");
            //var illustrativeJson = await RaceResultsPublishingSvcAgent.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync("Timestamps+2023-11-12T13-37-48.json");
            //JghConsoleHelper.WriteLineFollowedByOne("answer:");
            //JghConsoleHelper.WriteLineFollowedByOne(illustrativeJson);

            //JghConsoleHelper.WriteLineFollowedByOne("GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(CrossMgr_USAC_sampleformat.xml)");
            //var illustrativeXml = await RaceResultsPublishingSvcAgent.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync("CrossMgr_USAC_sampleformat.xml");
            //JghConsoleHelper.WriteLineFollowedByOne("answer:");
            //JghConsoleHelper.WriteLineWrappedInTwo(illustrativeXml);
            //JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(MyLapsTimingSystemExportOfCsvData.csv)");
            //var illustrativeCsv = await RaceResultsPublishingSvcAgent.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync("MyLapsTimingSystemExportOfCsvData.csv");
            //JghConsoleHelper.WriteLineFollowedByOne("answer:");
            //JghConsoleHelper.WriteLineFollowedByOne(illustrativeCsv);
            //JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            var testSeriesProfile = seasonProfile.SeriesProfiles.First();
            var importFileTargets = new PublisherImportFileTargetItem[] { new("ResultItemsAsXml", "SplitIntervalsForParticipants+2023-11-12T13-41-32.xml") };
            JghConsoleHelper.WriteLineFollowedByOne("ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(....)");
            var outputOfProcessing = await RaceResultsPublishingSvcAgent.ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync("21portal", "TEST MTB Series 2022", "Dummy event #13 August 16th", testSeriesProfile, importFileTargets);
            JghConsoleHelper.WriteLineFollowedByOne("answer:");
            JghConsoleHelper.WriteLineWrappedInTwo(JghSerialisation.ToXmlFromObject(outputOfProcessing, new[] { typeof(PublisherOutputItem) }));
            JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            JghConsoleHelper.ReadLine();

            var results = ResultItem.ToDataTransferObject(outputOfProcessing.ComputedResults);
            var uploadFileContents = JghSerialisation.ToXmlFromObject(results, new[] { typeof(ResultDto) });
            JghConsoleHelper.WriteLineFollowedByOne("SendFileOfCompletedResultsForSingleEventAsync(....)");
            var outcomeOfUploadOfResults = await RaceResultsPublishingSvcAgent.UploadPublishableResultsForSingleEventAsync(new EntityLocationItem("customertester", "testuploadcontainer", "mytestfile.xml"), uploadFileContents);
            JghConsoleHelper.WriteLineFollowedByOne($"answer = {outcomeOfUploadOfResults}");
            JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            #endregion

            await Main01(seriesLabel, eventLabel, seriesProfileItem);

            JghConsoleHelper.WriteLine("Thanks. Local test program complete. Press enter to exit...");
            JghConsoleHelper.ReadLine();
        }
        catch (Exception ex)
        {
            JghConsoleHelper.WriteLineFollowedByOne(ex.ToString());
            JghConsoleHelper.ReadLine();
        }
    }

    private static async Task Main01(string seriesLabel, string eventLabel, SeriesProfileItem seriesProfileItem)
    {
        JghConsoleHelper.WriteLineFollowedByOne("RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync()...");

        var publisherOutput = await RaceResultsPublishingSvcAgent.ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync("23mylaps", seriesLabel, eventLabel, seriesProfileItem, FilesForPublisherModuleToProcess.ToArray());
        
        if (publisherOutput == null)
        {
            throw new FileNotFoundException($"Failure. RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync() returned NULL. This should never ever happen.");
        }

        if (publisherOutput.ComputedResults == null)
        {
            JghConsoleHelper.WriteLine($"Failure. RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync() returned a NULL ComputedResults object array - signifying a handled failure (needing to be reported).");
            JghConsoleHelper.WriteLine("This is what the user sees in the conversion report TextBlock on thr GUI when the svc method returns:");
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

    #endregion

    #region helper methods

    private static string ConvertOutputToResultsDtoXmlFileContents(ResultItem[] resultItems)
    {
        ResultDto[] resultsDto = ResultItem.ToDataTransferObject(resultItems);

        return JghSerialisation.ToXmlFromObject(resultsDto, new[] { typeof(ResultDto) });
    }

    private static void SaveWorkToHardDriveAsXml(string resultsDtoAsXml)
    {
        var outPutFileName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', DateTime.Now.ToString(JghDateTime.SortablePattern)) + "______ResultsSynthesisedByPublishingSvc" + "." + StandardFileTypeSuffix.Xml;

        var pathOfXmlFile = OutputFolderForXmlResults + @"\" + outPutFileName;

        File.WriteAllText(pathOfXmlFile, resultsDtoAsXml);

        JghConsoleHelper.WriteLineFollowedByOne($"The publishable results have been saved for perusal at your leisure.");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Folder", 30)} : {OutputFolderForXmlResults}");
        JghConsoleHelper.WriteLineFollowedByOne($"{JghString.LeftAlign("FileName", 30)} : {outPutFileName}");
    }


    #endregion


}