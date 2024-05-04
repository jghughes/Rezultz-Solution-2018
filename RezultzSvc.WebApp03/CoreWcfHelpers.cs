using CoreWCF;
using CoreWCF.Configuration;
using RezultzSvc.WebApp03.Interfaces;
using RezultzSvc.WebApp03.Services;

namespace RezultzSvc.WebApp03
{
    // Note: For debug in localhost, the leading constituents of the endpoint addresses
    // ({URI-scheme}://{URI-host}) are defined by convention in appsettings.json.

    //(endpoints http://localhost:5000 and https://localhost:5001 are specified in appsettings.json)

    // Here in the ConfigureServices() method is where we specify the remainder of the
    // endpoint addresses, namely the resource path, such as "/IRaceResultsPublishingSvc/HttpText".
    // For example, if the URI-scheme is "https" and the URI-host is "localhost:5001"
    // the service address is "https://localhost:5001/IRaceResultsPublishingSvc/HttpsText"

    // http://localhost:5000/IRaceResultsPublishingSvc/HttpText?wsdl use in the client svc wizard to point at svc running on localhost (endpoint http://localhost:5000 is specified in appsettings.json)
    // https://rezultzsvccorewcf11.azurewebsites.net/IRaceResultsPublishingSvc/HttpsText?wsdl use in the client service wizard for to point at svc deployed to azure
    // https://rezultzsvccorewcf11.azurewebsites.net/IRaceResultsPublishingSvc/HttpsBinary use in production in the connector to the client to point at svc deployed to Azure

    // use HttpText in the client when we wish to intercept the server traffic in human readable format with a proxy such as Fiddler

    // binary is faster than text for large data transfers. our default in production 

    internal static class CoreWcfHelpers
	{
        private const string HttpTextSuffix = "/HttpText";
        private const string HttpsTextSuffix = "/HttpsText";
        private const string HttpsBinarySuffix = "/HttpsBinary";

        private const string DataPreprocessorAddressPrefix = "/IRaceResultsPublishingSvc";

        public static void SpecifyServiceOptions(ServiceOptions serviceOptions)
        {
            serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
        }

        public static void ConfigureServices(IServiceBuilder serviceBuilder)
        {
            #region configure RaceResultsPublishingSvcWcf 

            serviceBuilder.AddService<RaceResultsPublishingSvcWcf>(SpecifyServiceOptions);

            serviceBuilder.AddServiceEndpoint<RaceResultsPublishingSvcWcf, IRaceResultsPublishingSvc>(BindingConfigs.GetHttpTextBinding(), DataPreprocessorAddressPrefix + HttpTextSuffix);

            serviceBuilder.AddServiceEndpoint<RaceResultsPublishingSvcWcf, IRaceResultsPublishingSvc>(BindingConfigs.GetHttpsTextBinding(), DataPreprocessorAddressPrefix + HttpsTextSuffix); 

            serviceBuilder.AddServiceEndpoint<RaceResultsPublishingSvcWcf, IRaceResultsPublishingSvc>(BindingConfigs.GetCustomBindingWithHttpsBinaryEncoding(), DataPreprocessorAddressPrefix + HttpsBinarySuffix);

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