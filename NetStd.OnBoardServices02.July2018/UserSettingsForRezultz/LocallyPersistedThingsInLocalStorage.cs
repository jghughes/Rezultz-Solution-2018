using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.OnBoardServices02.July2018.Enums;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace NetStd.OnBoardServices02.July2018.UserSettingsForRezultz
{
    /// <summary>
    ///    This is the class to use as a last resort on platforms that fail to provide dedicated classes for application settings.
    ///     WPF is one such platform. Server side platforms are other examples. The expectation here is that ISettingsService
    ///     will be platform specific and built on top of local or remote file or blob storage. ISettingsService needs to be told
    ///     what file path to use for saving settings, hence the ingestion of IStorageDirectoryPaths which needs to be  
    ///     implemented inside each app that uses this class. 
    /// </summary>
    public class ThingsPersistedInLocalStorageInLocalStorage : IThingsPersistedInLocalStorage
    {
        #region fields

        public readonly ISettingsService SettingsService;

        #endregion

        #region ctor

        public ThingsPersistedInLocalStorageInLocalStorage(ISettingsService settingsService, IStorageDirectoryPaths storageDirectoryPaths)
        {
            SettingsService = settingsService;

            SettingsService.StorageDirectoryPaths = storageDirectoryPaths;

            SettingsService.StorageDirectoryPaths.KindOfInventory =
                IsolatedStoragePathsForRezultz.NameOfFolderForUserSettings;
        }

        #endregion

        #region the settings

        #region SeasonProfileId

        private const string SeasonMetadataIdKey = "SeasonMetadataIdKey";

        private readonly int _defaultSeasonMetadataId = 0;

        public async Task<int> GetSeasonMetadataIdAsync()
        {
            var id = await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(SeasonMetadataIdKey, _defaultSeasonMetadataId);

            return id;
        }

        public async Task<bool> SaveSeasonMetadataIdAsync(int id)
        {
            return await SettingsService.AddOrUpdateSettingAsync(SeasonMetadataIdKey, id);
        }

        #endregion

        #region TimingDataPreprocessorCodeName

        private const string TimingDataPreprocessorCodeNameKey = "TimingDataPreprocessorCodeNameKey";

        private const string DefaultTimingDataPreprocessorCodeName = "";

        public async Task<string> GetTimingDataPreProcessorIdAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(TimingDataPreprocessorCodeNameKey, DefaultTimingDataPreprocessorCodeName);
        }

        public async Task<bool> SaveTimingDataPreProcessorIdAsync(string converterCodeName)
        {
            return await SettingsService.AddOrUpdateSettingAsync(TimingDataPreprocessorCodeNameKey, converterCodeName);
        }

        #endregion

        #region NavigationContextQueryString - an anomaly. Sync method essential because we access this setting in methods that do navigation calls on the UI thread, and those methods can't be or contain async code

        private const string NavigationContextQueryStringKey = "NavigationContextQueryStringKey";

        private readonly KeyValuePair<string, string>[] _defaultNavigationContextQueryString =
            [];

        public async Task<Dictionary<string, string>> GetNavigationContextQueryStringAsync()
        {
            // we can't store a Dictionary in settings because we can't deserialise a Dictionary. We have to use a KeyValuePair[] instead

            var kvpArrayRetrieved = await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(NavigationContextQueryStringKey, _defaultNavigationContextQueryString);

            var answer = kvpArrayRetrieved.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return answer;
        }

        public Dictionary<string, string> GetNavigationContextQueryString()
        {
            // we can't store a Dictionary in settings because in times of old when this class was written we couldn't deserialise a Dictionary. We had to use a KeyValuePair[] instead

            var kvpArrayRetrieved = SettingsService.GetSettingOrFailingThatResortToDefault(NavigationContextQueryStringKey, _defaultNavigationContextQueryString);

            var answer = kvpArrayRetrieved.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return answer;
        }

        public async Task<bool> SaveNavigationContextQueryStringAsync(Dictionary<string, string> queryStringDictionary)
        {
            // we can't store a Dictionary in settings because we can't deserialise a Dictionary. We have to use a KeyValuePair[] instead

            var kvpArrayToBeStored = queryStringDictionary.Select(kvp => kvp).ToArray();

            return await SettingsService.AddOrUpdateSettingAsync(NavigationContextQueryStringKey, kvpArrayToBeStored);
        }


        #endregion

        #region MustOpenListOfFavoritesOnLaunch

        private const string MustOpenListOfFavoritesOnLaunchKey = "MustOpenListOfFavoritesOnLaunchKey";
        private const bool DefaultMustOpenListOfFavoritesOnLaunch = false;

        public async Task<bool> GetMustOpenListOfFavoritesOnLaunchAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(
                MustOpenListOfFavoritesOnLaunchKey, DefaultMustOpenListOfFavoritesOnLaunch);
        }

        public async Task<bool> SaveMustOpenListOfFavoritesOnLaunchAsync(bool valueToBeSaved)
        {
            return await SettingsService.AddOrUpdateSettingAsync(MustOpenListOfFavoritesOnLaunchKey,
                valueToBeSaved);
        }

        #endregion

        #region MustSelectAllResultsOnFirstTimeThroughForAnEvent

        // when we use a datagrid to display results this must always be false and fixed. when we use a webpage, the user can be given the option to do whatever she likes

        private const string MustSelectAllRacesOnFirstTimeThroughForAnEventKey = "MustSelectAllRacesOnFirstTimeThroughForAnEventKey";
        private const bool DefaultMustSelectAllRacesOnFirstTimeThroughForAnEvent = false; 

        public async Task<bool> GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(
                MustSelectAllRacesOnFirstTimeThroughForAnEventKey, DefaultMustSelectAllRacesOnFirstTimeThroughForAnEvent);
        }

        public async Task<bool> SaveMustSelectAllRacesOnFirstTimeThroughForAnEventAsync(bool valueToBeSaved)
        {
            return await SettingsService.AddOrUpdateSettingAsync(MustSelectAllRacesOnFirstTimeThroughForAnEventKey,
                valueToBeSaved);
        }

        #endregion

        #region MustSelectCategoryOfResultsForSingleBibNumberOnLaunch

        private const string MustSelectOnePersonsCategoryOfResultsOnLaunchKey = "MustSelectOnePersonsCategoryOfResultsOnLaunchKey";
        private const bool DefaultMustSelectOnePersonsCategoryOfResultsOnLaunch = false;

        public async Task<bool> GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(
                MustSelectOnePersonsCategoryOfResultsOnLaunchKey,
                DefaultMustSelectOnePersonsCategoryOfResultsOnLaunch);
        }

        public async Task<bool> SaveMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync(bool valueToBeSaved)
        {
            return await SettingsService.AddOrUpdateSettingAsync(
                MustSelectOnePersonsCategoryOfResultsOnLaunchKey, valueToBeSaved);
        }

        #endregion

        #region MustUsePreviewDataOnLaunch

        private const string MustUsePreviewDataOnLaunchKey = "MustUsePreviewDataOnLaunchKey";
        private const bool DefaultMustUsePreviewDataOnLaunch = false;

        public async Task<bool> GetMustUsePreviewDataOnLaunchAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(
                MustUsePreviewDataOnLaunchKey,
                DefaultMustUsePreviewDataOnLaunch);
        }

        public async Task<bool> SaveMustUsePreviewDataOnLaunchAsync(bool valueToBeSaved)
        {
            return await SettingsService.AddOrUpdateSettingAsync(
                MustUsePreviewDataOnLaunchKey, valueToBeSaved);
        }

        #endregion

        #region MustDisplayConciseLeaderboardColumnsOnly

        private const string MustDisplayConciseLeaderboardColumnsOnlyKey = "MustDisplayConciseLeaderboardColumnsOnlyKey";
        private const bool DefaultMustDisplayConciseLeaderboardColumnsOnly = false;

        public async Task<bool> GetMustDisplayConciseLeaderboardColumnsOnlyAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(
                MustDisplayConciseLeaderboardColumnsOnlyKey,
                DefaultMustDisplayConciseLeaderboardColumnsOnly);
        }

        public async Task<bool> SaveMustDisplayConciseLeaderboardColumnsOnlyAsync(bool valueToBeSaved)
        {
            return await SettingsService.AddOrUpdateSettingAsync(
                MustDisplayConciseLeaderboardColumnsOnlyKey, valueToBeSaved);
        }

        #endregion

        #region TargetBibNumberForSelectedCategoryOfResultsOnLaunch

        private const string TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey =
            "TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey";

        private const string DefaultTargetBibNumberForSelectedCategoryOfResultsOnLaunch = "";

        public async Task<string> GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync()
        {
            return await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(
                TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey,
                DefaultTargetBibNumberForSelectedCategoryOfResultsOnLaunch);
        }

        public async Task<bool> SaveTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync(string converterCodeName)
        {
            return await SettingsService.AddOrUpdateSettingAsync(TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey, converterCodeName);
        }

        #endregion

        #region FavoritesListIdentities

        private const string FavoritesListIdentitiesKey = "FavoritesListIdentitiesKey";
        private readonly ThingWithNamesItem[] _defaultFavoritesListIdentities = [];

        public async Task<ThingWithNamesItem[]> GetFavoritesListIdentitiesAsync()
        {
            var kvpArrayRetrieved = await SettingsService.GetSettingOrFailingThatResortToDefaultAsync(FavoritesListIdentitiesKey, _defaultFavoritesListIdentities);

            var answer = kvpArrayRetrieved.Where(z => z.IsValid()).ToArray();

            return answer;
        }

        public async Task<bool> SaveFavoritesListIdentitiesAsync(ThingWithNamesItem[] thingsWithNames)
        {
            var thingsToBeStored = thingsWithNames.Select(z => z.IsValid()).ToArray();

            return await SettingsService.AddOrUpdateSettingAsync(FavoritesListIdentitiesKey, thingsToBeStored);
        }

        public Task<long> GetDateTimeUtcOfMostRecentKnownInsertionOfStopwatchDataAsync()
        {
	        throw new System.NotImplementedException();
        }

        public Task<bool> SaveDateTimeUtcOfMostRecentKnownInsertionOfStopwatchDataAsync(long utcNowAsBinary)
        {
	        throw new System.NotImplementedException();
        }

        public async Task<bool> ClearAllSettingsAsync()
        {
            return await SettingsService.ClearAllSettingsAsync();
        }

        #endregion

        #endregion

    }
}