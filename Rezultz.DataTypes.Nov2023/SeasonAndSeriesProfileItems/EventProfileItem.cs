using System;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class EventProfileItem : IHasAdvertisedDate, IHasItemID
    {

        #region props

        public int NumInSequence { get; set; }

        public string TxxColumnHeader { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty;

        public int IndexOfEventInSeriesOverallCalc { get; set; } //calculated field for series points calculations

        public bool IsExcludedFromSeriesPointsTotal { get; set; }

        public DateTime AdvertisedDate { get; set; }

        public EventSettingsItem EventSettingsItem { get; set; } = new();

        public EntityLocationItem[] LocationsOfPublishedResultsXmlFiles { get; set; } = [];

        public ResultItem[] ResultItemsForEventAsPublished { get; set; } = []; // N.B - this is not an intrinsic prop. I lazily use it to be subsequently populated when results are to hand

        public string HtmlDocumentNameForPostedResults { get; set; } = string.Empty;

        public int ID { get; set; }

        public int DisplayRank { get; set; }

        public string EnumString { get; set; } = string.Empty; 
    
        #endregion

        #region methods

        public static EventProfileItem FromDataTransferObject(EventProfileDto dto)
        {
            const string failure = "Populating EventItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new EventProfileDto();

                var answer = new EventProfileItem
                {
                    Label = x.Label,
                    Title = x.Title,
                    AdvertisedDate = DateTime.TryParse(x.AdvertisedDateAsString, out var dateTime) ? dateTime.Date : DateTime.Today,
                    IndexOfEventInSeriesOverallCalc = 0,
                    NumInSequence = x.OrderInSequence,
                    TxxColumnHeader = x.TxxColumnHeader,
                    HtmlDocumentNameForPostedResults = x.HtmlDocumentNameForPostedResults,
                    IsExcludedFromSeriesPointsTotal = x.IsExcludedFromSeriesPoints,
                    EventSettingsItem = EventSettingsItem.FromDataTransferObject(x.EventSettings),
                    ResultItemsForEventAsPublished = ResultItem.FromDataTransferObject(x.PublishedResultsForEvent)
                };

                var arrayOfPreprocessedResultsDataFileNames = x.XmlFileNamesForPublishedResults.Split(',');

                answer.LocationsOfPublishedResultsXmlFiles = arrayOfPreprocessedResultsDataFileNames
                    .Select(blobName => new EntityLocationItem() {EntityName = blobName})
                    .ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static EventProfileItem[] FromDataTransferObject(EventProfileDto[] dto)
        {
            const string failure = "Populating EventItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dto == null)
                    return [];

                var answer = dto.Select(FromDataTransferObject).Where(z => z != null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static EventProfileDto ToDataTransferObject(EventProfileItem profileItem)
        {
            const string failure = "Populating EventItemDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = profileItem ?? new EventProfileItem();

                var answer = new EventProfileDto
                {
                    Label = x.Label,
                    Title = x.Title,
                    AdvertisedDateAsString = x.AdvertisedDate.ToShortDateString(),
                    OrderInSequence = x.NumInSequence,
                    TxxColumnHeader = x.TxxColumnHeader,
                    HtmlDocumentNameForPostedResults = x.HtmlDocumentNameForPostedResults,
                    IsExcludedFromSeriesPoints = x.IsExcludedFromSeriesPointsTotal,
                    EventSettings = EventSettingsItem.ToDataTransferObject(x.EventSettingsItem),
                    PublishedResultsForEvent = ResultItem.ToDataTransferObject(x.ResultItemsForEventAsPublished)
                };

                var arrayOfBlobNames = x.LocationsOfPublishedResultsXmlFiles.Select(location => location.EntityName).ToArray();

                var arrayOfBlobNamesAsJoinedString = JghString.ConcatWithSeparator(",", arrayOfBlobNames);

                answer.XmlFileNamesForPublishedResults = arrayOfBlobNamesAsJoinedString;

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static EventProfileDto[] ToDataTransferObject(EventProfileItem[] item)
        {
            const string failure = "Populating EventItemDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                if (item == null)
                    return [];

                var answer = item.Select(ToDataTransferObject).Where(z => z != null).ToArray();


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
            return JghString.ConcatAsSentences(Label, Title);
        }

        #endregion

    }
}