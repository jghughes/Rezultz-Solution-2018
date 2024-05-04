using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.OnBoardServices02.July2018.Enums;

namespace NetStd.OnBoardServices02.July2018.FileStoreForRezultz
{
    /// <summary>
    ///     A service for managing files for Rezultz in on-board local storage. This class is platform agnostic, but it drags in IFileService and IStorageDirectoryPaths which might or might not be platform specific 
    /// </summary>
    public class FileServiceForRezultz : IFileServiceForRezultz
    {
        #region fields

        private readonly IFileService _fileService;

        #endregion

        #region ctor 

        public FileServiceForRezultz(IFileService fileService, IStorageDirectoryPaths storageDirectoryPaths)
        {
            _fileService = fileService;

            _fileService.StorageDirectoryPaths = storageDirectoryPaths;

            _fileService.StorageDirectoryPaths.KindOfInventory = IsolatedStoragePathsForRezultz.NameOfFolderForPersistedFiles;
        }

        #endregion

        #region methods 
        // this is where we provide file handling methods for custom named files used by the viewmodels in Rezultz - i.e. specified files for which names are hard-coded,
        //  analogous to what we have for ThingsPersistedInLocalStorageInLocalStorage. for now i'm just bringing in a subset of IFileService methods - as an placeholder


        public async Task<string> GetFullyQualifiedDirectoryPathForThisStoreAsync()
        {
            return await _fileService.GetFullyQualifiedDirectoryPathForThisStoreAsync();
        }

        public async Task<string[]> EnumerateFilePathsInStoreAsync()
        {
            var answer = await _fileService.EnumerateFilePathsInStoreAsync();

            return answer;
        }

        public async Task<bool> RemoveStoreAsync()
        {
            var answer = await _fileService.RemoveStoreAsync();

            return true;
        }

        public async Task<string> WriteTextFileAsync(string fileName, string textContents)
        {
            var answer = await _fileService.WriteTextFileAsync(fileName, textContents);

            return answer;
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            var answer = await _fileService.FileExistsAsync(fileName);

            return answer;
        }

        public async Task<long> GetSizeOfFileAsync(string fileName)
        {
            var answer = await _fileService.GetSizeOfFileAsync(fileName);

            return answer;
        }

        public async Task<string> ReadTextFileAsync(string fileName)
        {
            var answer = await _fileService.ReadTextFileAsync(fileName);

            return answer;
        }

        public async Task<string> DeleteFileAsync(string fileName)
        {
            var answer = await _fileService.DeleteFileAsync(fileName);

            return answer;
        }

        public async Task<string> SaveSerialisableObjectAsync<T>(string fileName, T objectToSerialise)
        {
            var answer = await  _fileService.SaveSerialisableObjectAsync(fileName, objectToSerialise);

            return answer;
        }

        public async Task<T> ReadSerializedObjectAsync<T>(string fileName)
        {
            var answer = await _fileService.ReadSerializedObjectAsync<T>(fileName);

            return answer;
        }

        #endregion
    }
}