using System;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;

namespace Rezultz.DataTypes.Nov2023
{
    public class CboLookupItem : IHasCollectionLineItemPropertiesV1
    {

        #region properties

        public string Title { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string Blurb { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty;

        public DateTime AdvertisedDateTime { get; set; } 

        public int DisplayRank { get; set; } 

        public int ID { get; set; } 


        #endregion

        #region methods

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Title, Label, EnumString);
        }

        #endregion

    }
}