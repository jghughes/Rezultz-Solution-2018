using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using NetStd.Interfaces01.July2018;
using NetStd.SymbolsStringsConstants.July2018;

namespace NetStd.Azure.Types.July2018
{
    [DataContract]
    [XmlRoot(ElementName = AzureStorageLocationItemConstantXeNames.AzureStorageLocation, IsNullable = false)]
    public class AzureStorageLocationItem : IHasID
    {
        #region methods

        public override string ToString()
        {
            return string.Join(" ", ID.ToString(CultureInfo.InvariantCulture), AzureStorageAccountName,
                AzureStorageContainerName, BlobName);
        }

        #endregion

        #region properties

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = UbiquitousConstantXeNames.ID, IsNullable = false)]
        public int ID { get; set; } // primary key

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageLocationItemConstantXeNames.AzureStorageAccountConnectionString,
            IsNullable = false)]
        public string AzureStorageAccountConnectionString { get; set; } // beware of security risks

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageLocationItemConstantXeNames.AzureStorageAccountBlobServiceEndpoint,
            IsNullable = false)]
        public string AzureStorageAccountBlobServiceEndpoint { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageLocationItemConstantXeNames.AzureStorageAccountName, IsNullable = false)]
        public string AzureStorageAccountName { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageLocationItemConstantXeNames.AzureStorageContainerName,
            IsNullable = false)]
        public string AzureStorageContainerName { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageBlobItemConstantXeNames.BlobUrl, IsNullable = false)]
        public string BlobUrl { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [XmlElement(ElementName = AzureStorageBlobItemConstantXeNames.BlobName, IsNullable = false)]
        public string BlobName { get; set; }

        #endregion

        #region constructors - be sure to initialise custom-type properties

        public AzureStorageLocationItem()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public AzureStorageLocationItem(string accountName, string containerName)
            : this(string.Empty, accountName, containerName, string.Empty, string.Empty)
        {
        }

        public AzureStorageLocationItem(string accountName, string containerName, string blobName)
            : this(string.Empty, accountName, containerName, string.Empty, blobName)
        {
        }

        public AzureStorageLocationItem(string storageAccountConnectionString, string accountName, string containerName,
            string blobUrl, string blobName)
        {
            ID = 0;
            AzureStorageAccountConnectionString = storageAccountConnectionString;
            AzureStorageAccountBlobServiceEndpoint = string.Empty;
            AzureStorageAccountName = accountName;
            AzureStorageContainerName = containerName;
            BlobUrl = blobUrl;
            BlobName = blobName;
        }

        #endregion
    }
}