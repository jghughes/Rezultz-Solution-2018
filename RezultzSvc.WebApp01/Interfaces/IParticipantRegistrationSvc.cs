using CoreWCF;

namespace RezultzSvc.WebApp01.Interfaces
{
    [ServiceContract(Name = "IParticipantRegistrationSvc", Namespace = "urn:wsdlnamespace.RezultzSvc.WebApp01")]
    public interface IParticipantRegistrationSvc
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
        [OperationContract(Name = "PostParticipantItem")]
        Task<string> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, string participantItemAsJson);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "PostParticipantItemArray")]
        Task<string> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, byte[] itemArrayAsJsonAsCompressedBytes);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetParticipantItem")]
        Task<string> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey);
        
        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetParticipantItemArray")]
        Task<byte[]> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer);

    }
}
