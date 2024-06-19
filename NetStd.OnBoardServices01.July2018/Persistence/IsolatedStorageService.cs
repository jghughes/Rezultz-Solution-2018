using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;

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

namespace NetStd.OnBoardServices01.July2018.Persistence
{
    /// <summary>
    ///     Provides storage using the System.IO.IsolatedStorage Namespace. To interact with storage, we use
    ///     the synchronous file handling methods provided by the System.IO.IsolatedStorage.IsolatedStorageFile class,
    ///     storing data in System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForAssembly()
    ///     
    ///     https://docs.microsoft.com/en-us/dotnet/api/system.io.isolatedstorage?view=netframework-4.8
    ///
    ///     For the trickery to lock non-atomic operations that are async, see; -
    ///     https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx

    /// </summary>
    public class IsolatedStorageService : LocalStorageServiceBase
    {
        private const string Locus2 = nameof(IsolatedStorageService);
        private const string Locus3 = "[NetStd.OnBoardServices01.July2018]";

        private const string StorageProblemMessage = "Problem occurred trying to conduct a local storage operation.";

        #region implementation of abstract methods in LocalStorageServiceBase - asynchronous file handling methods of System.IO.IsolatedStorage classes

        /// <summary>
        ///     Creates a directory in isolated storage for this app if it doesn't already exist.
        ///     Returns the fully qualified directory path created.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>The name of the path created. Null if folder is invalid.</returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<string> CreateDirectoryAsync(string folder)
        {
            const string failure = "Unable create directory in local storage.";
            const string locus = "[CreateDirectoryAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                #endregion

                string answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    store.CreateDirectory(folder); // the way this method works is that a pre-existing directory won't be overwritten

                    answer = folder;

                    store.Close();
                }

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
        ///     Determines if a directory exists in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>
        ///     True if directory is found. False if folder is invalid or directory is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<bool> DirectoryExistsAsync(string folder)
        {
            const string failure = "Unable determine if directory exists in local storage.";
            const string locus = "[DirectoryExistsAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return false;

                #endregion

                bool answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    answer = store.DirectoryExists(folder);

                    store.Close();
                }

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
        ///     Deletes a directory and all its files and subdirectories.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>
        ///     Collection of fully qualified file and directory paths of all deletions. Null if folder is invalid or not found.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<string[]> DeleteDirectoryAsync(string folder)
        {
            const string failure = "Unable to delete directory in local storage.";
            const string locus = "[DeleteDirectoryAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                if (folder == "*")
                    return null; //belt and braces. guard against an idiot user mistakenly entering a wildcard

                #endregion

                string[] answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    #region do work

                    if (!store.DirectoryExists(folder))
                    {
                        answer = null;
                        // method is intended to return null if not found. (Note. store.GetFiles() throws a DirectoryNotFoundException) 
                    }
                    else
                    {
                        var deletedThings = new List<string>();

                        #region first of all, delete all files in this directory

                        var searchPattern = JghPath.CombineNoChecks(folder, "*");

                        var filesInDirectoryPath = store.GetFileNames(searchPattern);

                        foreach (var file in filesInDirectoryPath)
                        {
                            var filePath = Path.Combine(folder, file);

                            store.DeleteFile(filePath);

                            deletedThings.Add(filePath);
                        }

                        #endregion

                        #region recursively drill down and delete all the subdirectories

                        foreach (var subDirectory in store.GetDirectoryNames(searchPattern))
                            deletedThings.AddRange(
                                await DeleteDirectoryAsync(Path.Combine(folder, subDirectory)));

                        #endregion

                        #region finally, now and only now that it's empty, delete this directory

                        store.DeleteDirectory(folder);

                        deletedThings.Add(folder);

                        answer = deletedThings.OrderBy(z => z).ToArray(); // do this in case order is not guaranteed above

                        #endregion
                    }

                    #endregion

                    store.Close();
                }

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
        
