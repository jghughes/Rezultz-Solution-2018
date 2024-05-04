using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using NetStd.AzureStorageAccess.July2018;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

ConfigureServices(builder.Services);

RegisterDependencyInjections(builder.Services, true);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

void ConfigureServices(IServiceCollection services)
{
    // NB. must add Newtonsoft and override the default because the default serialiser based on System.Text.Json does not support Swagger as at March 2021.
    // Furthermore, I use Newtonsoft in my HTTP REST clients. System.Text.Json enforces camelCasing when it serialises, which is not what we want at all. Adding NewtonSoft ensures
    // that the intrinsic deserialisation of controller method requests and responses will be done by NewtonsSoft. I have determined empirically that if you don't do this things will blow up sooner or later in .Net 6
    services.AddControllers().AddNewtonsoftJson();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();

    // Register the Swagger generator, defining one or more Swagger documents
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "RezultzSvc.Mvc", Version = "v1" });
    });

}

void RegisterDependencyInjections(IServiceCollection services, bool mustUseInMemoryDataService)
{
    // our mock and production services are one and the same thing for now

    if (mustUseInMemoryDataService)
    {
        // nothing at time of writing
    }
    else
    {
        // todo convert the in-memory stores into xml or json data files and store/retrieve them as blobs. saves recompiling and publishing the svc when the data changes
        // todo better still, avoid duplication (massively) and merely import the preexisting profiles and settings documents and ingest them into the same repository we already have in NetStd.Rezultz01 
    }

    services.AddTransient<IAzureStorageAccessor, AzureStorageAccessor>();

}

