using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jgh.ConnectionStrings.Mar2024;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using RezultzSvc.Library02.Mar2024.Factory;

namespace RezultzSvc.Library02.Mar2024.SvcHelpers
{
    public class RaceResultsPublishingServiceMethodsHelper
    {
        private const string Locus2 = nameof(RaceResultsPublishingServiceMethodsHelper);
        private const string Locus3 = "[RezultzSvc.Library02.Mar2024]";

        #region fields

        private readonly AzureStorageServiceMethodsHelper _azureStorage = new(new AzureStorageAccessor());

        #endregion

        #region helpers

        private async Task<XElement> GetPublisherProfileAsXElementAsync(string fileNameFragment)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetPublisherProfileAsXElementAsync]";

        try
        {
            if (string.IsNullOrEmpty(fileNameFragment))
                throw new JghPublisherProfileFile404Exception("Unable to obtain the publisher profile file. The provided filename fragment is blank.");

            var entryPoint = ConnectionStringRepository.GetPublisherProfileFilesEntryPoint();

            var targetAzureBlobName = string.Concat(AzureStorageObjectNames.PublisherProfileFileNamePrefix, fileNameFragment,
                AzureStorageObjectNames.PublisherProfileFileNameExtension);

            var blobDoesExist = await _azureStorage.GetIfBlobExistsAsync(
                entryPoint.AzureStorageAccountName,
                entryPoint.AzureStorageContainerName,
                targetAzureBlobName);

            if (!blobDoesExist)
            {
                var namesOfAllBlobs = await _azureStorage.GetNamesOfBlobsInContainerAsync(
                    entryPoint.AzureStorageAccountName,
                    entryPoint.AzureStorageContainerName,
                    AzureStorageObjectNames.PublisherProfileFileNamePrefix,
                    false);

                if (namesOfAllBlobs is null || !namesOfAllBlobs.Any())
                    throw new JghPublisherProfileFile404Exception(
                        $"Can't find any files containing publisher profiles in the designated remote storage location. Container=<{entryPoint.AzureStorageContainerName}>");

                var sb = new StringBuilder();
                sb.AppendLine("Unable to find a file containing a publisher profile corresponding to the provided filename fragment.  Your options are:");
                sb.AppendLine("");

                foreach (var thisBlobName in namesOfAllBlobs)
                {
                    var fileName = JghString.Replace(thisBlobName, AzureStorageObjectNames.PublisherProfileFileNamePrefix, string.Empty);

                    sb.AppendLine($"{fileName,-15}");
                }

                throw new JghPublisherProfileFile404Exception(sb.ToString());
            }

            var profileAsXmlAsBytes = await _azureStorage.DownloadBlockBlobAsBytesAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, targetAzureBlobName);

            var profileAsXmlString = JghConvert.ToStringFromUtf8Bytes(profileAsXmlAsBytes);

            var answer = XElement.Parse(profileAsXmlString); // throws if invalid XML

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region svc methods

