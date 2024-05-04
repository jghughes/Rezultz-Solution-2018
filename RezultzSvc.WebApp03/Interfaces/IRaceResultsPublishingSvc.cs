using CoreWCF;

namespace RezultzSvc.WebApp03.Interfaces

{
    [ServiceContract(Name = "IRaceResultsPublishingSvc", Namespace = "urn:wsdlnamespace.RezultzSvc.WebApp03")]
    public interface IRaceResultsPublishingSvc
    {
        /* Don't fall into the trap of adding an 'Async' suffix to these operation contract names. The Wcf svc Wizard will do this automatically (when appropriate) when it generates your client proxy in the client */

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfServiceIsAnswering")]
        Task<bool> GetIfServiceIsAnsweringAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetServiceEndpointsInfo")]
        Task<string[]> GetServiceEndpointsInfoAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIfFileNameFragmentOfPublishingProfileIsRecognised")]
        Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetPublishingProfile")]
        Task<string> GetPublishingProfileAsync(string fileNameFragment);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetFileNameFragmentsOfAllPublishingProfiles")]
        Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync();

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetIllustrativeExampleOfDatasetExpectedForProcessing")]
        Task<string> GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(string fileNameWithExtension);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "UploadDatasetToBeProcessedSubsequently")]
        Task<bool> UploadDatasetToBeProcessedSubsequentlyAsync(string identifierOfDataset, string accountName, string containerName, string fileName, string datasetAsRawString); 

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasets")]
        Task<string> GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(string publisherInputDtoAsJson);

        [FaultContract(typeof(JghFault))]
        [OperationContract(Name = "UploadFileOfCompletedResultsForSingleEvent")]
        Task<bool> UploadFileOfCompletedResultsForSingleEventAsync(string accountName, string containerName, string fileName, string completedResultsAsXml);

    }
}
