using System;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces01.July2018.Objects;
using NetStd.Interfaces02.July2018.Interfaces;
using Rezultz.Library01.Mar2024.Repository_interfaces;

namespace Rezultz.Library01.Mar2024.Repositories
{
	/// <summary>
	/// Intended for speedy data insertion (random) and somewhat superior
	/// retrievals. Since all retrievals have to be in sort order, the general idea
	/// is to do some tricks to take a fast path (when you only need a shortlist of most recent entries) and
	/// avoid redundant sorts (when a previous sort is still valid).
	/// </summary>
	public class RepositoryOfHubStyleEntriesWithStorageBackup<T> : RepositoryOfHubStyleEntries<T>, IRepositoryOfHubStyleEntriesWithStorageBackup<T> where  T: class, IHubItem, IHasGuid, new()
	{
		#region ctor

		public RepositoryOfHubStyleEntriesWithStorageBackup(ILocalStorageService localStorageServiceInstance)
		{
			_storageService = localStorageServiceInstance ?? throw new JghNullObjectInstanceException(nameof(localStorageServiceInstance));
		}

		#endregion

		#region fields

		private readonly ILocalStorageService _storageService;

		#endregion

		#region methods

        public async Task<Tuple<int, int>> SaveMemoryCacheToLocalStorageBackupAsync(string folderName, string fileName)
        {

            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                return new Tuple<int, int>(0,0);
            
            var savedCount = FlagAllEntriesAsSaved(); // unfortunately we must do this prior to saving, which is a little presumptuous that the save succeeds

            await _storageService.SaveSerialisableObjectAsync(
                folderName,
                fileName,
                GetAllEntriesAsRawData());

            return savedCount;
        }

        public async Task<int> RestoreMemoryCacheFromLocalStorageBackupAsync(string folderName, string fileName)
        {
            var recoveredArray = await GetFromLocalStorageBackupAsync(folderName, fileName);

            ClearCache();

            var recoveredItemsThatAreMinimallyValid = recoveredArray.Where(z => !string.IsNullOrWhiteSpace(z.Guid)).Where(z => !string.IsNullOrWhiteSpace(z.OriginatingItemGuid));

            TryAddRangeNoDuplicates(recoveredItemsThatAreMinimallyValid, out var dummy); // hope and pray that something succeeded in getting into storage because this flies through without error

            ReorderRawEntriesByDescendingTimestamp();

            MakeDictionaryOfMostRecentItemForEachOriginatingItemGuid();

            return Count;
        }

        public async Task<T[]> GetFromLocalStorageBackupAsync(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                return [];

            var recoveredArray =
                await _storageService.ReadSerializedObjectAsync<T[]>(
                    folderName,
                    fileName) ?? [];

            return recoveredArray;
        }

        public async Task<bool> ClearLocalStorageBackupAsync(string folderName, string fileName)
        {
            // don't make the tempting mistake of introducing a read to see how many
            // things are in storage before overwriting it. The whole idea here is to expunge
            // local storage, including local storage that might have become corrupted, or contain
            // something other than a T[], and which will therefore be unreadable and cause the
            // read attempt to throw an exception.

            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                return true;

            await _storageService.SaveSerialisableObjectAsync(
                folderName,
                fileName,
                Array.Empty<T>());

            return true;
        }


        #endregion

    }
}