        public async Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string xmlFileNameFragment)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync]";

        try
        {
            if (string.IsNullOrEmpty(xmlFileNameFragment))
                return false;

            var entryPoint = ConnectionStringRepository.GetPublisherProfileFilesEntryPoint();

            var targetAzureBlobName = string.Concat(AzureStorageObjectNames.PublisherProfileFileNamePrefix, xmlFileNameFragment,
                AzureStorageObjectNames.PublisherProfileFileNameExtension);

            var blobDoesExist = await _azureStorage.GetIfBlobExistsAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, targetAzureBlobName);

            return blobDoesExist;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public async Task<PublisherModuleProfileItem> GetPublisherModuleProfileItemAsync(string xmlFileNameFragment)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetPublisherModuleProfileItemAsync]";

        try
        {
            var profileAsXElement = await GetPublisherProfileAsXElementAsync(xmlFileNameFragment); // will throw if file missing or fails to parse as a valid XElement

            var factory = new PublisherFactory();

            var publisher = factory.ManufacturePublisher(profileAsXElement); // null check: will throw if unable to manufacture a legit publisher module instance

            var profileItem = publisher.ParseAssociatedProfile();

            return profileItem;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public async Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync()
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetFileNameFragmentsOfAllPublishingProfilesAsync]";

        try
        {
            #region do everything

            var entryPoint = ConnectionStringRepository.GetPublisherProfileFilesEntryPoint();

            var namesOfAllBlobs = await _azureStorage.GetNamesOfBlobsInContainerAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, AzureStorageObjectNames.PublisherProfileFileNamePrefix, false);

            if (namesOfAllBlobs is null || !namesOfAllBlobs.Any())
                throw new JghPublisherProfileFile404Exception(
                    $"No files of publisher profiles found. Designated remote container=<{entryPoint.AzureStorageContainerName}>");

            var substringDictionary = new Dictionary<string, string>
            {
                {AzureStorageObjectNames.PublisherProfileFileNamePrefix, string.Empty},
                {AzureStorageObjectNames.PublisherProfileFileNameExtension, string.Empty}
            };


            var allFileNameFragments = namesOfAllBlobs.Select(thisBlobName => JghString.Replace(thisBlobName, substringDictionary)).ToArray();

            return allFileNameFragments;

            #endregion
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public async Task<string> GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(string fileNameWithExtension)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIllustrativeExampleOfDatasetExpectedByPublisherAsync]";

        try
        {
            #region null checks

            if (string.IsNullOrEmpty(fileNameWithExtension))
                throw new Jgh404Exception("Unable to obtain example of dataset. The provided filename for the dataset is blank.");

            #endregion

            #region do everything

            var entryPoint = ConnectionStringRepository.GetExamplesOfInputDatasetsForPublishingEntryPoint();

            var blobDoesExist = await _azureStorage.GetIfBlobExistsAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, fileNameWithExtension);

            if (!blobDoesExist)
                return string.Empty; // no big deal. at the user-end this is a low priority

            var contentOfFileAsBytes = await _azureStorage.DownloadBlockBlobAsBytesAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, fileNameWithExtension);

            var contentOfFileAsRawString = JghConvert.ToStringFromUtf8Bytes(contentOfFileAsBytes);

            return contentOfFileAsRawString;

            #endregion
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public async Task<bool> UploadDatasetAsBytesAsync(string accountName, string containerName, string fileNameWithExtension, byte[] datasetContentsAsStringAsUncompressedBytes)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[UploadDatasetAsBytesAsync]";

        try
        {
            if (string.IsNullOrEmpty(accountName))
                throw new Jgh404Exception("Unable to upload dataset. The provided account name for the storage location is blank.");

            if (string.IsNullOrEmpty(containerName))
                throw new Jgh404Exception("Unable to upload dataset. The provided container name for the storage location is blank.");

            if (string.IsNullOrEmpty(fileNameWithExtension))
                throw new Jgh404Exception("Unable to upload dataset. The provided dataset filename for the storage location is blank.");

            if (datasetContentsAsStringAsUncompressedBytes is null || datasetContentsAsStringAsUncompressedBytes.Length == 0)
                throw new Jgh404Exception("Nothing to upload. The provided dataset is blank.");

            var uploadedDidSucceed = await _azureStorage.UploadToBlockBlobAsBytesAsync(accountName, containerName, fileNameWithExtension, true, datasetContentsAsStringAsUncompressedBytes);


            return uploadedDidSucceed;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public async Task<PublisherOutputItem> ConvertPreviouslyUploadedDatasetsToResultsForSingleEventAsync(PublisherInputItem publisherInputItem)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[ConvertPreviouslyUploadedDatasetsToResultsForSingleEventAsync]";

        try
        {
            var profileFileAsXElement = await GetPublisherProfileAsXElementAsync(publisherInputItem.FileNameFragmentOfAssociatedPublishingProfile);

            var factory = new PublisherFactory();

            var publisher = factory.ManufacturePublisher(profileFileAsXElement); // blows if anything at all is wrong

            var results = await publisher.DoAllTranslationsAndComputationsToGenerateResultsAsync(publisherInputItem);

            return results;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion
    }
}