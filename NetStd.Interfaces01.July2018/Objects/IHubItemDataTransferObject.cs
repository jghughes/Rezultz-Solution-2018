using NetStd.Interfaces01.July2018.HasProperty;

namespace NetStd.Interfaces01.July2018.Objects
{
    public interface IHubItemDataTransferObject: IHasGuid
    {
        string OriginatingItemGuid { get; set; }

    }
}