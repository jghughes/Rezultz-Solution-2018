using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;

// ReSharper disable RedundantNameQualifier


namespace NetStd.OnBoardServices01.July2018.Persistence
{

    /// <summary>
    ///     Provides old storage storage based on System.IO Namespace, founded deep under the hood
    ///     buried in the platform-specific/app-specific implementation of IStorageDirectoryPaths
    ///     you choose to use. Done right a suitably customised implementation can work for Xamarin (Android/iOS, and UWP) or WPF!
    ///     The methods are intrinsically sync, disguised as sync methods. 
    ///     
    ///     https://docs.microsoft.com/en-us/dotnet/api/system.environment.getfolderpath?view=netframework-4.8
    ///
    ///     For the trickery to lock non-atomic operations that are async, see; -
    ///     https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx
    /// </summary>
    public class SystemIoStorageService : LocalStorageServiceBase
    {
        private const string Locus2 = nameof(SystemIoStorageService);
        private const string Locus3 = "[NetStd.OnBoardServices01.July2018]";

        #region ctor

        public SystemIoStorageService(IStorageDirectoryPaths storageDirectoryPaths)
        {
	        _localStoragePath =
		        storageDirectoryPaths.GetFullyQualifiedDirectoryPathForThisStore();
        }

        #endregion

        #region fields

        private readonly string _localStoragePath;

        #endregion

        #region constants

        private const string StorageProblemMessage = "Problem occurred trying to conduct a local storage operation.";

        #endregion

        #region implementation of abstract methods in LocalStorageServiceBase - wrapper for static synchronous file handling methods of System.IO.File class

