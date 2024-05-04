using System.Runtime.Serialization;

namespace NetStd.DataTransferObjects.Mar2024;

[DataContract(Namespace = "", Name = XeEntityLocation)]
public class EntityLocationDto
{
    #region Names

    private const string XeEntityLocation = "entity-location";
    private const string XeAccountName = "account-name";
    private const string XeContainerName = "container-name";
    private const string XeEntityItemName = "entity-name";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeAccountName)]
    public string AccountName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeContainerName)]
    public string ContainerName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeEntityItemName)]
    public string EntityName { get; set; } = string.Empty;

    #endregion
}