to test this program, you can find some very simple but perfect raw timing-tent data in the folder : -

<<C:\Users\johng\OneDrive\!REZULTZ\!Results 2016\Kelso2016-mtb\Kelso2016mtbRawData\race0 - test>>

The test data files are in a suite with names like the following : <<Kelso2014mtb-1 - dummy valid test version V1.xml>>

You can use "jgh" as the test userID and Password. The UserAuthenticationPage is the point of entry to the app. in the method 
"CreateDummyNavigationContextForDebug" choose the proffered <<SettingsProfileId=104>> and <<CodeNameOfConverterOfTimingTentData="15m">>
you will find this profile in my Azure Storage Account <<rezultz2>> in blob container <<profiles>>. The contents of the profile are : -

<root>
  <GroupOfSeries>
    <IdOfThisProfile>104</IdOfThisProfile>
    <Particulars>
      <CodeName>profile-104</CodeName>
      <Label>Test Profile id=104</Label>
      <Title>Kelso Test SettingsProfileID=104</Title>
      <Subtitle>text</Subtitle>
      <AdvertisedDateTime>2015-05-21T18:45:00.0</AdvertisedDateTime>
      <IsEnabled>1</IsEnabled>
    </Particulars>
    <Organizer>
      <Particulars>
        <CodeName>kelso</CodeName>
        <Label>Conservation Halton</Label>
        <Title>Halton Conservation Authority</Title>
        <Subtitle>Kelso/Glen Eden</Subtitle>
      </Particulars>
    </Organizer>
    <ArrayOfTarget>      
      <Target>
        <BlobName>Kelso2016-mtb-series-settings.xml</BlobName>
        <AzureContainerName>testcontainerforpublishportal</AzureContainerName>
        <AzureAccountName>rezultz2</AzureAccountName>
      </Target>
      <Target>
        <BlobName>Kelso2015-mtb-series-settings.xml</BlobName>
        <AzureContainerName>testcontainerforpublishportal</AzureContainerName>
        <AzureAccountName>rezultz2</AzureAccountName>
      </Target>     
    </ArrayOfTarget>
  </GroupOfSeries>
</root>

As you can see, the targets point to <<testcontainerforpublishportal>> and in there will will see the pair of required settings files : -
<<Kelso2016-mtb-series-settings.xml>> and <<Kelso2015-mtb-series-settings.xml>>.

This is pretty much all you need to understand about the testing rig. In Visual Studio, just set <<Rezultz Portal.Web>> as the start up project and compile and run in debug mode. 
All the output files generated in your tests will end up in <<testcontainerforpublishportal>> and <<testcontainerforpublishportal-stagingdata>>. 
You can clear out generated files therein freely, you can overwrite them, or you can generate new ones. Enjoy. 

When you reference Jgh.AzureStorageAccess.SilverlightApp.August2016 and employ the AzureBlobOperationsServiceVersionWcf class from there to call WCF service for a front-end to Azure storage
remember to copy an up to date ServiceReferences.ClientConfig file from there into this project. The endpoint details must be identical.


