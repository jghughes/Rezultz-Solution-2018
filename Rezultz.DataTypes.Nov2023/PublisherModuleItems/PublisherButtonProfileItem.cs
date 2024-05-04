using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;

namespace Rezultz.DataTypes.Nov2023.PublisherModuleItems;

public class PublisherButtonProfileItem
{
    #region props

    public string ShortDescriptionOfAssociatedDataset { get; set; } = string.Empty;

    public string IdentifierOfAssociatedDataset { get; set; } = string.Empty;

    public string GuiButtonContent { get; set; } = string.Empty;

    public string FileNameExtensionFiltersForBrowsingHardDrive { get; set; } = string.Empty; // comma separated list of file extensions

    public string FileNameOfExampleOfAssociatedDataset { get; set; } = string.Empty;

    #endregion

    #region methods

    public static PublisherButtonProfileItem FromDataTransferObject(PublisherButtonProfileItemDto dto)
    {
        const string failure = "Populating PublisherButtonProfileItem.";
        const string locus = "[FromDataTransferObject]";
        
        try
        {
            var x = dto ?? new PublisherButtonProfileItemDto();

            var answer = new PublisherButtonProfileItem
            {
                ShortDescriptionOfAssociatedDataset = x.ShortDescriptionOfAssociatedDataset,
                IdentifierOfAssociatedDataset = x.IdentifierOfAssociatedDataset,
                GuiButtonContent = x.GuiButtonContent,
                FileNameExtensionFiltersForBrowsingHardDrive = x.FileNameExtensionFiltersForBrowsingHardDrive,
                FileNameOfExampleOfAssociatedDataset = x.FileNameOfExampleOfAssociatedDataset,
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

    public static PublisherButtonProfileItem[] FromDataTransferObject(PublisherButtonProfileItemDto[] dto)
    {
        const string failure = "Populating PublisherButtonProfileItem[].";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto == null)
                return Array.Empty<PublisherButtonProfileItem>();

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

    public static PublisherButtonProfileItemDto ToDataTransferObject(PublisherButtonProfileItem item)
    {
        const string failure = "Populating PublisherButtonProfileItemDto.";
        const string locus = "[ToDataTransferObject]";

        try
        {
            var x = item ?? new PublisherButtonProfileItem();

            var answer = new PublisherButtonProfileItemDto
            {
                ShortDescriptionOfAssociatedDataset = x.ShortDescriptionOfAssociatedDataset,
                IdentifierOfAssociatedDataset = x.IdentifierOfAssociatedDataset,
                GuiButtonContent = x.GuiButtonContent,
                FileNameExtensionFiltersForBrowsingHardDrive = x.FileNameExtensionFiltersForBrowsingHardDrive,
                FileNameOfExampleOfAssociatedDataset = x.FileNameOfExampleOfAssociatedDataset,
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

    public static PublisherButtonProfileItemDto[] ToDataTransferObject(PublisherButtonProfileItem[] item)
    {
        const string failure = "Populating PublisherButtonProfileItem[].";
        const string locus = "[ToDataTransferObject]";

        try
        {
            if (item == null)
                return Array.Empty<PublisherButtonProfileItemDto>();

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

    #endregion
}