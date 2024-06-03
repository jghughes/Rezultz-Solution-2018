using System;
using System.Runtime.Serialization;
using NetStd.DataTransferObjects.Mar2024;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles
{
    [DataContract(Namespace = "", Name = XeSeasonProfile)]
    public class SeasonProfileDto
    {
        #region Names

        private const string XeSeasonProfile = "season-profile";
        private const string XeFragmentInThisFileName = "fragment-in-this-fileName";
        private const string XeSecurityAccessCodes = "security-access-codes";
        private const string XeTitleOfSeason = "title";
        private const string XeLabelOfSeason = "label";
        private const string XeAdvertisedDate = "advertised-date";
        private const string XeOrganizerOfSeason = "organizer";
        private const string XeSeriesProfileFileLocations = "series-profile-file-locations";
        private const string XeSeriesProfiles = "series-profiles";
        private const string XeAuthorisedIdentities = "authorised-identities";

        #endregion

        #region DataMembers

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeFragmentInThisFileName)]
        public string FragmentInFileNameOfAssociatedProfileFile { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeSecurityAccessCodes)]
        public string SecurityCodes { get; set; } = string.Empty; // list of comma-separated codes

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeTitleOfSeason)]
        public string Title { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeLabelOfSeason)]
        public string Label { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeAdvertisedDate)]
        public string AdvertisedDateAsString { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6, Name = XeOrganizerOfSeason)]
        public OrganizerDto Organizer { get; set; } = new();

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7, Name = XeSeriesProfileFileLocations)]
        public EntityLocationDto[] SeriesProfileFileLocationCollection { get; set; } = Array.Empty<EntityLocationDto>();

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 8, Name = XeSeriesProfiles)]
        public SeriesProfileDto[] SeriesProfileCollection { get; set; } = Array.Empty<SeriesProfileDto>(); // list that becomes populated later according toSeriesProfileFileLocationCollection

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 9, Name = XeAuthorisedIdentities)]
        public AuthorisedUserIdentityDto[] AuthorisedIdentityCollection { get; set; } = Array.Empty<AuthorisedUserIdentityDto>();

        #endregion
    }
}