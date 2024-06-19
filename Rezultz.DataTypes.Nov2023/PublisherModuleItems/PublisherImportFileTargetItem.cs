using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;

namespace Rezultz.DataTypes.Nov2023.PublisherModuleItems
{
    public class PublisherImportFileTargetItem
    {

        #region ctor


        public PublisherImportFileTargetItem(string identifierOfDataset, string fileName)
    {
        IdentifierOfDataset = identifierOfDataset ?? string.Empty;
        FileName = fileName ?? string.Empty;
    }

        public PublisherImportFileTargetItem()
    {
    }


        #endregion

        #region props

        public string IdentifierOfDataset { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        #endregion

        #region methods

        public static PublisherImportFileTargetItem FromDataTransferObject(PublisherFileImportTargetItemDto importTargetItemDto)
    {
        const string failure = "Populating PublisherImportFileTargetItem.";
        const string locus = "[FromDataTransferObject]";

        try
        {
            var x = importTargetItemDto ?? new PublisherFileImportTargetItemDto();

            var answer = new PublisherImportFileTargetItem
            {
                IdentifierOfDataset = x.DatasetIdentifier,
                FileName = x.FileName,
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

        public static PublisherImportFileTargetItem[] FromDataTransferObject(PublisherFileImportTargetItemDto[] dto)
    {
        const string failure = "Populating PublisherImportFileTargetItem[].";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto is null)
                return [];

            var answer = dto.Select(FromDataTransferObject).Where(z => z is not null).ToArray();

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

        public static PublisherFileImportTargetItemDto ToDataTransferObject(PublisherImportFileTargetItem targetItem)
    {
        const string failure = "Populating PublisherFileImportTargetItemDto.";
        const string locus = "[ToDataTransferObject]";

        try
        {
            var x = targetItem ?? new PublisherImportFileTargetItem();

            var answer = new PublisherFileImportTargetItemDto
            {
                DatasetIdentifier = x.IdentifierOfDataset,
                FileName = x.FileName,
                //DatasetContents = x.ContentsOfFile,
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

        public static PublisherFileImportTargetItemDto[] ToDataTransferObject(PublisherImportFileTargetItem[] item)
    {
        const string failure = "Populating PublisherFileImportTargetItemDto[].";
        const string locus = "[ToDataTransferObject]";

        try
        {
            if (item is null)
                return [];

            var answer = item.Select(ToDataTransferObject).Where(z => z is not null).ToArray();

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
}