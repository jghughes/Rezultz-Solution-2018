using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

[DataContract(Namespace = "", Name = XeBlobSpecification)]
public class BlobSpecificationDto
{
    #region Names

    private const string XeBlobSpecification = "blob-specification";
    private const string XeNameOfBlob = "blob-name";
    private const string XeDisplayRankOfItem = "display-rank";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeNameOfBlob)]
    public string BlobName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeDisplayRankOfItem)]
    public int DisplayRank { get; set; } // used only for enum items or items that don't otherwise have a self-evident ordering of display in a collection

    #endregion
}