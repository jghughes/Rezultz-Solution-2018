using System;
using System.Linq;
using System.Xml.Linq;
using NetStd.Exceptions.July2018;
using NetStd.Goodies.Xml.July2018;
using NetStd.SymbolsStringsConstants.July2018;

namespace NetStd.Azure.Types.July2018
{
    public static class AzureStorageTypeXmlParsers
    {
        private const string Locus2 = "<AzureStorageTypeXmlParsers>";

        public static AzureStorageLocationItem ParseToStorageLocationItemEntity(XElement storageLocationXe)
        {
            const string failure = "Unable to convert XElement into type of AzureStorageLocationItem.";
            const string locus = "<ParseToStorageLocationItemEntity>";

            if (storageLocationXe == null) return null;

            var answer = new AzureStorageLocationItem();

            try
            {
                answer.ID = JghXElementHelpers.AsInt32(storageLocationXe.Element(UbiquitousConstantXeNames.ID));
                answer.AzureStorageAccountConnectionString =
                    JghXElementHelpers.AsTrimmedString(
                        storageLocationXe.Element(AzureStorageLocationItemConstantXeNames
                            .AzureStorageAccountConnectionString));
                answer.AzureStorageAccountName =
                    JghXElementHelpers.AsTrimmedString(
                        storageLocationXe.Element(AzureStorageLocationItemConstantXeNames.AzureStorageAccountName));
                answer.AzureStorageAccountBlobServiceEndpoint =
                    JghXElementHelpers.AsTrimmedString(
                        storageLocationXe.Element(AzureStorageLocationItemConstantXeNames
                            .AzureStorageAccountBlobServiceEndpoint));
                answer.AzureStorageContainerName =
                    JghXElementHelpers.AsTrimmedString(
                        storageLocationXe.Element(AzureStorageLocationItemConstantXeNames.AzureStorageContainerName));

                answer.BlobUrl =
                    JghXElementHelpers.AsTrimmedString(
                        storageLocationXe.Element(AzureStorageBlobItemConstantXeNames.BlobUrl));

                answer.BlobName =
                    JghXElementHelpers.AsTrimmedString(
                        storageLocationXe.Element(AzureStorageBlobItemConstantXeNames.BlobName));


                return answer;
            }
            catch (Exception ex)
            {
                throw ExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex);
            }
        }

        public static BlobItem ParseToBlobItemEntity(XElement blobItemXe)
        {
            const string failure = "Unable to convert XElement into type of BlobItem.";
            const string locus = "<ParseToBlobItemEntity>";

            if (blobItemXe == null) return null;

            var answer = new BlobItem();

            try
            {
                #region parse particulars. NB. return type inherits from ParticularsItem. but property values derive from children of the parent, not of a child of the parentXE

                answer =
                    ParticularsItemHelpers.CopyParticularsItemPropertiesFromSourceToTarget(
                        ParticularsItemXmlParsers.ParseToParticularsItemEntity(blobItemXe), answer);

                #endregion

                answer.BlobName =
                    JghXElementHelpers.AsTrimmedString(
                        blobItemXe.Element(AzureStorageBlobItemConstantXeNames.BlobName));
                answer.BlobUrl =
                    JghXElementHelpers.AsTrimmedString(blobItemXe.Element(AzureStorageBlobItemConstantXeNames.BlobUrl));

                return answer;
            }
            catch (Exception ex)
            {
                throw ExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex);
            }
        }

        public static BlobItem[] ParseToArrayOfBlobItemEntities(XElement parentOfArrayOfBlobItemXe)
        {
            const string failure = "Unable to convert XElement into type of BlobItem[].";
            const string locus = "<ParseToArrayOfBlobItemEntities>";

            if (parentOfArrayOfBlobItemXe == null) return null;

            var answer = new BlobItem[0];

            try
            {
                var arrayOfBlobItemXe =
                    parentOfArrayOfBlobItemXe.Element(AzureStorageBlobItemConstantXeNames.ArrayOfBlobItem);

                if (arrayOfBlobItemXe != null)
                    answer = arrayOfBlobItemXe
                        .Descendants(AzureStorageBlobItemConstantXeNames.BlobItem)
                        .Where(z => z != null)
                        .Select(ParseToBlobItemEntity)
                        .Where(z => z != null)
                        //.Where(z => z.IsEnabled)
                        .OrderBy(z => z.DisplayRank)
                        .ToArray();

                return answer;
            }
            catch (Exception ex)
            {
                throw ExceptionHelpers.ConvertToCarrier(failure, locus, parentOfArrayOfBlobItemXe.ToString(), ex);
            }
        }
    }
}