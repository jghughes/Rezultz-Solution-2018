using CoreWCF;

namespace RezultzSvc.WebApp01.Interfaces
{
    [ServiceContract(Name = "IAzureStorageSvc", Namespace = "urn:wsdlnamespace.RezultzSvc.WebApp01")]
    public interface IAzureStorageSvc
    {
        /* Don't fall into the trap of adding an 'Async' suffix to these operation contract names. The Wcf svc Wizard will do so automatically (when appropriate) when it generates your client proxy in the client */

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfServiceIsAnswering")]
        Task<bool> GetIfServiceIsAnsweringAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetServiceEndpointsInfo")]
        Task<string[]> GetServiceEndpointsInfoAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfContainerExists")]
        Task<bool> GetIfContainerExistsAsync(string accountName, string containerName);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetNamesOfBlobsInContainer")]
        Task<string[]> GetNamesOfBlobsInContainerAsync(string accountName, string containerName, string contains, bool mustPrintDescriptionAsOpposedToBlobName);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfBlobExists")]
        Task<bool> GetIfBlobExistsAsync(string accountName, string containerName, string blobName);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetAbsoluteUriOfBlockBlob")]
        Task<string> GetAbsoluteUriOfBlockBlobAsync(string accountName, string containerName, string blobName);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "DeleteBlockBlobIfExistsAsync")]
        Task<bool> DeleteBlockBlobIfExistsAsync(string accountName, string containerName, string blobName);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "UploadStringToBlockBlob")]
        Task<bool> UploadStringToBlockBlobAsync(string accountName, string containerName, string blobName, bool createContainerIfNotExists, string content);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "UploadBytesToBlockBlob")]
        Task<bool> UploadBytesToBlockBlobAsync(string accountName, string containerName, string blobName, bool createContainerIfNotExists, byte[] content);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "DownloadBlockBlob")]
        Task<byte[]> DownloadBlockBlobAsync(string accountName, string containerName, string blobName);
    }
}