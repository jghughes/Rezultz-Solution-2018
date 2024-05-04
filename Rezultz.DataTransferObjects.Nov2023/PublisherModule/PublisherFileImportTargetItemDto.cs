using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.PublisherModule;

[DataContract(Namespace = "", Name = XePublisherFileImportTarget)]
public class PublisherFileImportTargetItemDto
{
    #region Names

    private const string XePublisherFileImportTarget = "publisher-file-import-target";
    private const string XeDatasetId = "dataset-identifier";
    private const string XeNameOfFile = "filename";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeDatasetId)]
    public string DatasetIdentifier { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeNameOfFile)]
    public string FileName { get; set; } = string.Empty;

    #endregion
}