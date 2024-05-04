using CoreWCF;

namespace RezultzSvc.WebApp01.Interfaces
{
    [ServiceContract(Name = "ILeaderboardResultsSvc", Namespace = "urn:wsdlnamespace.RezultzSvc.WebApp01")]
    public interface ILeaderboardResultsSvc
    {
        /* Don't fall into the trap of adding an 'Async' suffix to these operation contract names. The Wcf svc Wizard will do this automatically (when appropriate) when it generates your client proxy in the client */

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfServiceIsAnswering")]
        Task<bool> GetIfServiceIsAnsweringAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetServiceEndpointsInfo")]
        Task<string[]> GetServiceEndpointsInfoAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfFileNameOfSeasonProfileIsRecognised")]
        Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetSeasonProfile")]
        Task<byte[]> GetSeasonProfileAsync(string profileFileNameFragment);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetAllSeasonProfiles")]
        Task<byte[]> GetAllSeasonProfilesAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "PopulateSingleEventWithResults")]
        Task<byte[]> PopulateSingleEventWithResultsAsync(byte[] eventItemAsJsonAsCompressedBytes);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "PopulateAllEventsInSingleSeriesWithAllResults")]
        Task<byte[]> PopulateAllEventsInSingleSeriesWithAllResultsAsync(byte[] seriesItemAsJsonAsCompressedBytes);


    }
}
