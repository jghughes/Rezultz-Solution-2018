using System;
using System.Threading.Tasks;
using NetStd.Interfaces01.July2018.Objects;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
	public interface IRepositoryOfHubStyleEntriesWithStorageBackup<T> : IRepositoryOfHubStyleEntries<T> where T : IHubItem, new()
	{
        abstract Task<Tuple<int, int>> SaveMemoryCacheToLocalStorageBackupAsync(string folderName, string fileName);

        abstract Task<int> RestoreMemoryCacheFromLocalStorageBackupAsync(string folderName, string fileName);

        abstract Task<T[]> GetFromLocalStorageBackupAsync(string folderName, string fileName);

        abstract Task<bool> ClearLocalStorageBackupAsync(string folderName, string fileName);

    }
}