using CoreWCF;
using CoreWCF.Configuration;
using RezultzSvc.WebApp01.Interfaces;
using RezultzSvc.WebApp01.Services;

namespace RezultzSvc.WebApp01
{
    // Note: For debug in localhost, the leading constituents of the endpoint addresses
    // ({URI-scheme}://{URI-host}) are defined by convention in appsettings.json.

    //(endpoints http://localhost:5000 and https://localhost:5001 are specified in appsettings.json)

    // Here in the ConfigureServices() method is where we specify the remainder of the
    // endpoint addresses, namely the resource path, such as "/IAzureStorageSvc/HttpText".
    // For example, if the URI-scheme is "https" and the URI-host is "localhost:5001"
    // the service address is "https://localhost:5001//IAzureStorageSvc/HttpsText"

    // http://localhost:5000//IAzureStorageSvc/HttpText?wsdl use in the client svc wizard to point at svc running on localhost 
    // https://rezultzsvccorewcf.azurewebsites.net//IAzureStorageSvc/HttpsText?wsdl use in the client service wizard for to point at svc deployed to azure
    // https://rezultzsvccorewcf.azurewebsites.net//IAzureStorageSvc/HttpsBinary use in production in the connector to the client to point at svc deployed to Azure

    // use HttpText in the client when we wish to intercept the server traffic in human readable format with a proxy such as Fiddler

    // binary is faster than text for large data transfers. our default in production 

    internal static class CoreWcfHelpers
	{
        private const string HttpTextSuffix = "/HttpText";
        private const string HttpsTextSuffix = "/HttpsText";
        private const string HttpsBinarySuffix = "/HttpsBinary";

        private const string AzureStorageAddressPrefix = "/IAzureStorageSvc";
        private const string LeaderboardResultsSvcAddressPrefix = "/ILeaderboardResultsSvc";
        private const string ParticipantRegistrationSvcAddressPrefix = "/IParticipantRegistrationSvc";
        private const string TimeKeepingSvcAddressPrefix = "/ITimeKeepingSvc";


        public static void SpecifyServiceOptions(ServiceOptions serviceOptions)
        {
            serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
        }

