using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class CboLookupItemDisplayObject : BindableBase, IHasTitle, IHasAdvertisedDate, IHasDisplayRank, IHasCollectionLineItemPropertiesV2
    {
        #region properties

        public string Title { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string Blurb { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty; 

        public DateTime AdvertisedDate { get; set; } 

        public int DisplayRank { get; set; } 

        public int ID { get; set; }

        public string Guid { get; set; } = string.Empty;

        #endregion

        #region field

        private CboLookupItem _sourceItem;

        #endregion

        #region methods

        public static CboLookupItemDisplayObject FromModel(CboLookupItem model)
        {
            const string failure = "Populating CboLookupItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new CboLookupItem();

                var answer = new CboLookupItemDisplayObject
                {
                    ID = model.ID,
                    Label = model.Label,
                    EnumString = model.EnumString,
                    Guid = string.Empty,
                    Title = model.Title,
                    Blurb = model.Blurb,
                    AdvertisedDate = model.AdvertisedDateTime,
                    DisplayRank = model.DisplayRank,
                    _sourceItem = model
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

        public static CboLookupItemDisplayObject[] FromModel(CboLookupItem[] models)
        {
            const string failure = "Populating CboLookupItemDisplayObject[].";
            const string locus = "[FromModel]";

            try
            {
                if (models is null)
                    return [];

                var answer = models.Select(FromModel).Where(z => z is not null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static CboLookupItem ObtainSourceModel(CboLookupItemDisplayObject displayObject)
        {
            return displayObject?._sourceItem;
        }

        public static CboLookupItem[] ObtainSourceModel(CboLookupItemDisplayObject[] viewModels)
        {
            const string failure = "Obtaining source model.";
            const string locus = "[ObtainSourceModel]";

            try
            {
                if (viewModels is null)
                    return [];

                var answer = viewModels.Select(ObtainSourceModel).Where(z => z is not null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static CboLookupItemDisplayObject FromLabel(string label)
        {
            const string failure = "Populating CboLookupItemDisplayObject.";
            const string locus = "[FromLabel]";

            try
            {
                var answer = new CboLookupItemDisplayObject
                {
                    Label = label ?? string.Empty,
                    _sourceItem = new CboLookupItem(){Label = label}
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

        public static CboLookupItemDisplayObject[] FromLabel(string[] labels)
        {
            const string failure = "Populating CboLookupItemDisplayObject.";
            const string locus = "[FromLabel]";

            try
            {
                if (labels is null)
                    return [];

                var answer = labels.Select(FromLabel).Where(z => z is not null).ToArray();

                // P.S. don't filter out blank labels. if they are there we deem them as being there for a reason

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
            return JghString.ConcatAsSentences(Title, Label);
        }

        #endregion
    }
}