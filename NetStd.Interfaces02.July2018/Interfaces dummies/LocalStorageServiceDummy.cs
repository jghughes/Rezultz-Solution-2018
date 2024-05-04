using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.Interfaces02.July2018.Interfaces_dummies
{
    public class LocalStorageServiceDummy : ILocalStorageService
    {

        public Task<string> CreateDirectoryAsync(string folder)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> DirectoryExistsAsync(string folder)
        {
            return Task.FromResult(false);
        }

        public Task<string[]> DeleteDirectoryAsync(string folder)
        {
            return Task.FromResult(new string[0]);
        }

        public Task<string[]> EnumerateDirectoryPathsInDirectoryAsync(string folder)
        {
            return Task.FromResult(new string[0]);
        }

        public Task<string> WriteTextFileAsync(string folder, string fileName, string textContents)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> FileExistsAsync(string folder, string fileName)
        {
            return Task.FromResult(false);
        }

        public Task<long> GetSizeOfFileAsync(string folder, string fileName)
        {
            return Task.FromResult(new long());
        }

        public Task<string> ReadTextFileAsync(string folder, string fileName)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<string> DeleteFileAsync(string folder, string fileName)
        {
            return Task.FromResult(string.Empty);
        }
 
        public Task<string[]> EnumerateFilePathsInDirectoryAsync(string folder)
        {
            return Task.FromResult(new string[0]);
        }

        public Task<string> SaveSerialisableObjectAsync<T>(string folder, string fileName, T objectToSerialise)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<T> ReadSerializedObjectAsync<T>(string folder, string fileName)
        {
            return Task.FromResult(default(T));
        }

    }
}