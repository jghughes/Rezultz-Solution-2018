using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class BlobSpecificationItemViewModel : BindableBase
    {
        #region properties

        public string BlobName { get; set; } = string.Empty;

        public int DisplayRank { get; set; } 

        #endregion

        #region methods

        public static BlobSpecificationItemViewModel FromModel(BlobSpecificationItem model)
        {
            const string failure = "Populating BlobSpecificationItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new BlobSpecificationItem();

                var viewModel = new BlobSpecificationItemViewModel
                {
                    BlobName = model.BlobName,
                    DisplayRank = model.DisplayRank,
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

        public static BlobSpecificationItemViewModel[] FromModel(BlobSpecificationItem[] model)
        {
            const string failure = "Populating BlobSpecificationItemViewModel.";
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

        public override string ToString()
        {
            return string.Join(" ", BlobName);
        }

        #endregion

    }
}