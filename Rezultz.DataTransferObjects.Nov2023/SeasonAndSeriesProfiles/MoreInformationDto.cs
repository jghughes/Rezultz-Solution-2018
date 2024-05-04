using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

[DataContract(Namespace = "", Name = XeMoreInformation)]
public class MoreInformationDto
{
    #region Names

    private const string XeMoreInformation = "more-information";
    private const string XeTitleOfItem = "title";
    private const string XeLabelOfItem = "label";
    private const string XeEnumStringOfItem = "enum-string"; // switch in "case" statements
    private const string XeBlurbOfItem = "blurb";
    private const string XeDisplayRankOfItem = "display-rank";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeTitleOfItem)]
    public string Title { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeLabelOfItem)]
    public string Label { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeEnumStringOfItem)]
    public string EnumString { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeBlurbOfItem)]
    public string Blurb { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeDisplayRankOfItem)]
    public int DisplayRank { get; set; } // used only for enum items or items that don't otherwise have a self-evident ordering of display in a collection

    #endregion
}