        public override async Task<string[]> EnumerateDirectoryPathsInDirectoryAsync(string folder)
        {
            const string failure = "Unable to enumerate directory paths in local storage.";
            const string locus = "[EnumerateDirectoryPathsInDirectoryAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                if (folder == "*")
                    return null; //belt and braces. guard against an idiot user mistakenly entering a wildcard

                #endregion

                string[] answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    answer = store.DirectoryExists(folder) ? GetDirectoryPaths(folder, store) : null;

                    store.Close();
                }

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
        ///     Writes a text file to local storage using FileMode.OpenOrCreate. If the specified directory doesn't exist, it will
        ///     be created.
        /// </summary>
        /// <param name="folder">
        ///     Fully qualified directory path such as "MyApp1\Dir1\SubDir2". For the root directory use
        ///     '/'. Null or whitespace is invalid, as are any characters that are not allowed in paths.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <param name="textContents">The text contents.</param>
        /// <returns>
        ///     The fully qualified file path of the file written. Null if folder or fileName is invalid or textContents are null.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<string> WriteTextFileAsync(string folder, string fileName,
            string textContents)
        {
            const string failure = "Unable to save a text file in local storage.";
            const string locus = "[WriteTextFileAsync]";

            var fullyQualifiedFileName = string.Empty;

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return null;

                if (textContents is null)
                    return null;

                #endregion

                fullyQualifiedFileName = Path.Combine(folder, fileName);

                using (await mutex.LockAsync())
                {
                    var outcome = await IncreaseSizeQuotaIfNotEnoughSpace(folder, fileName, textContents);

                    if (outcome == false)
                        throw new Exception("File not saved. Isolated storage has insufficient space.");

                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    if (!store.DirectoryExists(folder))
                        store.CreateDirectory(folder);

                    if (store.FileExists(fullyQualifiedFileName))
                        store.DeleteFile(fullyQualifiedFileName); 
                    // under no account remove this just because it seems superfluous. appearances deceive.
                    // if you don't believe me run all the tests for the usersettings class in the UWP test harness and watch a crucial test fail inexplicably

                    using (var stream = store.OpenFile(fullyQualifiedFileName, FileMode.OpenOrCreate))
                    {
                        using var writer = new StreamWriter(stream);

                        await writer.WriteAsync(textContents);

                        await writer.FlushAsync();
                    }

                    store.Close();
                }

                return fullyQualifiedFileName;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{fullyQualifiedFileName}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Determines if a file exists in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are the
        ///     many characters that are not allowed in paths. For purposes here, wildcards are illegal.
        /// </param>
        /// <returns>
        ///     True if file is found. False if folder or fileName is invalid or file is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<bool> FileExistsAsync(string folder, string fileName)
        {
            const string failure = "Unable determine if file exists in local storage.";
            const string locus = "[FileExistsAsync]";

            var fullyQualifiedFileName = string.Empty;

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return false;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return false;

                #endregion

                fullyQualifiedFileName = Path.Combine(folder, fileName);

                bool answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    answer = store.DirectoryExists(folder) && store.FileExists(fullyQualifiedFileName);

                    store.Close();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{fullyQualifiedFileName}>.";

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
            const string failure = "Unable to get size of file.";
            const string locus = "[GetSizeOfFileAsync]";

            var fullyQualifiedFileName = string.Empty;

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return 0;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return 0;

                #endregion

                fullyQualifiedFileName = Path.Combine(folder, fileName);

                long answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    if (!store.DirectoryExists(folder) || !store.FileExists(fullyQualifiedFileName))
                    {
                        answer = 0;
                    }
                    else
                    {
                        using var isoStream = new IsolatedStorageFileStream(fullyQualifiedFileName, FileMode.Open, store);
                        
                        using var reader = new StreamReader(isoStream);

                        isoStream.Position = 0;

                        var textContentsOfFile = await reader.ReadToEndAsync();

                        answer = JghConvert.ToBytesUtf8FromString(textContentsOfFile).LongLength; // this will understate the size of the file, by how much?

                        //isoStream.Close();
                    }

                    store.Close();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{fullyQualifiedFileName}>.";

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
        ///     characters that are not allowed in paths.
        /// </param>
        /// <returns>
        ///     Text contents, or null if folder or fileName is invalid or doesn't exist.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<string> ReadTextFileAsync(string folder, string fileName)
        {
            const string failure = "Unable to read a text file in local storage.";
            const string locus = "[ReadTextFileAsync]";

            var fullyQualifiedFileName = string.Empty;

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return null;

                #endregion


                fullyQualifiedFileName = Path.Combine(folder, fileName);

                string answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    if (!store.DirectoryExists(folder) || !store.FileExists(fullyQualifiedFileName))
                    {
                        answer = null;
                    }
                    else
                    {
                        using var isoStream = new IsolatedStorageFileStream(fullyQualifiedFileName, FileMode.Open, store);
                            
                        using var reader = new StreamReader(isoStream);

                        isoStream.Position = 0;

                        answer = await reader.ReadToEndAsync();
                    }

                    store.Close();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was <{fullyQualifiedFileName}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Deletes a file in a directory in local storage.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are any
        ///     characters that are not allowed in paths.
        /// </param>
        /// <returns>
        ///     The fully qualified file path of the deletion. Null if folder or fileName is invalid or file is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public override async Task<string> DeleteFileAsync(string folder, string fileName)
        {
            const string failure = "Unable to delete file or files in local storage.";
            const string locus = "[DeleteFileAsync]";

            var fullyQualifiedFileName = string.Empty;

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return null;

                #endregion

                fullyQualifiedFileName = Path.Combine(folder, fileName);

                string answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    if (!store.DirectoryExists(folder) || !store.FileExists(fullyQualifiedFileName))
                    {
                        answer = null;
                    }
                    else
                    {
                        store.DeleteFile(fullyQualifiedFileName);

                        answer = fullyQualifiedFileName;
                    }

                    store.Close();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} Problem occurred trying to conduct an local storage operation. The filePath in question was <{fullyQualifiedFileName}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Enumerates the files in a directory including those in all its subdirectories.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>
        ///     Collection of fully qualified FilePaths. Null if directory Path is
        ///     invalid or not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string[]> EnumerateFilePathsInDirectoryAsync(string folder)
        {
            const string failure = "Unable to enumerate file paths in local storage.";
            const string locus = "[EnumerateFilePathsInDirectoryAsync]";

            string[] answer;

            
            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (!IsValidFullyQualifiedDirectoryPath(folder)) return null;

                if (folder == "*")
                    return null; //belt and braces. guard against an idiot user mistakenly entering a wildcard

                #endregion

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    var filePaths = new List<string>();

                    #region first of all, enumerate all filePaths in this folder

                    var listOfFilePaths = GetFilePaths(folder, store);

                    filePaths.AddRange(listOfFilePaths);

                    #endregion

                    #region recursively drill down and enumerate all the subDirectoryPaths 

                    var subDirectoryPaths = GetDirectoryPaths(folder, store);

                    #endregion

                    #region enumerate all the filepaths in all the subDirectoryPaths 

                    foreach (var subDirectoryPath in subDirectoryPaths)
                    {
                        var moreFilePaths = GetFilePaths(subDirectoryPath, store);

                        if (moreFilePaths is not null) filePaths.AddRange(moreFilePaths);
                    }

                    #endregion

                    answer = filePaths.ToArray();

                    store.Close();
                }

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

        public async Task<string> PrintOnelinerReportAboutFreeSpaceAvailableAsync()
        {
            const string failure = "Unable to determine the status of free available space in local store.";
            const string locus = "[PrintOnelinerReportAboutFreeSpaceAvailableAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                string answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    var curQuota = JghConvert.SizeOfBytesInHighestUnitOfMeasure(store.Quota);

                    var spaceUsed = JghConvert.SizeOfBytesInHighestUnitOfMeasure(store.UsedSize);

                    var spaceAvailable = JghConvert.SizeOfBytesInHighestUnitOfMeasure(store.AvailableFreeSpace);

                    store.Close();

                    var sb = new StringBuilder();

                    sb.AppendLine($"Current quota: {curQuota}. Used: {spaceUsed}. Available: {spaceAvailable}.");

                    answer = sb.ToString();
                }

                return answer;
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
        ///     If necessary, increases the quota for the size of local storage sufficiently to ensure a requested amount of free
        ///     space.
        /// </summary>
        /// <param name="requestedFreeSpace">The requested free space as an integer number.</param>
        /// <returns>
        ///     True if no increase is necessary or if it is and the increase succeeds.
        /// </returns>
        public async Task<bool> IncreaseQuotaToAccommodateRequestedFreeSpaceAsync(long requestedFreeSpace)
        {
            const string failure = "Unable increase free space in local storage.";
            const string locus = "[IncreaseQuotaToAccommodateRequestedFreeSpaceAsync]";

            long currentSizeQuota = 0;

            long availableFreeSpace = 0;

            var mutex = new JghAsyncLock();

            try
            {
                bool answer;

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    currentSizeQuota = store.Quota; // only used for exception reporting

                    availableFreeSpace = store.AvailableFreeSpace;

                    if (availableFreeSpace >= requestedFreeSpace)
                        return true; // nothing required

                    answer = store.IncreaseQuotaTo(store.Quota + requestedFreeSpace - availableFreeSpace);

                    store.Close();
                }

                return answer;

            }

            #region try catch handling

            catch (Exception ex)
            {
                if (ex is not IsolatedStorageException)
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


        #endregion

        #region other methods that might come in handy

        /// <summary>
        ///     Finds all directory paths whose opening sequence of characters match a wild-card search pattern.
        /// </summary>
        /// <param name="searchPatternForDirectoryPath">
        ///     Fully qualified directory path such as "MyApp1\Dir1\SubDir2" or "MyApp1\Dir1\SubDir?" or "MyApp1\Dir1\Sub*".
        ///     Null or whitespace is invalid, as are any characters that are not allowed in paths. The wildcard specifiers for
        ///     a single-character ("?") and multi-characters ("*") may only be inserted in the final subdirectory of the path
        ///     and that subdirectory may not be enclosed in a final slash. To find everything in storage use '*'
        /// </param>
        /// <returns>
        ///     Collection of directory paths. Empty collection if searchPatternForDirectoryPath is invalidly formatted or no
        ///     matches are found.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public async Task<string[]> FindDirectoryPathsThatMatchAsync(string searchPatternForDirectoryPath)
        {
            const string failure = "Unable to find directory paths with matching pattern in local storage.";
            const string locus = "[FindDirectoryPathsThatMatchAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (string.IsNullOrWhiteSpace(searchPatternForDirectoryPath))
                    return []; // no point bothering with anything further

                if (!JghFilePathValidator.IsValidNtfsDirectoryPath(searchPatternForDirectoryPath, false))
                    return [];
                // no point bothering with anything further NB. false deems wildcards to be valid

                //var searchPatternWithoutWildCards = searchPatternForDirectoryPath.Replace('?', 'x').Replace('*', 'x'); // neutralise any wild card characters because the validator regards them as forbidden 

                //if (!JghFilePathValidator.IsValidNtfsDirectoryPath(searchPatternWithoutWildCards, true))
                //    return new string[0]; // no point bothering with anything further

                #endregion

                #region figure out the root inferred from the searchPatternForRootDirectoryName 

                // Get the root of the search string.
                var rootPathAtThisLevel = Path.GetDirectoryName(searchPatternForDirectoryPath); // todo

                

                if (rootPathAtThisLevel != "") rootPathAtThisLevel += Path.DirectorySeparatorChar.ToString();
                //if (rootPathAtThisLevel != "") rootPathAtThisLevel += _directorySeparatorCharAsString;

                #endregion

                List<string> flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern;

                // because these are not atomic operations we should lock them. but they are async, so we can't easily. read more here https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    // Retrieve matching first level directories whose paths match the searchPatternForRootDirectoryName (in second level and deeper recursions of this method, the search attern invariably end with "*"

                    try
                    {
                        flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern =
                            [..store.GetDirectoryNames(searchPatternForDirectoryPath)];
                    }
                    catch (Exception)
                    {
                        return [];
                        // no point bothering with anything further. probably because the searchPattern is invalidly formatted
                    }

                    #region drill down. for each matching directory found, do exhaustive recursive drill down to get all directories in all subsequent levels and add them to the list

                    for (int i = 0, max = flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern.Count;
                         i < max;
                         i++)
                    {
                        //var matchingSubDirectoryAtThisLevel =
                        //    flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern[i] +
                        //    _directorySeparatorCharAsString;

                        var matchingSubDirectoryAtThisLevel =
                            flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern[i] +
                            Path.DirectorySeparatorChar.ToString();

                        // from this point on in the recursive drill-down we get "all" subdirectories  i.e. searchPatternForRootDirectoryName always ends with "*" from now on

                        var more =
                            await
                                FindDirectoryPathsThatMatchAsync(rootPathAtThisLevel + matchingSubDirectoryAtThisLevel +
                                                                 "*");

                        // For each subdirectory found, add in the base path

                        for (var j = 0; j < more.Length; j++) more[j] = matchingSubDirectoryAtThisLevel + more[j];

                        // Insert the subdirectories into the list and update the counter and upper bound

                        flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern.InsertRange(i + 1, more);

                        i += more.Length;

                        max += more.Length;
                    }

                    #endregion


                    store.Close();
                }

                return flattenedListOfAllDirectoryPathsThatStartWithAMatchToTheSearchPattern.ToArray();
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The searchPattern in question was <{searchPatternForDirectoryPath}>.";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Finds all file paths whose filenames match a wild-card search pattern.
        /// </summary>
        /// <param name="searchPatternForFileName">
        ///     Filename such as "MyFileA.txt" or "MyFile?.txt" or "My*".
        ///     Null or whitespace is invalid, as are any characters that are not allowed in paths.
        ///     The wildcard specifiers for a single-character ("?") and multi-characters ("*") are valid.
        /// </param>
        /// <returns>
        ///     Collection of file paths. Empty collection if searchPatternForFileName is invalidly formatted or no matches are
        ///     found.
        /// </returns>
        /// <exception cref="JghCarrierException">A storage operation fails or some other failure occurs.</exception>
        public async Task<string[]> FindFilePathsWithFilenamesThatMatchAsync(string searchPatternForFileName)
        {
            const string failure = "Unable to search for matching filenames in local storage.";
            const string locus = "[FindFilePathsWithFilenamesThatMatchAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                #region null checks

                if (string.IsNullOrWhiteSpace(searchPatternForFileName))
                    return []; // no point bothering with anything further


                var searchPatternWithoutWildCards = searchPatternForFileName.Replace('?', 'x').Replace('*', 'x');
                // neutralise any wild card characters because the validator regards them as forbidden 

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(searchPatternWithoutWildCards))
                    return []; // no point bothering with anything further

                #endregion

                List<string> filePaths;

                // Get the root and file portions of the search string

                // because these are not atomic operations we should lock them. but they are async, so we can't easily. read more here https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx

                using (await mutex.LockAsync())
                {
                    using var store = IsolatedStorageFile.GetUserStoreForAssembly();

                    filePaths = [..store.GetFileNames(searchPatternForFileName)];

                    // Loop through the subdirectories, collect matches, and make separators consistent

                    foreach (var directory in await FindDirectoryPathsThatMatchAsync("*"))
                        foreach (var file in store.GetFileNames(Path.Combine(directory, searchPatternForFileName)))
                            filePaths.Add(Path.Combine(directory, file));

                    store.Close();
                }

                return filePaths.ToArray();
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The searchPattern in question was <{searchPatternForFileName}>.";

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
        public Task<bool> RemoveStoreAsync()
        {
            const string failure = "Unable to delete local storage store and all its contents.";
            const string locus = "[RemoveStoreAsync]";

            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    store.Remove();
                }

                return Task.FromResult(true);
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

        #region helpers

        private static List<string> GetFilePaths(string directoryPath, IsolatedStorageFile store)
        {
            if (!store.DirectoryExists(directoryPath)) return [];

            var searchPattern = JghPath.CombineNoChecks(directoryPath, "*");

            var fileNames = store.GetFileNames(searchPattern);

            return fileNames.Select(fileName => Path.Combine(directoryPath, fileName)).ToList();
        }

        private static string[] GetDirectoryPaths(string directoryPath, IsolatedStorageFile store)
        {
            if (!store.DirectoryExists(directoryPath)) return [];

            #region first of all, enumerate first-level directories in this folder

            var searchPattern = JghPath.CombineNoChecks(directoryPath, "*");

            var directoryNames = store.GetDirectoryNames(searchPattern);

            var directoryPaths = directoryNames.Select(directoryName => Path.Combine(directoryPath, directoryName)).OrderBy(z=> z).ToList();

            #endregion

            #region recursively drill down and repeat for all subsequent-level subdirectories

            foreach (var directoryName in directoryNames)
            {
                var subDirectoryPath = Path.Combine(directoryPath, directoryName);

                var moreNames = GetDirectories(JghPath.CombineNoChecks(subDirectoryPath, "*"), store);

                foreach (var name in moreNames)
                {
                    directoryPaths.Add(Path.Combine(subDirectoryPath, name));
                }
            }

            #endregion

            var answer = directoryPaths.ToArray();

            return answer;
        }

        private static List<string> GetDirectories(string pattern, IsolatedStorageFile store)
        {
            // Get the root of the search string.
            string root = Path.GetDirectoryName(pattern);

            if (root != "")
            {
                root += Path.DirectorySeparatorChar.ToString();
                //root += _directorySeparatorCharAsString;
            }

            // Retrieve directories.
            var directoryList = new List<string>(store.GetDirectoryNames(pattern).OrderBy(z=>z));

            // Retrieve subdirectories of matches.
            for (int i = 0, max = directoryList.Count; i < max; i++)
            {
                var directory = directoryList[i] + Path.DirectorySeparatorChar.ToString();
                //var directory = directoryList[i] + _directorySeparatorCharAsString;

                var more = GetDirectories(root + directory + "*", store);

                // For each sub-directory found, add in the base path.
                for (int j = 0; j < more.Count; j++)
                {
                    more[j] = directory + more[j];
                }

                // Insert the subdirectories into the list and
                // update the counter and upper bound.
                directoryList.InsertRange(i + 1, more);

                i += more.Count;

                max += more.Count;
            }

            return directoryList;
        }

        private bool IsValidFullyQualifiedDirectoryPath(string directoryPath)
        {
            return JghFilePathValidator.IsValidNtfsDirectoryPath(directoryPath, true);

            //if (!string.IsNullOrWhiteSpace(folder))
            //    if (!JghFilePathValidator.IsValidNtfsDirectoryPath(folder, true))
            //        return false;

            //return true;
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

    }
}