        public static void ConfigureServices(IServiceBuilder serviceBuilder)
        {
            #region configure IAzureStorageSvc 

            serviceBuilder.AddService<AzureStorageSvcWcf>(SpecifyServiceOptions);

            serviceBuilder.AddServiceEndpoint<AzureStorageSvcWcf, IAzureStorageSvc>(BindingConfigs.GetHttpTextBinding(), AzureStorageAddressPrefix + HttpTextSuffix);

            serviceBuilder.AddServiceEndpoint<AzureStorageSvcWcf, IAzureStorageSvc>(BindingConfigs.GetHttpsTextBinding(), AzureStorageAddressPrefix + HttpsTextSuffix);

            serviceBuilder.AddServiceEndpoint<AzureStorageSvcWcf, IAzureStorageSvc>(BindingConfigs.GetCustomBindingWithHttpsBinaryEncoding(), AzureStorageAddressPrefix + HttpsBinarySuffix);

            #endregion

            #region configure ILeaderboardResultsSvc 

            serviceBuilder.AddService<LeaderboardResultsSvcWcf>(SpecifyServiceOptions);

            serviceBuilder.AddServiceEndpoint<LeaderboardResultsSvcWcf, ILeaderboardResultsSvc>(BindingConfigs.GetHttpTextBinding(), LeaderboardResultsSvcAddressPrefix + HttpTextSuffix);

            serviceBuilder.AddServiceEndpoint<LeaderboardResultsSvcWcf, ILeaderboardResultsSvc>(BindingConfigs.GetHttpsTextBinding(), LeaderboardResultsSvcAddressPrefix + HttpsTextSuffix);

            serviceBuilder.AddServiceEndpoint<LeaderboardResultsSvcWcf, ILeaderboardResultsSvc>(BindingConfigs.GetCustomBindingWithHttpsBinaryEncoding(), LeaderboardResultsSvcAddressPrefix + HttpsBinarySuffix);

            #endregion

            #region configure IParticipantRegistrationSvc 

            serviceBuilder.AddService<ParticipantRegistrationSvcWcf>(SpecifyServiceOptions);

            serviceBuilder.AddServiceEndpoint<ParticipantRegistrationSvcWcf, IParticipantRegistrationSvc>(BindingConfigs.GetHttpTextBinding(), ParticipantRegistrationSvcAddressPrefix + HttpTextSuffix);

            serviceBuilder.AddServiceEndpoint<ParticipantRegistrationSvcWcf, IParticipantRegistrationSvc>(BindingConfigs.GetHttpsTextBinding(), ParticipantRegistrationSvcAddressPrefix + HttpsTextSuffix);

            serviceBuilder.AddServiceEndpoint<ParticipantRegistrationSvcWcf, IParticipantRegistrationSvc>(BindingConfigs.GetCustomBindingWithHttpsBinaryEncoding(), ParticipantRegistrationSvcAddressPrefix + HttpsBinarySuffix);

            #endregion

            #region configure ITimeKeepingSvc 

            serviceBuilder.AddService<TimeKeepingSvcWcf>(SpecifyServiceOptions);

            serviceBuilder.AddServiceEndpoint<TimeKeepingSvcWcf, ITimeKeepingSvc>(BindingConfigs.GetHttpTextBinding(), TimeKeepingSvcAddressPrefix + HttpTextSuffix);

            serviceBuilder.AddServiceEndpoint<TimeKeepingSvcWcf, ITimeKeepingSvc>(BindingConfigs.GetHttpsTextBinding(), TimeKeepingSvcAddressPrefix + HttpsTextSuffix);

            serviceBuilder.AddServiceEndpoint<TimeKeepingSvcWcf, ITimeKeepingSvc>(BindingConfigs.GetCustomBindingWithHttpsBinaryEncoding(), TimeKeepingSvcAddressPrefix + HttpsBinarySuffix);

            #endregion

            #region to be deleted

            //serviceBuilder.AddService<AzureStorageSvcWcf>(SpecifyServiceOptions);

            //serviceBuilder.AddServiceEndpoint<AzureStorageSvcWcf, IAzureStorageSvc>(BindingConfigs.GetBasicHttpsBinding(), "/IAzureStorageSvc/Text"); // handy for wsdl

            //serviceBuilder.AddServiceEndpoint<AzureStorageSvcWcf, IAzureStorageSvc>(BindingConfigs.GetCustomBindingWithBinaryEncoding(), "/IAzureStorageSvc/Binary"); // lighter and faster than text for large data transfers


            //serviceBuilder.AddService<ParticipantRegistrationSvcWcf>(SpecifyServiceOptions);

            //serviceBuilder.AddServiceEndpoint<ParticipantRegistrationSvcWcf, IParticipantRegistrationSvc>(BindingConfigs.GetBasicHttpsBinding(), "/IParticipantRegistrationSvc/Text"); 

            //serviceBuilder.AddServiceEndpoint<ParticipantRegistrationSvcWcf, IParticipantRegistrationSvc>(BindingConfigs.GetCustomBindingWithBinaryEncoding(), "/IParticipantRegistrationSvc/Binary");


            //serviceBuilder.AddService<LeaderboardResultsSvcWcf>(SpecifyServiceOptions);

            //serviceBuilder.AddServiceEndpoint<LeaderboardResultsSvcWcf, ILeaderboardResultsSvc>(BindingConfigs.GetBasicHttpsBinding(), "/ILeaderboardResultsSvc/Text");

            //serviceBuilder.AddServiceEndpoint<LeaderboardResultsSvcWcf, ILeaderboardResultsSvc>(BindingConfigs.GetCustomBindingWithBinaryEncoding(), "/ILeaderboardResultsSvc/Binary");


            //serviceBuilder.AddService<TimeKeepingSvcWcf>(SpecifyServiceOptions);

            //serviceBuilder.AddServiceEndpoint<TimeKeepingSvcWcf, ITimeKeepingSvc>(BindingConfigs.GetBasicHttpsBinding(), "/ITimeKeepingSvc/Text");

            //serviceBuilder.AddServiceEndpoint<TimeKeepingSvcWcf, ITimeKeepingSvc>(BindingConfigs.GetCustomBindingWithBinaryEncoding(), "/ITimeKeepingSvc/Binary");

            #endregion

        }

        public static string[] PrettyPrintOperationContextInfo(OperationContext context)
        {
            var endpoints = context.Host.Description.Endpoints;

            var answers = new List<string>();
            var i = 1;

            foreach (var endpoint in endpoints)
            {
                answers.Add($"{i}");
                answers.Add($"Name             {endpoint.Name}");
                answers.Add($"Address          {endpoint.Address}");
                answers.Add($"ListenUri        {endpoint.ListenUri}");
                answers.Add($"Binding.Name     {endpoint.Binding?.Name}");
                answers.Add($"Contract.Name    {endpoint.Contract.Name}");
                answers.Add($"Address.Uri      {endpoint.Address.Uri}");

                i += 1;
            }

            return answers.ToArray();
        }

    }
}