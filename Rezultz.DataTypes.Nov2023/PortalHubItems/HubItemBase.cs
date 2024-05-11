using System;
using System.Linq;
using System.Runtime.Serialization;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces01.July2018.Objects;


// ReSharper disable ValueParameterNotUsed

namespace Rezultz.DataTypes.Nov2023.PortalHubItems
{
    // the only reason we need this class to be serialisable is so that we can do backups of the repository in local storage

    [Serializable]
    public abstract class HubItemBase : IHubItem, IHasIdentifier, IHasTimeStampBinaryFormat, IHasRecordingModeEnum, IHasWhenTouchedBinaryLocal, IHasCollectionLineItemPropertiesV2
    {
        #region constants

        public const string Spacer = "   "; // size governed by width of screen limitation and size of font

        #endregion

        #region properties

        [DataMember] public int ClickCounter { get; set; }

        [DataMember] public string Bib { get; set; } = string.Empty;

        [DataMember] public string Rfid { get; set; } = string.Empty;

        [DataMember] public string RecordingModeEnum { get; set; } = string.Empty;

        [DataMember] public string DatabaseActionEnum { get; set; } = string.Empty;

        [DataMember] public bool MustDitchOriginatingItem { get; set; }

        [DataMember] public bool IsStillToBeBackedUp { get; set; } = true;

        [DataMember] public bool IsStillToBePushed { get; set; } = true;

        [DataMember] public string TouchedBy { get; set; } = string.Empty;

        [DataMember] public long TimeStampBinaryFormat { get; set; }

        [DataMember] public long WhenTouchedBinaryFormat { get; set; }

        [DataMember] public long WhenPushedBinaryFormat { get; set; }

        [DataMember] public string OriginatingItemGuid { get; set; } = string.Empty;

        [DataMember] public string Guid { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public int ID { get; set; }

        public string EnumString { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        #endregion

        #region method
        
        public string GetBothGuids()
        {
            return JghString.Concat(OriginatingItemGuid, Guid);
        }

        #endregion

        #region static methods

        public static JghListDictionary<string, T> ToListDictionaryGroupedByBib<T>(T[] listHubItems) where T : class, IHubItem
        {
            JghListDictionary<string, T> answer = new();

            if (listHubItems == null) return answer;

            foreach (var hubItem in listHubItems.Where(z => z != null).Where(z => z.Bib != null).OrderBy(z => z.Bib).ThenByDescending(z => z.WhenTouchedBinaryFormat))
            {
                answer.Add(hubItem.Bib, hubItem);
            }

            return answer;
        }

        public static JghListDictionary<string, T> ToListDictionaryGroupedByOriginatingItemGuid<T>(T[] listHubItems) where T : class, IHubItem
        {
            JghListDictionary<string, T> answer = new();

            if (listHubItems == null) return answer;

            foreach (var hubItem in listHubItems.Where(z => z != null).Where(z => !string.IsNullOrWhiteSpace(z.OriginatingItemGuid)).OrderByDescending(z => z.WhenTouchedBinaryFormat))
            {
                answer.Add(hubItem.OriginatingItemGuid, hubItem);
            }

            return answer;
        }

        #endregion

    }
}
