using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class SettingsForEventItemViewModel : BindableBase
    {
        #region props

        public string AlgorithmForCalculatingPointsEnumString { get; set; } = string.Empty;

        public string OneLinerContactMessage { get; set; } = string.Empty;

        public string OrganizerEmailAddress { get; set; } = string.Empty;

        public string OrganizerFacebookUri { get; set; } = string.Empty;

        public string OrganizerInstagramUri { get; set; } = string.Empty;

        public string OrganizerTwitterUri { get; set; } = string.Empty;

        public MoreInformationItemDisplayObject[] ArrayOfMoreEventInformationItems { get; set; } = Array.Empty<MoreInformationItemDisplayObject>();

        public MoreInformationItemDisplayObject[] ArrayOfEventAnalysisItems { get; set; } = Array.Empty<MoreInformationItemDisplayObject>();

        public RaceSpecificationItemDisplayObject[] ArrayOfRaceSpecificationItems { get; set; } = Array.Empty<RaceSpecificationItemDisplayObject>();

        public AgeGroupSpecificationItemViewModel[] ArrayOfAgeGroupSpecificationItems { get; set; } = Array.Empty<AgeGroupSpecificationItemViewModel>();

        public UriItemDisplayObject[] ArrayOfUriItems { get; set; } = Array.Empty<UriItemDisplayObject>();

        #endregion

        #region methods

        public static SettingsForEventItemViewModel FromModel(EventSettingsItem model)
        {
            const string failure = "Populating SettingsForEventItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new EventSettingsItem();

                var viewModel = new SettingsForEventItemViewModel
                {
                    AlgorithmForCalculatingPointsEnumString = model.AlgorithmForCalculatingPointsEnumString,
                    OneLinerContactMessage = model.OneLinerContactMessage,
                    OrganizerEmailAddress = model.OrganizerEmailAddress,
                    OrganizerFacebookUri = model.OrganizerFacebookUri,
                    OrganizerInstagramUri = model.OrganizerInstagramUri,
                    OrganizerTwitterUri = model.OrganizerTwitterUri,
                    ArrayOfMoreEventInformationItems = MoreInformationItemDisplayObject.FromModel(model.MoreInformationItems),
                    ArrayOfEventAnalysisItems = MoreInformationItemDisplayObject.FromModel(model.EventAnalysisItems),
                    ArrayOfRaceSpecificationItems = RaceSpecificationItemDisplayObject.FromModel(model.RaceSpecificationItems),
                    ArrayOfAgeGroupSpecificationItems = AgeGroupSpecificationItemViewModel.FromModel(model.AgeGroupSpecificationItems).ToArray(),
                    ArrayOfUriItems = UriItemDisplayObject.FromModel(model.UriItems),
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



        #endregion
    }
}