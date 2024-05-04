using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.OnBoardServices01.July2018.Persistence
{
    /// <summary>
    ///     This class intended to save files in local or remote file or blob storage.
    ///     The nature of underlying persistence can be any form of storage that is
    ///     amenable to the purposes of ILocalStorageService. The implementation of ILocalStorageService
    ///     has to be platform specific obviously. It is ingested by means of ctor injection.
    /// </summary>
    /// <seealso cref="ILocalStorageService" />
    public class FileService : IFileService
    {
		#region fields

		public IStorageDirectoryPaths StorageDirectoryPaths { get; set; }

        private readonly ILocalStorageService _localStorageService;

        #endregion

        #region ctor

        public FileService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        #endregion

        #region methods

        public Task<string> GetFullyQualifiedDirectoryPathForThisStoreAsync()
        {
            return Task.FromResult(StorageDirectoryPaths.GetFullyQualifiedDirectoryPathForThisStore()); 
            // the StorageDirectoryPaths property must be set externally before the class is used, typically by
            // the classes in the stack that owns this class, all the way up to the Application/platform that uses
            // the local storage stack. See the class NetStd.RezultzServices.July2018.FileStoreServiceForRezultz for example.
            // And the class TestHarnessWPF.StorageDirectoryPaths. 
        }

        public async Task<string[]> EnumerateFilePathsInStoreAsync()
        {
            var answer = await _localStorageService.EnumerateFilePathsInDirectoryAsync(StorageDirectoryPaths.GetFullyQualifiedDirectoryPathForThisStore());

            return answer;
        }

        public async Task<bool> RemoveStoreAsync()
        {
            await _localStorageService.DeleteDirectoryAsync(StorageDirectoryPaths.GetFullyQualifiedDirectoryPathForThisStore());

            return true;
        }

        public async Task<string> WriteTextFileAsync(string fileName, string textContents)
        {
            var answer = await _localStorageService.WriteTextFileAsync(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName, textContents);

            return answer;
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            var answer = await _localStorageService.FileExistsAsync(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName);

            return answer;
        }

        public async Task<long> GetSizeOfFileAsync(string fileName)
        {
            var answer = await _localStorageService.GetSizeOfFileAsync(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName);

            return answer;
        }

        public async Task<string> ReadTextFileAsync(string fileName)
        {
            var answer = await _localStorageService.ReadTextFileAsync(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName);

            return answer;
        }

        public async Task<string> DeleteFileAsync(string fileName)
        {
            var answer = await _localStorageService.DeleteFileAsync(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName);

            return answer;
        }
        
        public async Task<string> SaveSerialisableObjectAsync<T>(string fileName, T objectToSerialise)
        {
            var answer = await  _localStorageService.SaveSerialisableObjectAsync(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName, objectToSerialise);

            return answer;
        }

        public async Task<T> ReadSerializedObjectAsync<T>(string fileName)
        {
            var answer = await _localStorageService.ReadSerializedObjectAsync<T>(await GetFullyQualifiedDirectoryPathForThisStoreAsync(), fileName);

            return answer;

        }
        
        #endregion

    }
}