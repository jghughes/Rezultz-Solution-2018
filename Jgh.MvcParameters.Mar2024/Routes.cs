namespace Jgh.MvcParameters.Mar2024
{
    // you can add to the list of routes - but never never change any of the string constants of preexisting routes. the constants are compiled into all the apps that call service methods
    public static class Routes
    {
	    #region common

        public const string GetIfServiceIsAnswering = "GetIfServiceIsAnswering";
        public const string GetServiceEndpointsInfo = "GetServiceEndpointsInfo";

        #endregion

        #region AzureStorageController

        public const string AzureStorageController = "Rezultz/AzureStorage";

        public const string GetIfContainerExists = "GetIfContainerExists";
        public const string GetNamesOfBlobsInContainer = "GetNamesOfBlobsInContainer";
        public const string GetIfBlobExists = "GetIfBlobExists";
        public const string UploadBytesToBlockBlob = "UploadBytesToBlockBlob";
        public const string DownloadBlockBlobAsBytes = "DownloadBlockBlobAsBytes";
        public const string UploadStringToBlockBlob = "UploadStringToBlockBlob";
        public const string GetAbsoluteUriOfBlockBlob = "GetAbsoluteUriOfBlockBlob";
        public const string DeleteBlockBlobIfExists = "DeleteBlockBlobIfExists";

        #endregion

        #region LeaderboardResultsController

        public const string LeaderboardResultsController = "Rezultz/LeaderboardResults";

        public const string GetIfFileNameOfSeasonProfileIsRecognised = "GetIfFileNameOfSeasonProfileIsRecognised";
        public const string GetSeasonProfile = "GetSeasonProfile";
        public const string GetAllSeasonProfiles = "GetAllSeasonProfiles";
        public const string PopulateSingleEventWithResults = "PopulateSingleEventWithResults";
        public const string PopulateAllEventsInSingleSeriesWithAllResults = "PopulateAllEventsInSingleSeriesWithAllResults";

        #endregion

        #region RaceResultsPublishingController

        public const string RaceResultsPublishingController = "Rezultz/RaceResultsPublishing";

        public const string GetIfPublisherIdIsRecognised = "GetIfPublisherIdIsRecognised";
        public const string GetPublisherModuleProfile = "GetPublisherModuleProfile";
        public const string GetAllPublisherModuleId = "GetAllPublisherModuleId";
        public const string GetIllustrativeExampleOfRawDataset = "GetIllustrativeExampleOfRawDataset";
        public const string SaveFileToBeProcessedSubsequently = "SaveFileToBeProcessedSubsequently";
        public const string ComputeResultsForSingleEvent = "ComputeResultsForSingleEvent";
        public const string PublishFileOfCompletedResultsForSingleEvent = "PublishFileOfCompletedResultsForSingleEvent";

        #endregion

        #region TimeKeepingController

        public const string TimeKeepingController = "Rezultz/Timekeeping";

        public const string PostTimeStampItem = "PostTimeStampItem";
        public const string GetTimeStampItem = "GetTimeStampItem";
        public const string PostTimeStampItemArray = "PostTimeStampItemArray";
        public const string GetTimeStampItemArray = "GetTimeStampItemArray";

        #endregion

        #region ParticipantRegistrationController

        public const string ParticipantRegistrationController = "Rezultz/ParticipantRegistration";

        public const string PostParticipantItem = "PostParticipantItem";
        public const string GetParticipantItem = "GetParticipantItem";
        public const string PostParticipantItemArray = "PostParticipantItemArray";
        public const string GetParticipantItemArray = "GetParticipantItemArray";

        #endregion
    }
}
