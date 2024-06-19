using System;
using System.Globalization;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class UriItemDisplayObject : BindableBase, IHasCollectionLineItemPropertiesV2
    {

        #region properties

        public string SourceUriString { get; set; } = string.Empty;

        public string ReferenceUriString { get; set; } = "https://dummy"; // this is mandatory. don't delete it. critical for newing up album items 

        public string BlobName { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty;

        public int DisplayRank { get; set; }

        public string Guid { get; set; } = string.Empty;

        public int ID { get; set; }

        public string Label { get; set; } = string.Empty;

        #endregion

        #region methods

        public static UriItemDisplayObject FromModel(UriItem model)
        {
            const string failure = "Populating UriItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new UriItem();

                var viewModel = new UriItemDisplayObject()
                {
                    Guid = string.Empty,
                    DisplayRank = model.DisplayRank,
                    SourceUriString = model.SourceUriString,
                    ReferenceUriString = model.ReferenceUriString,
                    BlobName = model.BlobName,
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

        public static UriItemDisplayObject[] FromModel(UriItem[] model)
        {
            const string failure = "Populating UriItemViewModel.";
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

        public override string ToString()
        {
            return JghString.ConcatAsSentences(BlobName, ReferenceUriString, DisplayRank.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

    }
}