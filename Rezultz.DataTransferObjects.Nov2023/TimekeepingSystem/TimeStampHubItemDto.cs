using System.Runtime.Serialization;
using NetStd.Interfaces01.July2018.Objects;

namespace Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem
{
    [DataContract(Namespace = "", Name = XeTimeStamp)]
    public class TimeStampHubItemDto : IHubItemDataTransferObject
    {
        #region Names

        public const string XeRootForContainerOfSimpleStandAloneArray = "ArrayOf" + $"{XeTimeStamp}";
        // this is the obligatorily named root element for a container of an array of simple stand alone elements.
        // The format is "ArrayOf" + the name of the repeating element.
        // The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

        public const string XeTimeStamp = "timestamp"; // the repeating element of the array
        public const string XeDnxSymbol = "dnx-symbol";

        #endregion

        #region DataMembers

        // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 0, Name = XeDnxSymbol)]
        public string DnxSymbol { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = HubItemDtoNames.XeClickCounter)]
        public int ClickCounter { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = HubItemDtoNames.XeBib)]
        public string Bib { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = HubItemDtoNames.XeRfid)]
        public string Rfid { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = HubItemDtoNames.XeRecordingModeEnum)]
        public string RecordingModeEnum { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = HubItemDtoNames.XeDatabaseActionEnum)]
        public string DatabaseActionEnum { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = HubItemDtoNames.XeMustDitchOriginatingItem)]
        public bool MustDitchOriginatingItem { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 7, Name = HubItemDtoNames.XeIsStillToBeBackedUp)]
        public bool IsStillToBeBackedUp { get; set; } = true;

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 8, Name = HubItemDtoNames.XeIsStillToBePushed)]
        public bool IsStillToBePushed { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = HubItemDtoNames.XeTouchedBy)]
        public string TouchedBy { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 10, Name = HubItemDtoNames.XeTimeStampBinaryFormat)]
        public long TimeStampBinaryFormat { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = HubItemDtoNames.XeWhenTouchedBinaryFormat)]
        public long WhenTouchedBinaryFormat { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12, Name = HubItemDtoNames.XeWhenPushedBinaryFormat)]
        public long WhenPushedBinaryFormat { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 13, Name = HubItemDtoNames.XeOriginatingItemGuid)]
        public string OriginatingItemGuid { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 14, Name = HubItemDtoNames.XeGuid)]
        public string Guid { get; set; } = string.Empty;

        #endregion
    }
}
