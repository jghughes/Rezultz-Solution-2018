using System;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class EventSettingsItem
    {
        #region props

        public string AlgorithmForCalculatingPointsEnumString { get; set; } = string.Empty;

        public string OneLinerContactMessage { get; set; } = string.Empty;

        public string OrganizerEmailAddress { get; set; } = string.Empty;

        public string OrganizerFacebookUri { get; set; } = string.Empty;

        public string OrganizerInstagramUri { get; set; } = string.Empty;

        public string OrganizerTwitterUri { get; set; } = string.Empty;

        public MoreInformationItem[] MoreInformationItems { get; set; } = [];

        public MoreInformationItem[] EventAnalysisItems { get; set; } = [];

        public RaceSpecificationItem[] RaceSpecificationItems { get; set; } = [];

        public AgeGroupSpecificationItem[] AgeGroupSpecificationItems { get; set; } = [];

        public UriItem[] UriItems { get; set; } = [];

        #endregion

        #region methods

        public static EventSettingsItem FromDataTransferObject(DefaultEventSettingsDto dto)
        {
            const string failure = "Populating EventSettings.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new DefaultEventSettingsDto();

                var answer = new EventSettingsItem
                {
                    AlgorithmForCalculatingPointsEnumString = x.AlgorithmForCalculatingPointsEnumString,
                    OneLinerContactMessage = x.OneLinerContactMessage,
                    OrganizerEmailAddress = x.OrganizerEmailAddress,
                    OrganizerFacebookUri = x.OrganizerFacebookUri,
                    OrganizerInstagramUri = x.OrganizerInstagramUri,
                    OrganizerTwitterUri = x.OrganizerTwitterUri,
                    MoreInformationItems = MoreInformationItem.FromDataTransferObject(x.EventInformationArray),
                    EventAnalysisItems = MoreInformationItem.FromDataTransferObject(x.EventAnalyses),
                    RaceSpecificationItems = RaceSpecificationItem.FromDataTransferObject(x.RaceSpecifications),
                    AgeGroupSpecificationItems = AgeGroupSpecificationItem.FromDataTransferObject(x.AgeGroupSpecifications).ToArray(),
                    UriItems = UriItem.FromDataTransferObject(x.Uris),
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

        public static DefaultEventSettingsDto ToDataTransferObject(EventSettingsItem item)
        {
            const string failure = "Populating SettingsForEventItemDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new EventSettingsItem();

                var answer = new DefaultEventSettingsDto
                {
                    AlgorithmForCalculatingPointsEnumString = x.AlgorithmForCalculatingPointsEnumString,
                    OneLinerContactMessage = x.OneLinerContactMessage,
                    OrganizerEmailAddress = x.OrganizerEmailAddress,
                    OrganizerFacebookUri = x.OrganizerFacebookUri,
                    OrganizerInstagramUri = x.OrganizerInstagramUri,
                    OrganizerTwitterUri = x.OrganizerTwitterUri,
                    EventInformationArray = MoreInformationItem.ToDataTransferObject(x.MoreInformationItems),
                    EventAnalyses = MoreInformationItem.ToDataTransferObject(x.EventAnalysisItems),
                    RaceSpecifications = RaceSpecificationItem.ToDataTransferObject(x.RaceSpecificationItems),
                    AgeGroupSpecifications = AgeGroupSpecificationItem.ToDataTransferObject(x.AgeGroupSpecificationItems).ToArray(),
                    Uris = UriItem.ToDataTransferObject(x.UriItems),
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


        #endregion
    }
}