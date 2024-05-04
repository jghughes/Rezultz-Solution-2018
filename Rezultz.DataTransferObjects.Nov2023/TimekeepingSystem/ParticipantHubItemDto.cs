using System.Runtime.Serialization;
using NetStd.Interfaces01.July2018.Objects;

namespace Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;

[DataContract(Namespace = "", Name = XeParticipant)]
public class ParticipantHubItemDto : IHubItemDataTransferObject
{
    #region Names

    public const string XeArrayOfParticipant = "ArrayOfParticipant";
    public const string XeParticipant = "Participant";
    public const string XeFirstName = "FirstName";
    public const string XeMiddleInitial = "MiddleInitial";
    public const string XeLastName = "LastName";
    public const string XeGender = "Gender";
    public const string XeBirthYear = "BirthYear";
    public const string XeAge = "Age";
    public const string XeAgeGroup = "AgeGroup";
    public const string XeCity = "City";
    public const string XeTeam = "Team";
    public const string XeRace = "Race";
    public const string XeRaceGroupBeforeTransition = "RaceGroupBeforeTransition";
    public const string XeRaceGroupAfterTransition = "RaceGroupAfterTransition";
    public const string XeDateOfRaceGroupTransition = "XeDateOfRaceGroupTransition";
    public const string XeIsSeries = "IsSeries";
    public const string XeSeries = "Series";
    public const string XeEventIdentifiers = "EventIdentifiers";

    #endregion

    #region DataMembers

    // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = HubItemXeNames.Identifier)]
    public string Identifier { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeFirstName)]
    public string FirstName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeLastName)]
    public string LastName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XeMiddleInitial)]
    public string MiddleInitial { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeGender)]
    public string Gender { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeBirthYear)]
    public int BirthYear { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeCity)]
    public string City { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeTeam)]
    public string Team { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeRaceGroupBeforeTransition)]
    public string RaceGroupBeforeTransition { get; set; } // originates from Label element of RaceSpecificationItem in SeriesSettings

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XeRaceGroupAfterTransition)]
    public string RaceGroupAfterTransition { get; set; } // originates from Label element of RaceSpecificationItem in SeriesSettings

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = XeDateOfRaceGroupTransition)]
    public string DateOfRaceGroupTransitionAsString { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 12, Name = XeIsSeries)]
    public bool IsSeries { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 13, Name = XeSeries)]
    public string Series { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 14, Name = XeEventIdentifiers)]
    public string EventIdentifiers { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 15, Name = HubItemXeNames.ClickCounter)]
    public int ClickCounter { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 16, Name = HubItemXeNames.RecordingModeEnum)]
    public string RecordingModeEnum { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 17, Name = HubItemXeNames.DatabaseActionEnum)]
    public string DatabaseActionEnum { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 18, Name = HubItemXeNames.MustDitchOriginatingItem)]
    public bool MustDitchOriginatingItem { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 19, Name = HubItemXeNames.IsStillToBeBackedUp)]
    public bool IsStillToBeBackedUp { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 20, Name = HubItemXeNames.IsStillToBePushed)]
    public bool IsStillToBePushed { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 21, Name = HubItemXeNames.TouchedBy)]
    public string TouchedBy { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 22, Name = HubItemXeNames.TimeStampBinaryFormat)]
    public long TimeStampBinaryFormat { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 23, Name = HubItemXeNames.WhenTouchedBinaryFormat)]
    public long WhenTouchedBinaryFormat { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 24, Name = HubItemXeNames.WhenPushedBinaryFormat)]
    public long WhenPushedBinaryFormat { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 25, Name = HubItemXeNames.OriginatingItemGuid)]
    public string OriginatingItemGuid { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 26, Name = HubItemXeNames.Guid)]
    public string Guid { get; set; } = string.Empty;

    #endregion
}