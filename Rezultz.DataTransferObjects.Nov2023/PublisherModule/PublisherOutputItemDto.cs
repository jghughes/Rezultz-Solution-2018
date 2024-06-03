using System;
using System.Runtime.Serialization;
using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Rezultz.DataTransferObjects.Nov2023.PublisherModule
{
    [DataContract(Namespace = "", Name = XePublisherOutput)]
    public class PublisherOutputItemDto
    {
        #region Names

        // Note: names of the XML elements corresponding to the data members should be private. only public because they are elements within a free-form XDocument and so that we can access them publicly in free-form deserialisation

        public const string XePublisherOutput = "publisher-output";
        public const string XeLabelOfEventAsId = "label-of-event-as-identifier";
        public const string XeRanToCompletionMsg = "ran-to-completion-message";
        public const string XeComputationReport = "conversion-report";
        public const string XeComputationsFailed = "conversion-did-fail";
        public const string XeComputedResults = "computed-results";

        #endregion

        #region DataMembers

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeLabelOfEventAsId)]
        public string LabelOfEventAsIdentifier { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeRanToCompletionMsg)]
        public string RanToCompletionMessage { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeComputationReport)]
        public string ConversionReport { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeComputationsFailed)]
        public bool ConversionDidFail { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeComputedResults)]
        public ResultDto[] ComputedResultsCollection { get; set; } = [];

        #endregion
    }
}