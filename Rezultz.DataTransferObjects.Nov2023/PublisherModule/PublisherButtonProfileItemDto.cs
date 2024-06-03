using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.PublisherModule
{
    [DataContract(Namespace = "", Name = XeGuiButtonProfile)]
    public class PublisherButtonProfileItemDto
    {
        #region Names

        // Note: names of the XML elements corresponding to the data members should be private. only public because they are elements within a free-form XDocument and so that we can access them publicly in free-form deserialisation

        public const string XeGuiButtonProfile = "gui-button-profile";
        public const string XeDatasetIdentifier = "input-dataset-identifier";
        public const string XeGuiButtonText = "gui-button-text";
        public const string XeDatasetDescription = "dataset-description";
        public const string XeAllowableFileNameExtensions = "allowable-filename-extensions";
        public const string XeDatasetExampleFileName = "filename-of-example-of-dataset";

        #endregion

        #region DataMembers

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XeDatasetIdentifier)]
        public string IdentifierOfAssociatedDataset { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeGuiButtonText)]
        public string GuiButtonContent { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeDatasetDescription)]
        public string ShortDescriptionOfAssociatedDataset { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XeAllowableFileNameExtensions)]
        public string FileNameExtensionFiltersForBrowsingHardDrive { get; set; } = string.Empty; // comma separated list of file allowable extensions. be sure to include the dot prefix for each of them

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeDatasetExampleFileName)]
        public string FileNameOfExampleOfAssociatedDataset { get; set; } = string.Empty;

        #endregion
    }
}