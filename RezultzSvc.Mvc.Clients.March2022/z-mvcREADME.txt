To debug these controllers, go to NetStd.ConnectionStrings.July2020 project and comment in/out the mvc endpoint/s which point to local host. 
Ensure that you comment them out again for compiling for production!

Debugging a REST service is complicated. for a detailed readyreckoner, see ERROR LOG  FOR REST API FROM NOVEMBER 2019.docx. 
What follows below is a cut and paste from there as at jan 2021. That is the source of latest guidance, not this.

When doing any kind of API debugging, you must always open VS as an administrator with administrator privileges. 
Make sure you run the application directly, rather than behind IIS Express, which ignores 
non-local requests by default. Go into Properties>Debug of the API project and change the Profile and Launch options 
from their defaults (IIS Express) to the API project, and specify some App URLs for http and https endpoints. 
Use Swagger to emit the Http REST URLs. Copy and paste all the URLs from Swagger directly into the client-side 
code you are creating. The copy and paste approach minimises the chance of mistakes creeping in. As time goes by 
during development of the client errors do inevitably creep in that bugger up the action calls 
or serialization. You can rely on Swagger to generate the correct URLs.

When testing using a local test client project (be it a console, WPF, UWP or Xamarin client), you need the host 
and the client to fire up simultaneously. You need to set multiple start up projects in VS 2019. Make sure that 
the API comes first in the start order and the test harness comes afterwards. Then launch both from the VS 2019 
tool bar in the usual way. 

1.	In Solution Explorer, select the solution (the top node).
2.	Choose the solution node's context (right-click) menu and then choose Properties. The Solution Property Pages dialog box appears.
3.	Expand the Common Properties node and choose Startup Project.
4.	Choose the Multiple Startup Projects option and set the appropriate actions.

When you click Start, the console opens, the Swagger UI opens in your browser, and your test client project launches. 
The console shows that the API project is running on localhost and it lists the port numbers that the API project 
is listening on. The ports are what you specified in your Properties>Debug>App URL. For your test client to function 
properly, the Rest Api endpoints hard coded there are those displayed by Swagger. In my case I store the endpoints centrally 
in NetStd.ConnectionStrings.July2020 so that I can use them in multiple test clients. If they are 
correct, your test client will effortlessly connect to the API project and you can set breakpoints for a full 
debugging experience in both the API project and the test client. 
