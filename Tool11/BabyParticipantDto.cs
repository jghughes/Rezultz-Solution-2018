using System.Runtime.Serialization;

namespace Tool11
{
    [DataContract(Namespace = "", Name = XeParticipant)]
    public class BabyParticipantDto
    {
        #region Names

        public const string XeDataRootForSimpleArray = "ArrayOf" + $"{XeParticipant}";
        // this is the obligatorily named root element for a container of an array of simple stand-alone elements.
        // The format is "ArrayOf" + the name of the repeating element. The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

        public const string XeBib = "bib";
        public const string XeParticipant = "participant";
        public const string XeFirstName = "first-name";
        public const string XeLastName = "last-name";
        public const string XeDateOfBirth = "date-of-birth";
        public const string XeIsSeries = "is-series";
        public const string XeIsPreSeriesBirthday = "is-preseries-birthday";
        public const string XeIsMidSeriesBirthday = "is-midseries-birthday";
        public const string XeIsPostSeriesBirthday = "is-postseries-birthday";
        public const string XeDoesSwitchAgeGroup = "does-switch-agegroup";
        public const string XeAgeGroupAtStartOfSeries = "agegroup-at-start-of-series";
        public const string XeAgeGroupAtEndOfSeries = "agegroup-at-end-of-series";
        public const string XeComment = "comment";

        #endregion

        #region DataMembers

        // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XeBib)]
        public string Bib { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeFirstName)]
        public string FirstName { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeLastName)]
        public string LastName { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XeDateOfBirth)]
        public string DateOfBirthAsString { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeIsSeries)]
        public bool IsSeries { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeIsPreSeriesBirthday)]
        public bool IsPreSeriesBirthday { get; set; }
    
        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeIsMidSeriesBirthday)]
        public bool IsMidSeriesBirthday { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeIsPostSeriesBirthday)]
        public bool IsPostSeriesBirthday { get; set; }
    
        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeDoesSwitchAgeGroup)]
        public bool DoesSwitchAgeGroup { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XeAgeGroupAtStartOfSeries)]
        public string AgeGroupAtStartOfSeries { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 12, Name = XeAgeGroupAtEndOfSeries)]
        public string AgeGroupAtEndOfSeries { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 13, Name = XeComment)]
        public string Comment { get; set; } = string.Empty;


        #endregion
    }
}