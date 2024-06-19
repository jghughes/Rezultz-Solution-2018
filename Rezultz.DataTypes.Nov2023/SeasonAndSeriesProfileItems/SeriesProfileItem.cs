using System;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class SeriesProfileItem : IHasAdvertisedDate, IHasItemID
    {
        #region properties

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty; 

        public DateTime AdvertisedDate { get; set; } 

        public int NumOfScoresToCountForSeriesTotalPoints { get; set; }

        public string HtmlDocumentNameForSeriesTotalPointsStandings { get; set; } = string.Empty;

        public string HtmlDocumentNameForSeriesTourDurationStandings { get; set; } = string.Empty;

        public EntityLocationItem ContainerForTimestampHubItemData { get; set; } = new();

        public EntityLocationItem ContainerForParticipantHubItemData { get; set; } = new();

        public EntityLocationItem  ContainerForPublishingDatasetsToBeProcessed { get; set; } = new();

        public EntityLocationItem ContainerForResultsDataFilesPreviewed { get; set; } = new();

        public EntityLocationItem ContainerForResultsDataFilesPublished { get; set; } = new();

        public EntityLocationItem ContainerForDocumentsPosted { get; set; } = new();

        public EntityLocationItem ContainerForTemporaryStuff { get; set; } = new();

        public EntityLocationItem ContainerForMedia { get; set; } = new();

        public EventProfileItem[] EventProfileItems { get; set; } = [];

        public MoreInformationItem[] MoreSeriesInformationItems { get; set; } = [];

        public EventSettingsItem DefaultEventSettingsForAllEvents { get; set; } = new();

        public int DisplayRank { get; set; }

        public int ID { get; set; }


        #endregion

        #region methods

        public static SeriesProfileItem FromDataTransferObject(SeriesProfileDto dto)
        {
            const string failure = "Populating SeriesProfile.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new SeriesProfileDto();

                var answer = new SeriesProfileItem
                {
                    Label = x.Label,
                    Title = x.Title,
                    AdvertisedDate = DateTime.TryParse(x.AdvertisedDateAsString, out var dateTime) ? dateTime.Date : DateTime.Today,
                    NumOfScoresToCountForSeriesTotalPoints = x.SeriesTotalPointsNumOfScoresToCount,
                    HtmlDocumentNameForSeriesTotalPointsStandings = x.SeriesTotalPointsStandingsPostedDocumentName,
                    HtmlDocumentNameForSeriesTourDurationStandings = x.SeriesTourDurationStandingsPostedDocumentName,
                    ContainerForTimestampHubItemData = EntityLocationItem.FromDataTransferObject(x.TimestampHubItemDataContainer),
                    ContainerForParticipantHubItemData = EntityLocationItem.FromDataTransferObject(x.ParticipantHubItemDataContainer),
                    ContainerForPublishingDatasetsToBeProcessed = EntityLocationItem.FromDataTransferObject(x.DatasetsUploadedForProcessingContainer),
                    ContainerForResultsDataFilesPublished = EntityLocationItem.FromDataTransferObject(x.ResultsDataFilesPublishedContainer),
                    ContainerForResultsDataFilesPreviewed = EntityLocationItem.FromDataTransferObject(x.ResultsDataFilesPreviewedContainer),
                    ContainerForDocumentsPosted = EntityLocationItem.FromDataTransferObject(x.DocumentsPostedContainer),
                    ContainerForTemporaryStuff = EntityLocationItem.FromDataTransferObject(x.TemporaryStuffContainer),
                    ContainerForMedia = EntityLocationItem.FromDataTransferObject(x.MediaContainer),
                    EventProfileItems = EventProfileItem.FromDataTransferObject(x.EventProfileCollection),
                    MoreSeriesInformationItems = MoreInformationItem.FromDataTransferObject(x.SeriesInformationCollection),
                    DefaultEventSettingsForAllEvents = EventSettingsItem.FromDataTransferObject(x.DefaultSettingsForAllEvents)
                };

                // we save space in our json data by not duplicating settings that are common to all events. need to populate them here

                foreach (var eventItem in answer.EventProfileItems)
                {
                    eventItem.EventSettingsItem = answer.DefaultEventSettingsForAllEvents;
                }

                // we need to allocate each event a sequential ID otherwise any attempt to compute series standings will blow up 

                int i = 0;

                foreach (var eventItem in answer.EventProfileItems.OrderBy(z=> z.AdvertisedDate))
                {
                    eventItem.ID = i;

                    i += 1;
                }


                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SeriesProfileItem[] FromDataTransferObject(SeriesProfileDto[] dto)
        {
            const string failure = "Populating SeriesProfile.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dto is null)
                    return [];

                var answer = dto.Select(FromDataTransferObject).Where(z => z is not null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SeriesProfileDto ToDataTransferObject(SeriesProfileItem item)
        {
            const string failure = "Populating SeriesMetadataItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var src = item ?? new SeriesProfileItem();

                var answer = new SeriesProfileDto
                {
                    Label = src.Label,
                    Title = src.Title,
                    AdvertisedDateAsString = src.AdvertisedDate.ToShortDateString(),
                    SeriesTotalPointsNumOfScoresToCount = src.NumOfScoresToCountForSeriesTotalPoints,
                    SeriesTotalPointsStandingsPostedDocumentName = src.HtmlDocumentNameForSeriesTotalPointsStandings,
                    SeriesTourDurationStandingsPostedDocumentName = src.HtmlDocumentNameForSeriesTourDurationStandings,
                    TimestampHubItemDataContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForTimestampHubItemData),
                    ParticipantHubItemDataContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForParticipantHubItemData),
                    DatasetsUploadedForProcessingContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForPublishingDatasetsToBeProcessed),
                    ResultsDataFilesPublishedContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForResultsDataFilesPublished),
                    ResultsDataFilesPreviewedContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForResultsDataFilesPreviewed),
                    DocumentsPostedContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForDocumentsPosted),
                    TemporaryStuffContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForTemporaryStuff),
                    MediaContainer = EntityLocationItem.ToDataTransferObject(src.ContainerForMedia),
                    EventProfileCollection = EventProfileItem.ToDataTransferObject(src.EventProfileItems),
                    SeriesInformationCollection = MoreInformationItem.ToDataTransferObject(src.MoreSeriesInformationItems),
                    DefaultSettingsForAllEvents = EventSettingsItem.ToDataTransferObject(src.DefaultEventSettingsForAllEvents)
                };

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SeriesProfileDto[] ToDataTransferObject(SeriesProfileItem[] item)
        {
            const string failure = "Populating SeriesMetadataItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                if (item is null)
                    return [];

                var answer = item.Select(ToDataTransferObject).Where(z => z is not null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Label, Title, $"items={EventProfileItems.Count()}");
        }

        #endregion

    }
}