using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.Results;

[DataContract(Namespace = "", Name = XeResult)] //it is critical that we set the Namespace in the DataContract attribute to a blank otherwise the deserialisation at the other end fails.
public class ResultDto
{
    #region Names

    public const string XeRootForContainerOfSimpleStandAloneArray = "ArrayOf" + $"{XeResult}"; 
    // this is the obligatorily named root element for a container of an array of simple stand alone elements.
    // The format is "ArrayOf" + the name of the repeating element.
    // The format and content is obligatory for the deserialisation to work when using the System DataContractSerializer.

    public const string XeResult = "result"; // the repeating element of the array

    public const string XeBib = "bib";
    public const string XeRfid = "rfid";
    public const string XeFirst = "first";
    public const string XeMiddle = "middle";
    public const string XeLast = "last";
    public const string XeSex = "sex"; // NB
    public const string XeAge = "age";
    public const string XeIsSeries = "isseries";

    public const string XeRace = "race";
    public const string XeAgeGroup = "agegroup";
    public const string XeTeam = "team";
    public const string XeCity = "city";

    public const string XeT01 = "t01";
    public const string XeT02 = "t02";
    public const string XeT03 = "t03";
    public const string XeT04 = "t04";
    public const string XeT05 = "t05";
    public const string XeT06 = "t06";
    public const string XeT07 = "t07";
    public const string XeT08 = "t08";
    public const string XeT09 = "t09";
    public const string XeT10 = "t10";
    public const string XeT11 = "t11";
    public const string XeT12 = "t12";
    public const string XeT13 = "t13";
    public const string XeT14 = "t14";
    public const string XeT15 = "t15";

    public const string XeDnxString = "dnx";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 0, Name = XeBib)]
    public string Bib { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XeRfid)]
    public string Rfid { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeFirst)]
    public string First { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeLast)]
    public string Last { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XeMiddle)]
    public string MiddleInitial { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeSex)]
    public string Sex { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeRace)]
    public string RaceGroup { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeAge)]
    public int Age { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 8, Name = XeAgeGroup)]
    public string AgeGroup { get; set; } = string.Empty; // this is intended to be AgeGroup.Label from TimeSplitItem

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 9, Name = XeCity)]
    public string City { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 10, Name = XeTeam)]
    public string Team { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 11, Name = XeIsSeries)]
    public bool IsSeries { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 12, Name = XeDnxString)]
    public string DnxString { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 21, Name = XeT01)]
    public string T01 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 22, Name = XeT02)]
    public string T02 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 23, Name = XeT03)]
    public string T03 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 24, Name = XeT04)]
    public string T04 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 25, Name = XeT05)]
    public string T05 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 26, Name = XeT06)]
    public string T06 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 27, Name = XeT07)]
    public string T07 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 28, Name = XeT08)]
    public string T08 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 29, Name = XeT09)]
    public string T09 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 30, Name = XeT10)]
    public string T10 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 31, Name = XeT11)]
    public string T11 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 32, Name = XeT12)]
    public string T12 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 33, Name = XeT13)]
    public string T13 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 34, Name = XeT14)]
    public string T14 { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 35, Name = XeT15)]
    public string T15 { get; set; } = string.Empty;

    #endregion
}