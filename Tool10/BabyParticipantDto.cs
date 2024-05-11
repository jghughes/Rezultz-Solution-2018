using System.Runtime.Serialization;
using NetStd.Interfaces01.July2018.Objects;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;

namespace Tool10;

[DataContract(Namespace = "", Name = XeParticipant)]
public class BabyParticipantDto
{
    #region Names

    public const string XeDataRootForContainerOfSimpleStandAloneArray = "ArrayOf" + $"{XeParticipant}";
    // this is the obligatorily named root element for a container of an array of simple stand-alone elements.
    // The format is "ArrayOf" + the name of the repeating element. The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

    public const string XeParticipant = "participant";
    public const string XeFirstName = "first-name";
    public const string XeMiddleInitial = "middle-initial";
    public const string XeLastName = "last-name";
    public const string XeGender = "gender";
    public const string XeBirthYear = "year-of-birth";
    public const string XeCity = "city";
    public const string XeTeam = "team";
    public const string XeRaceGroupBeforeTransition = "racegroup-before-transition";
    public const string XeRaceGroupAfterTransition = "racegroup-after-transition";
    public const string XeDateOfRaceGroupTransition = "racegroup-transition-date";
    public const string XeIsSeries = "is-series";
    public const string XeSeries = "series";
    public const string XeReservation = "reservation";
    public const string XeRfid = "rfid";

    #endregion

    #region DataMembers

    // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = HubItemDto.XeBib)]
    public string Bib { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeFirstName)]
    public string FirstName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeLastName)]
    public string LastName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XeMiddleInitial)]
    public string MiddleInitial { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeGender)]
    public string Gender { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeBirthYear)]
    public string BirthYear { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeCity)]
    public string City { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeTeam)]
    public string Team { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeRaceGroupBeforeTransition)]
    public string RaceGroupBeforeTransition { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XeRaceGroupAfterTransition)]
    public string RaceGroupAfterTransition { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = XeDateOfRaceGroupTransition)]
    public string DateOfRaceGroupTransitionAsString { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 12, Name = XeIsSeries)]
    public bool IsSeries { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 13, Name = XeSeries)]
    public string Series { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 14, Name = XeRfid)]
    public string Rfid { get; set; } = string.Empty;


    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 15, Name = XeReservation)]
    public string Reservation { get; set; } = string.Empty;



    #endregion
}