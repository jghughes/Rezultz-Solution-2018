
At the time of writing in Aug 2023, you musn't go beyond the the v4.5.3 series of System.Service.Model classes
if you are in a .Net Standard class library serving a UWP or .Net Framework app. You can only go beyond if your
class library is serving a .Net Core app. The reason is that the System.ServiceModel classes in the v4.6 series
and beyond are not compatible with UWP and .Net Framework. They are only compatible with .Net 5 +

Here's where to point the Add Service Reference Wizard when generating a client object for each of the services. These pointers 
are valid at the time of writing in Aug 2023. These can be refactored utterly over time. In a CoreWcf service, they are given arbitrary, discretionary names 
as string parameters to the AddServiceEndpoint method in Program.cs in ConfigureServices(IServiceBuilder serviceBuilder). Like so: -

serviceBuilder.AddServiceEndpoint<AzureStorageSvc, IAzureStorageSvc>(BindingConfigs.GetHttpTextBinding(), "/IAzureStorageSvc/HttpsText");

The first parameter is the name of the service class. The second is the name of the interface that the service class implements. 
The third is the binding. The fourth is the arbitrarly specified url. The consequential url for the CoreEcf services are: -

https://rezultzsvccorewcf.azurewebsites.net/IAzureStorageSvc/HttpsText?wsdl
https://rezultzsvccorewcf.azurewebsites.net/IParticipantRegistrationSvc/HttpsText?wsdl
https://rezultzsvccorewcf.azurewebsites.net/ILeaderboardResultsSvc/HttpsText?wsdl
https://rezultzsvccorewcf.azurewebsites.net/ITimeKeepingSvc/HttpsText?wsdl
https://rezultzsvccorewcf11.azurewebsites.net/IPublishingSvc/HttpsText?wsdl // note the 11 in the url. different standalone CoreWcf project

... where the CoreWcf apps are rezultzsvccorewcf.azurewebsites.net or rezultzsvccorewcf11.azurewebsites.net

For debugging the service, you might or might not need to open Visual Studio as an Administrator.
Go to the the client-side project that houses the WCF svc wizard generated clients, in this 
case  RezultzSvc.Clients.Wcf.March2023. Select the Connected Services>ServiceReferenceX Folder> select the service reference you wish to debug e.g ServiceReference3 for 

https://rezultzsvccorewcf.azurewebsites.net/ILeaderboardResultsSvc/HttpsText?wsdl. Delete it. 

Now you are going to recreate the reference to point to the corresponding sevice hosted locally 
on IIS running in localhost. Set the RezultzSvc.WebApp project as the start-up project and start it up in IIS in debug mode. Right 
click the client-side project  RezultzSvc.Clients.Wcf.March2023. Select Add>Service Reference. This opens the WcfService wizard. 
Under the Service References drop-down click the green + sign to add a service ref, then choose WCF Service>Next>Discover. 
Wait a few seconds for the list of Wcf services in your solution to light up in the Services: combo box. If the connection fails with 
an inscrutable error message, go back and inspect the web page for the svc WSDL that IIS throws up. Click on the Services 
folder, then the svcname file (eg. AzureStorageSvc.svc). If you don't get a pretty description of the svc it 
means that something is wrong, most probably a mismatch between a folder name and namespace in the wcf 
project and in the Web.config in the project. When fixed, select your desired service 
in the wizard. This reveals the hierarchy for the service. Two levels down is the Interface of the svc. Click it 
to see the list of operations it provides. If that looks OK, review the Namespace text box to ensure that the 
wizard has volunteered the namespace you want. Click next and wait a few seconds for the wizard to move forward to the next screen. 
Uncheck Reuse types in all referenced assemblies. Click next. Ensure that the Access level for generated classes 
is Public. Don't check the box for Synchronous options. Click Finish. After a few seconds the wizard will report 
that it has successfully added the service reference. Close the wizard. Return to the client project. A folder named 
after the Namespace will be there. Open it and click on ConnectedService.json. At the very top, inside the array 
of "inputs" property will be the url pointing to your svc on local host. Like so:

        https://localhost:5001/IAzureStorageSvc/HttpsText?wsdl
        https://localhost:5001/IParticipantRegistrationSvc/HttpsText?wsdl
        https://localhost:5001/ILeaderboardResultsSvc/HttpsText?wsdl
        https://localhost:5001/ITimeKeepingSvc/HttpsText?wsdl
        https://localhost:5001/IPublishingSvc/HttpsText?wsdl

Recompile the project that houses the service references and you are ready to do your testing. Go to headline title 
of the solution in Solution Explorer. Right click it and choose Properties. In the Startup projects popup select 
Multiple Startup projects. Scroll down the list and set RezultzSvc.WebApp to Start. Drag and drop the RezultzSvc.WebApp 
line-item so that it precedes the testing app in the list. Set the testing app to Start. When you commence 
debugging, Visual Studio will fire up localhost and RezultzSvc.WebApp within it. I recommend putting your initial breakpoint 
exactly on the line in  RezultzSvc.Clients.Wcf.March2023 where the method/svc you are debugging is invoked. Step the 
debugger from there Into the service and hey presto the debugger should now materialse inside the service if you did 
everything right. You can now step through your code. 
