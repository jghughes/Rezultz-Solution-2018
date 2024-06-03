using System;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace Rezultz.DataTypes.Nov2023.PublisherModuleItems
{
    public class PublisherOutputItem
    {

        #region properties


        public string LabelOfEventAsIdentifier { get; set; } = string.Empty;

        public string RanToCompletionMessage { get; set; } = string.Empty;

        public string ConversionReport { get; set; } = string.Empty;

        public bool ConversionDidFail { get; set; } = false;

        public ResultItem[] ComputedResults { get; set; } = [];  

        #endregion

        #region methods

        public static PublisherOutputItem FromDataTransferObject(PublisherOutputItemDto itemDto)
    {
        const string failure = "Populating PublisherOutputItem.";
        const string locus = "[FromDataTransferObject]";

        try
        {
            var x = itemDto ?? new PublisherOutputItemDto();

            var answer = new PublisherOutputItem
            {
                LabelOfEventAsIdentifier = x.LabelOfEventAsIdentifier,
                RanToCompletionMessage = x.RanToCompletionMessage,
                ConversionReport = x.ConversionReport,
                ConversionDidFail = x.ConversionDidFail,
                ComputedResults = ResultItem.FromDataTransferObject(x.ComputedResultsCollection) 
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

        public static PublisherOutputItemDto ToDataTransferObject(PublisherOutputItem item)
    {
        const string failure = "Populating PublisherOutputItemDto.";
        const string locus = "[ToDataTransferObject]";

        try
        {
            var x = item ?? new PublisherOutputItem();

            var answer = new PublisherOutputItemDto
            {
                LabelOfEventAsIdentifier = x.LabelOfEventAsIdentifier,
                RanToCompletionMessage = x.RanToCompletionMessage,
                ConversionReport = x.ConversionReport,
                ConversionDidFail = x.ConversionDidFail,
                ComputedResultsCollection = ResultItem.ToDataTransferObject(x.ComputedResults)
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

        #endregion
    }
}