using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class SeriesItemDisplayObject : BindableBase, IHasDisplayRank, IHasCollectionLineItemPropertiesV1
    {
        #region properties

        #region Title -INPC

        private string _backingstoreTitle;

        public string Title
        {
            get => _backingstoreTitle ??= string.Empty;
            set => SetProperty(ref _backingstoreTitle, value);
        }

        #endregion

        #region Label - INPC

        private string _backingstoreLabel;

        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
        }

        #endregion

        public DateTime AdvertisedDateTime { get; set; } 

        public int NumOfScoresToCountForSeriesTotalPoints { get; set; }

        public string HtmlDocumentNameForSeriesTotalPointsStandings { get; set; } = string.Empty;

        public string HtmlDocumentNameForSeriesTourDurationStandings { get; set; } = string.Empty;

        public EntityLocationItemDisplayObject LocationOfTimestampData { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfParticipantData { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfCustomDatasetsUploadedForProcessing { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfResultsForPreview { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfResultsPublished { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfDocumentsPosted { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfRandomTemporaryStuff { get; set; } = new();

        public EntityLocationItemDisplayObject LocationOfMedia { get; set; } = new();

        public EventItemDisplayObject[] ArrayOfEventItems { get; set; } = [];

        public MoreInformationItemDisplayObject[] ArrayOfMoreSeriesInformationItems { get; set; } = [];

        public SettingsForEventItemViewModel DefaultSettingsForAllEvents { get; set; } = new();

        public string EnumString { get; set; } = string.Empty;

        public int DisplayRank { get; set; }

        public int ID { get; set; }



        #endregion

        #region field

        private SeriesProfileItem _sourceProfileItem;

        #endregion

        #region methods

        public static SeriesItemDisplayObject FromModel(SeriesProfileItem model)
        {
            const string failure = "Populating SeriesItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new SeriesProfileItem();

                var viewModel = new SeriesItemDisplayObject
                {
                    ID = model.ID,
                    Label = model.Label,
                    //EnumString = src.EnumString,
                    //Guid = string.Empty,
                    Title = model.Title,
                    //Blurb = src.Blurb,
                    AdvertisedDateTime = model.AdvertisedDate,
                    DisplayRank = model.DisplayRank,
                    LocationOfTimestampData = EntityLocationItemDisplayObject.FromModel(model.ContainerForTimestampHubItemData),
                    LocationOfParticipantData = EntityLocationItemDisplayObject.FromModel(model.ContainerForParticipantHubItemData),
                    LocationOfCustomDatasetsUploadedForProcessing = EntityLocationItemDisplayObject.FromModel(model.ContainerForPublishingDatasetsToBeProcessed),
                    LocationOfResultsPublished = EntityLocationItemDisplayObject.FromModel(model.ContainerForResultsDataFilesPublished),
                    LocationOfResultsForPreview = EntityLocationItemDisplayObject.FromModel(model.ContainerForResultsDataFilesPreviewed),
                    LocationOfDocumentsPosted = EntityLocationItemDisplayObject.FromModel(model.ContainerForDocumentsPosted),
                    LocationOfRandomTemporaryStuff = EntityLocationItemDisplayObject.FromModel(model.ContainerForTemporaryStuff),
                    LocationOfMedia = EntityLocationItemDisplayObject.FromModel(model.ContainerForMedia),
                    ArrayOfEventItems = EventItemDisplayObject.FromModel(model.EventProfileItems),
                    ArrayOfMoreSeriesInformationItems = MoreInformationItemDisplayObject.FromModel(model.MoreSeriesInformationItems),
                    NumOfScoresToCountForSeriesTotalPoints = model.NumOfScoresToCountForSeriesTotalPoints,
                    HtmlDocumentNameForSeriesTotalPointsStandings = model.HtmlDocumentNameForSeriesTotalPointsStandings,
                    HtmlDocumentNameForSeriesTourDurationStandings = model.HtmlDocumentNameForSeriesTourDurationStandings,
                    DefaultSettingsForAllEvents = SettingsForEventItemViewModel.FromModel(model.DefaultEventSettingsForAllEvents),
                    _sourceProfileItem = model
                };

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SeriesItemDisplayObject[] FromModel(SeriesProfileItem[] model)
        {
            const string failure = "Populating SeriesItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                if (model == null)
                    return [];

                var viewModel = model.Select(FromModel).Where(z => z != null).ToArray();

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SeriesProfileItem ObtainSourceModel(SeriesItemDisplayObject displayObject)
        {
            return displayObject?._sourceProfileItem;
        }

        public static SeriesProfileItem[] ObtainSourceModel(SeriesItemDisplayObject[] viewModel)
        {
            const string failure = "Populating SeriesItem.";
            const string locus = "[ObtainSourceModel]";


            try
            {
                if (viewModel == null)
                    return [];

                var answer = viewModel.Select(ObtainSourceModel).Where(z => z != null).ToArray();

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
            return JghString.ConcatAsSentences(Label, Title, $"items={ArrayOfEventItems.Count()}");
        }

        #endregion

    }
}