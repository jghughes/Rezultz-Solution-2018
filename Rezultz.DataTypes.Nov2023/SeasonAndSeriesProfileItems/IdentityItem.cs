using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class IdentityItem
    {
        #region ctor

        public IdentityItem() { }

        public IdentityItem(string userName, string password, string accessLevel, string[] arrayOfAuthorisedWorkRoles)
    {
        UserName = userName ?? string.Empty;
        Password = password ?? string.Empty;
        AccessLevel = accessLevel ?? string.Empty;
        ArrayOfAuthorisedWorkRoles = arrayOfAuthorisedWorkRoles ?? [];
    }

        #endregion

        #region properties

        public string UserName { get; }

        public string Password { get; }

        public string AccessLevel { get; set; }

        public string[] ArrayOfAuthorisedWorkRoles { get; }


        #endregion

        #region methods

        public Tuple<string, string> GetPrimaryKey()
    {
        return new Tuple<string, string>(UserName, Password);
    }

        public static bool Equals(IdentityItem identity1, IdentityItem identity2)
    {
        return JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(identity1.UserName, identity2.UserName) &&
               JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(identity1.Password, identity2.Password);
    }

        public static IdentityItem FromDataTransferObject(AuthorisedUserIdentityDto dto)
    {
        const string failure = "Populating AuthorisedIdentity.";
        const string locus = "[FromDataTransferObject]";

        try
        {
            var x = dto ?? new AuthorisedUserIdentityDto();

            var answer = new IdentityItem(
                x.UserName,
                x.Password,
                x.AccessLevel,
                arrayOfAuthorisedWorkRoles: string.IsNullOrWhiteSpace(x.AuthorisedWorkRoles) ? [] : x.AuthorisedWorkRoles.Split(','));

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

        public static IdentityItem[] FromDataTransferObject(AuthorisedUserIdentityDto[] dto)
    {
        const string failure = "Populating AuthorisedIdentity[].";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto is null)
                return [];

            var answer = dto.Select(FromDataTransferObject).Where(z => z is not null).ToArray();

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

        public static AuthorisedUserIdentityDto ToDataTransferObject(IdentityItem item)
    {
        const string failure = "Populating AuthorisedIdentityDto.";
        const string locus = "[ToDataTransferObject]";


        try
        {
            var x = item ?? new IdentityItem();

            var answer = new AuthorisedUserIdentityDto
            {
                UserName = x.UserName,
                Password = x.Password,
                AccessLevel = x.AccessLevel,
                AuthorisedWorkRoles = x.ArrayOfAuthorisedWorkRoles is null ? string.Empty : string.Join(",", x.ArrayOfAuthorisedWorkRoles),
            };

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

        public static AuthorisedUserIdentityDto[] ToDataTransferObject(IdentityItem[] item)
    {
        const string failure = "Populating AuthorisedIdentityDto[].";
        const string locus = "[ToDataTransferObject]";


        try
        {
            if (item is null)
                return [];

            var answer = item.Select(ToDataTransferObject).Where(z => z is not null).ToArray();

            return answer;

        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

        #endregion
    }
}