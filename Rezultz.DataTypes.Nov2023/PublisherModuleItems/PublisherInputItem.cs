using System;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.PublisherModuleItems;

public class PublisherInputItem
{
    #region props

    public string FileNameFragmentOfAssociatedPublishingProfile { get; set; }

    public string SeriesLabelAsEventIdentifier { get; set; }

    public string EventLabelAsEventIdentifier { get; set; }

    public SeriesProfileItem SeriesProfile { get; set; }

    public PublisherImportFileTargetItem[] DatasetTargetsToBeProcessed { get; set; } = Array.Empty<PublisherImportFileTargetItem>();

    public string NullChecksFailureMessage
    {
        get
        {
            var sb = new JghStringBuilder();

            if (string.IsNullOrWhiteSpace(FileNameFragmentOfAssociatedPublishingProfile))
                sb.AppendLineFollowedByOne("Input Error. FileName fragment of associated publishing profile is blank. FileName fragment is required to find the file.");

            if (string.IsNullOrWhiteSpace(SeriesLabelAsEventIdentifier))
                sb.AppendLineFollowedByOne("Input Error. Series Label is blank. The Label is required to identify the Series.");

            if (string.IsNullOrWhiteSpace(EventLabelAsEventIdentifier))
                sb.AppendLineFollowedByOne("Input Error. Event Label is blank. The Label is required to identify the Event.");

            if (SeriesProfile == null)
                sb.AppendLineFollowedByOne("Input error. Series profile is null. The Series profile is required.");

            if (DatasetTargetsToBeProcessed == null || !DatasetTargetsToBeProcessed.Any())
                sb.AppendLineFollowedByOne("Input error. No datasets have been specified for processing. (Maybe nothing was uploaded?)");

            return sb.ToString();
        }
    }

    #endregion


    #region methods

    public static PublisherInputItem FromDataTransferObject(PublisherInputItemDto itemDto)
    {
        const string failure = "Populating PublisherInputItem.";
        const string locus = "[FromDataTransferObject]";

        try
        {
            var x = itemDto ?? new PublisherInputItemDto();

            var answer = new PublisherInputItem
            {
                FileNameFragmentOfAssociatedPublishingProfile = x.FileNameFragmentOfAssociatedComputerProfile,
                SeriesLabelAsEventIdentifier = x.SeriesLabelAsEventIdentifier,
                EventLabelAsEventIdentifier = x.EventLabelAsEventIdentifier,
                SeriesProfile = SeriesProfileItem.FromDataTransferObject(x.ParentSeriesProfile),
                DatasetTargetsToBeProcessed = PublisherImportFileTargetItem.FromDataTransferObject(x.DatasetTargetToBeProcessedCollection)
            };

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public static PublisherInputItem[] FromDataTransferObject(PublisherInputItemDto[] dto)
    {
        const string failure = "Populating PublisherInputItem[].";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto == null)
                return Array.Empty<PublisherInputItem>();

            var answer = dto.Select(FromDataTransferObject).Where(z => z != null).ToArray();

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public static PublisherInputItemDto ToDataTransferObject(PublisherInputItem item)
    {
        const string failure = "Populating PublisherInputItemDto.";
        const string locus = "[ToDataTransferObject]";

        try
        {
            var x = item ?? new PublisherInputItem();

            var answer = new PublisherInputItemDto
            {
                FileNameFragmentOfAssociatedComputerProfile = x.FileNameFragmentOfAssociatedPublishingProfile,
                SeriesLabelAsEventIdentifier = x.SeriesLabelAsEventIdentifier,
                EventLabelAsEventIdentifier = x.EventLabelAsEventIdentifier,
                ParentSeriesProfile = SeriesProfileItem.ToDataTransferObject(x.SeriesProfile),
                DatasetTargetToBeProcessedCollection = PublisherImportFileTargetItem.ToDataTransferObject(x.DatasetTargetsToBeProcessed)
            };

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public static PublisherInputItemDto[] ToDataTransferObject(PublisherInputItem[] item)
    {
        const string failure = "Populating PublisherInputItemDto[].";
        const string locus = "[ToDataTransferObject]";

        try
        {
            if (item == null)
                return Array.Empty<PublisherInputItemDto>();

            var answer = item.Select(ToDataTransferObject).Where(z => z != null).ToArray();

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public EntityLocationItem DeduceStorageLocation(string identifierOfDataset)
    {
        if (SeriesProfile == null || DatasetTargetsToBeProcessed == null || string.IsNullOrWhiteSpace(identifierOfDataset))
            return null;

        var accountName = SeriesProfile.ContainerForPublishingDatasetsToBeProcessed?.AccountName;
        var containerName = SeriesProfile.ContainerForPublishingDatasetsToBeProcessed?.ContainerName;

        if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(containerName))
            return null;

        var targetFile = DatasetTargetsToBeProcessed.FirstOrDefault(z => z.IdentifierOfDataset == identifierOfDataset);

        
        if (targetFile == null || string.IsNullOrWhiteSpace(targetFile.FileName))
            return null;

        var answer = new EntityLocationItem(accountName, containerName, targetFile.FileName);

        return answer;
    }

    public EntityLocationItem[] DeduceStorageLocations(string identifierOfDataset)
    {
        if (SeriesProfile == null || DatasetTargetsToBeProcessed == null || string.IsNullOrWhiteSpace(identifierOfDataset))
            return Array.Empty<EntityLocationItem>();

        var accountName = SeriesProfile.ContainerForPublishingDatasetsToBeProcessed?.AccountName;
        var containerName = SeriesProfile.ContainerForPublishingDatasetsToBeProcessed?.ContainerName;

        if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(containerName))
            return Array.Empty<EntityLocationItem>();

        var answer = DatasetTargetsToBeProcessed
            .Where(z => z.IdentifierOfDataset == identifierOfDataset)
            .Where(z=> !string.IsNullOrWhiteSpace(z.FileName))
            .Select(fileTargetItem => new EntityLocationItem(accountName, containerName, fileTargetItem.FileName));


        return answer.ToArray();
    }

    #endregion

}