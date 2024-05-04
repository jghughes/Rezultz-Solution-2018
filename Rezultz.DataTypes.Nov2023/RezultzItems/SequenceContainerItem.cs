using System;
using System.Collections.Generic;
using NetStd.Goodies.Mar2022;

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    public class SequenceContainerItem
    {

        #region properties

        public ResultItem MostRecentResultItemToWhichThisSequenceApplies { get; set; } = new(); // src

        public Dictionary<int, double> SequenceOfSourceData { get; set; } = new(); // the Meat

        public double SequenceTotal { get; set; }

        public int SequenceTotalRankInt { get; set; }
    
        public double RelativeRankInRaceAsDecimalRatio { get; set; }

        public string FractionalRankInRaceInNumeratorOverDenominatorFormat { get; set; }

        public string FractionalRankBySexInNumeratorOverDenominatorFormat { get; set; }

        public string FractionalRankBySexPlusCategoryInNumeratorOverDenominatorFormat { get; set; }

        public double GapBehindBestInRaceInPrevailingUnitsOfSequence { get; set; }

        public int ID { get; set; } 

        #endregion

        #region methods

        public override string ToString()
        {
            return
                $"{Math.Round(SequenceTotal, 1).ToString(JghFormatSpecifiers.DecimalFormat1Dp)} {MostRecentResultItemToWhichThisSequenceApplies.FirstName ?? string.Empty} {MostRecentResultItemToWhichThisSequenceApplies.LastName ?? string.Empty} {MostRecentResultItemToWhichThisSequenceApplies.RaceGroup ?? string.Empty}";
        }


        #endregion

    }
}