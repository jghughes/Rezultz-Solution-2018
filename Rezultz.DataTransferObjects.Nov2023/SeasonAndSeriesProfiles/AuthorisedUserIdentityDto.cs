using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

[DataContract(Namespace = "", Name = XeAuthorisedIdentity)]
public class AuthorisedUserIdentityDto
{
    #region Names

    private const string XeAuthorisedIdentity = "authorised-identity";
    private const string XeNameOfUser = "user-name";
    private const string XePasswordOfUser = "password";
    private const string XeAuthorisedWorkRolesOfUser = "authorised-work-roles";
    private const string XeAccessLevelOfUser = "access-level";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeNameOfUser)]
    public string UserName { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XePasswordOfUser)]
    public string Password { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeAuthorisedWorkRolesOfUser)]
    public string AuthorisedWorkRoles { get; set; } // comma separated values

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeAccessLevelOfUser)]
    public string AccessLevel { get; set; }


    #endregion
}