namespace Jgh.MvcParameters.Mar2024
{
    public static class QueryParameters
    {
        public const string Account = "Account";
        public const string Container = "Container";
        public const string Blob = "Blob";

        #region AzureController

        public const string CreateContainerIfNotExist = "CreateContainerIfNotExist";
	    public const string RequiredSubstring = "RequiredSubstring";
	    public const string MustPrintDescriptionAsOpposedToBlobName = "MustPrintDescriptionAsOpposedToBlobName";

        #endregion

        #region LeaderboardResultsController

        public const string SeasonId = "SeasonId";
		public const string UserName = "UserName";

        #endregion

        #region RaceResultsPublishingController

        public const string DatasetIdentifier = "DatasetIdentifier";
        public const string FileNameFragment = "FileNameFragment"; // pattern is prefix-fragment.xml
        public const string FileNameWithExtension= "FileNameWithExtension"; 

        #endregion

        #region TimeKeepingController and ParticipantRegistrationController

        public const string TablePartition = "TablePartition";
		public const string TableRowKey = "TableRowKey";
		public const string Fragment = "Fragment";

		#endregion

	}
}
