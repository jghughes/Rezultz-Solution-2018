using System.Runtime.Serialization;

namespace Tool04;

[DataContract(Name = "MyLapsResultObjectDataTransferObject", Namespace = "")] //it is critical that we set the Namespace in the DataContract attribute to a blank otherwise the deserialisation at the other end fails for reasons unknown.
internal class MyLapsResultObjectDto
{
    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1)] public string BibNumber { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2)] public string FullName { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3)] public string Duration { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4)] public string RaceGroup { get; set; } = string.Empty;
}