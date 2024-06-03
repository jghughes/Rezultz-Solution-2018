using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles
{
    [DataContract(Namespace = "", Name = XeOrganizer)]
    public class OrganizerDto
    {
        #region Names

        private const string XeOrganizer = "organizer";
        private const string XeTitleOfItem = "title";
        private const string XeLabelOfItem = "label";

        #endregion

        #region DataMembers

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeTitleOfItem)]
        public string Title { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeLabelOfItem)]
        public string Label { get; set; } = string.Empty;

        #endregion
    }
}