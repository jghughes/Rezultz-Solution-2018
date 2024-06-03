using System;
using System.IO;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Xml.July2018;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.OnBoardServices01.July2018.Persistence
{
    /// <summary>
    ///     Useful file handling helper methods that wrap abstract methods that must be implemented
    ///     in a derived class.The derived class is the class that actually touches local storage of one kind
    ///     or another. The derived class might or might not be platform specific. 
    /// </summary>
    public abstract class LocalStorageServiceBase : ILocalStorageServiceCustomMethods, ILocalStorageService
    {
        private const string Locus2 = nameof(LocalStorageServiceBase);
        private const string Locus3 = "[NetStd.OnBoardServices01.July2018]";

        #region abstract ILocalStorageService methods

        public abstract Task<string> CreateDirectoryAsync(string folder);

        public abstract Task<bool> DirectoryExistsAsync(string folder);
        
        public abstract Task<string[]> DeleteDirectoryAsync(string folder);

        public abstract Task<string[]> EnumerateDirectoryPathsInDirectoryAsync(string folder);

        public abstract Task<string> WriteTextFileAsync(string folder, string fileName, string textContents);

        public abstract Task<bool> FileExistsAsync(string folder, string fileName);

        public abstract Task<long> GetSizeOfFileAsync(string folder, string fileName);

        public abstract Task<string> ReadTextFileAsync(string folder, string fileName);

        public abstract Task<string> DeleteFileAsync(string folder, string fileName);

        public abstract Task<string[]> EnumerateFilePathsInDirectoryAsync(string folder);

        #endregion

        #region ILocalStorageServiceCustomMethods methods

        /// <summary>
        ///     Serialises and saves a typed object to isolated storage using FileMode.OpenOrCreate. Object must be serialisable by
        ///     JghSerialisation.
        /// </summary>
        /// <param name="folder">
        ///     Last subdirectory such as "mysubdirectory" Don't include path separators because these are platform specific.
        /// </param>
        /// <param name="fileName">
        ///     Name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <param name="theTypedObject">The object.</param>
        /// <returns>
        ///     The file path of the file written. Empty string if folder or fileName is invalid or textContents are null.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public async Task<string> SaveSerialisableObjectAsync<T>(string folder, string fileName,
            T theTypedObject)
        {
            const string failure = "Unable to serialise object to Json and save it to local file storage.";
            const string locus = "[SaveSerialisableObjectAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (theTypedObject == null)
                    return string.Empty;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return string.Empty;

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return string.Empty;

                filePath = Path.Combine(folder, fileName);

                #endregion

                #region existence checks

                if (!await DirectoryExistsAsync(folder))
                    await CreateDirectoryAsync(folder);

                #endregion

                var json = JghSerialisation.ToJsonFromObject(theTypedObject);
                //var json = await JghSerialisation.ToJsonFromObjectAsync(theTypedObject);

                var filePathOfWrittenFile = await WriteTextFileAsync(folder, fileName, json);

                return filePathOfWrittenFile;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Reads and deserialises a typed object from a file in isolated storage. Object must be serialisable by
        ///     System.Runtime.Serialization.Json.
        /// </summary>
        /// <param name="folder">
        ///     Fully qualified directory path such as "MyApp1\Dir1\SubDir2". For the root directory use
        ///     "\\". Null or whitespace is invalid, as are any characters that are not allowed in paths.
        /// </param>
        /// <param name="fileName">
        ///     Name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <returns>
        ///     The deserialised type. Null if folder or fileName is invalid or non-existent.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public async Task<T> ReadSerializedObjectAsync<T>(string folder, string fileName)
        {
            const string failure = "Unable to read a file in local storage for application data.";
            const string locus = "[ReadSerializedObjectAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return default; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return default; // no point bothering with anything further

                filePath = Path.Combine(folder, fileName);

                #endregion

                #region existence checks

                if (!await DirectoryExistsAsync(folder))
                    return default;

                if (!await FileExistsAsync(folder, fileName))
                    return default;

                #endregion

                var stringInStorage = await ReadTextFileAsync(folder, fileName);

                if (string.IsNullOrWhiteSpace(stringInStorage))
                    return default;

                var theTypedObject = JghSerialisation.ToObjectFromJson<T>(stringInStorage);

                return theTypedObject;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Saves identities of named things to isolated storage in free-format as xml file using FileMode.OpenOrCreate.
        /// </summary>
        /// <param name="directoryPath">
        ///     Fully qualified directory path such as "MyApp1\Dir1\SubDir2". For the root directory use
        ///     "\\". Null or whitespace is invalid, as are any characters that are not allowed in paths.
        /// </param>
        /// <param name="fileName">
        ///     Name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <param name="namedItems">Any generic type that satisfies the method's generic interfaces.</param>
        /// <returns>
        ///     The file path of the file written. Empty string if either folder or fileName is invalid or textContents are
        ///     null.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public async Task<string> WriteFreeformIdentitiesOfNamedItemsAsync<T>(string directoryPath, string fileName,
            T[] namedItems)
            where T : class, IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName
        {
            const string failure = "Unable to save xml file to local file storage for application data.";
            const string locus = "[WriteFreeformIdentitiesOfNamedItemsAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (namedItems == null)
                    return string.Empty;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return string.Empty;

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(directoryPath, true))
                    return string.Empty;

                filePath = Path.Combine(directoryPath, fileName);

                #endregion

                #region existence checks

                if (!await DirectoryExistsAsync(directoryPath))
                    await CreateDirectoryAsync(directoryPath);

                #endregion

                var rootElementAsString =
                    JghFreeFormXmlSerialisationHelpers.ConvertNamedThingsIntoSimpleIdentitiesAsFreeFormXml(namedItems);

                // for reasons i am utterly unable to deduce despite exhaustive investigation, FileMode.OpenCreate fails in the case of a pre-existing XML file upon subsequently attempting to read it with ReadTextFileAsync. so no choice but to pre-delete
                if (await FileExistsAsync(directoryPath, fileName))
                    await DeleteFileAsync(directoryPath, fileName);

                var filePathOfWrittenFile = await WriteTextFileAsync(directoryPath, fileName, rootElementAsString);

                return filePathOfWrittenFile;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Reads and deserialises an XML file containing free-format identities of named things. File must contain valid xml
        ///     generated by WriteFreeformIdentitiesOfNamedItemsAsync
        /// </summary>
        /// <param name="directoryPath">
        ///     Fully qualified directory path such as "MyApp1\Dir1\SubDir2". For the root directory use
        ///     "\\". Null or whitespace is invalid, as are any characters that are not allowed in paths.
        /// </param>
        /// <param name="fileName">
        ///     Name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <returns>
        ///     Any generic type that satisfies the method's generic interfaces. Null if folder or fileName is invalid or
        ///     non-existent.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public async Task<T[]> ReadFreeformIdentitiesOfNamedItemsAsync<T>(string directoryPath, string fileName)
            where T : class, IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName, new()
        {
            const string failure = "Unable to read xml file from local file storage for application data.";
            const string locus = "[ReadFreeformIdentitiesOfNamedItemsAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return []; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(directoryPath, true))
                    return []; // no point bothering with anything further

                filePath = Path.Combine(directoryPath, fileName);

                #endregion

                #region existence checks

                if (!await DirectoryExistsAsync(directoryPath))
                    return [];

                if (!await FileExistsAsync(directoryPath, fileName))
                    return [];

                #endregion

                var stringInStorage = await ReadTextFileAsync(directoryPath, fileName);

                if (string.IsNullOrWhiteSpace(stringInStorage))
                    return [];

                var answer =
                    JghFreeFormXmlSerialisationHelpers
                        .ConvertCollectionOfSimpleIdentitiesAsFreeFormXmlBackIntoNamedThings<T>(stringInStorage);

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion


    }
}