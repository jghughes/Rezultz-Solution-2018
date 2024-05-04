using System.Runtime.Serialization;
using NetStd.Interfaces01.July2018.Objects;

namespace Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;

[DataContract(Name = XeTimeStamp, Namespace = "")]
public class TimeStampHubItemDto : IHubItemDataTransferObject
{
    #region Names

    public const string XeArrayOfTimeStamp = "ArrayOfTimeStamp";
    public const string XeTimeStamp = "TimeStamp";
    public const string XeDnxSymbol = "DnxSymbol";

    #endregion

    #region DataMembers

    // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XeDnxSymbol)]
    public string DnxSymbol { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = HubItemXeNames.ClickCounter)]
    public int ClickCounter { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = HubItemXeNames.Identifier)]
    public string Identifier { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = HubItemXeNames.RecordingModeEnum)]
    public string RecordingModeEnum { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = HubItemXeNames.DatabaseActionEnum)]
    public string DatabaseActionEnum { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = HubItemXeNames.MustDitchOriginatingItem)]
    public bool MustDitchOriginatingItem { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 7, Name = HubItemXeNames.IsStillToBeBackedUp)]
    public bool IsStillToBeBackedUp { get; set; } = true;

    [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 8, Name = HubItemXeNames.IsStillToBePushed)]
    public bool IsStillToBePushed { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = HubItemXeNames.TouchedBy)]
    public string TouchedBy { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 10, Name = HubItemXeNames.TimeStampBinaryFormat)]
    public long TimeStampBinaryFormat { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11, Name = HubItemXeNames.WhenTouchedBinaryFormat)]
    public long WhenTouchedBinaryFormat { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12, Name = HubItemXeNames.WhenPushedBinaryFormat)]
    public long WhenPushedBinaryFormat { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 13, Name = HubItemXeNames.OriginatingItemGuid)]
    public string OriginatingItemGuid { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 14, Name = HubItemXeNames.Guid)]
    public string Guid { get; set; } = string.Empty;

    #endregion
}
