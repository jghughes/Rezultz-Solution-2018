using CoreWCF.Configuration;
using CoreWCF.Description;
using RezultzSvc.WebApp03.Services;

// Note: for this inspiration, see https://github.com/CoreWCF/CoreWCF/blob/main/Documentation/Walkthrough.md

// and https://devblogs.microsoft.com/dotnet/corewcf-v1-released/

namespace RezultzSvc.WebApp03
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            AddDependencyInjectionsToDependencyInjectionContainer(builder);

            var app = builder.Build();

            ConfigureApp(app);

            app.Run();
        }
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddServiceModelServices();

            builder.Services.AddServiceModelMetadata();
        }

        private static void AddDependencyInjectionsToDependencyInjectionContainer(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            // Transient lifetime is required for the service to be created for each request

            builder.Services.AddTransient<RaceResultsPublishingSvcWcf>();
        }


        private static void ConfigureApp(WebApplication app)
        {
            // Configure WSDL to be available over https as well as http

            app.UseServiceModel(CoreWcfHelpers.ConfigureServices);

            var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();

            serviceMetadataBehavior.HttpsGetEnabled = true;
        }
    }
}
