using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class MoreInformationItemDisplayObject : BindableBase, IHasTitle, IHasAdvertisedDate, IHasDisplayRank, IHasCollectionLineItemPropertiesV2
    {
        #region properties

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty; 

        public string Blurb { get; set; } = string.Empty; 

        public int DisplayRank { get; set; } 

        public DateTime AdvertisedDate { get; set; } 
    
        public string Guid { get; set; } = string.Empty; 

        public int ID { get; set; }


        #endregion

        #region field

        private MoreInformationItem _sourceItem = new();

        #endregion

        #region methods

        public static MoreInformationItemDisplayObject FromModel(MoreInformationItem model)
        {
            const string failure = "Populating MoreInformationItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new MoreInformationItem();

                var viewModel = new MoreInformationItemDisplayObject
                {
                    //ID = src.ID,
                    Label = model.Label,
                    EnumString = model.EnumString,
                    Guid = string.Empty,
                    Title = model.Title,
                    Blurb = model.Blurb,
                    //AdvertisedDateAsString = src.AdvertisedDateAsString,
                    DisplayRank = model.DisplayRank,
                    _sourceItem = model
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

        public static MoreInformationItemDisplayObject[] FromModel(MoreInformationItem[] model)
        {
            const string failure = "Populating MoreInformationItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                if (model is null)
                    return [];

                var viewModel = model.Select(FromModel).Where(z => z is not null).ToArray();

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static MoreInformationItem ObtainSourceModel(MoreInformationItemDisplayObject displayObject)
        {
            return displayObject?._sourceItem;

        }

        public static MoreInformationItem[] ObtainSourceModel(MoreInformationItemDisplayObject[] viewModel)
        {
            const string failure = "Populating MoreInformationItem.";
            const string locus = "[ObtainSourceModel]";

            try
            {
                if (viewModel is null)
                    return [];

                var model = viewModel.Select(ObtainSourceModel).Where(z => z is not null).ToArray();

                return model;
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
            return JghString.ConcatAsSentences(Title, Label);
        }

        #endregion
    }
}