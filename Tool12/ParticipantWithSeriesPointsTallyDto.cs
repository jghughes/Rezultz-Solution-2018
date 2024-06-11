using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NetStd.Goodies.Mar2022;


//< Participant >

//    < ID > 1 </ ID >

//    < POS > 91 </ POS >

//    < Name > Wes  Hamilton </ Name >

//    < Product > Race Series - Full Series(all weeks) - get discount and free t-shirt-Member</Product>
//    <PLATE>277</PLATE>
//    <BIBTAG>951</BIBTAG>
//    <Date_x0020_of_x0020_Birth>2007-04-20</Date_x0020_of_x0020_Birth>
//    <Age>17</Age>
//    <Sex>M</Sex>
//    <Category>Sport</Category>
//    <Top_x0020_9_x0020_Points>715</Top_x0020_9_x0020_Points>
//    <Total_x0020_Points>715</Total_x0020_Points>
//    <R1>362</R1>
//    <R2>353</R2>
//    <R3>0</R3>
//    <R4>0</R4>
//    <R5>0</R5>
//    <R6>0</R6>
//    <R7>0</R7>
//    <R8>0</R8>
//    <R9>0</R9>
//    <R10>0</R10>
//    <R11>0</R11>
//    <R12>0</R12>
//    </Participant>

namespace Tool12;

[DataContract(Namespace = "", Name = XeSeriesParticipant)]
public class ParticipantWithSeriesPointsTallyDto
{
    #region Names

    public const string XeDataRootForSimpleArray = "ArrayOf" + $"{XeSeriesParticipant}";
    // this is the obligatorily named root element for a container of an array of simple stand-alone elements.
    // The format is "ArrayOf" + the name of the repeating element. The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

    public const string XeSeriesParticipant = "Participant";
    public const string XePos = "POS";
    public const string XeName = "Name";
    public const string XeProduct = "Product";
    public const string XePlate = "PLATE";
    public const string XeBibTag = "BIBTAG";
    public const string XeDateOfBirth = "Date_x0020_of_x0020_Birth";
    public const string XeAge = "Age";
    public const string XeSex = "Sex";
    public const string XeCategory = "Category";
    public const string XePointsTotalTopNine = "Top_x0020_9_x0020_Points";
    public const string XePointsTotalOverall = "Total_x0020_Points";
    public const string XeR1 = "R1";
    public const string XeR2 = "R2";
    public const string XeR3 = "R3";
    public const string XeR4 = "R4";
    public const string XeR5 = "R5";
    public const string XeR6 = "R6";
    public const string XeR7 = "R7";
    public const string XeR8 = "R8";
    public const string XeR9 = "R9";
    public const string XeR10 = "R10";
    public const string XeR11 = "R11";
    public const string XeR12 = "R12";
    public const string XeComment = "comment"; // ony used in the output, not in input from Andrew

    #endregion

    #region DataMembers

    // NB. empirically determined that JSONCONVERT fails to correctly roundtrip booleans unless EmitDefaultValue = true

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XePos)]
    public string Position { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeName)]
    public string FullName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeProduct)]
    public string Product { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XePlate)]
    public string Bib { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeBibTag)]
    public string Rfid { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeDateOfBirth)]
    public string DateOfBirthAsString { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeAge)]
    public string Age { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeSex)]
    public string Sex { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeCategory)]
    public string RaceGroup { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XePointsTotalTopNine)]
    public string PointsTotalTopNine { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 11, Name = XePointsTotalOverall)]
    public string PointsTotalOverall { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12, Name = XeR1)]
    public string T01 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 13, Name = XeR2)]
    public string T02 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 14, Name = XeR3)]
    public string T03 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 15, Name = XeR4)]
    public string T04 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 16, Name = XeR5)]
    public string T05 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 17, Name = XeR6)]
    public string T06 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 18, Name = XeR7)]
    public string T07 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 19, Name = XeR8)]
    public string T08 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 20, Name = XeR9)]
    public string T09 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 21, Name = XeR10)]
    public string T10 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 22, Name = XeR11)]
    public string T11 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 23, Name = XeR12)]
    public string T12 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 24, Name = XeComment)]
    public string Comment { get; set; } = string.Empty;


    #endregion
}