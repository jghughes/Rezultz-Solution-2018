namespace Jgh.SymbolsStringsConstants.Mar2022
{
    /// <summary>
    /// In the publisher profile file, the XML property value of DatasetIdentifier in
    /// the relevant GuiButtonProfile XElement must be identical to these enums for data that
    /// is pulled from the Rezultz Portal Hub. For 3rd party data obtained from the hard
    /// drive, you are free to choose a value of your own, preferably a meaningful one.
    /// This needs to be hard-coded in the corresponding publisher module. 
    /// This is how we tie portal buttons up to the publisher modules.
    /// </summary>
    public class EnumsForPublisherModule
    {
        public const string TimestampsAsConsolidatedSplitIntervalsFromRemotePortalHubAsJson = "ConsolidatedSplitIntervalsFromHubAsJson";
        public const string ParticipantsAsJsonFromRemotePortalHub = "ParticipantHubItemsAsJson";
        public const string ResultItemsAsXmlFromPortalNativeTimingSystem = "ResultItemsAsXml";
    }
}
