To design the CoreWcf svc to enable debugging, you need to include at least one text endpoint - enabled for wsdl. This must be Http not https. The Https endpoint will be for production.

1. Create the text bindings in code. 

            var answer = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                Name = "MyHttpTextBinding", 
                CloseTimeout = TimeSpan.FromSeconds(20),
                OpenTimeout = TimeSpan.FromSeconds(20),
                ReceiveTimeout = TimeSpan.FromSeconds(20),
                SendTimeout = TimeSpan.FromSeconds(20),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
            };

            var answer = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
            {
                Name = "MyHttpsTextBinding",
                CloseTimeout = TimeSpan.FromSeconds(20),
                OpenTimeout = TimeSpan.FromSeconds(20),
                ReceiveTimeout = TimeSpan.FromSeconds(20),
                SendTimeout = TimeSpan.FromSeconds(20),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
            };

2. Add the url components for the text endpoints we are going to use for debugging. Do this as part of configuring the service. 

            private const string HttpTextSuffix = "/HttpText";
            private const string HttpsTextSuffix = "/HttpsText";
            private const string HttpsBinarySuffix = "/HttpsBinary"; - not used in this example, not for debugging. production only.
            private const string DataPreprocessorAddressPrefix = "/IRaceResultsPublishingSvc";

            serviceBuilder.AddServiceEndpoint<DataPublishingModuleSvc, IRaceResultsPublishingSvc>(BindingConfigs.GetHttpTextBinding(), DataPreprocessorAddressPrefix + HttpTextSuffix);
            serviceBuilder.AddServiceEndpoint<DataPublishingModuleSvc, IRaceResultsPublishingSvc>(BindingConfigs.GetHttpsTextBinding(), DataPreprocessorAddressPrefix + HttpsTextSuffix); 

3. Add some basic endpoint addresses for some urls in localhost in the appsettings.json file. These are the urls that the service will listen on when debugging locally.
    
            {
              "Urls": "http://localhost:5000;https://localhost:5001;", // added by JGH for debugging locally
              "Logging": {
                "LogLevel": {
                  "Default": "Information",
                  "Microsoft.AspNetCore": "Warning",
                  "RezultzSvc.CoreWcf.11.May2023.DataPublishingModuleSvc": "Information"
                }
              },
              "AllowedHosts": "*"
            }

4. In Program.cs, cater for wsdl metadata discovery behaviour. Explicitly cater for Https as well as Http.

            builder.Services.AddServiceModelMetadata();
            builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            app.UseServiceModel(CoreWcfHelpers.ConfigureServices);
            var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
            serviceMetadataBehavior.HttpsGetEnabled = true;

5. To see if we have done all the right things to get ready for debugging, start up the svc in debug mode. In Solution explorer go Debug>Start without debugging. The svc will be listening on the urls specified in the appsettings.json file, with the wsdl behaviour automatically added to the text endpoints. This is what you should see in the console window that materialises in the system tray:

            info: CoreWCF.Channels.ServiceModelHttpMiddleware[0]
                  Mapping CoreWCF branch app for path /IRaceResultsPublishingSvc/HttpText
            info: CoreWCF.Channels.ServiceModelHttpMiddleware[0]
                  Mapping CoreWCF branch app for path /IRaceResultsPublishingSvc/HttpsText
            info: CoreWCF.Channels.ServiceModelHttpMiddleware[0]
                  Mapping CoreWCF branch app for path /IRaceResultsPublishingSvc/HttpsBinary
            info: CoreWCF.Channels.MetadataMiddleware[0]
                  Configuring metadata to /IRaceResultsPublishingSvc/HttpText
            info: CoreWCF.Channels.MetadataMiddleware[0]
                  Configuring metadata to /IRaceResultsPublishingSvc/HttpsText
            info: Microsoft.Hosting.Lifetime[14]
                  Now listening on: http://localhost:5000
            info: Microsoft.Hosting.Lifetime[14]
                  Now listening on: https://localhost:5001
            info: Microsoft.Hosting.Lifetime[0]
                  Application started. Press Ctrl+C to shut down.
            info: Microsoft.Hosting.Lifetime[0]
                  Hosting environment: Development
            info: Microsoft.Hosting.Lifetime[0]
                  Content root path: C:\Users\johng\source\repos\Rezultz Solution 2018\RezultzSvc.CoreWcf.11.May2023\

6. To debug the guts of the svc, I use the svcutil.exe tool (the wcf service wizard) to generate a client proxy in a suitable client test-harness project/class library. In this here Solution, the library project is RezultzSvc.Wcf.Clients.March2022. I can then call the service and step through the svc code during execution like normal. When running the wcf svc wizard to set up the client proxy to connect to localhost, it doesn't seem to matter if you choose the http or https protocol, but I usually play safe and use the http endpoint on localhost. BUT debugging MUST be done in plain text. It cannot be done using the binary endpoint: this will generate an inscrutable error message upon attempting to call the svc during debugging, to whit "The certificate authority is invalid or incorrect."

            http://localhost:5000/IRaceResultsPublishingSvc/HttpText?wsdl

8. To debug the svc calls, you need need to ensure that the service client project is set up appropriately. In the service client class, RaceResultsPublishingServiceClientWcf, go into the private method where we need to specify the binding as text not binary - CreateSvcProxy(). In the ctor of the svc client proxy in this method, opt for the (wizard generated) enum for the plain HttpText endpoint. Be sure to comment out the binary enum (MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc). You can now proceed with your doubtless many lengthy debugging sessions.
   
            _svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpTextBinding_IRaceResultsPublishingSvc);

7. When debugging is complete, I publish the svc to production in Azure and generally regenerate the production svc client using the Wizard from the https endpoint. I use https over http for no reason whatsoever. Http would be equally good.

              https://rezultzsvccorewcf11.azurewebsites.net/IRaceResultsPublishingSvc/HttpsText?wsdl
    
8. Finally, when I am ready to recomple the svc for upload to production, I must go into the private CreateSvcProxy() method again and
    specify my preferred enum for the binary endpoint. In real life, I choose binary transmission becuase is more performant.. 

            _svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc); //when svc deployed to Azure and running



---- 0000 ----

1st April 2024
