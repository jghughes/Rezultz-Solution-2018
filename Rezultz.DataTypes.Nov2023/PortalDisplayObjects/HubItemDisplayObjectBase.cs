using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

namespace Rezultz.DataTypes.Nov2023.PortalDisplayObjects
{
    public class HubItemDisplayObjectBase : BindableBase, IHasCollectionLineItemPropertiesV2
    {
        internal HubItemBase SourceHubItem;

        #region props

        public string FirstName { get; set; } = string.Empty;

        public string MiddleInitial { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string RaceGroup { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

        public string ClickCounter { get; set; }

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public string RecordingModeEnum { get; set; } = string.Empty;

        public string DatabaseActionEnum { get; set; } = string.Empty;

        public string MustDitchOriginatingItem { get; set; }

        public string IsStillToBeBackedUp { get; set; }

        public string IsStillToBePushed { get; set; }

        public string TouchedBy { get; set; } = string.Empty;

        public string TimeStamp { get; set; } = string.Empty;

        public string WhenTouched { get; set; } = string.Empty;

        public string WhenPushed { get; set; } = string.Empty;

        public string DisplayVersionOfSourceItemOriginatingItemGuid { get; set; } = string.Empty;

        public string DisplayVersionOfSourceItemGuid { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string Guid { get; set; } = string.Empty; 

        #endregion

        #region unused props to satisfy IHasCollectionLineItemPropertiesV2

        public int ID { get; set; }
        public string Label { get; set; } = string.Empty;
        public string EnumString { get; set; } = string.Empty;

        #endregion

        #region methods

        public string GetSourceItemOriginatingItemGuid()
        {
            return SourceHubItem.OriginatingItemGuid;
        }

        public string GetSourceItemGuid()
        {
            return SourceHubItem.Guid;
        }

        public string GetSourceItemBothGuids()
        {
            return JghString.Concat(SourceHubItem.OriginatingItemGuid, SourceHubItem.Guid);
        }

        #endregion
    }
}