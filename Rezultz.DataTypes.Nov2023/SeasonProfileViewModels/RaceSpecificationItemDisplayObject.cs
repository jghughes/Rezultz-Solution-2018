using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class RaceSpecificationItemDisplayObject : BindableBase, IHasCollectionLineItemPropertiesV1
    {
        #region properties

        //public string GroupStartTimeStampIdentifier { get; set; } = string.Empty;

        public int SeniorityRankForPointsTransfer { get; set; }

        public double TrophyPoints { get; set; }

        public string PointsScaleAsCsv { get; set; } = string.Empty;

        public double MultiplicationFactorForNormalisationOfAverageSplitDuration { get; set; }

        public double DistanceKm { get; set; }

        public string EnumString { get; set; } = string.Empty;

        public int DisplayRank { get; set; }

        public int ID { get; set; }

        public string Label { get; set; } = string.Empty;

        #endregion

        #region methods

        public static RaceSpecificationItemDisplayObject FromModel(RaceSpecificationItem model)
        {
            const string failure = "Populating RaceSpecificationItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new RaceSpecificationItem();

                var viewModel = new RaceSpecificationItemDisplayObject
                {
                    ID = model.ID,
                    Label = model.Label,
                    EnumString = model.EnumString,
                    DisplayRank = model.DisplayRank,
                    SeniorityRankForPointsTransfer = model.SeniorityRankForPointsTransfer,
                    TrophyPoints = model.TrophyPoints,
                    PointsScaleAsCsv = model.PointsScaleAsCsv,
                    MultiplicationFactorForNormalisationOfAverageSplitDuration = model.MultiplicationFactorForNormalisationOfAverageSplitDuration,
                    DistanceKm = model.DistanceKm,
                    //GroupStartTimeStampIdentifier = model.GroupStartTimeStampIdentifier,
                };

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static RaceSpecificationItemDisplayObject[] FromModel(RaceSpecificationItem[] model)
        {
            const string failure = "Populating RaceSpecificationItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                if (model is null)
                    return [];

                var viewModel = model.Select(FromModel).Where(z => z is not null).ToArray();

                return viewModel;
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
            return JghString.ConcatAsSentences(Label, EnumString);
        }

        #endregion

    }
}