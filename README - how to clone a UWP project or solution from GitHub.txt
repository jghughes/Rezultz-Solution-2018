1. In the solution Start Window, select clone a repository from GitHUb.
2. For the Repository location, enter the URL of the repository you want to clone. To to the repository in GitHub, select the solution you want, and copy the URL from serach bar e.g. https://github.com/jghughes/Rezultz-Solution-2018
3. For the Parent Directory, enter the path where you want to clone the repository. The default for me is C:\Users\johng\Source\Repos.
4. The Wizard automatically does everything else for you. Click Clone. It willl create athe twin of the repository on your local machine inside the solution folder.
5. In the Solution Explorer, you will see the new project. You can now work on the project locally. When you are ready to commit your changes locally and push your changes to GitHub, go > Git Commit of Stash.
5. For UWP projects in your solution, you need to create a new self-signed certificate for the project for your development environment in which local builds are done. Right-click on the project in the Solution Explorer, and select Properties > Signing > Create Test Certificate. Follow the wizard to create a new certificate. You will need to create a password for the certificate - which you might need in future so write it down. The test certificate is typically called Rezultz.Uwp_TemporaryKey.pfx  If reusing an existing certificate, it will most likely be found in the project folder. You will need to add the certificate to the project in the Solution Explorer. Right-click on the project, and select Add > Existing Item. Navigate to the project folder, and select the certificate. It takes several seconds to be processed so be patient.
6.In order to successfully publish a new or existing UWP app in the store from your newly cloned solution, you need to reassociate the app with the store. Right-click on the project in the Solution Explorer, and select	Publish > Associate App with the Store. Follow the wizard to associate the app with the store. You will need to be signed in to your developer account obviously. This process generates a Package.StoreAssociation.xml file.
7. Somewhere along the line a key for the Store is generated (or perhaps reused). Not sure when. Typical name is Rezultz.Uwp_StoreKey.pfx

You are all done. You can now work on the project locally, and push your changes to GitHub. When you are ready to publish the app in the store, you can do so from the cloned solution. Go Publish > Create App Packages. Follow the wizard to create the app packages. 

Good luck.

2024-06-07