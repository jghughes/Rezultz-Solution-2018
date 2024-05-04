using System;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

// ReSharper disable InconsistentNaming

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class OrganizerItemViewModel : BindableBase
    {
        #region props

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty;

        #endregion


        #region methods

        public static OrganizerItemViewModel FromModel(OrganizerItem model)
        {
            const string failure = "Populating OrganizerItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new OrganizerItem();

                var viewModel = new OrganizerItemViewModel
                {
                    Label = model.Label,
                    Title = model.Title,
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

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Label, Title);
        }

        #endregion
    }
}