using System.Runtime.Serialization;

namespace NetStd.DataTransferObjects.Mar2024;

[DataContract(Namespace = "", Name = XeAzureStorageLocation)]
public class AzureStorageLocationDto
{
    #region Names

    private const string XeAzureStorageLocation = "azure-storage-location";
    private const string XeAccountName = "azure-storage-account-name";
    private const string XeContainerName = "azure-storage-container-name";
    private const string XeBlobName = "azure-storage-blob-name";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeAccountName)]
    public string AzureStorageAccountName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeContainerName)]
    public string AzureStorageContainerName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeBlobName)]
    public string AzureStorageBlobName { get; set; } = string.Empty;

    #endregion
}