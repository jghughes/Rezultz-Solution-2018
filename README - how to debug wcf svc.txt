The following is a summary of the steps I took to get the debugger to work for me. I am using Visual Studio 2022. For important info on how to create a client svc reference, and the differences between doing so on Framework projects and the new .Net and Core projects, see https://learn.microsoft.com/en-us/visualstudio/data-tools/how-to-add-update-or-remove-a-wcf-data-service-reference?view=vs-2022. For debugging the CoreWcf service, go read https://github.com/CoreWCF/CoreWCF/blob/main/Documentation/Walkthrough.md\\ The walkthrough is deficient in a couple of small but crucial respects. After creating the test client (the console app here), you need to fire up the service by right clicking the service project Debug>Startwithoutdebugging. The walkthrough doesn't mention the need to fire up. It just says to run the client. But the client will fail to connect to the service if it isn't fired up. You can check the console/terminal to see that the svc is listening, and where, and which endpoints make WSDL available. The terminal looks something like this:

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

The second crucial omission is that the walkthrough fails to show that the ?wsdl query parameter needs to be appended to the service endpoint when using the Wcf service wizard for Discovery. So then, what is the full URL for the debug endpoint? In the CoreWCF template, it is arranged so that for debug in localhost, the parameters for the service {URI-scheme}://{URI-host} are defined in appsettings.json. When we define (in code) the remainder of the endpoint, we merely add the resource path (e.g. "/IRaceResultsPublishingSvc/HttpsText"), which of course serves for debug and release alike. Then in the service wizard to create a client class "Add>Service reference"" we append the query parameters ie “?wsdl”. So, the URLs to use in the wcf service wizard in the client are either http or https, it makes no difference it seems, but you MUST use a Text endpoint not binary. The same applies for all our other services comprising the following parts:   {uri-scheme}: // {uri-host} / {resource-path} ? {query}

                http://localhost:5000/IRaceResultsPublishingSvc/HttpText?wsdl or  
                https://localhost:5001/IRaceResultsPublishingSvc/HttpsText?wsdl

                https://localhost:5001/IAzureStorageSvc/HttpsText?wsdl
                https://localhost:5001/IParticipantRegistrationSvc/HttpsText?wsdl
                https://localhost:5001/ISeasonDataSvc/HttpsText?wsdl
                https://localhost:5001/ITimeKeepingSvc/HttpsText?wsdl

In the C# svc client project, create the reference to point to the corresponding service hosted locally in Kestrel running in localhost. I write the svc clients in the client-side project RezultzSvc.Clients.Wcf.Mar2023. Now you are going to recreate the reference to point to the corresponding service hosted locally. Set the CoreWcf project RezultzSvc.WebApp01 as the start-up project and start it up in IIS in debug mode. Right-click the client-side project and Select Add>Service Reference. This opens the client generation Wcf service wizard. Enter the url for the service in local host as specified above, or look for it in the dropdown if you have used it before. Click Go. Wait a few seconds for the list of Wcf services in your solution to light up in the Services: combo box. Select your desired service. This reveals the hierarchy for the service. Two levels down is the Interface of the svc. Click it to see the list of operations it provides. If that looks OK, review the Namespace text box to ensure that the wizard has volunteered the namespace you want e.g. ServiceReference5. Click next and wait a few seconds for the wizard to move forward to the next screen. Uncheck Reuse types in all referenced assemblies. Click next. Ensure that the Access level for generated classes is Public. Don't check the box for Synchronous options. Click Finish. After a few seconds the wizard will report that it has successfully added the service reference. Close the wizard. Return to the client project. By convention, the Wizard generates a subfolder called Connected Services/ServiceReference5. Take a peek at the two files in there, namely ConnectedService.json and Reference.cs.Now that you have generated a svc reference, you need to instantiate a client class based on the reference in RezultzSvc.Clients.Wcf.Mar2023. When newing up the client type, the constructor will throw an exception if multiple endpoints have been defined in the service host and if you don't specify the name of the desired endpoint as an enum or string parameter. But what is the Enum or string parameter name of the endpoint, and where do you find it?  This depends on the output that the Wizard generates. If your client class is a NET standard library class, the Wizard will generate a file called Reference.cs. Scroll to the very bottom and there and you will see the Enums for the endpoints. So you will need to use the ctor that takes the EndpointConfiguration Enum as a parameter. For example:

            _svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc); //when svc deployed to Azure and running

