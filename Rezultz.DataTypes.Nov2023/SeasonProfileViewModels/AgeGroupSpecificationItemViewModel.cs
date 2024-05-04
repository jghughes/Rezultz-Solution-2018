using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class AgeGroupSpecificationItemViewModel : BindableBase
    {
        #region props

        public int AgeLower { get; set; }

        public int AgeUpper { get; set; }

        public string Label { get; set; } = string.Empty;

        public int DisplayRank { get; set; } 

        #endregion

        #region methods

        public static AgeGroupSpecificationItemViewModel FromModel(AgeGroupSpecificationItem model)
        {
            const string failure = "Populating AgeGroupSpecificationItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new AgeGroupSpecificationItem();

                var viewModel = new AgeGroupSpecificationItemViewModel
                {
                    Label = model.Label,
                    DisplayRank = model.DisplayRank,
                    AgeLower = model.AgeLower,
                    AgeUpper = model.AgeUpper,
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

        public static AgeGroupSpecificationItemViewModel[] FromModel(AgeGroupSpecificationItem[] model)
        {
            const string failure = "Populating AgeGroupSpecificationItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                if (model == null)
                    return Array.Empty<AgeGroupSpecificationItemViewModel>();

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
            return JghString.Concat(Label);
        }

        #endregion
    }
}