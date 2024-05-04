using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{

    public interface ILocalStorageService
    {
        Task<string> CreateDirectoryAsync(string folder);

        Task<bool> DirectoryExistsAsync(string folder);
        
        Task<string[]> DeleteDirectoryAsync(string folder);

        Task<string[]> EnumerateDirectoryPathsInDirectoryAsync(string folder);

        Task<string> WriteTextFileAsync(string folder, string fileName, string textContents);

        Task<bool> FileExistsAsync(string folder, string fileName);

        Task<long> GetSizeOfFileAsync(string folder, string fileName);

        Task<string> ReadTextFileAsync(string folder, string fileName);

        Task<string> DeleteFileAsync(string folder, string fileName);

        Task<string[]> EnumerateFilePathsInDirectoryAsync(string folder);

        Task<string> SaveSerialisableObjectAsync<T>(string folder, string fileName, T objectToSerialise);
        
        Task<T> ReadSerializedObjectAsync<T>(string folder, string fileName);
    }
}
