using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.OnBoardServices01.July2018.Persistence;

// ReSharper disable UnusedVariable

// Guidance for Local, LocalCache, Roaming, and Temporary files.
//
// Files are ideal for storing large data-sets, databases, or data that is
// in a common file-format.
//
// Files can exist in either the Local, LocalCache, Roaming, or Temporary folders.
//
// Roaming files will be synchronized across machines on which the user has
// singed in with a connected account.  Roaming of files is not instant; the
// system weighs several factors when determining when to send the data.  Usage
// of roaming data should be kept below the quota (available via the 
// RoamingStorageQuota property), or else roaming of data will be suspended.
// Files cannot be roamed while an application is writing to them, so be sure
// to close your application's file objects when they are no longer needed.
//
// Local files are not synchronized, but are backed up, and can then be restored to a 
// machine different than where they were originally written. These should be for 
// important files that allow the feel that the user did not loose anything
// when they restored to a new device.
//
// Temporary files are subject to deletion when not in use.  The system 
// considers factors such as available disk capacity and the age of a file when
// determining when or whether to delete a temporary file.
//
// LocalCache files are for larger files that can be recreated by the app, and for
// machine specific or private files that should not be restored to a new device.

namespace Jgh.Uwp.Common.July2018.OnBoardServices
{
    /// <summary>
    ///     Provides methods for interacting with local storage safely and asynchronously.
    /// </summary>
    public class LocalStorageServiceUwp : LocalStorageServiceBase
    {
        private const string Locus2 = nameof(LocalStorageServiceUwp);
        private const string Locus3 = "[Jgh.Uwp.Common.July2018]";

        #region constants

        private const string StorageProblemMessage = "Problem occurred trying to conduct a local storage operation.";

        #endregion

        #region fields

        private readonly StorageFolder _systemDataStore;

        #endregion

        #region ctor

        public LocalStorageServiceUwp()
        {
            _systemDataStore = ApplicationData.Current.LocalFolder;
            // Gets the local app data store.
        }

        #endregion

        #region helpers

        private async Task<StorageFolder[]> GetFolders(StorageFolder thisFolder)
        {

            var subFolders = (await thisFolder.GetFoldersAsync(CommonFolderQuery.DefaultQuery)).ToArray();

            var allSubFolders = new List<StorageFolder>(subFolders);

            foreach (var storageFolder in subFolders)
            {
                allSubFolders.AddRange((await GetAllSubFoldersAsync(storageFolder)));
            }

            var answer = allSubFolders.Where(z => z is not null).ToArray();

            return answer;
        }

        private async Task<StorageFolder[]> GetAllSubFoldersAsync(StorageFolder thisFolder)
        {
            #region first of all, enumerate first-level directories in this folder

            var subFolders = (await thisFolder.GetFoldersAsync(CommonFolderQuery.DefaultQuery)).OrderBy(z=>z.DisplayName).ToArray();

            #endregion

            var allSubFolders = new List<StorageFolder>();

            allSubFolders.AddRange(subFolders);

            #region recursively drill down and repeat for all subsequent-level subdirectories

            foreach (var folder in subFolders)
            {
                var moreFolders = await GetAllSubFoldersAsync(folder);

                allSubFolders.AddRange(moreFolders);
            }

            #endregion

            return allSubFolders.ToArray();
        }

        private async Task<bool> IncreaseSizeQuotaIfNotEnoughSpace(string directoryPath, string fileName,
            string textContents)
        {
            var sizeOfSamePreexistingFileIfAny = await GetSizeOfFileAsync(directoryPath, fileName);

            var sizeOfFile = JghConvert.ToBytesUtf8FromString(textContents).LongLength;

            var sizeOfFreeSpaceNeeded = sizeOfFile - sizeOfSamePreexistingFileIfAny;

            var outcome =
                await IncreaseQuotaToAccommodateRequestedFreeSpaceAsync(
                    sizeOfFreeSpaceNeeded + 100); // add arbitrarily small number size as a safety margin

            return outcome;
        }

        #endregion

        #region implementation of abstract methods in LocalStorageServiceBase - platform specific

