using NetStd.Prism.July2018;

namespace Rezultz.DataTypes.Nov2023
{
    public class SearchQueryItem : BindableBase
    {

        #region constructors

        public SearchQueryItem()
            : this(0, string.Empty, string.Empty)
        {
        }

        public SearchQueryItem(int tagAsInt, string tagAsString, string suggestion)
        {
            TagAsInt = tagAsInt;
            TagAsString = tagAsString ?? string.Empty;
            SearchQueryAsString = suggestion ?? "enter";
        }

        #endregion

        #region props

        public int TagAsInt { get; set; }

        public string TagAsString { get; set; }

        public string SearchQueryAsString { get; set; }

        #endregion


    }
}