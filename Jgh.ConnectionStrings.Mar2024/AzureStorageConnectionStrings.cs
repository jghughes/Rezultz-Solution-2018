namespace Jgh.ConnectionStrings.Mar2024
{
    /// <summary>
    /// This class is the one and only place where all Azure storage connection strings are recorded and maintained (or should be the one and only!).
    /// If new accounts are created, or passwords change, this is the place to reflect the updates.
    /// For usage, see RezultzSvc.Library01.Mar2024 and RezultzSvc.Library02.Mar2024
    /// </summary>
    public class AzureStorageConnectionStrings
    {

        public const string AzureStorageAccountOfEntryPointToRezultzDataStorageHierarchy = @"systemrezultzlevel1";

        public const string AzureStorageContainerForSeasonProfiles = @"seasonprofiles";

        public const string AzureStorageContainerForPublishingModuleProfiles = @"publishingmoduleprofiles";

        public const string AzureStorageContainerForExamplesOfDatasetsUsedInPublishingProcess = @"examplesofdatasetsusedinpublishingprocess";

        public static  string[] GetAzureStorageAccountConnectionStrings()
        {
            string[] allAzureStorageConnectionStrings = new[]
            {
                "DefaultEndpointsProtocol=https;AccountName=systemrezultzlevel1;AccountKey=uMCoUvu9y3mlam/YSLhEXZ31NmZY4ErNI+sH74cG+rzLtG904ZXYE3RrqNBtMj9Ahn5myVXsOiUonAomlQMvUQ==;EndpointSuffix=core.windows.net;",
                "DefaultEndpointsProtocol=https;AccountName=coldstorageaccount;AccountKey=zMTIhP26G5P9+mjkj77dgVMhPDOoPF39diZYx16ey68jCmxeRICQobAty+ZOjUO/wgLUw8YkL4WDZH+YP5UL+w==;EndpointSuffix=core.windows.net;",
                "DefaultEndpointsProtocol=https;AccountName=customerkelso;AccountKey=J9qnP5vKR06CnwRT438DAF27dCJT1hYeSe5ZgThrn8hb7U92EAqTdJgK/E4ubQEZF/ynFa8wDyVRBRnWxEXTYA==;EndpointSuffix=core.windows.net;",
                "DefaultEndpointsProtocol=https;AccountName=customertester;AccountKey=X4F7GjmE1y+rH3cmXvHuO49XxDXB5Q1AMwa6KPiY+knOKwYu9OeBUbmtEw5zhDhyX6v1apr5nu8sJpOjJvUvRQ==;EndpointSuffix=core.windows.net;",
            };

        return allAzureStorageConnectionStrings;
    }

}
}
