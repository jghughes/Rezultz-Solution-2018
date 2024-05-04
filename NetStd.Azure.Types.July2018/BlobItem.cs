using System.Runtime.Serialization;
using System.Xml.Serialization;
using NetStd.HandyTypes.July2018;

namespace NetStd.Azure.Types.July2018
{
    [DataContract]
    [XmlRoot(ElementName = AzureStorageBlobItemConstantXeNames.BlobItem, IsNullable = false)]
    public class BlobItem : ParticularsItem
    {
        #region constructor - be sure to initialise custom-type properties

        public BlobItem()
        {
            BlobName = string.Empty;
            BlobUrl = string.Empty;
        }

        #endregion

        #region methods

        public override string ToString()
        {
            return string.Join(" ", BlobName, BlobUrl);
        }

        #endregion

        #region properties

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageBlobItemConstantXeNames.BlobName, IsNullable = false)]
        public string BlobName { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageBlobItemConstantXeNames.BlobUrl, IsNullable = false)]
        public string BlobUrl { get; set; }

        #endregion
    }
}