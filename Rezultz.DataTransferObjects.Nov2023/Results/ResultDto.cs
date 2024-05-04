using System.Runtime.Serialization;
using Jgh.SymbolsStringsConstants.Mar2022;

namespace Rezultz.DataTransferObjects.Nov2023.Results;

[DataContract(Namespace = "", Name = XeResult)] //it is critical that we set the Namespace in the DataContract attribute to a blank otherwise the deserialisation at the other end fails for reasons unknown.
public class ResultDto
{
    #region Names

    public const string XeResult = "Result";
    public const string XeArrayOfResult = "ArrayOfResult";

    public const string XeFirst = "First";
    public const string XeMiddle = "Middle";
    public const string XeLast = "Last";
    public const string XeSex = "Sex"; // NB
    public const string XeAge = "Age";
    public const string XeIsSeries = "IsSeries";

    public const string XeRace = "Race";
    public const string XeAgeGroup = "AgeGroup";
    public const string XeTeam = "Team";
    public const string XeCity = "City";

    public const string XeT01 = "T01";
    public const string XeT02 = "T02";
    public const string XeT03 = "T03";
    public const string XeT04 = "T04";
    public const string XeT05 = "T05";
    public const string XeT06 = "T06";
    public const string XeT07 = "T07";
    public const string XeT08 = "T08";
    public const string XeT09 = "T09";
    public const string XeT10 = "T10";
    public const string XeT11 = "T11";
    public const string XeT12 = "T12";
    public const string XeT13 = "T13";
    public const string XeT14 = "T14";
    public const string XeT15 = "T15";

    public const string XeDnxString = "Dnx";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = UbiquitousFieldNames.XeBib)]
    public string Bib { get; set; } = string.Empty;

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