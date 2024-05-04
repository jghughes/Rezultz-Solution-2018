using CoreWCF;

namespace RezultzSvc.WebApp01.Interfaces
{
    [ServiceContract(Name = "ITimeKeepingSvc", Namespace = "urn:wsdlnamespace.RezultzSvc.WebApp01")]
    public interface ITimeKeepingSvc
    {
        /* Don't fall into the trap of adding an 'Async' suffix to these operation contract names. The Wcf svc Wizard will do this automatically (when appropriate) when it generates your client proxy in the client */

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfServiceIsAnswering")]
        Task<bool> GetIfServiceIsAnsweringAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetServiceEndpointsInfo")]
        Task<string[]> GetServiceEndpointsInfoAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfContainerExists")]
        Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "PostTimeStampItem")]
        Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, string clockItemAsJson);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "PostTimeStampItemArray")]
        Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, byte[] itemArrayAsJsonAsCompressedBytes);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetTimeStampItem")]
        Task<string> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetTimeStampItemArray")]
        Task<byte[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer);

    }
}
