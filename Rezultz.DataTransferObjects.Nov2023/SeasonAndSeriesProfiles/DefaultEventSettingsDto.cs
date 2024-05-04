using System;
using System.Runtime.Serialization;
using NetStd.DataTransferObjects.Mar2024;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

[DataContract(Namespace = "", Name = XeEventSettings)]
public class DefaultEventSettingsDto
{
    #region Names

    private const string XeEventSettings = "event-settings";
    private const string XeAlgorithmForCalculatingPointsId = "algorithm-for-calculating-points-enum-string";
    private const string XeOneLinerBlurbMessage = "oneliner-contact-message";
    private const string XeEventOrganizerEmailAddress = "organizer-email-address";
    private const string XeEventOrganizerFacebookUri = "organizer-facebook-uri";
    private const string XeEventOrganizerInstagramUri = "organizer-instagram-uri";
    private const string XeEventOrganizerTwitterUri = "organizer-twitter-uri";
    private const string XeMoreEventInformationItems = "event-information";
    private const string XeMoreEventAnalysisItems = "event-analyses";
    private const string XeAllRaceSpecifications = "race-specifications";
    private const string XeAllAgeGroupSpecifications = "age-group-specifications";
    private const string XeAllUris = "uris";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeAlgorithmForCalculatingPointsId)]
    public string AlgorithmForCalculatingPointsEnumString { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeOneLinerBlurbMessage)]
    public string OneLinerContactMessage { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeEventOrganizerEmailAddress)]
    public string OrganizerEmailAddress { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeEventOrganizerFacebookUri)]
    public string OrganizerFacebookUri { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeEventOrganizerInstagramUri)]
    public string OrganizerInstagramUri { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6, Name = XeEventOrganizerTwitterUri)]
    public string OrganizerTwitterUri { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7, Name = XeMoreEventInformationItems)]
    public MoreInformationDto[] EventInformationArray { get; set; } = Array.Empty<MoreInformationDto>();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 8, Name = XeMoreEventAnalysisItems)]
    public MoreInformationDto[] EventAnalyses { get; set; } = Array.Empty<MoreInformationDto>();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 9, Name = XeAllRaceSpecifications)]
    public RaceSpecificationDto[] RaceSpecifications { get; set; } = Array.Empty<RaceSpecificationDto>();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 10, Name = XeAllAgeGroupSpecifications)]
    public AgeGroupSpecificationDto[] AgeGroupSpecifications { get; set; } = Array.Empty<AgeGroupSpecificationDto>();

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = XeAllUris)]
    public UriDto[] Uris { get; set; } = Array.Empty<UriDto>();

    #endregion
}