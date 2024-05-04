using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;

// ReSharper disable IdentifierTypo
#pragma warning disable CS8321 // Local function is declared but never used

namespace RezultzSvc.Library02.Mar2024.PublisherModules;

public abstract class PublisherBase : IPublisher
{
    private const string Locus2 = nameof(PublisherBase);
    private const string Locus3 = "[RezultzSvc.Library02.Mar2024]]";

    public abstract void ExtractCustomXmlInformationFromAssociatedPublisherProfileFile();

    public virtual PublisherModuleProfileItem ParseAssociatedProfile()
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[ParseAssociatedProfile]";

        // Note: first try the system serialiser, and only if that blows, resort to the hand-written ManuallyDeserialiseXmlToPublisherProfileDto(...)
        // Alternatively override this method in a derived class, for example to de-serialise a custom file with custom data elements in it. 

        try
        {
            PublisherModuleProfileItemDto profileDto;

            try
            {
                profileDto = JghSerialisation.ToObjectFromXml<PublisherModuleProfileItemDto>(AssociatedProfileFile.ToString(), new[] { typeof(PublisherModuleProfileItemDto) });
            }
            catch (Exception)
            {
                profileDto = ManuallyDeserialiseXmlToPublisherProfileDto(AssociatedProfileFile.ToString());
                // belt and braces: if system deserialiser blows up for any reason, resort to this Plan B. Manual deserialisation can be more robust.
            }

            var profileItem = PublisherModuleProfileItem.FromDataTransferObject(profileDto);

            return profileItem;
        }

        #region try-catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion

    }

    public abstract Task<PublisherOutputItem> DoAllTranslationsAndComputationsToGenerateResultsAsync(PublisherInputItem publisherInputItem);

    #region prop

    public XElement AssociatedProfileFile { get; set; }

    #endregion

    #region helpers

    private static PublisherModuleProfileItemDto ManuallyDeserialiseXmlToPublisherProfileDto(string freeFormComputerProfileXmlFile)

    {
        const string failure = "Unable to convert string data into ComputerProfileDto.";
        const string locus = "[ManuallyDeserialiseXmlToPublisherProfileDto]";

        try
        {
            XElement parentXElement;

            try
            {
                parentXElement = ParsePlainTextIntoXElement(freeFormComputerProfileXmlFile);
            }
            catch (Exception ex)
            {
                throw new JghPublisherServiceFaultException("System.Xml.Linq.XElement.Parse() blew up when it tried to parse contents of this file as a single top-level XElement. File contents are unexpected.", ex);
            }

            PublisherModuleProfileItemDto answer = new()
            {
                FragmentInNameOfThisFile = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeThisFileNameFragment),
                CaptionForModule = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeCaption),
                DescriptionOfModule = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeDescription),
                OverviewOfModule = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeOverview),
                CSharpModuleCodeName = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeCSharpModuleId)
            };

            var portalButtonArrayXe = GetChildElement(parentXElement, PublisherModuleProfileItemDto.XeGuiButtonsForPullingDatasetsFromPortalHub);
            var browseButtonArrayXe = GetChildElement(parentXElement, PublisherModuleProfileItemDto.XeGuiButtonsForBrowsingFileSystemForDatasets);

            var portalButtons = GetComputerGuiButtonProfileDtos(portalButtonArrayXe);
            var browseButtons = GetComputerGuiButtonProfileDtos(browseButtonArrayXe);

            answer.GuiButtonProfilesForPullingDatasetsFromPortalHub = portalButtons;
            answer.GuiButtonProfilesForBrowsingFileSystemForDatasets = browseButtons;

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion

        #region helpers

        static XElement ParsePlainTextIntoXElement(string inputText)
        {
            var failure = "Unable to parse text into xml.";
            const string locus = "[ParsePlainTextIntoXElement]";

            try
            {
                if (inputText == null)
                    throw new ArgumentNullException(nameof(inputText));

                return XElement.Parse(inputText); // automatically throws if invalid
            }

            #region try-catch

            catch (Exception ex)
            {
                failure = JghString.ConcatAsParagraphs(failure, "Parsing the contents of this file into a XElement failed.",
                    "(Unfortunately even the tiniest error in format, syntax and/or content causes failure.)");

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        static string GetChildElementValue(XElement parentXe, string nameOfChild)
        {
            var childXe = parentXe?.Element(nameOfChild);

            if (childXe == null)
                return string.Empty;

            var value = childXe.Value; // empirically this blows up when the Value itself contains xml : for that use our cunning alternative method: GetChildElementValueVerbatim()

            return value.Trim();
        }

#pragma warning disable CS8321 // Local function is declared but never used
        static string GetChildElementValueVerbatim(XElement parentXe, string nameOfChild)
#pragma warning restore CS8321 // Local function is declared but never used
        {
            var childXe = parentXe?.Element(nameOfChild);

            if (childXe == null)
                return string.Empty;

            var value = childXe.ToString(); // this works

            return value.Trim();

            //var rubbish1 = childXe.Value; // Note. empirically this alternative fails when the Value itself contains xml
            //var rubbish2 = (string) childXe; // Note. empirically this alternative fails when the Value itself contains xml
        }

        static XElement GetChildElement(XElement parentXe, string nameOfChild)
        {
            var childXe = parentXe?.Element(nameOfChild);

            return childXe;
        }

        static XElement[] GetChildElements(XElement parentXe, string nameOfChild)
        {
            var childrenXe = parentXe?.Elements(nameOfChild).ToArray();

            return childrenXe;
        }

        // ReSharper disable once IdentifierTypo
        static PublisherButtonProfileItemDto[] GetComputerGuiButtonProfileDtos(XElement parentXe)
        {
            if (parentXe == null)
                return Array.Empty<PublisherButtonProfileItemDto>();

            var buttonXElements = GetChildElements(parentXe, PublisherButtonProfileItemDto.XeGuiButtonProfile);

            if (buttonXElements == null || !buttonXElements.Any())
                return Array.Empty<PublisherButtonProfileItemDto>();

            return buttonXElements.Select(thisButtonXe => new PublisherButtonProfileItemDto
            {
                ShortDescriptionOfAssociatedDataset = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeDatasetDescription),
                IdentifierOfAssociatedDataset = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeDatasetIdentifier),
                GuiButtonContent = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeGuiButtonText),
                FileNameExtensionFiltersForBrowsingHardDrive = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeAllowableFileNameExtensions),
                FileNameOfExampleOfAssociatedDataset = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeDatasetExampleFileName)
            }).ToArray();
        }

        #endregion
    }

    #endregion

}