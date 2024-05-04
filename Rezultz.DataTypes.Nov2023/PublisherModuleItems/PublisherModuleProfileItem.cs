using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;

// ReSharper disable InconsistentNaming

namespace Rezultz.DataTypes.Nov2023.PublisherModuleItems;

public class PublisherModuleProfileItem
{
    #region props

    public string FragmentInNameOfFile { get; set; } = string.Empty;

    public string VeryShortDescriptionOfModule { get; set; } = string.Empty;

    public string ShortDescriptionOfModule { get; set; } = string.Empty;

    public string GeneralOverviewOfModule { get; set; } = string.Empty;

    public string CSharpModuleCodeName { get; set; } = string.Empty;

    public string CSharpModuleVersionNumber { get; set; } = string.Empty;

    public PublisherButtonProfileItem[] GuiButtonProfilesForPullingDatasetsFromPortalHub { get; set; } = Array.Empty<PublisherButtonProfileItem>();

    public PublisherButtonProfileItem[] GuiButtonProfilesForBrowsingFileSystemForDatasets { get; set; } = Array.Empty<PublisherButtonProfileItem>();


    #endregion

    #region methods

    public static PublisherModuleProfileItem FromDataTransferObject(PublisherModuleProfileItemDto dto)
    {
        const string failure = "Populating PublisherModuleProfileItem.";
        const string locus = "[FromDataTransferObject]";

        try
        {
            var x = dto ?? new PublisherModuleProfileItemDto();

            var answer = new PublisherModuleProfileItem
            {
                FragmentInNameOfFile = x.FragmentInNameOfThisFile,
                VeryShortDescriptionOfModule = x.CaptionForModule,
                ShortDescriptionOfModule = x.DescriptionOfModule,
                GeneralOverviewOfModule = x.OverviewOfModule,
                CSharpModuleCodeName = x.CSharpModuleCodeName,
                GuiButtonProfilesForPullingDatasetsFromPortalHub = PublisherButtonProfileItem.FromDataTransferObject(x.GuiButtonProfilesForPullingDatasetsFromPortalHub),
                GuiButtonProfilesForBrowsingFileSystemForDatasets = PublisherButtonProfileItem.FromDataTransferObject(x.GuiButtonProfilesForBrowsingFileSystemForDatasets)
            };

            return answer;
        }
        catch (Exception e)
        {
            throw new Exception(failure + locus, e);
        }
    }

    public static PublisherModuleProfileItem[] FromDataTransferObject(PublisherModuleProfileItemDto[] dto)
    {
        const string failure = "Populating PublisherModuleProfileItem[].";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto == null)
                return Array.Empty<PublisherModuleProfileItem>();

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

    public static PublisherModuleProfileItemDto ToDataTransferObject(PublisherModuleProfileItem item)
    {
        const string failure = "Populating PublisherModuleProfileItemDto.";
        const string locus = "[ToDataTransferObject]";

        try
        {
            var x = item ?? new PublisherModuleProfileItem();

            var answer = new PublisherModuleProfileItemDto
            {
                FragmentInNameOfThisFile = x.FragmentInNameOfFile,
                CaptionForModule = x.VeryShortDescriptionOfModule,
                DescriptionOfModule = x.ShortDescriptionOfModule,
                OverviewOfModule = x.GeneralOverviewOfModule,
                CSharpModuleCodeName = x.CSharpModuleCodeName,
                GuiButtonProfilesForPullingDatasetsFromPortalHub = PublisherButtonProfileItem.ToDataTransferObject(x.GuiButtonProfilesForPullingDatasetsFromPortalHub),
                GuiButtonProfilesForBrowsingFileSystemForDatasets = PublisherButtonProfileItem.ToDataTransferObject(x.GuiButtonProfilesForBrowsingFileSystemForDatasets)
            };

            return answer;
        }
        catch (Exception e)
        {
            throw new Exception(failure + locus, e);
        }
    }

    public static PublisherModuleProfileItemDto[] ToDataTransferObject(PublisherModuleProfileItem[] item)
    {
        const string failure = "Populating PublisherModuleProfileItemDto[].";
        const string locus = "[ToDataTransferObject]";

        try
        {
            if (item == null)
                return Array.Empty<PublisherModuleProfileItemDto>();

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