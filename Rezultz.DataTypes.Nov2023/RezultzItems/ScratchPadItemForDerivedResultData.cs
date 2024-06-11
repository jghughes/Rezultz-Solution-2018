using System.Globalization;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    public class ScratchPadItemForDerivedResultData : IHasItemID
    {
        #region constructors

        public ScratchPadItemForDerivedResultData(DerivedDataItem derivedDataItem)
        {
            ID = derivedDataItem.ID;
        }

        #region properties

        public int ID { get; set; }

        public double TimeBehindWinnerOfSubsetInSeconds { get; set; }

        public int SplitsBehindWinnerOfSubset { get; set; }

        public int TotalItemsInSubset { get; set; }

        public int RankInSubsetInt { get; set; }

        public int RankInSubsetIntSkippingOverNonSeriesParticipants { get; set; }

        #endregion

        #endregion

        #region methods

        public override string ToString()
        {
            return
                $"{ID.ToString(CultureInfo.InvariantCulture)} {RankInSubsetInt} {TotalItemsInSubset.ToString(JghFormatSpecifiers.DecimalFormat0Dp)} {TimeBehindWinnerOfSubsetInSeconds}";
        }

        #endregion
    }
}