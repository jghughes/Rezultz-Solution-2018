using System;
using System.Runtime.Serialization;
using NetStd.DataTransferObjects.Mar2024;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

[DataContract(Namespace = "", Name = XeSeriesProfile)]
public class SeriesProfileDto
{
    #region Names

    private const string XeSeriesProfile = "series-profile";
    private const string XeTitleOfSeries = "title";
    private const string XeLabelOfSeries = "label";
    private const string XeAdvertisedDateOfSeries = "advertised-date";
    private const string XeNumOfScoresToCountForSeriesTotalPoints = "series-total-points-num-of-scores-to-count";
    private const string XeFilenameOfSeriesTotalPointsStandingsDocumentPosted = "series-total-points-standings-document-posted";
    private const string XeFilenameOfSeriesTourDurationStandingsDocumentPosted = "series-tour-duration-standings-document-posted";
    private const string XeContainerForTimestampHubData = "container-for-timestamp-hub-data";
    private const string XeContainerForParticipantHubData = "container-for-participant-hub-data";
    private const string XeContainerForDatasetsUploadedForProcessing = "container-for-datasets-uploaded-for-processing";
    private const string XeContainerForResultsDataFilesPreviewed = "container-for-results-data-files-previewed";
    private const string XeContainerForResultsDataFilesPublished = "container-for-results-data-files-published";
    private const string XeContainerForDocumentsPosted = "container-for-documents-posted";
    private const string XeContainerForTemporaryStuff = "container-for-temporary-stuff";
    private const string XeContainerForMedia = "container-for-media";
    private const string XeConstituentEventProfiles = "event-profiles";
    private const string XeMoreSeriesInformationItems = "series-information";
    private const string XeEventSettingsDefaultsForAllEvents = "default-settings-for-all-events";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeTitleOfSeries)]
    public string Title { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeLabelOfSeries)]
    public string Label { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeAdvertisedDateOfSeries)]
    public string AdvertisedDateAsString { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeNumOfScoresToCountForSeriesTotalPoints)]
    public int SeriesTotalPointsNumOfScoresToCount { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeFilenameOfSeriesTotalPointsStandingsDocumentPosted)]
    public string SeriesTotalPointsStandingsPostedDocumentName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6, Name = XeFilenameOfSeriesTourDurationStandingsDocumentPosted)]
    public string SeriesTourDurationStandingsPostedDocumentName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7, Name = XeContainerForTimestampHubData)]
    public EntityLocationDto TimestampHubItemDataContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 8, Name = XeContainerForParticipantHubData)]
    public EntityLocationDto ParticipantHubItemDataContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 9, Name = XeContainerForDatasetsUploadedForProcessing)]
    public EntityLocationDto DatasetsUploadedForProcessingContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 10, Name = XeContainerForResultsDataFilesPreviewed)]
    public EntityLocationDto ResultsDataFilesPreviewedContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = XeContainerForResultsDataFilesPublished)]
    public EntityLocationDto ResultsDataFilesPublishedContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12, Name = XeContainerForDocumentsPosted)]
    public EntityLocationDto DocumentsPostedContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 13, Name = XeContainerForTemporaryStuff)]
    public EntityLocationDto TemporaryStuffContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 14, Name = XeContainerForMedia)]
    public EntityLocationDto MediaContainer { get; set; } = new();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 15, Name = XeConstituentEventProfiles)]
    public EventProfileDto[] EventProfileCollection { get; set; } = Array.Empty<EventProfileDto>();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 16, Name = XeMoreSeriesInformationItems)]
    public MoreInformationDto[] SeriesInformationCollection { get; set; } = Array.Empty<MoreInformationDto>();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 17, Name = XeEventSettingsDefaultsForAllEvents)]
    public DefaultEventSettingsDto DefaultSettingsForAllEvents { get; set; } = new();

    #endregion
}