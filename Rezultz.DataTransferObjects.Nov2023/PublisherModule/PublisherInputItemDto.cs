using System.Runtime.Serialization;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTransferObjects.Nov2023.PublisherModule;

[DataContract(Namespace = "", Name = XePublisherInputItem)]
public class PublisherInputItemDto
{
    #region Names

    private const string XePublisherInputItem = "publisher-input-item";
    private const string XeFragmentOfAssociatedComputerProfileFilename = "filename-fragment-of-associated-computer-profile";
    private const string XeSeriesLabelAsEventId = "series-label-as-event-identifier";
    private const string XeEventLabelAsEventId = "event-label-as-event-identifier";
    private const string XeDatasetTargetsToBeProcessed = "collection-of-dataset-target-to-be-processed";
    private const string XeSeriesProfile = "series-profile";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeFragmentOfAssociatedComputerProfileFilename)]
    public string FileNameFragmentOfAssociatedComputerProfile { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeSeriesLabelAsEventId)]
    public string SeriesLabelAsEventIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeEventLabelAsEventId)]
    public string EventLabelAsEventIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeSeriesProfile)]
    public SeriesProfileDto ParentSeriesProfile { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeDatasetTargetsToBeProcessed)]
    public PublisherFileImportTargetItemDto[] DatasetTargetToBeProcessedCollection { get; set; }

    #endregion
}