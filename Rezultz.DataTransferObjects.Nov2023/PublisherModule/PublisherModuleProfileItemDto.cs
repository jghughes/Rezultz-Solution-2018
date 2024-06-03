using System;
using System.Runtime.Serialization;

namespace Rezultz.DataTransferObjects.Nov2023.PublisherModule
{
    [DataContract(Namespace = "", Name = XePublisherModuleProfile)]
    public class PublisherModuleProfileItemDto
    {
        #region Names

        // Note: names of the XML elements corresponding to the data members should be private. only public because they are elements within a free-form XDocument and so that we can access them publicly in free-form deserialisation

        public const string XePublisherModuleProfile = "publisher-module-profile";
        public const string XeThisFileNameFragment = "this-filename-fragment";
        public const string XeCSharpModuleId = "csharp-module-codename";
        public const string XeCaption = "caption";
        public const string XeDescription = "description";
        public const string XeOverview = "overview";
        public const string XeGuiButtonsForPullingDatasetsFromPortalHub = "gui-button-profiles-for-importing-input-datasets-from-portal-hub";
        public const string XeGuiButtonsForBrowsingFileSystemForDatasets = "gui-buttons-for-browsing-file-system-for-input-datasets";

        #endregion

        #region DataMembers

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1, Name = XeThisFileNameFragment)]
        public string FragmentInNameOfThisFile { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 2, Name = XeCSharpModuleId)]
        public string CSharpModuleCodeName { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 3, Name = XeCaption)]
        public string CaptionForModule { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 4, Name = XeDescription)]
        public string DescriptionOfModule { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 5, Name = XeOverview)]
        public string OverviewOfModule { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 6, Name = XeGuiButtonsForPullingDatasetsFromPortalHub)]
        public PublisherButtonProfileItemDto[] GuiButtonProfilesForPullingDatasetsFromPortalHub { get; set; } = Array.Empty<PublisherButtonProfileItemDto>();

        [DataMember(EmitDefaultValue = true, IsRequired = false, Order = 7, Name = XeGuiButtonsForBrowsingFileSystemForDatasets)]
        public PublisherButtonProfileItemDto[] GuiButtonProfilesForBrowsingFileSystemForDatasets { get; set; } = Array.Empty<PublisherButtonProfileItemDto>();

        #endregion
    }
}