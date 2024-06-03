using System.Runtime.Serialization;
using NetStd.Interfaces01.July2018.Objects;

namespace Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem
{
    [DataContract(Namespace = "", Name = XeParticipant)]
    public class ParticipantHubItemDto : IHubItemDataTransferObject
    {
        #region Names

        public const string XeRootForContainerOfSimpleStandAloneArray = "ArrayOf" + $"{XeParticipant}";
        // this is the obligatorily named root element for a container of an array of simple stand alone elements.
        // The format is "ArrayOf" + the name of the repeating element.
        // The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

        public const string XeParticipant = "participant"; // the repeating element of the array
        public const string XeFirstName = "first-name";
        public const string XeMiddleInitial = "middle-initial";
        public const string XeLastName = "last-name";
        public const string XeGender = "gender";
        public const string XeBirthYear = "year-of-birth";
        public const string XeAge = "age";
        public const string XeAgeGroup = "age-group";
        public const string XeCity = "city";
        public const string XeTeam = "team";
        public const string XeRace = "race";
        public const string XeRaceGroupBeforeTransition = "racegroup-before-transition";
        public const string XeRaceGroupAfterTransition = "racegroup-after-transition";
        public const string XeDateOfRaceGroupTransition = "racegroup-transition-date";
        public const string XeIsSeries = "is-series";
        public const string XeSeries = "series";
        public const string XeEventIdentifiers = "event-identifiers";
        public const string XeReservation = "reservation";
        public const string XeRfid = "rfid";


        #endregion

        #region DataMembers

        // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = HubItemDtoNames.XeBib)]
        public string Bib { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeRfid)]
        public string Rfid { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeReservation)]
        public string Reservation { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = HubItemDtoNames.XeClickCounter)]
        public int ClickCounter { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeFirstName)]
        public string FirstName { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeLastName)]
        public string LastName { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeMiddleInitial)]
        public string MiddleInitial { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeGender)]
        public string Gender { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeBirthYear)]
        public int BirthYear { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XeCity)]
        public string City { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 11, Name = XeTeam)]
        public string Team { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 12, Name = XeSeries)]
        public string Series { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 13, Name = XeEventIdentifiers)]
        public string EventIdentifiers { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 14, Name = XeIsSeries)]
        public bool IsSeries { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 15, Name = XeRaceGroupBeforeTransition)]
        public string RaceGroupBeforeTransition { get; set; } // originates from Label element of RaceSpecificationItem in SeriesSettings

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 16, Name = XeRaceGroupAfterTransition)]
        public string RaceGroupAfterTransition { get; set; } // ditto

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 17, Name = XeDateOfRaceGroupTransition)]
        public string DateOfRaceGroupTransitionAsString { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 18, Name = HubItemDtoNames.XeRecordingModeEnum)]
        public string RecordingModeEnum { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 19, Name = HubItemDtoNames.XeDatabaseActionEnum)]
        public string DatabaseActionEnum { get; set; } = string.Empty;
    
        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 20, Name = HubItemDtoNames.XeIsStillToBePushed)]
        public bool IsStillToBePushed { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 21, Name = HubItemDtoNames.XeIsStillToBeBackedUp)]
        public bool IsStillToBeBackedUp { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 22, Name = HubItemDtoNames.XeMustDitchOriginatingItem)]
        public bool MustDitchOriginatingItem { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 23, Name = HubItemDtoNames.XeTouchedBy)]
        public string TouchedBy { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 24, Name = HubItemDtoNames.XeTimeStampBinaryFormat)]
        public long TimeStampBinaryFormat { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 25, Name = HubItemDtoNames.XeWhenTouchedBinaryFormat)]
        public long WhenTouchedBinaryFormat { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 26, Name = HubItemDtoNames.XeWhenPushedBinaryFormat)]
        public long WhenPushedBinaryFormat { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 27, Name = HubItemDtoNames.XeOriginatingItemGuid)]
        public string OriginatingItemGuid { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 28, Name = HubItemDtoNames.XeGuid)]
        public string Guid { get; set; } = string.Empty;

        #endregion
    }
}