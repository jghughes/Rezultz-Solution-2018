using System;
using System.Linq;
using System.Threading.Tasks;
using Jgh.ConnectionStrings.Mar2024;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;

namespace RezultzSvc.Library01.Mar2024
{
    public static class ConnectionStringRepository
    {
        private const string Locus2 = nameof(ConnectionStringRepository);
        private const string Locus3 = "[RezultzSvc.Library01.Mar2024]";

        #region methods

        public static AzureStorageLocationDto GetStorageHierarchyEntryPoint()
        {
            // For better security and flexibility this data should not be hard coded.
            // It should be stored in a secure key vault so that no recompilation is required when accounts and/or passwords change. 

            var answer = new AzureStorageLocationDto
            {
                AzureStorageAccountName = AzureStorageConnectionStrings.AzureStorageAccountOfEntryPointToRezultzDataStorageHierarchy,
                AzureStorageContainerName = AzureStorageConnectionStrings.AzureStorageContainerForSeasonProfiles
            };

            return answer;
        }

        /// <summary>
        /// Searches for the full connection string for the provided account name, null if the account is not on file.  
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns>Full connection string or string.Empty if the account is not on file</returns>
        public static async Task<string> GetAzureStorageAccountConnectionStringAsync(string accountName)
        {
            const string failure = "Unable to obtain storage account connection string.";
            const string locus = "[GetAzureStorageAccountConnectionStringAsync]";

            try
            {
                if (accountName is null)
                    throw new ArgumentNullException(nameof(accountName));

                var allAzureStorageConnectionStrings = await GetConnectionStringsOfAllRecognisedAccountsInDatabaseAsync();

                var connectionString =
                    allAzureStorageConnectionStrings.Where(z => z is not null)
                        .FirstOrDefault(z => z.Contains($"AccountName={accountName};"));


                return string.IsNullOrWhiteSpace(connectionString) ? null : connectionString;
                // Note: by returning empty, all WCF calls will fail in WcfServices2014.AzureStorageSvc.svc because the svc is programmed to require valid security keys, whether for public or private blobs
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

        }

        #endregion

        /// <summary>
        /// Fetches the connection strings from a named Secret stored in JGH's AzureKeyVault. The secret contains a JSON array of confidential account particulars
        /// </summary>
        /// <returns></returns>
        private static async Task<string[]> GetConnectionStringsOfAllRecognisedAccountsInDatabaseAsync()
        {
            var answer = await AzureStorageConnectionStrings.GetAzureStorageAccountConnectionStringsForRezultzAsync();

            return answer;
        }


    }


}