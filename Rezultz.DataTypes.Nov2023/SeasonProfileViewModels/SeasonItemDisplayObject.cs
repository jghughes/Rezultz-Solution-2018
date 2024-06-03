using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

// ReSharper disable InconsistentNaming

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class SeasonItemDisplayObject : BindableBase, IHasItemID, IHasLabel, IHasEnumString
    {
        #region properties

        public string FragmentInFileNameOfAssociatedProfileFile { get; set; } = string.Empty;

        public string AccessCodes { get; set; } = string.Empty; // list of comma-separated codes

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty;

        public DateTime AdvertisedDateTime { get; set; }

        public string Tag { get; set; }

        public OrganizerItemViewModel OrganizerItem { get; set; } = new();

        public EntityLocationItemDisplayObject[] ArrayOfDatabaseLocationOfSeriesItems { get; set; } = [];

        public SeriesItemDisplayObject[] ArrayOfSeriesItems { get; set; }= [];

        public int ID { get; set; } // not used. merely to satisfy required interface

        public string EnumString { get; set; } = string.Empty; // not used. merely to satisfy required interface
        #endregion

        #region field

        private SeasonProfileItem _sourceProfileItem;

        #endregion

        #region methods

        public static SeasonItemDisplayObject FromModel(SeasonProfileItem model)
        {
            const string failure = "Populating SeasonItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new SeasonProfileItem();

                var viewModel = new SeasonItemDisplayObject
                {
                    //ID = src.ID,
                    Label = model.Label,
                    //EnumString = src.EnumString,
                    //Guid = string.Empty,
                    Title = model.Title,
                    //Blurb = src.Blurb,
                    AdvertisedDateTime = model.AdvertisedDate,
                    //DisplayRank = src.DisplayRank,
                    FragmentInFileNameOfAssociatedProfileFile = model.FragmentInFileNameOfAssociatedProfileFile,
                    AccessCodes = model.AccessCodes,
                    OrganizerItem = OrganizerItemViewModel.FromModel(model.Organizer),
                    ArrayOfDatabaseLocationOfSeriesItems = EntityLocationItemDisplayObject.FromModel(model.SeriesProfileFileLocations),
                    ArrayOfSeriesItems = SeriesItemDisplayObject.FromModel(model.SeriesProfiles),
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


        public static SeasonItemDisplayObject[] FromModel(SeasonProfileItem[] arrayOfModel)
        {
            const string failure = "Populating SeasonItemDisplayObject[].";
            const string locus = "[FromModel]";
            try
            {
                arrayOfModel ??= [];

                var arrayOfViewModel = arrayOfModel.Select(FromModel).Where(z => z != null).ToArray();

                return arrayOfViewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }
            #endregion
        }

        public static SeasonProfileItem ObtainSourceItem(SeasonItemDisplayObject displayObject)
        {
            return displayObject?._sourceProfileItem;

        }

        public override string ToString()
        {
            return JghString.ConcatAsSentences(FragmentInFileNameOfAssociatedProfileFile, Title, $"items={ArrayOfSeriesItems.Count()}");
        }

        #endregion

    }
}