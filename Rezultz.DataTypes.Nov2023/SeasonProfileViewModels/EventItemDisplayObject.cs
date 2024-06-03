using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class EventItemDisplayObject : BindableBase, IHasDisplayRank, IHasCollectionLineItemPropertiesV1
    {
        #region props

        #region Title

        private string _backingstoreTitle;

        public string Title
        {
            get => _backingstoreTitle ??= string.Empty;
            set => SetProperty(ref _backingstoreTitle, value);
        }

        #endregion

        #region Label

        private string _backingstoreLabel;

        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
        }

        #endregion
    
        public string EnumString { get; set; } = string.Empty;

        public DateTime AdvertisedDateTime { get; set; }

        public int DisplayRank { get; set; }

        public int ID { get; set; }

        public int NumInSequence { get; set; }

        public int IndexOfEventInSeriesOverallCalc { get; set; } //calculated field for series points calculations

        public string TxxColumnHeader { get; set; } = string.Empty;

        public SettingsForEventItemViewModel SettingsForEventItem { get; set; } = new();

        public EntityLocationItemDisplayObject[] ArrayOfLocationOfPreprocessedResultsDataFiles { get; set; } = [];
    
        public ResultItemDisplayObject[] ArrayOfResultItemForEvent { get; set; } = []; // N.B

        public string HtmlDocumentNameForPostedResults { get; set; } = string.Empty;

        #endregion

        #region field

        private EventProfileItem _sourceProfileItem;

        #endregion

        #region methods

        public static EventItemDisplayObject FromModel(EventProfileItem model)
        {
            const string failure = "Populating EventItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new EventProfileItem();

                var viewModel = new EventItemDisplayObject
                {
                    ID = model.ID,
                    Label = model.Label,
                    EnumString = model.EnumString,
                    Title = model.Title,
                    AdvertisedDateTime = model.AdvertisedDate,
                    DisplayRank = model.DisplayRank,
                    IndexOfEventInSeriesOverallCalc = 0,
                    NumInSequence = model.NumInSequence,
                    TxxColumnHeader = model.TxxColumnHeader,
                    HtmlDocumentNameForPostedResults = model.HtmlDocumentNameForPostedResults,
                    ArrayOfLocationOfPreprocessedResultsDataFiles = EntityLocationItemDisplayObject.FromModel(model.LocationsOfPublishedResultsXmlFiles),
                    SettingsForEventItem = SettingsForEventItemViewModel.FromModel(model.EventSettingsItem),
                    ArrayOfResultItemForEvent = ResultItemDisplayObject.FromModel(model.ResultItemsForEventAsPublished),
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

        public static EventItemDisplayObject[] FromModel(EventProfileItem[] model)
        {
            const string failure = "Populating EventItemDisplayObject.";
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

        public static EventProfileItem ObtainSourceModel(EventItemDisplayObject displayObject)
        {
            return displayObject?._sourceProfileItem;
        }

        public static EventProfileItem[] ObtainSourceModel(EventItemDisplayObject[] viewModel)
        {
            const string failure = "Populating EventItem.";
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
            return JghString.ConcatAsSentences(Label, Title);
        }

        #endregion

    }
}