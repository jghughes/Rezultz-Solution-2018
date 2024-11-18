using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Jgh.ConnectionStrings.Mar2024
{
    /// <summary>
    ///     This class is the one and only place where all Azure storage connection strings are obtained and maintained (or
    ///     should be the one and only!). Connection strings are stored in my Azure Key Vault in the form of Secrets.
    ///     This here class retrieves the Secrets we want for Rezultz in particular, although it could easily be extended for other
    ///     apps. If new storage accounts are created, you need to add a secret for each one. If account security keys change or are rotated
    ///     you need to update the value of the secret by creating a new version of the secret in the key vault. Rezultz and Rezultz Portal
    ///     access storage through my service webapps - RezultzSvc.Library01.Mar2024 and RezultzSvc.Library02.Mar2024. Only they use the
    ///     connection string and therefore reference this class. The KeyVault and Secret names are settings in launchSettings.json.
    ///     These must be properly itemized separately for development and production, and separately for http and https.
    ///     Be sure that you have set them up everywhere. At the time of writing, my two WCF service apps and my two
    ///     MVC services apps need to set these values. For Debugging and testing the services, go into project
    ///     Properties>Debug>General>Open Debug Launch Profile>http (as well as https) and specify them there.
    ///     Double-check that you have done it right by inspecting the environmentalVariables section
    ///     in the launchSettings.json file. Microsoft's ingenious modern Identity/access security system automatically
    ///     cascades down the program environment during runtime, starting with the launchSettings.json file or appSettings.json
    ///     and uses the obtained Environment defaults as the fallback for secrets when the relevant app is in Debug. To be
    ///     able to access the key vault in debug mode, this instance of Visual Studio has to be certified and authenticated by
    ///     your Azure account. Somewhere along the line you will be prompted to download a fresh certificate if required.
    ///     In production, the Azure App Service will have the necessary permissions to access the key vault.
    ///     See https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?tabs=azure-cli
    ///     For testing in Tool.ManageAzureSecretKeyVault.Nov2024, the environmental variables are set in the launchSettings.json file.
    ///     The newed-up DefaultAzureCredential() object authenticates to key vault using the credentials of the local development user logged
    ///     into the Azure CLI. When the application is deployed to Azure, the same DefaultAzureCredential() code can automatically discover
    ///     and use a managed identity that is assigned to an App Service, Virtual Machine, or other services.
    /// </summary>
    public class AzureStorageConnectionStrings
    {

        #region props

        public const string AzureStorageAccountOfEntryPointToRezultzDataStorageHierarchy = @"systemrezultzlevel1";

        public const string AzureStorageContainerForSeasonProfiles = @"seasonprofiles";

        public const string AzureStorageContainerForPublishingModuleProfiles = @"publishingmoduleprofiles";

        public const string AzureStorageContainerForExamplesOfDatasetsUsedInPublishingProcess = @"examplesofdatasetsusedinpublishingprocess";

        #endregion

        #region variables

        private static List<string> _allAzureStorageAccountConnectionStrings = new List<string>();

        #endregion


        #region methods

        public static async Task<string[]> GetAzureStorageAccountConnectionStringsForRezultzAsync()
        {
            #region bale if we already got them

            if (_allAzureStorageAccountConnectionStrings.Count > 0) return _allAzureStorageAccountConnectionStrings.ToArray();

            #endregion

            #region dig out the necessary environmental variables - KeyVault name and Secret names. The secrets contain the connection strings for the Azure storage accounts

            var azureConnectionStringsKeyVaultName = Environment.GetEnvironmentVariable("KEYVAULT_01");

            var secretName01 = Environment.GetEnvironmentVariable("SECRET_01");
            var secretName02 = Environment.GetEnvironmentVariable("SECRET_02");
            var secretName03 = Environment.GetEnvironmentVariable("SECRET_03");
            var secretName04 = Environment.GetEnvironmentVariable("SECRET_04");

            if (string.IsNullOrEmpty(azureConnectionStringsKeyVaultName)) throw new InvalidOperationException("The environment variable [KEYVAULT_01] is not set.");
            if (string.IsNullOrWhiteSpace(secretName01)) throw new InvalidOperationException("The environment variable 'SECRET_01' is not set.");
            if (string.IsNullOrWhiteSpace(secretName02)) throw new InvalidOperationException("The environment variable 'SECRET_02' is not set.");
            if (string.IsNullOrWhiteSpace(secretName03)) throw new InvalidOperationException("The environment variable 'SECRET_03' is not set.");
            if (string.IsNullOrWhiteSpace(secretName04)) throw new InvalidOperationException("The environment variable 'SECRET_04' is not set.");


            #endregion

            #region do work - fetch all the secret connection strings

            try
            {
                var keyVaultUri = $"https://{azureConnectionStringsKeyVaultName}.vault.azure.net/";

                var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

                var response01 = await client.GetSecretAsync(secretName01);
                var response02 = await client.GetSecretAsync(secretName02);
                var response03 = await client.GetSecretAsync(secretName03);
                var response04 = await client.GetSecretAsync(secretName04);

                _allAzureStorageAccountConnectionStrings = new List<string>
                {
                    response01.Value.Value,
                    response02.Value.Value,
                    response03.Value.Value,
                    response04.Value.Value
                };

                return _allAzureStorageAccountConnectionStrings.ToArray();
            }
            catch (CredentialUnavailableException)
            {
                throw;
            }
            catch (AuthenticationFailedException)
            {
                // host is not authenticated.In Debug because this Visual Studio account 'john.gerald.hughes@outlook.com' needs(re)authentication for this Visual Studio development environment.
                // Please go to Tools->Options->Azure Services Authentication, and re - authenticate the account you want to use.
                // for a full explanation https://learn.microsoft.com/en-us/dotnet/api/overview/azure/app-auth-migration?view=azure-dotnet
                // and https://learn.microsoft.com/en-us/entra/identity-platform/scopes-oidc#the-default-scope

                throw;
            }
            catch (Azure.RequestFailedException)
            {
                // you will end up here if any SecretName is invalid or not found

                throw;
            }
            catch (AggregateException)
            {
                // the named key vault could not be found. SecretClient.GetSecretAsync() automatically does several retries. If vault is not found, it will throw an agg exception.

                throw;
            }

            #endregion
        }

        #endregion


        #region archive

        //public static string[] GetAzureStorageAccountConnectionStrings()
        //{
        //    var azureStorageConnections = JsonConvert.DeserializeObject<AzureStorageAccountInfo[]>(SecretAzureStorageAccountConnectionStringsAsJson);

        //    var allAzureStorageAccountConnectionStrings = new List<string>();

        //    foreach (var item in azureStorageConnections)
        //    {
        //        var accountConnectionString =
        //            $"DefaultEndpointsProtocol={item.DefaultEndpointsProtocol};AccountName={item.AccountName};AccountKey={item.AccountKey};EndpointSuffix={item.EndpointSuffix};";

        //        allAzureStorageAccountConnectionStrings.Add(accountConnectionString);
        //    }

        //    return allAzureStorageAccountConnectionStrings.ToArray();

        //    //string[] allAzureStorageConnectionStrings =
        //    //            {
        //    //                "DefaultEndpointsProtocol=https;AccountName=systemrezultzlevel1;AccountKey=uMCoUvu9y3mlam/YSLhEXZ31NmZY4ErNI+sH74cG+rzLtG904ZXYE3RrqNBtMj9Ahn5myVXsOiUonAomlQMvUQ==;EndpointSuffix=core.windows.net;",
        //    //                "DefaultEndpointsProtocol=https;AccountName=coldstorageaccount;AccountKey=zMTIhP26G5P9+mjkj77dgVMhPDOoPF39diZYx16ey68jCmxeRICQobAty+ZOjUO/wgLUw8YkL4WDZH+YP5UL+w==;EndpointSuffix=core.windows.net;",
        //    //                "DefaultEndpointsProtocol=https;AccountName=customerkelso;AccountKey=J9qnP5vKR06CnwRT438DAF27dCJT1hYeSe5ZgThrn8hb7U92EAqTdJgK/E4ubQEZF/ynFa8wDyVRBRnWxEXTYA==;EndpointSuffix=core.windows.net;",
        //    //                "DefaultEndpointsProtocol=https;AccountName=customertester;AccountKey=X4F7GjmE1y+rH3cmXvHuO49XxDXB5Q1AMwa6KPiY+knOKwYu9OeBUbmtEw5zhDhyX6v1apr5nu8sJpOjJvUvRQ==;EndpointSuffix=core.windows.net;"
        //    //            };

        //    //return allAzureStorageConnectionStrings;
        //}

        #endregion

    }
}