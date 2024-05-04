namespace NetStd.Azure.Types.July2018
{
    public static class AzureStorageTypeHelpers
    {
        public static T2 CopyAzureStorageLocationItemPropertiesFromSourceToTarget<T1, T2>(T1 source, T2 target)
            where T2 : AzureStorageLocationItem, new() where T1 : AzureStorageLocationItem
        {
            if (source == null) return target;

            if (target == null) target = new T2();

            target.AzureStorageContainerName = source.AzureStorageContainerName;
            target.AzureStorageAccountBlobServiceEndpoint = source.AzureStorageAccountBlobServiceEndpoint;
            target.AzureStorageAccountConnectionString = source.AzureStorageAccountConnectionString;
            target.AzureStorageAccountName = source.AzureStorageAccountName;
            target.BlobName = source.BlobName;
            target.BlobUrl = source.BlobUrl;
            target.ID = source.ID;
            return target;
        }
    }
}