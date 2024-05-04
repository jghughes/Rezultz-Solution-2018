using System.Threading.Tasks;
using System.Xml.Linq;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;

namespace RezultzSvc.Library02.Mar2024.PublisherModules
{
    public interface IPublisher
    {

        #region prop
       
        public XElement AssociatedProfileFile { get; set; } // leave null here

        #endregion

        #region methods

        public abstract void ExtractCustomXmlInformationFromAssociatedPublisherProfileFile(); // use this to fish out any custom stuff in AssociatedProfileFile that goes above and beyond the members of PublisherModuleProfileItem

        public abstract PublisherModuleProfileItem ParseAssociatedProfile(); // use system deserialiser inside here, but also provide a handwritten deserialiser as a Plan B upon failure

        public abstract Task<PublisherOutputItem> DoAllTranslationsAndComputationsToGenerateResultsAsync(PublisherInputItem publisherInputItem);

        #endregion
    }

}