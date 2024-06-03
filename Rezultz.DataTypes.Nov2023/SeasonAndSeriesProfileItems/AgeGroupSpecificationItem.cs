using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class AgeGroupSpecificationItem
    {
        #region props
  

        public int AgeLower { get; set; }

        public int AgeUpper { get; set; }

        public int DisplayRank { get; set; } 

        public string Label { get; set; } = string.Empty; 

        #endregion

        #region methods

        public override string ToString()
        {
            return JghString.Concat(Label);
        }

        public static AgeGroupSpecificationItem FromDataTransferObject(AgeGroupSpecificationDto dto)
        {
            const string failure = "Populating AgeGroupSpecification.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new AgeGroupSpecificationDto();

                var answer = new AgeGroupSpecificationItem
                {
                    Label = x.Label,
                    DisplayRank = x.DisplayRank,
                    AgeLower = x.AgeLowerBound,
                    AgeUpper = x.AgeUpperBound
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

        public static AgeGroupSpecificationItem[] FromDataTransferObject(AgeGroupSpecificationDto[] dto)
        {
            const string failure = "Populating AgeGroupSpecification.";
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

        public static AgeGroupSpecificationDto ToDataTransferObject(AgeGroupSpecificationItem item)
        {
            const string failure = "Populating AgeGroupSpecificationDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new AgeGroupSpecificationItem();

                var answer = new AgeGroupSpecificationDto
                {
                    Label = x.Label,
                    DisplayRank = x.DisplayRank,
                    AgeLowerBound = x.AgeLower,
                    AgeUpperBound = x.AgeUpper
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

        public static AgeGroupSpecificationDto[] ToDataTransferObject(AgeGroupSpecificationItem[] item)
        {
            const string failure = "Populating AgeGroupSpecificationDto.";
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

        #endregion
    }
}