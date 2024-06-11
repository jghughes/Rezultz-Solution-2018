using System.Runtime.Serialization;

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    public class DerivedDataItem
    {

        #region props

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int ID { get; set; } // primary key

        public DerivedDataItem ShallowMemberwiseCloneCopy
        {
            get
            {
                var other = (DerivedDataItem)MemberwiseClone();
                //be sure to new up Reference types if any. do so here
                // ....

                return other;
            }
        }

        public bool IsValidDuration { get; set; }

        public bool IsValidDnx { get; set; }

        public int PlaceCalculatedOverallInt { get; set; }

        public int PlaceCalculatedOverallIntExcludingNonSeriesParticipants { get; set; }

        public string DnxStringFromAlgorithm { get; set; }

        public double TotalDurationFromAlgorithmInSeconds { get; set; }

        public double TimeGapBehindWinnerOfRaceInSeconds { get; set; }

        public double TimeGapBehindWinnerOfSubsetOfSexWithinRaceInSeconds { get; set; }

        public double TimeGapBehindWinnerOfSubsetOfAgeGroupWithinSexWithinRaceInSeconds { get; set; }

        public int SplitsBehindWinnerOfRace { get; set; }

        public int SplitsBehindWinnerOfSubsetOfSexWithinRace { get; set; }

        public int SplitsBehindWinnerOfSubsetOfAgeGroupWithinSexWithinRace { get; set; }

        public int CalculatedNumOfSplitsCompleted { get; set; }

        public int CalculatedRankInSubsetOfSexWithinRace { get; set; }

        public int CalculatedRankInSubsetOfAgeGroupWithinSexWithinRace { get; set; }

        public int CalculatedRankInSubsetOfSexWithinRaceExcludingNonSeriesParticipants { get; set; }

        public int CalculatedRankInSubsetOfAgeGroupWithinSexWithinRaceExcludingNonSeriesParticipants { get; set; }

        public int TotalFinishersInRace { get; set; }

        public int TotalFinishersInSubsetOfSexWithinRace { get; set; }

        public int TotalFinishersInSubsetOfAgeGroupWithinSexWithinRace { get; set; }

        public double PointsCalculated { get; set; }

        //public double AverageSplitTimeInSeconds { get; set; }

        public double SpeedKph { get; set; }

        #endregion

    }
}