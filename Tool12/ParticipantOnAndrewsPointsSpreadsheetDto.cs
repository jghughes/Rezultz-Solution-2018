using System.Runtime.Serialization;


namespace Tool12;

[DataContract(Namespace = "", Name = XeParticipant)]
public class ParticipantOnAndrewsPointsSpreadsheetDto
{
    #region DataMember names

    public const string XeDataRootForSimpleArray = "ArrayOf" + $"{XeParticipant}";
    // this is the obligatorily named root element for a container of an array of simple stand-alone elements.
    // The format is "ArrayOf" + the name of the repeating element. The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

    public const string XeParticipant = "participant";
    public const string XePos = "pos";
    public const string XeFullName = "fullname";
    public const string XeProduct = "product";
    public const string XePlate = "plate";
    public const string XeBibTag = "bibtag";
    public const string XeDateOfBirth = "dateofbirth";
    public const string XeAge = "age";
    public const string XeSex = "sex";
    public const string XeCategory = "category";
    public const string XePointsTopN = "points-topn";
    public const string XePointsOverall = "points-overall";
    public const string XeR1 = "r1";
    public const string XeR2 = "r2";
    public const string XeR3 = "r3";
    public const string XeR4 = "r4";
    public const string XeR5 = "r5";
    public const string XeR6 = "r6";
    public const string XeR7 = "r7";
    public const string XeR8 = "r8";
    public const string XeR9 = "r9";
    public const string XeR10 = "r10";
    public const string XeR11 = "r11";
    public const string XeR12 = "r12";
    public const string XeComment = "comment"; // ony used in the output, not input. a computed field for the output.

    #endregion

    #region DataMembers

    // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XePos)]
    public string Position { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeFullName)]
    public string FullName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeProduct)]
    public string Product { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XePlate)]
    public string Plate { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeBibTag)]
    public string BibTag { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeDateOfBirth)]
    public string DateOfBirthAsString { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeAge)]
    public string Age { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeSex)]
    public string Sex { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeCategory)]
    public string Category { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XePointsTopN)]
    public string PointsTopNine { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 11, Name = XePointsOverall)]
    public string PointsOverall { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12, Name = XeR1)]
    public string R1 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 13, Name = XeR2)]
    public string R2 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 14, Name = XeR3)]
    public string R3 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 15, Name = XeR4)]
    public string R4 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 16, Name = XeR5)]
    public string R5 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 17, Name = XeR6)]
    public string R6 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 18, Name = XeR7)]
    public string R7 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 19, Name = XeR8)]
    public string R8 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 20, Name = XeR9)]
    public string R9 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 21, Name = XeR10)]
    public string R10 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 22, Name = XeR11)]
    public string R11 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 23, Name = XeR12)]
    public string R12 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 24, Name = XeComment)]
    public string Comment { get; set; } = string.Empty;


    #endregion
}