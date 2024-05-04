To add a Telerik datagrid to an app, best practice is as follows. It is a big palaver if you want to do it well.  
Follow through this example to accomplish it.  Allow three to four days work. It goes something like this.

In NetStd.Rezultz00.July2018, we have a TimeStampHubItem object we want to display to the user in a datagrid in the portal 
on the TimekeepingInitialisePage. In NetStd.ViewModels02.April2022, write a TimeStampHubItemViewModel. This is a substantial 
undertaking, you can leave the bulk off the effort until later on if you wish. 

In Jgh.Uwp.Rezultz.July2018, write a TimeStampItemCollectionNullConverter and a TimeStampItemNullConverter. 

In NetStd.Rezultz02.July2018, write the interface for the presentation service, ITimeStampEntriesInLocalStoragePresentationService.
 
In NetStd.Rezultz02.July2018, add a property of type ItemDrivenTablePresenterViewModel<T,V>, call it EntriesInLocalStorageDataGridPresenter, to 
PortalTimeKeepingAndParticipantAdminPageViewModelBase<T, TU, TV> (this is the base vm for PortalTimeKeepingPagesViewModel 
where the guts of the business logic is located). In this ViewmodelBase, add a property of type DataGridDesigner for the 
datagrid, call it EntriesInLocalStorageDataGridDesigner. Don’t forget to “new up” the presenter 
and the designer in the ctor of the the ViewmodelBase. The presenter and the designer go hand in hand.

In RezultzPortal.Uwp, write a dummy implementation of ITimeStampEntriesInLocalStorageDataGridPresentationService. This is an empty implementation that does 
nothing, namely TimeStampEntriesInLocalStoragePresentationServiceDummy. Wire up a method 
in the viewmodel DependencyLocator of RezultzPortal.Uwp to register/deregister the dummy as the default transient presentation service 
provider, namely RegisterITimeStampEntriesInStoragePresentationServiceProvider(). 


In RezultzPortal.Uwp, wire up the converters in App.xaml. Create a new user control that genuinely implements 
the service interface (implemented in the code-behind of the user control), namely TimeStampEntriesInLocalStorageDataGridPresentationServiceUserControl. 
In the Xaml, drop in the RadDataGrid user control - making use of the converters. Drop the presentation service control 
into TimekeepingInitialisePage where we wish to employ it. The crucial step here
is to specify EntriesInLocalStorageDataGridPresenter as the datacontext for the presentation service control. 
This is what binds all the moving parts together. In the codebehind of the page, wire up the presentation service 
provider in the Page_Onloaded() method and the OnNavigatingFrom() method.

Finally, back in NetStd.Rezultz02.July2018, in the PortalTimeKeepingPagesViewModel look for the 
RefreshHubReceptacleCollectionDataGridPresentersAsync() method. In here is where the magic happens and 
where everything comes to fruition. Using the designer and presenter objects, write the code 
to firstly populate the Designer to generate a collection of non-empty of column specification items, 
then inject the column collection into the PresentationService, then populate the presenter 
with the desired row collection for the PresentationService. Because the EntriesInLocalStorageDataGridPresenter is the 
datacontext of the Telerik datagrid, the row collection in the presenter is magically 
revealed in the datagrid, with the collection of columns you injected. 

That’s it. You are done.