        /// <summary>
        ///     Creates a directory/folder in local storage if it doesn't already exist.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>The name of the path whether existing or created. Blank if folder is invalid.</returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string> CreateDirectoryAsync(string folder)
        {
            const string failure = "Unable create directory/folder in local storage.";
            const string locus = "[CreateDirectoryAsync]";

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return string.Empty; // no point bothering with anything further

                #endregion

                await
                    _systemDataStore.CreateFolderAsync(folder,
                        CreationCollisionOption.OpenIfExists);

                var answer = folder;

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was <{folder}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Determines if a directory/folder exists in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>
        ///     True if directory/folder is found. False if directory/folderPath is invalid or directory/folder is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<bool> DirectoryExistsAsync(string folder)
        {
            const string failure = "Unable determine if directory/folder exists in local storage.";
            const string locus = "[DirectoryExistsAsync]";

#pragma warning disable 168
            StorageFolder rubbish;
#pragma warning restore 168

            try
            {

                Debug.WriteLine($"directory existence to be checked: folder=<<{folder}>>");

                #region null checks

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return false; // no point bothering with anything further

                #endregion

                await _systemDataStore.GetFolderAsync(folder); // throws if doesn't exist

                return true;
            }

            #region try catch handling

            catch (FileNotFoundException ex)
            {
                // this is not a error on JGH's part. trying to get a missing folder can throw this
                var xx = ex.Message;

                return false;
            }
            catch (DirectoryNotFoundException ex)
            {
                var xx = ex.Message;

                return false;
            }
            catch (ArgumentException ex)
            {
                var xx = ex.Message;

                return false;
            }
            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was <{folder}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Deletes a directory/folder and all its files and subdirectories.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>
        ///     Collection of file and directory/folder paths of all deletions. Null if directory/folder Path is
        ///     invalid or not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string[]> DeleteDirectoryAsync(string folder)
        {
            const string failure = "Unable to delete directory/folder in local storage.";
            const string locus = "[DeleteDirectoryAsync]";

            var listOfNamesOfThingsDeletedInTheFolder = new List<string>();

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return null;

                if (folder == "*")
                    return null; //belt and braces. guard against an idiot user mistakenly entering a wildcard


                #endregion

                #region first of all, delete all files in this folder

                if (!await DirectoryExistsAsync(folder))
                    return null; // no point bothering with anything further


                var thisFolder = await _systemDataStore.GetFolderAsync(folder);

                var filesInThisFolderToBeDeleted = await thisFolder.GetFilesAsync();

                foreach (var fileToBeDeleted in filesInThisFolderToBeDeleted)
                {
                    listOfNamesOfThingsDeletedInTheFolder.Add(fileToBeDeleted.Path);

                    await fileToBeDeleted.DeleteAsync();
                }

                #endregion

                #region recursively drill down and delete all the subfolders

                var subFolders = await thisFolder.GetFoldersAsync();

                foreach (var subfolder in subFolders)
                {
                    var namesOfMoreDeletedThings = await DeleteDirectoryAsync(subfolder.Path);

                    if (namesOfMoreDeletedThings is not null)
                        listOfNamesOfThingsDeletedInTheFolder.AddRange(namesOfMoreDeletedThings);
                }

                listOfNamesOfThingsDeletedInTheFolder.Add(thisFolder.Path);

                await thisFolder.DeleteAsync();

                #endregion

                return listOfNamesOfThingsDeletedInTheFolder.ToArray();
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was <{folder}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public override async Task<string[]> EnumerateDirectoryPathsInDirectoryAsync(string folder)
        {
            const string failure = "Unable to enumerate directory paths in local storage.";
            const string locus = "[EnumerateDirectoryPathsInDirectoryAsync]";

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return null;

                if (folder == "*")
                    return null; //belt and braces. guard against an idiot user mistakenly entering a wildcard

                if (!await DirectoryExistsAsync(folder))
                    return null; // no point bothering with anything further

                #endregion

                var thisFolder = await _systemDataStore.GetFolderAsync(folder);

                var allSubFolders = await GetFolders(thisFolder);

                var answer = allSubFolders.Where(z => z is not null).Select(z => z.Path).ToArray();

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The directory in question was <{folder}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Writes a text file to local storage using CreationCollisionOption.ReplaceExisting.
        ///     If the specified directory/folder doesn't exist, it will be created.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are illegal in paths.
        /// </param>
        /// <param name="textContents">The text contents.</param>
        /// <returns>
        ///     The file path of the file written. Empty string if directory/folder
        ///     path or fileName is invalid or textContents are null.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or some other failure occurs.
        /// </exception>
        public override async Task<string> WriteTextFileAsync(string folder, string fileName,
            string textContents)
        {
            const string failure = "Unable to save a text file in local storage.";
            const string locus = "[WriteTextFileAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return string.Empty; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return string.Empty; // no point bothering with anything further

                if (textContents is null)
                    return string.Empty;

                #endregion

                if (!await DirectoryExistsAsync(folder))
                {
                    var folderPathWritten = await CreateDirectoryAsync(folder);

                    if (folderPathWritten == string.Empty)
                        return string.Empty;
                }

                if (await FileExistsAsync(folder, fileName))
                    await DeleteFileAsync(folder, fileName);

                filePath = Path.Combine(folder, fileName);

                var outcome = await IncreaseSizeQuotaIfNotEnoughSpace(folder, fileName, textContents);

                if (outcome == false)
                    throw new Exception("File not saved. Isolated storage is maxed out.");

                var containingFolder = await _systemDataStore.GetFolderAsync(folder);

                var newFile = await containingFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(newFile, textContents);

                return newFile.Path;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Reads a text file in local storage, converts the contents to a byte array and measures the size of the array in
        ///     bytes.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <returns>
        ///     Size if array (Int64), or zero if folder or fileName is invalid or file doesn't exist or file is empty.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<long> GetSizeOfFileAsync(string folder, string fileName)
        {
            var contentsOfFile = await ReadTextFileAsync(folder, fileName);

            if (string.IsNullOrEmpty(contentsOfFile))
                return 0;

            var bytes = JghConvert.ToBytesUtf8FromString(contentsOfFile);

            return bytes.LongLength;
        }

        /// <summary>
        ///     Determines if a file exists in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are the
        ///     many characters that are illegal in paths. For purposes here, wildcards are illegal.
        /// </param>
        /// <returns>
        ///     True if file is found. False if directory/folder Path or fileName is invalid or file is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<bool> FileExistsAsync(string folder, string fileName)
        {
            const string failure = "Unable determine if file exists in local storage.";
            const string locus = "[FileExistsAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return false; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return false; // no point bothering with anything further

                #endregion

                await _systemDataStore.GetFileAsync(Path.Combine(folder, fileName));

                return true;
            }

            #region try catch handling

            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Reads a text file in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are illegal in paths.
        /// </param>
        /// <returns>
        ///     Text contents, or null if directory/folder Path or fileName is invalid or doesn't exist.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string> ReadTextFileAsync(string folder, string fileName)
        {
            const string failure = "Unable to read a text file in local storage.";
            const string locus = "[ReadTextFileAsync]";

            var filePath = string.Empty;

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return null; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return null; // no point bothering with anything further

                #endregion

                #region existence checks

                if (!await DirectoryExistsAsync(folder))
                    return null;

                if (!await FileExistsAsync(folder, fileName))
                    return null;

                #endregion

                filePath = Path.Combine(folder, fileName);

                var containingFolder = await _systemDataStore.GetFolderAsync(folder);

                var fileToBeRead = await containingFolder.GetFileAsync(fileName);

                var stringInStorage = await FileIO.ReadTextAsync(fileToBeRead);

                return string.IsNullOrWhiteSpace(stringInStorage) ? string.Empty : stringInStorage;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{filePath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Deletes a file in a directory/folder in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are illegal in paths.
        /// </param>
        /// <returns>
        ///     The file path of the deletion. Empty string if directory/folder Path or fileName is invalid or file is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string> DeleteFileAsync(string folder, string fileName)
        {
            const string failure = "Unable to delete file or files in local storage.";
            const string locus = "[DeleteFileAsync]";

            var pathOfFileToBeDeleted = string.Empty;

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return string.Empty; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return string.Empty; // no point bothering with anything further

                if (folder == "*")
                    return string.Empty; //belt and braces. guard against an idiot user mistakenly entering a wildcard

                if (!await DirectoryExistsAsync(folder))
                    return string.Empty; // no point bothering with anything further

                if (!await FileExistsAsync(folder, fileName))
                    return string.Empty; // no point bothering with anything further

                #endregion

                if (!await FileExistsAsync(folder, fileName))
                    return string.Empty;

                var thisFolder = await _systemDataStore.GetFolderAsync(folder);

                var fileToBeDeleted = await thisFolder.GetFileAsync(fileName);

                pathOfFileToBeDeleted = fileToBeDeleted.Path;

                await fileToBeDeleted.DeleteAsync();

                return pathOfFileToBeDeleted;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} Problem occurred trying to conduct an local storage operation. The filePath in question was <{pathOfFileToBeDeleted}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public override async Task<string[]> EnumerateFilePathsInDirectoryAsync(string folder)
        {
            const string failure = "Unable to enumerate file paths in directory.";
            const string locus = "[EnumerateFilePathsInDirectoryAsync]";

            try
            {
                #region null checks

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
                    return null;

                if (folder == "*")
                    return null; //belt and braces. guard against an idiot user mistakenly entering a wildcard

                if (!await DirectoryExistsAsync(folder))
                    return null; // no point bothering with anything further

                #endregion

                #region enumerate all the folders and subfolders

                var allFolders = new List<StorageFolder>();

                var thisFolder = await _systemDataStore.GetFolderAsync(folder);

                allFolders.Add(thisFolder);

                allFolders.AddRange(await GetFolders(thisFolder));

                #endregion

                #region enumerate all the files

                var allStorageFiles = new List<StorageFile>();

                foreach (var aFolder in allFolders)
                {
                    allStorageFiles.AddRange(await aFolder.GetFilesAsync(CommonFileQuery.DefaultQuery));   
                }

                #endregion

                var filePaths = allStorageFiles.Where(z => z is not null).Select(z => z.Path);

                return filePaths.ToArray();
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The directory in question was <{folder}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }


        /// <summary>
        ///     Pretty-prints the status of the local storage quota and available space.
        /// </summary>
        /// <returns>
        ///     One-line status report
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public async Task<string> PrintOnelinerReportAboutFreeSpaceAvailableAsync()
        {
            const string failure = "Unable to determine the status of free available space in local store.";
            const string locus = "[PrintOnelinerReportAboutFreeSpaceAvailableAsync]";

            try
            {
                return await Task.FromResult("In Windows, there is no explicit ceiling on storage space.");
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage}";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Not relevant in UWP?. Not implemented.
        /// </summary>
        /// <param name="requestedFreeSpace">The requested free space as an integer number of megabytes.</param>
        public Task<bool> IncreaseQuotaToAccommodateRequestedFreeSpaceAsync(long requestedFreeSpace)
        {
            const string failure = "Unable increase free space in local storage.";
            const string locus = "[IncreaseQuotaToAccommodateRequestedFreeSpaceAsync]";

            long currentSizeQuota = 0;

            long availableFreeSpace = 0;


            try
            {
                // no idea if this is necessary. no idea if there is a size constraint. tbd

                return Task.FromResult(true);
            }

            #region try catch handling

            catch (Exception ex)
            {
                if (!(ex is IsolatedStorageException))
                    throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);


                var currentQuotaAsString = JghConvert.SizeOfBytesInHighestUnitOfMeasure(currentSizeQuota);

                var currentFreeSpaceAsString = JghConvert.SizeOfBytesInHighestUnitOfMeasure(availableFreeSpace);

                var requestedFreeSpaceAsString = JghConvert.SizeOfBytesInHighestUnitOfMeasure(requestedFreeSpace);

                var failureMsg =
                    $"{failure} {StorageProblemMessage} Current local storage quota is {currentQuotaAsString}. Current free space is {currentFreeSpaceAsString}. Requested free space is {requestedFreeSpaceAsString}";
                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Removes the local store thus deleting all its contents.
        ///     Size quota of the store is unaffected.
        /// </summary>
        /// <returns>
        ///     True
        /// </returns>
        public async Task<bool> RemoveStoreAsync()
        {
            const string failure = "Unable to delete ApplicationData.Current.LocalFolder and all its contents.";
            const string locus = "[RemoveStoreAsync]";

            try
            {
                await ApplicationData.Current.ClearAsync();

                return true;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage}";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion
    }
}