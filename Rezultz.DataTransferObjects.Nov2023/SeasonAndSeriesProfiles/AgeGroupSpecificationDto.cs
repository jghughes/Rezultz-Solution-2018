using System.Runtime.Serialization;


// ReSharper disable InconsistentNaming

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles
{
    [DataContract(Namespace = "", Name = XeAgeGroupSpecification)]
    public class AgeGroupSpecificationDto
    {
        #region Names

        private const string XeAgeGroupSpecification = "age-group-specification";
        private const string XeAgeLower = "age-lower";
        private const string XeAgeUpper = "age-upper";
        private const string XeLabelOfItem = "label";
        private const string XeDisplayRankOfItem = "display-rank";

        #endregion

        #region DataMembers

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeAgeLower)]
        public int AgeLowerBound { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeAgeUpper)]
        public int AgeUpperBound { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeLabelOfItem)]
        public string Label { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeDisplayRankOfItem)]
        public int DisplayRank { get; set; } // used only for enum items or items that don't otherwise have a self-evident ordering of display in a collection

        #endregion
    }
}