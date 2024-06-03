namespace RezultzSvc.Clients.Mvc.Mar2023
{
    /// <summary>
    ///     Be very careful when messing with the localhost versus remote Endpoints. These constants
    ///     are ingested at compile-time in many different client projects, not just the one you are
    ///     working on. The remote endpoints are for production. The localhost endpoints are for local
    ///     development and testing. To find out what the constants are, in RezultzSvc.WebApp02,
    ///     go Properties>Debug>General>Open debug launch profiles UI>http>AppUrl where you will
    ///     see what the app template has allocated in its wisdom for http i.e. http://localhost:5196.
    ///     Then go to >https>AppUrl where you will see what the wizard has allocated for https
    ///     i.e. https://localhost:7285;http://localhost:5196. Copy and paste these into the constants
    ///     below. At the time of writing, we have all five services in a single MVC website. This is different
    ///     to the WCF counterparts where the Publishing svc has been moved into a standalone webapp.
    /// </summary>
    public static class AppSettings
    {
        // localhost for debugging
        //public const string AzureStorageControllerBaseUrl = "http://localhost:5196";
        //public const string LeaderboardResultsControllerBaseUrl = "http://localhost:5196";
        //public const string TimeKeepingControllerBaseUrl = "http://localhost:5196";
        //public const string ParticipantRegistrationControllerBaseUrl = "http://localhost:5196";
        //public const string RaceResultsPublishingControllerBaseUrl = "http://localhost:5052"; // NB separate WebApp

        // localhost
        //public const string AzureStorageControllerBaseUrl = "https://localhost:7285";
        //public const string LeaderboardResultsControllerBaseUrl = "https://localhost:7285";
        //public const string TimeKeepingControllerBaseUrl = "https://localhost:7285";
        //public const string ParticipantRegistrationControllerBaseUrl = "https://localhost:7285";
        //public const string RaceResultsPublishingControllerBaseUrl = "https://localhost:7178"; // NB separate WebApp

        // remote Kestrel host for production - .Net 8 WebApp  
        public const string AzureStorageControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string LeaderboardResultsControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string TimeKeepingControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string ParticipantRegistrationControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string RaceResultsPublishingControllerBaseUrl = "https://rezultzsvcmvc11.azurewebsites.net"; // NB separate WebApp

    }
}