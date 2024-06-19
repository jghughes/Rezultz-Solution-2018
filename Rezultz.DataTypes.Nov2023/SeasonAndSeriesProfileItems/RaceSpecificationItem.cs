using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class RaceSpecificationItem
    {
        #region properties

        public int SeniorityRankForPointsTransfer { get; set; }

        public double TrophyPoints { get; set; }

        public string PointsScaleAsCsv { get; set; } = string.Empty;

        public double MultiplicationFactorForNormalisationOfAverageSplitDuration { get; set; }

        public double DistanceKm { get; set; }

        public string Label { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty;

        public int DisplayRank { get; set; }

        public int ID { get; set; }


        #endregion

        #region methods

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Label, EnumString);
        }

        public static RaceSpecificationItem FromDataTransferObject(RaceSpecificationDto dto)
        {
            const string failure = "Populating RaceSpecificationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new RaceSpecificationDto();

                var answer = new RaceSpecificationItem
                {
                    Label = x.Label,
                    DisplayRank = x.DisplayRank,
                    SeniorityRankForPointsTransfer = x.SeniorityRankForPointsTransfer,
                    TrophyPoints = x.TrophyPoints,
                    PointsScaleAsCsv = x.PointsScaleAsCsv,
                    MultiplicationFactorForNormalisationOfAverageSplitDuration = x.MultiplicationFactorForNormalizationOfAverageSplitDuration,
                    DistanceKm = x.DistanceKm,
                    //GroupStartTimeStampIdentifier = x.GroupStartTimeStampIdentifier
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

        public static RaceSpecificationItem[] FromDataTransferObject(RaceSpecificationDto[] dto)
        {
            const string failure = "Populating RaceSpecificationItem.";
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

        public static RaceSpecificationDto ToDataTransferObject(RaceSpecificationItem item)
        {
            const string failure = "Populating RaceSpecificationDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new RaceSpecificationItem();

                var answer = new RaceSpecificationDto
                {
                    Label = x.Label,
                    DisplayRank = x.DisplayRank,
                    SeniorityRankForPointsTransfer = x.SeniorityRankForPointsTransfer,
                    TrophyPoints = x.TrophyPoints,
                    PointsScaleAsCsv = x.PointsScaleAsCsv,
                    MultiplicationFactorForNormalizationOfAverageSplitDuration = x.MultiplicationFactorForNormalisationOfAverageSplitDuration,
                    DistanceKm = x.DistanceKm,
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

        public static RaceSpecificationDto[] ToDataTransferObject(RaceSpecificationItem[] item)
        {
            const string failure = "Populating RaceSpecificationDto.";
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