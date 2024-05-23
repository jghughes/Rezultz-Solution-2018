using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.DataTypes.Mar2024;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.SvcAgents;
using RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService;

// ReSharper disable UnusedMember.Local

namespace Tool08;

internal class Program
{
    private const string Description = "This console program (Tool08) is a scratchpad for testing RaceResultsPublishingSvcAgent, their injected Wcf or Mvc clients and services," +
                                       " and the C# publisher profile and module [PublisherForRezultzPortalTimingSystem2021]";

    private static async Task Main()
    {
        #region intro

        JghConsoleHelper.WriteLineFollowedByOne("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Test input folder for xml results exported from Rezultz Portal", LhsWidth)} : {InputFolderFromRezultz}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Test output folder for publishable xml results", LhsWidth)} : {OutputFolderForXmlResults}");
        JghConsoleHelper.WriteLinePrecededByOne("Are you ready to go? Press enter to continue.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineFollowedByOne("Working. Please wait...");

        #endregion

        try
        {
            #region get ready

            #region confirm existence of folders

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

            #region confirm existence of files of input data

            foreach (var timingDataFileTuple in ArrayOfRezultzPortalFileNameTuples)
            {
                var myFileInfo = new FileInfo(InputFolderFromRezultz + timingDataFileTuple.Item1);

                if (!myFileInfo.Exists)
                {
                    JghConsoleHelper.WriteLine($"Timing data file not found: {myFileInfo.Name}");
                    return;
                }
            }

            #endregion

            #region confirm existence of directories and files for outcome of processing data

            try
            {
                foreach (var item in FileOfResultsExportedFromRezultzPortal)
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

            #region load the timing data files from hard drive

            try
            {
                foreach (var tuple in ArrayOfRezultzPortalFileNameTuples)
                {
                    if (!Path.GetFileName(tuple.Item1).EndsWith($".{RequiredRezultzPortalFileFormat}"))
                    {
                        JghConsoleHelper.WriteLine($"Skipping {tuple.Item1} because it's not a {RequiredRezultzPortalFileFormat} file as you appear to have specified. Does this make sense?");
                        continue;
                    }

                    var item = new FileItem
                    {
                        FileInfo = new FileInfo(InputFolderFromRezultz + tuple.Item1),
                        FileContentsAsLines = new List<string>(),
                        OutputFileName = tuple.Item2
                    };

                    FileOfResultsExportedFromRezultzPortal.Add(item);

                    JghConsoleHelper.WriteLine($"Will look for {item.FileInfo.Name}.");
                }

                JghConsoleHelper.WriteLine();


                try
                {
                    foreach (var item in FileOfResultsExportedFromRezultzPortal)
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

                if (FileOfResultsExportedFromRezultzPortal.Count == 0)
                    throw new Exception("Found not even a single results file.");
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine($"Failed to locate designated results file. {e.Message}");
                JghConsoleHelper.WriteLine("");
                return;
            }

            JghConsoleHelper.WriteLine();

            #endregion

            #region optional - first things first - test if all our svc connectors are working - comment/uncomment out those you do or don't want at any point

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

            string seriesLabel;
            string eventLabel;
            string accountName;
            string containerName;

            SeasonProfileItem? seasonProfileItem;
            SeriesProfileItem seriesProfileItem;

            try
            {
                JghConsoleHelper.WriteLineFollowedByOne("leaderboardResultsSvcAgent.GetSeasonProfileAsync('998')");

                seasonProfileItem = await LeaderboardResultsSvcAgent.GetSeasonProfileAsync("998");

                JghConsoleHelper.WriteLineFollowedByOne($"answer = [{seasonProfileItem.Label}]");

                seriesProfileItem = seasonProfileItem.SeriesProfiles.Last();

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

            #region upload timing data files required for preprocessing

            foreach (var fileItem in FileOfResultsExportedFromRezultzPortal)
            {
                var prettyTimeNow1 = DateTime.Now.ToString(JghDateTime.SortablePattern);

                var fileName = fileItem.FileInfo.Name;

                var blobName3 = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', $"{prettyTimeNow1}____{fileName}");

                var didSucceed3 = await RaceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(IdentifierOfResultsFromRezultzPortal, new EntityLocationItem(accountName, containerName, blobName3), fileItem.FileContentsAsText,
                    CancellationToken.None);

                if (didSucceed3) FilesToBeProcessedByPublisherModule.Add(new PublisherImportFileTargetItem(IdentifierOfResultsFromRezultzPortal, blobName3));
            }

            #endregion

            #endregion

            #region optional - miscellaneous unrelated tests of the Wcf/Mvc services ability to handle json and csv upload/downloads - comment/uncomment out those you do or don't want at any point

            //JghConsoleHelper.WriteLineWrappedInOne("GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(Timestamps+2023-11-12T13-37-48.json)");
            //try
            //{
            //    var illustrativeJson = await RaceResultsPublishingSvcAgent.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync("Timestamps+2023-11-12T13-37-48.json");
            //    JghConsoleHelper.WriteLineFollowedByOne($"answer:");
            //    JghConsoleHelper.WriteLineFollowedByOne(illustrativeJson);
            //}
            //catch (Exception ex)
            //{
            //    JghConsoleHelper.WriteLineFollowedByOne($"the service threw a fault, most likely indicating a valid determination, for e.g. the test file isn't there :");
            //    JghConsoleHelper.WriteLineFollowedByOne(ex.Message);
            //}

            //JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(CrossMgr_USAC_sampleformat.xml)");
            //var illustrativeXml = await RaceResultsPublishingSvcAgent.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync("CrossMgr_USAC_sampleformat.xml");
            //JghConsoleHelper.WriteLineFollowedByOne($"answer:");
            //JghConsoleHelper.WriteLineWrappedInTwo(illustrativeXml);
            //JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(MyLapsTimingSystemExportOfCsvData.csv)");
            //var illustrativeCsv = await RaceResultsPublishingSvcAgent.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync("MyLapsTimingSystemExportOfCsvData.csv");
            //JghConsoleHelper.WriteLineFollowedByOne($"answer:");
            //JghConsoleHelper.WriteLineFollowedByOne(illustrativeCsv);
            //JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            #endregion

            #region optional miscellaneous precursor tests - comment/uncomment out those you do or don't want at any point

            //JghConsoleHelper.WriteLineFollowedByOne("GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync('21portal')");
            //var isRecognised = await RaceResultsPublishingSvcAgent.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync("21portal");
            //JghConsoleHelper.WriteLineFollowedByOne($"answer = {isRecognised}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();

            //JghConsoleHelper.WriteLineFollowedByOne("GetFileNameFragmentsOfAllPublishingProfilesAsync()");
            //var fileNamesArray = await RaceResultsPublishingSvcAgent.GetFileNameFragmentsOfAllPublishingProfilesAsync();
            //JghConsoleHelper.WriteLineFollowedByOne($"answer:");
            //foreach (var fileName in fileNamesArray)
            //    JghConsoleHelper.WriteLine(fileName);
            //JghConsoleHelper.WriteLinePrecededByOne("Press enter to continue to next test.");
            //JghConsoleHelper.ReadLine();


            //var testSeriesProfile = seasonProfileItem.SeriesProfiles.First();
            //var importFileTargets = new PublisherImportFileTargetItem[] { new("ResultItemsAsXml", "SplitIntervalsForParticipants+2023-11-12T13-41-32.xml") };
            //JghConsoleHelper.WriteLineFollowedByOne("ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(.... SplitIntervalsForParticipants+2023-11-12T13-41-32.xml)");

            //PublisherOutputItem outputOfProcessing =
            //    await RaceResultsPublishingSvcAgent.GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync("21portal", "TEST MTB Series 2022", "Dummy event #13 August 16th", testSeriesProfile, importFileTargets);
            //JghConsoleHelper.WriteLineFollowedByOne($"outputOfProcessing (in the form of a serialised PublisherOutPutItem:");
            //JghConsoleHelper.WriteLineWrappedInTwo(JghSerialisation.ToXmlFromObject(outputOfProcessing, new[] {typeof(PublisherOutputItem)}));
            //var results = ResultItem.ToDataTransferObject(outputOfProcessing.ComputedResults);
            //var uploadFileContents = JghSerialisation.ToXmlFromObject(results, new[] { typeof(ResultDto) });
            //JghConsoleHelper.WriteLineFollowedByOne("SendFileOfCompletedResultsForSingleEventAsync(....)");
            //var outcomeOfUploadOfResults = await RaceResultsPublishingSvcAgent.UploadFileOfCompletedResultsForSingleEventAsync(new EntityLocationItem("customertester", "testuploadcontainer", "mytestfile.xml"), uploadFileContents);
            //JghConsoleHelper.WriteLineFollowedByOne($"answer = {outcomeOfUploadOfResults}");
            //JghConsoleHelper.WriteLineFollowedByOne("Press enter to continue to next test.");
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
        JghConsoleHelper.WriteLineFollowedByOne("RaceResultsPublishingSvcAgent.ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync() ....");

        
        var publisherOutput = await RaceResultsPublishingSvcAgent.ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync("21portal", seriesLabel, eventLabel, seriesProfileItem, FilesToBeProcessedByPublisherModule.ToArray());

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

        return JghSerialisation.ToXmlFromObject(resultsDto, new[] {typeof(ResultDto)});
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

    #region constants

    private const string InputFolderFromRezultz = @"C:\Users\johng\holding pen\StuffFromRezultzAzure\PublishedPreprocessedResults\";

    private const string RequiredRezultzPortalFileFormat = "xml";

    private const string OutputFolderForXmlResults = @"C:\Users\johng\holding pen\StuffByJohn\Output\";

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

    private static readonly List<FileItem> FileOfResultsExportedFromRezultzPortal = new();

    private static readonly List<PublisherImportFileTargetItem> FilesToBeProcessedByPublisherModule = new();

    #endregion
}