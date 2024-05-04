using System.Runtime.Serialization;

namespace NetStd.DataTransferObjects.Mar2024;

[DataContract(Namespace = "", Name = XeUri)]
public class UriDto
{
    #region Names

    private const string XeUri = "uri";
    private const string XeSourceUriString = "source-uri-string";
    private const string XeReferenceUriString = "reference-uri-string";
    private const string XeBlobName = "blob-name";
    private const string XeDisplayRank = "display-rank";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeSourceUriString)]
    public string SourceUriString { get; set; } = "http://www.dummy";

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeReferenceUriString)]
    public string ReferenceUriString { get; set; } = "http://www.dummy";

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeBlobName)]
    public string BlobName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeDisplayRank)]
    public int DisplayRank { get; set; } // used only for enum items or items that don't otherwise have a self-evident ordering of display in a collection

    #endregion
}