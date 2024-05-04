using System;
using System.Linq;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;

namespace NetStd.DataTypes.Mar2024
{
    public class EntityLocationItem
    {
        #region properties

        public string AccountName { get; set; }
        public string ContainerName { get; set; }
        public string EntityName { get; set; }


        #endregion

        #region constructors - be sure to initialise custom-type properties

        public EntityLocationItem()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public EntityLocationItem(string accountName, string containerName)
            : this(accountName, containerName, string.Empty)
        {
        }

        public EntityLocationItem(string accountName, string containerName, string entityName)
        {
            AccountName = accountName ?? string.Empty;
            ContainerName = containerName ?? string.Empty;
            EntityName = entityName ?? string.Empty;

        }

        #endregion

        #region methods

        public override string ToString()
        {
            return string.Join(" ", AccountName, ContainerName, EntityName);
        }

        public static EntityLocationItem FromDataTransferObject(EntityLocationDto dto)
        {
            const string failure = "Populating EntityLocationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new EntityLocationDto();


                var answer = new EntityLocationItem
                {
                    AccountName = x.AccountName,
                    ContainerName = x.ContainerName,
                    EntityName = x.EntityName,
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

        public static EntityLocationItem[] FromDataTransferObject(EntityLocationDto[] dto)
        {
            const string failure = "Populating EntityLocationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dto == null)
                    return Array.Empty<EntityLocationItem>();

                var answer = dto.Select(FromDataTransferObject).Where(z => z != null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static EntityLocationDto ToDataTransferObject(EntityLocationItem item)
        {
            const string failure = "Populating EntityLocationDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {

                EntityLocationItem x = item ?? new EntityLocationItem();

                var answer = new EntityLocationDto
                {
                    AccountName = x.AccountName,
                    ContainerName = x.ContainerName,
                    EntityName = x.EntityName,
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

        public static EntityLocationDto[] ToDataTransferObject(EntityLocationItem[] item)
        {
            const string failure = "Populating EntityLocationDto[].";
            const string locus = "[ToDataTransferObject]";


            try
            {
                if (item == null)
                    return Array.Empty<EntityLocationDto>();

                var answer = item.Select(ToDataTransferObject).Where(z => z != null).ToArray();

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