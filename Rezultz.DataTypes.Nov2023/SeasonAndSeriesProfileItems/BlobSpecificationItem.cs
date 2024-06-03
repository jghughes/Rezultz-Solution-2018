using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class BlobSpecificationItem
    {

        #region properties

        public string BlobName { get; set; } = string.Empty;

        public int DisplayRank { get; set; } 

        #endregion

        #region methods

        public static BlobSpecificationItem FromDataTransferObject(BlobSpecificationDto dto)
        {
            const string failure = "Populating BlobSpecificationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new BlobSpecificationDto();


                var answer = new BlobSpecificationItem
                {
                    DisplayRank = x.DisplayRank,
                    BlobName = x.BlobName,
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

        public static BlobSpecificationItem[] FromDataTransferObject(BlobSpecificationDto[] dto)
        {
            const string failure = "Populating BlobSpecificationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dto == null)
                    return [];

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

        public static BlobSpecificationDto ToDataTransferObject(BlobSpecificationItem item)
        {
            const string failure = "Populating AzureStorageBlobSpecificationDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new BlobSpecificationItem();

                var answer = new BlobSpecificationDto
                {
                    DisplayRank = x.DisplayRank,
                    BlobName = x.BlobName,
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

        public static BlobSpecificationDto[] ToDataTransferObject(BlobSpecificationItem[] item)
        {
            const string failure = "Populating AzureStorageBlobSpecificationDto.";
            const string locus = "[ToDataTransferObject]";

            try
            {
                if (item == null)
                    return [];

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

        public override string ToString()
        {
            return string.Join(" ", BlobName);
        }

        #endregion

    }
}