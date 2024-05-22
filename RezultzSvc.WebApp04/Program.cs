
using Microsoft.OpenApi.Models;
using NetStd.AzureStorageAccess.July2018;

namespace RezultzSvc.WebApp04
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
            // NB. must add Newtonsoft and override the default because the default serialiser based on System.Text.Json does not support Swagger as at March 2021.
            // Furthermore, I use Newtonsoft in my HTTP REST clients. System.Text.Json enforces camelCasing when it serialises, which is not what we want at all.
            // Adding NewtonSoft ensures that the intrinsic deserialisation of controller method requests and responses will be done by NewtonsSoft.
            // I have determined empirically that if you don't do this things will blow up sooner or later in .Net 6. Don't know about .Net 8.

            builder.Services.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // It says there that the following line, AddEndpointsApiExplorer, is required only if
            // this is a Minimal API, which it is not. I am using good old MVC Controllers where
            // AddEndpointsApiExplorer is called automatically inside AddControllers(). Therefore I have commented it out.

            //builder.Services.AddEndpointsApiExplorer();

            // Register the Swagger generator, defining one or more Swagger documents
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RezultzSvcMvcRaceResultsPublishing", Version = "v1" });
            });
        }

        private static void AddDependencyInjectionsToDependencyInjectionContainer(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAzureStorageAccessor, AzureStorageAccessor>();
        }

        private static void ConfigureApp(WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Configure the HTTP request pipeline.
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

    }
}