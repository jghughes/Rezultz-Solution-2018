using System;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class SeasonProfileItem
    {
        #region properties

        public string FragmentInFileNameOfAssociatedProfileFile { get; set; } = string.Empty;

        public string AccessCodes { get; set; } = string.Empty; // list of comma-separated codes

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty; 

        public DateTime AdvertisedDate { get; set; } 

        public OrganizerItem Organizer { get; set; } = new();

        public IdentityItem[] AuthorisedIdentities { get; set; } = [];

        public EntityLocationItem[] SeriesProfileFileLocations { get; set; }=[];

        public SeriesProfileItem[] SeriesProfiles { get; set; } = []; // this prop is used as a carrier - populated by the Results service - GetSeasonProfile() method


        #endregion

        #region methods

        public static SeasonProfileItem FromDataTransferObject(SeasonProfileDto dto)
        {
            const string failure = "Populating SeasonProfileItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new SeasonProfileDto();

                var answer = new SeasonProfileItem
                {
                    Label = x.Label,
                    Title = x.Title,
                    AdvertisedDate = DateTime.TryParse(x.AdvertisedDateAsString, out var dateTime) ? dateTime.Date : DateTime.Today,
                    FragmentInFileNameOfAssociatedProfileFile = x.FragmentInFileNameOfAssociatedProfileFile,
                    AccessCodes = x.SecurityCodes,
                    Organizer = OrganizerItem.FromDataTransferObject(x.Organizer),
                    SeriesProfileFileLocations = EntityLocationItem.FromDataTransferObject(x.SeriesProfileFileLocationCollection),
                    SeriesProfiles = SeriesProfileItem.FromDataTransferObject(x.SeriesProfileCollection),
                    AuthorisedIdentities = IdentityItem.FromDataTransferObject(x.AuthorisedIdentityCollection)
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

        public static SeasonProfileItem[] FromDataTransferObject(SeasonProfileDto[] dto)
        {

            const string failure = "Populating SeasonProfileItem.";
            const string locus = "[SeasonProfileDto]";

            try
            {
                if (dto is null)
                    return [];

                var answer = dto.Select(FromDataTransferObject).Where(z => z is not null).ToArray();

                return answer;

            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SeasonProfileDto ToDataTransferObject(SeasonProfileItem item)
        {
            const string failure = "Populating SeasonProfileDto.";
            const string locus = "[ToDataTransferObject]";

            try
            {
                var x = item ?? new SeasonProfileItem();

                var answer = new SeasonProfileDto
                {
                    Label = x.Label,
                    Title = x.Title,
                    AdvertisedDateAsString = x.AdvertisedDate.ToShortDateString(),
                    FragmentInFileNameOfAssociatedProfileFile = x.FragmentInFileNameOfAssociatedProfileFile,
                    SecurityCodes = x.AccessCodes,
                    Organizer = OrganizerItem.ToDataTransferObject(x.Organizer),
                    SeriesProfileFileLocationCollection = EntityLocationItem.ToDataTransferObject(x.SeriesProfileFileLocations),
                    SeriesProfileCollection = SeriesProfileItem.ToDataTransferObject(x.SeriesProfiles),
                    AuthorisedIdentityCollection = IdentityItem.ToDataTransferObject(x.AuthorisedIdentities)
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

        public override string ToString()
        {
            return JghString.ConcatAsSentences(FragmentInFileNameOfAssociatedProfileFile, Title, $"items={SeriesProfiles.Count()}");
        }

        #endregion

    }
}