On the other hand, if your client class is a .Net Framework console app, the Wizard will generate an ostensibly similar file Connected Services/ServiceReference5. You can right click this to uncheck Reuse types in all referenced assemblies, which you must do. You can also reconfigure the service client in minor ways, or you can regenerate the svc proxy if the svc interface has changed, but there is no endpoint Enum in there. The names of the available endpoints are strings in the <endpoint name="MyHttpsTextBinding_IRaceResultsPublishingSvc"/> element in the System.ServiceModel section in App.config. For example:

                <client>
                    <endpoint address="http://localhost:5000/IRaceResultsPublishingSvc/HttpText"
                        binding="basicHttpBinding" bindingConfiguration="MyHttpTextBinding_IRaceResultsPublishingSvc"
                        contract="ServiceReference5.IRaceResultsPublishingSvc"
                        name="MyHttpTextBinding_IRaceResultsPublishingSvc" />
                    <endpoint address="https://localhost:5001/IRaceResultsPublishingSvc/HttpsText"
                        binding="basicHttpBinding" bindingConfiguration="MyHttpsTextBinding_IRaceResultsPublishingSvc"
                        contract="ServiceReference5.IRaceResultsPublishingSvc"
                        name="MyHttpsTextBinding_IRaceResultsPublishingSvc" />
                    <endpoint address="https://localhost:5001/IRaceResultsPublishingSvc/HttpsBinary"
                        binding="customBinding" bindingConfiguration="MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc"
                        contract="ServiceReference5.IRaceResultsPublishingSvc"
                        name="MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc" />
                </client>

So you will need to use the ctor that takes the EndpointConfiguration enum as a string parameter. For example:

                _svcProxy = new RaceResultsPublishingSvcClient("MyHttpsTextBinding_IRaceResultsPublishingSvc")

Or, if you wanted to create a client class manually, that you can physically copy into another project, you can do so by using the svcutil.exe tool. Like so:

                dotnet-svcutil --roll-forward LatestMaj or 
                http://localhost:5000/IRaceResultsPublishingSvc/HttpText?wsdl

You are not quite ready to debug yet. you need to ensure that the service client project is set up appropriately. In the service client class, RaceResultsPublishingSvcClient, go into the private method where we need to specify the binding as text not binary - CreateSvcProxy(). In the ctor of the svc client proxy in this method, opt for the (wizard generated)  enum for the plain HttpText endpoint. Be sure to comment out the binary enum (MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc).Debugging MUST be done in plain text. It cannot be done using the binary endpoint: this will generate an inscrutable error message upon attempting to call the svc during debugging, to whit "The certificate authority is invalid or incorrect."

                private void CreateSvcProxy()
                {
                    //when svc deployed to Azure and running over Https
                    _svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc);
       
                     //when debugging with svc running in localhost over Http (not Https)
                    _svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpTextBinding_IRaceResultsPublishingSvc);
                }

You can now proceed with your doubtless many lengthy debugging sessions. To be able to step through your code in the CoreWcf project you need two projects up and running. Righ click the title of the Solution in solution explorer and select "Configure startup projects...". In the popup, select "Multiple startup projects". In the list, set whatever test calling project you are using - be it a console app or a UWP app - to call the remote svc as one of the "Start" projects. Then drag and drop the CoreWCF project line item so that it precedes the calling project in the list. You can now click start and step through your code. If in a console app, the service will be listening on the port you specified in appsettings.json. The client will be trying to connect to the service on the same port. Final step is to publish the svc to production in Azure. Right click the title of the CoreWcf project in Solution Explorer and select Publish. After the publish is completed and the server is up and running, regenerate the svc proxy using the svc Wizard in the client project. This time, the Wizard must be provided with the production endpoint, like so:

            https://rezultzsvccorewcf.azurewebsites.net/IAzureStorageSvc/HttpsText?wsdl
            https://rezultzsvccorewcf.azurewebsites.net/IParticipantRegistrationSvc/HttpsText?wsdl
            https://rezultzsvccorewcf.azurewebsites.net/ISeasonDataSvc/HttpsText?wsdl
            https://rezultzsvccorewcf.azurewebsites.net/ITimeKeepingSvc/HttpsText?wsdl
            https://rezultzsvccorewcf11.azurewebsites.net/IRaceResultsPublishingSvc/HttpsText?wsdl

 When you are ready to recompile the svc for upload to production, be sure to back go into the private CreateSvcProxy() method again and specify the preferred Enum for the binary endpoint. In real life, we choose binary transmission because is more performant in Wcf. 

1st April 2024

