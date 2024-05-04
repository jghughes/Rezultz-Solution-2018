using System.Threading.Tasks;

namespace NetStd.OnBoardServices02.July2018.FileStoreForRezultz
{
    // these methods are mere placeholders. they should come to mimic the kinds of methods in IThingsPersistedInLocalStorage by analogy

    public interface IFileServiceForRezultz
    {
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