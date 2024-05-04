using System;
using System.Runtime.Serialization;
using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
// NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true?

[DataContract(Namespace = "", Name = XeEventProfile)]
public class EventProfileDto
{
    #region Names

    private const string XeEventProfile = "event-profile";
    private const string XeOrderInSequenceOfMultipleEvents = "order-in-sequence-of-events";
    private const string XeTxxColumnTitle = "txx-column-header";
    private const string XeLabelOfEvent = "label";
    private const string XeTitleOfEvent = "title";
    private const string XeAdvertisedDateOfEvent = "advertised-date";
    private const string XeAccountName = "account-name";
    private const string XeContainerName = "container-name";
    private const string XeResultsDocumentPosted = "results-document-posted";
    private const string XeResultsDataFilesPublished = "results-data-files-published";
    private const string XeMustExcludeEventFromSeriesPoints = "is-excluded-from-series-points";
    private const string XeDefaultEventSettings = "event-settings";
    private const string XeResultsForEvent = "results-for-event";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeOrderInSequenceOfMultipleEvents)]
    public int OrderInSequence { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeTxxColumnTitle)]
    public string TxxColumnHeader { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeLabelOfEvent)]
    public string Label { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeTitleOfEvent)]
    public string Title { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeAdvertisedDateOfEvent)]
    public string AdvertisedDateAsString { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6, Name = XeAccountName)]
    public string DatabaseAccountName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7, Name = XeContainerName)]
    public string DataContainerName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 8, Name = XeResultsDocumentPosted)]
    public string HtmlDocumentNameForPostedResults { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 9, Name = XeResultsDataFilesPublished)]
    public string XmlFileNamesForPublishedResults { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 10, Name = XeMustExcludeEventFromSeriesPoints)]
    public bool IsExcludedFromSeriesPoints { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = XeDefaultEventSettings)]
    public DefaultEventSettingsDto EventSettings { get; set; } = new();

    // note this is not strictly metadata, but it is a useful place to put it - this is what we use to ship back and forth the subsequently populated results for the event
    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12, Name = XeResultsForEvent)]
    public ResultDto[] PublishedResultsForEvent { get; set; } = Array.Empty<ResultDto>();

    #endregion
}