using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IFileService
    {
        IStorageDirectoryPaths StorageDirectoryPaths { get; set; }

        Task<string> GetFullyQualifiedDirectoryPathForThisStoreAsync();
        Task<string[]> EnumerateFilePathsInStoreAsync();
        Task<bool> RemoveStoreAsync();

        Task<string> WriteTextFileAsync(string fileName, string textContents);
        Task<bool> FileExistsAsync(string fileName);
        Task<long> GetSizeOfFileAsync(string fileName);
        Task<string> ReadTextFileAsync(string fileName);
        Task<string> DeleteFileAsync(string fileName);

        Task<string> SaveSerialisableObjectAsync<T>(string fileName, T objectToSerialise);
        Task<T> ReadSerializedObjectAsync<T>(string fileName);
    }
}