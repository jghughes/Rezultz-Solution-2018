using CoreWCF.Configuration;
using CoreWCF.Description;
using RezultzSvc.WebApp01.Services;

namespace RezultzSvc.WebApp01;

// Note: for this inspiration, see https://github.com/CoreWCF/CoreWCF/blob/main/Documentation/Walkthrough.md

// and https://devblogs.microsoft.com/dotnet/corewcf-v1-released/

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

        // Transient lifetime is required for services to be created for each request

        builder.Services.AddTransient<AzureStorageSvcWcf>();

        builder.Services.AddTransient<LeaderboardResultsSvcWcf>();

        builder.Services.AddTransient<ParticipantRegistrationSvcWcf>();

        builder.Services.AddTransient<TimeKeepingSvcWcf>();
    }

    private static void ConfigureApp(WebApplication app)
    {
        app.UseServiceModel(CoreWcfHelpers.ConfigureServices);

        // Configure WSDL to be available over https

        var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();

        serviceMetadataBehavior.HttpsGetEnabled = true;
    }
}
