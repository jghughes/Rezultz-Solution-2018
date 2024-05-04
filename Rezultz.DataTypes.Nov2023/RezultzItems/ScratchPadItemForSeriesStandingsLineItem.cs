using System;
using NetStd.Goodies.Mar2022;

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    public class ScratchPadItemForSeriesStandingsLineItem
    {
        #region ctor

        public ScratchPadItemForSeriesStandingsLineItem()
            : this(new SequenceContainerItem())
        {
        }

        public ScratchPadItemForSeriesStandingsLineItem(SequenceContainerItem pointsLineContainerItem)
        {
            ScratchPadItemGuid = Guid.NewGuid();

            SeriesStandingsTableLineItemId = pointsLineContainerItem.ID;
        }

        #endregion

        #region properties

        // primary key
        public Guid ScratchPadItemGuid { get; }

        // foreign key
        public int SeriesStandingsTableLineItemId { get; set; } // assign the ID ONLY when time comes to use it

        public int RankInSubsetInt { get; set; }

        public double RelativeRankInSubsetAsDecimalRatio { get; set; }

        public string FractionalRankInSubsetAsNumeratorOverDenominator { get; set; }

        public double GapBehindBestInSubsetInPrevailingUnitsOfSequence { get; set; }

        //public double MetricCalculated { get; set; }

        #endregion

        #region methods

        public override string ToString()
        {
            return
                $"{ScratchPadItemGuid} {SeriesStandingsTableLineItemId} {RankInSubsetInt} {RelativeRankInSubsetAsDecimalRatio.ToString(JghFormatSpecifiers.DecimalFormat0Dp)} {FractionalRankInSubsetAsNumeratorOverDenominator} ";
            //return
            //    $"{ScratchPadItemGuid} {SeriesStandingsTableLineItemId} {RankInSubsetInt} {RelativeRankInSubsetAsDecimalRatio.ToString(JghFormatSpecifiers.DecimalFormat0Dp)} {FractionalRankInSubsetAsNumeratorOverDenominator} {MetricCalculated.ToString(JghFormatSpecifiers.DecimalFormat0Dp)}";
        }

        #endregion


    }
}