using System;
using System.Linq;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;

namespace NetStd.DataTypes.Mar2024
{
    public class AzureStorageLocationItem
    {
        #region properties

        public string AzureStorageAccountName { get; set; }
        public string AzureStorageContainerName { get; set; }
        public string AzureStorageBlobName { get; set; }

        #endregion

        #region constructors

        public AzureStorageLocationItem()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public AzureStorageLocationItem(string accountName, string containerName, string blobName)
        {
            AzureStorageAccountName = accountName;
            AzureStorageContainerName = containerName;
            AzureStorageBlobName = blobName;
        }

        #endregion

        #region methods

        public static AzureStorageLocationItem FromDataTransferObject(AzureStorageLocationDto dto)
        {
            const string failure = "Populating AzureStorageLocationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new AzureStorageLocationDto();

                var answer = new AzureStorageLocationItem
                {
                    AzureStorageAccountName = x.AzureStorageAccountName,
                    AzureStorageContainerName = x.AzureStorageContainerName,
                    AzureStorageBlobName = x.AzureStorageBlobName,
                    //ID = x.ID,

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

        public static AzureStorageLocationItem[] FromDataTransferObject(AzureStorageLocationDto[] dto)
        {
            const string failure = "Populating AzureStorageLocationItem[].";
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

        public static AzureStorageLocationDto ToDataTransferObject(AzureStorageLocationItem item)
        {
            const string failure = "Populating AzureStorageLocationDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new AzureStorageLocationItem();

                var answer = new AzureStorageLocationDto
                {
                    AzureStorageAccountName = x.AzureStorageAccountName,
                    AzureStorageContainerName = x.AzureStorageContainerName,
                    AzureStorageBlobName = x.AzureStorageBlobName,
                    //ID = x.ID,

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

        public static AzureStorageLocationDto[] ToDataTransferObject(AzureStorageLocationItem[] item)
        {
            const string failure = "Populating AzureStorageLocationDto[].";
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

        public override string ToString()
        {
            return string.Join(" ", AzureStorageAccountName, AzureStorageContainerName, AzureStorageBlobName);
            //return string.Join(" ", ID.ToString(CultureInfo.InvariantCulture), AzureStorageAccountName, AzureStorageContainerName, BlobName);
        }

        #endregion
    }
}