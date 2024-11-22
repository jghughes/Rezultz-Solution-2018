To test the MVC services, the idea is to launch the svc so that it is running locally in localhost and tested via one or more clients, whether simply the browser or a custom built client project. Running on localhost you can step through the running service in the debugger. To accomplish this use a separate set of service endpoint urls for debugging. I store the production endpoints in the MVC svc clients project RezultzSvc.Clients.Mvc.Mar2023, class = AppSettings. If starting fresh, you determine the production endpoints for a service by going into the Azure Portal> App Services>RezultzSvcMvc or >RezultzSvcMvc11 > Default domains. Click on the domains and your browser attempts to load the routes. At the time of writing my services are Swagger enabled to display the Swagger endpoints in the OpenAPi browser for debugging. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle.

        https://rezultzsvcmvc.azurewebsites.net/swagger
        https://rezultzsvcmvc11.azurewebsites.net/swagger


        // uncomment the following line to disable Swagger in production
        // if (!app.Environment.IsDevelopment()) return; 

        // Configure the HTTP request pipeline. Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();
        app.UseSwaggerUI();

        // Register the Swagger generator, defining one or more Swagger documents
        builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "RezultzSvcMvc", Version = "v1" });});

The URLs for both production and debug are stored in RezultzSvc.Clients.Mvc.Mar2023.AppSettings. Comment in or out the URLs you want depending on whether you are in production or debug and whther you are http or https. Switch back and forth between options for full testing. The urls below are the base urls in production and what you must specify for the build you Publish:-

       // remote Kestrel host for production - .Net 8 WebApp  
        public const string AzureStorageControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string LeaderboardResultsControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string TimeKeepingControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string ParticipantRegistrationControllerBaseUrl = "https://rezultzsvcmvc.azurewebsites.net";
        public const string RaceResultsPublishingControllerBaseUrl = "https://rezultzsvcmvc11.azurewebsites.net"; // NB separate WebApp - mvc11

The localhost endpoints below are for local development and testing. To find out what the ports are for the projects RezultzSvc.WebApp02 and RezultzSvc.WebApp04, go Properties>Debug>General>Open debug launch profiles UI>http>AppUrl where you will see what the app template has allocated in its wisdom for http i.e. http://localhost:5196 and for https i.e. https://localhost:7285;http://localhost:5196. Copy and paste these into the URLS. As long as the settings in launchSettings.json are correct, you are free to debug using http or https interchangeably. http is the place to start because it eliminates a failure point, but https is what the svc will run in production.

        // localhost http for debugging
        public const string AzureStorageControllerBaseUrl = "http://localhost:5196";
        public const string LeaderboardResultsControllerBaseUrl = "http://localhost:5196";
        public const string TimeKeepingControllerBaseUrl = "http://localhost:5196";
        public const string ParticipantRegistrationControllerBaseUrl = "http://localhost:5196";
        public const string RaceResultsPublishingControllerBaseUrl = "http://localhost:5052"; // NB separate WebApp

        // localhost https for debugging
        public const string AzureStorageControllerBaseUrl = "https://localhost:7285";
        public const string LeaderboardResultsControllerBaseUrl = "https://localhost:7285";
        public const string TimeKeepingControllerBaseUrl = "https://localhost:7285";
        public const string ParticipantRegistrationControllerBaseUrl = "https://localhost:7285";
        public const string RaceResultsPublishingControllerBaseUrl = "https://localhost:7178"; // NB separate WebApp


To see if we have done all the right things to get ready for debugging, the plan is to start up the svc on its own in debug mode. Select the project in Solution Explorer and set the debug protocol to http or https to match the endpoints you are using. Now right click the project > Set as Startup Project and then in Solution explorer go Debug>Start without debugging. In the active Debug console it will be displayed that the service is up and running on localhost in the development environment.  This is what you should see in the terminal window. To display the terminal widow, click its icon in the system tray. 

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5196
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\johng\Source\Repos\Rezultz-Solution-2018\RezultzSvc.WebApp02
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.

Because your webapp is Swagger enabled, Visual Studio will automatically fire up your browser a rudimentary but handy test client to tell you everything about the running service in the Swagger UI OpenAPI browser client. You can go right ahead in the browser and test your API in with the Swagger client. After you have taken that as far as it will go, cut and paste the REST queries from Swagger into Postman where you can do sophisticated testing. 

To continue with more meticulous testing in a full blown test client project, or in a production-destined project, you must configure the projects you want to start up automatically for debugging. You want the MVC service projects to fire up first, followed by the app you are using as a client such as Rezultz.Uwp or RezultzPortal.Uwp. Right click the Solution node in Solution Explorer >Configure Startup Projects...>Multiple startup projects and do the settings Rezultz.Uwp>Start, RezultzSvc.WebApp02>Start, and RezultzSvc.WebApp04>Start. The secret is that the webapps must fire up first before the client app. The projects selected to be started up do so in the order of the popup menu from top to bottom, so be sure to use the scroller arrows to move the client project so that it is positioned beneath the MVC projects. You are now ready to debug!