        /// <summary>
        ///     Creates a directory/folder in local storage if it doesn't already exist.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>The name of the path whether existing or created. Null if folder is invalid.</returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string> CreateDirectoryAsync(string folder)
        {
            const string failure = "Unable to create directory/folder in local storage.";
            const string locus = "[CreateDirectoryAsync]";

            var fullyQualifiedDirectoryPath = string.Empty;

            try
            {
	            fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

                #region null checks

                if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

                #endregion

                Directory.CreateDirectory(fullyQualifiedDirectoryPath);

                return await Task.FromResult(fullyQualifiedDirectoryPath);
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was [{fullyQualifiedDirectoryPath}].";

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

            var fullyQualifiedDirectoryPath = string.Empty;

            try
            {
	            fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return false;

	            #endregion

                var answer = Directory.Exists(fullyQualifiedDirectoryPath);

                return await Task.FromResult(answer);
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was [{fullyQualifiedDirectoryPath}].";

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

            var deletedThings = new List<string>();

            var mutex = new JghAsyncLock();

            string fullyQualifiedDirectoryPath = string.Empty;

            try
            {

	            fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

	            #endregion

                string[] answer;

                using (await mutex.LockAsync())
                {
                    if (!Directory.Exists(fullyQualifiedDirectoryPath))
                        return null;
                    // method is intended to return null if not found. (Note. Directory.GetFiles() throws a DirectoryNotFoundException) 

                    #region first of all, delete all files in this folder

                    foreach (var filepathToBeDeleted in Directory.GetFiles(fullyQualifiedDirectoryPath))
                    {
                        File.Delete(filepathToBeDeleted);

                        deletedThings.Add(filepathToBeDeleted);

                    }

                    #endregion

                    #region recursively drill down and delete all the subfolders

                    foreach (var dirPath in Directory.GetDirectories(fullyQualifiedDirectoryPath))
                        deletedThings.AddRange(await DeleteDirectoryAsync(dirPath)); // recursive

                    #endregion

                    #region finally, now and only now that it's empty, delete this directory

                    Directory.Delete(fullyQualifiedDirectoryPath);

                    deletedThings.Add(fullyQualifiedDirectoryPath);

                    answer = deletedThings.OrderBy(z=> z).ToArray(); // do this in case order is not guaranteed above

                    #endregion
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was [{fullyQualifiedDirectoryPath}].";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public override async Task<string[]> EnumerateDirectoryPathsInDirectoryAsync(string folder)
        {
            const string failure = "Unable to enumerate sub-directories in directory/folder in local storage.";
            const string locus = "[EnumerateDirectoryPathsInDirectoryAsync]";

            var mutex = new JghAsyncLock();

            string fullyQualifiedDirectoryPath = string.Empty;

            try
            {
                fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

	            #endregion

                string[] answer;

                using (await mutex.LockAsync())
                {
                    if (!Directory.Exists(fullyQualifiedDirectoryPath))
                        return null;

                    var directoryPaths = new List<string>();

                    #region first of all, enumerate all directories in this folder

                    var directoriesInRoot = Directory.GetDirectories(fullyQualifiedDirectoryPath).OrderBy(z=>z).ToArray();

                    directoryPaths.AddRange(directoriesInRoot);

                    #endregion

                    #region recursively drill down into the subfolders of the directory

                    foreach (var directoryInRoot in directoriesInRoot)
                    {
                        var moreDirectoryPaths = (await EnumerateDirectoryPathsInDirectoryAsync(directoryInRoot));

                        if (moreDirectoryPaths != null) directoryPaths.AddRange(moreDirectoryPaths.OrderBy(z=>z));
                    }

                    #endregion

                    answer = directoryPaths.ToArray();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was [{fullyQualifiedDirectoryPath}].";

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
        ///     The file path of the file written. Null if directory/folder
        ///     path or fileName is invalid or textContents are null.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or some other failure occurs.
        /// </exception>
        public override async Task<string> WriteTextFileAsync(string folder, string fileName, string textContents)
        {
            const string failure = "Unable to save a text file in local storage.";
            const string locus = "[WriteTextFileAsync]";

            var fullyQualifiedFileName = string.Empty;

            var mutex = new JghAsyncLock();

            try
            {
	            var fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

                #region null checks

                if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

                if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
                    return null;

                #endregion

                fullyQualifiedFileName = Path.Combine(fullyQualifiedDirectoryPath, fileName);

                using (await mutex.LockAsync())
                {

                    if (!Directory.Exists(fullyQualifiedDirectoryPath))
                        Directory.CreateDirectory(fullyQualifiedDirectoryPath); // do this otherwise File.WriteAllText() throws DirectoryNotFoundException

                    if (!File.Exists(fullyQualifiedFileName))
                        File.Delete(fullyQualifiedFileName); 
                    // i strongly recommend this even though it seems superfluous

                    File.WriteAllText(fullyQualifiedFileName, textContents);
                }

                return await Task.FromResult(fullyQualifiedFileName);
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was [{fullyQualifiedFileName}].";

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

            var fullyQualifiedFileName = string.Empty;

            try
            {
	            var fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return false;

	            if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
		            return false;

	            #endregion

	            fullyQualifiedFileName = Path.Combine(fullyQualifiedDirectoryPath, fileName);

                var answer = File.Exists(fullyQualifiedFileName);

                return await Task.FromResult(answer);
            }

            #region try catch handling

            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was [{fullyQualifiedFileName}].";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Gets the size of the file, in bytes.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file. A file extension is optional. Null or whitespace is invalid, as are the
        ///     many characters that are illegal in paths. For purposes here, wildcards are illegal.
        /// </param>
        /// <returns>
        ///     The number of bytes if file is found. 0 if directory/folder Path or fileName is invalid or file is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<long> GetSizeOfFileAsync(string folder, string fileName)
        {
            const string failure = "Unable to get size of file.";
            const string locus = "[GetSizeOfFileAsync]";

            var mutex = new JghAsyncLock();

            var fullyQualifiedFileName = string.Empty;

            try
            {
	            var fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return 0;

	            if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
		            return 0;

	            #endregion

	            fullyQualifiedFileName = Path.Combine(fullyQualifiedDirectoryPath, fileName);

                long answer;

                using (await mutex.LockAsync())
                {
                    #region existence checks

                    if (!Directory.Exists(fullyQualifiedDirectoryPath))
                        return 0;

                    if (!File.Exists(fullyQualifiedFileName))
                        return 0;

                    #endregion

                    var fileInfo = new FileInfo(fullyQualifiedFileName);

                    answer = fileInfo.Length;
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was [{fullyQualifiedFileName}].";

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

            var mutex = new JghAsyncLock();

            var fullyQualifiedFileName = string.Empty;

            try
            {
	            var fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

	            if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
		            return null;

	            #endregion

	            fullyQualifiedFileName = Path.Combine(fullyQualifiedDirectoryPath, fileName);

                string answer;

                #region existence checks

                if (!Directory.Exists(fullyQualifiedDirectoryPath))
                    return null;

                if (!File.Exists(fullyQualifiedFileName))
                    return null;

                #endregion

                using (await mutex.LockAsync())
                {
                    answer = File.ReadAllText(fullyQualifiedFileName);
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg = $"{failure} {StorageProblemMessage} The filePath in question was [{fullyQualifiedFileName}].";

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
        ///     The file path of the deletion. Null if directory/folder Path or fileName is invalid or file is not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string> DeleteFileAsync(string folder, string fileName)
        {
            const string failure = "Unable to delete file or files in local storage.";
            const string locus = "[DeleteFileAsync]";

            var mutex = new JghAsyncLock();

            var fullyQualifiedFileName = string.Empty;

            try
            {
	            var fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

	            if (!JghFilePathValidator.IsValidNtfsFileOrFolderName(fileName))
		            return null;

	            #endregion

	            fullyQualifiedFileName = Path.Combine(fullyQualifiedDirectoryPath, fileName);

                using (await mutex.LockAsync())
                {
                    if (!Directory.Exists(fullyQualifiedDirectoryPath))
                        return null;

                    if (!File.Exists(fullyQualifiedFileName))
                        return null;

                    File.Delete(fullyQualifiedFileName);
                }

                return await Task.FromResult(fullyQualifiedFileName);
            }

            #region try catch handling

            catch (FileNotFoundException)
            {
                return await Task.FromResult(string.Empty);
            }

            catch (DirectoryNotFoundException)
            {
                return await Task.FromResult(string.Empty);
            }

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} Problem occurred trying to conduct an local storage operation. The filePath in question was [{fullyQualifiedFileName}].";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Enumerates the files in a directory/folder including those in its subdirectories/subfolders.
        /// </summary>
        /// <param name="folder">
        ///     The folder.
        /// </param>
        /// <returns>
        ///     Collection of FilePaths. Null if directory/folder Path is
        ///     invalid or not found.
        /// </returns>
        /// <exception cref="JghCarrierException">
        ///     A storage operation fails or
        ///     some other failure occurs.
        /// </exception>
        public override async Task<string[]> EnumerateFilePathsInDirectoryAsync(string folder)
        {
            const string failure = "Unable to enumerate directory/folder in local storage.";
            const string locus = "[EnumerateFilePathsInDirectoryAsync]";

            var mutex = new JghAsyncLock();

            string fullyQualifiedDirectoryPath = string.Empty;

            try
            {
	            fullyQualifiedDirectoryPath = Path.Combine(_localStoragePath, folder);

	            #region null checks

	            if (!IsValidFullyQualifiedNtfsDirectoryPath(fullyQualifiedDirectoryPath)) return null;

	            #endregion

                string[] answer;

                using (await mutex.LockAsync())
                {
                    if (!Directory.Exists(fullyQualifiedDirectoryPath))
                        return null;

                    var filePaths = new List<string>();

                    #region first of all, enumerate all files in this folder

                    var files = Directory.GetFiles(fullyQualifiedDirectoryPath).OrderBy(z=>z);

                    filePaths.AddRange(files);

                    #endregion

                    #region recursively drill down into the subfolders of the directory

                    foreach (var afolder in Directory.GetDirectories(fullyQualifiedDirectoryPath).OrderBy(z=>z))
                    {
                        var moreFilePaths = await EnumerateFilePathsInDirectoryAsync(afolder);

                        if (moreFilePaths != null) filePaths.AddRange(moreFilePaths);
                    }

                    #endregion

                    answer = filePaths.ToArray();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var failureMsg =
                    $"{failure} {StorageProblemMessage} The directory/folder in question was [{fullyQualifiedDirectoryPath}].";

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

		#endregion

		#region helper

		private static bool IsValidFullyQualifiedNtfsDirectoryPath(string directoryPath)
		{
			return JghFilePathValidator.IsValidNtfsDirectoryPath(directoryPath, true);
		}

		#endregion

	}
}