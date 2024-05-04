using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace NetStd.OnBoardServices02.July2018.UserSettingsForRezultz
{
    /// <summary>
    ///     This is the class you will almost always want to use for settings saved on the client-side. It is intended for
    ///     ingesting a platform-specific implementation
    ///     of ISettingsService for platforms such as UWP or Xamarin which come packaged with inbuilt classes for saving user
    ///     settings. If you are on
    ///     a platform that only has local file storage or remote blob storage, then
    ///     ThingsPersistedInLocalStorageInLocalStorage is the class to use.
    ///     For Xamarin, settings are saved with Xamarin.Essentials.Preferences. For UWP, settings are saved with
    ///     Windows.Storage.ApplicationData.Current.LocalSettings
    /// </summary>
    public class ThingsPersistedInLocalStorage : IThingsPersistedInLocalStorage
    {
        #region fields

        private readonly ISettingsService _settingsStorageService;

        #endregion

        #region ctor

        public ThingsPersistedInLocalStorage(ISettingsService settingsStorageService)
        {
            _settingsStorageService = settingsStorageService;
        }

        #endregion

        #region props (stored in application settings storage)

        #region SeasonId

        private const string SeasonIdKey = "SeasonIdKey";

        private readonly int _defaultSeasonId = 0;

        public async Task<int> GetSeasonMetadataIdAsync()
        {
            var id = await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(SeasonIdKey, _defaultSeasonId);

            return id;
        }

        public async Task<bool> SaveSeasonMetadataIdAsync(int id)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(SeasonIdKey, id);
        }

        #endregion

        #region TimingDataPreProcessorId

        private const string TimingDataPreProcessorIdKey = "TimingDataPreProcessorIdKey";

        private const string DefaultTimingDataPreProcessorId = "";

        public async Task<string> GetTimingDataPreProcessorIdAsync()
        {
            return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(TimingDataPreProcessorIdKey, DefaultTimingDataPreProcessorId);
        }

        public async Task<bool> SaveTimingDataPreProcessorIdAsync(string converterCodeName)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(TimingDataPreProcessorIdKey, converterCodeName);
        }

        #endregion

        #region NavigationContextQueryString - an anomaly. Sync method essential because we access this setting in methods that do navigation calls on the UI thread, and those methods can't be or contain async code

        private const string NavigationContextQueryStringKey = "NavigationContextQueryStringKey";

        private readonly KeyValuePair<string, string>[] _defaultNavigationContextQueryString =
            new KeyValuePair<string, string>[0];

        public async Task<Dictionary<string, string>> GetNavigationContextQueryStringAsync()
        {
            // we can't store a Dictionary in settings because we can't deserialise a Dictionary. We have to use a KeyValuePair[] instead

            var kvpArrayRetrieved = await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(NavigationContextQueryStringKey, _defaultNavigationContextQueryString);

            var answer = kvpArrayRetrieved.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return answer;
        }

        public Dictionary<string, string> GetNavigationContextQueryString()
        {
            // we can't store a Dictionary in settings because in times of old when this class was written we couldn't deserialise a Dictionary. We had to use a KeyValuePair[] instead

            var kvpArrayRetrieved = _settingsStorageService.GetSettingOrFailingThatResortToDefault(NavigationContextQueryStringKey, _defaultNavigationContextQueryString);

            var answer = kvpArrayRetrieved.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return answer;
        }

        public async Task<bool> SaveNavigationContextQueryStringAsync(Dictionary<string, string> queryStringDictionary)
        {
            // we can't store a Dictionary in settings because we can't deserialise a Dictionary. We have to use a KeyValuePair[] instead

            var kvpArrayToBeStored = queryStringDictionary.Select(kvp => kvp).ToArray();

            return await _settingsStorageService.AddOrUpdateSettingAsync(NavigationContextQueryStringKey, kvpArrayToBeStored);
        }

        #endregion

        #region MustOpenListOfFavoritesOnLaunch

        //private const string MustOpenListOfFavoritesOnLaunchKey = "MustOpenListOfFavoritesOnLaunchKey";
        //private const bool DefaultMustOpenListOfFavoritesOnLaunch = false;

        //public async Task<bool> GetMustOpenListOfFavoritesOnLaunchAsync()
        //{
        //    return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(
        //        MustOpenListOfFavoritesOnLaunchKey, DefaultMustOpenListOfFavoritesOnLaunch);
        //}

        //public async Task<bool> SaveMustOpenListOfFavoritesOnLaunchAsync(bool valueToBeSaved)
        //{
        //    return await _settingsStorageService.AddOrUpdateSettingAsync(MustOpenListOfFavoritesOnLaunchKey,
        //        valueToBeSaved);
        //}

        #endregion

        #region MustSelectAllRacesOnFirstTimeThroughForAnEvent

        // when we use a datagrid to display results this must always be false and fixed. when we use a webpage, the user can be given the option to do whatever she likes

        private const string MustSelectAllRacesOnFirstTimeThroughForAnEventKey = "MustSelectAllRacesOnFirstTimeThroughForAnEventKey";
        private const bool DefaultMustSelectAllRacesOnFirstTimeThroughForAnEvent = false;

        public async Task<bool> GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync()
        {
            return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(
                MustSelectAllRacesOnFirstTimeThroughForAnEventKey, DefaultMustSelectAllRacesOnFirstTimeThroughForAnEvent);
        }

        public async Task<bool> SaveMustSelectAllRacesOnFirstTimeThroughForAnEventAsync(bool valueToBeSaved)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(MustSelectAllRacesOnFirstTimeThroughForAnEventKey,
                valueToBeSaved);
        }

        #endregion

        #region MustSelectCategoryOfResultsForSingleBibNumberOnLaunch

        private const string MustSelectOnePersonsCategoryOfResultsOnLaunchKey =
            "MustSelectOnePersonsCategoryOfResultsOnLaunchKey";

        private const bool DefaultMustSelectOnePersonsCategoryOfResultsOnLaunch = false;

        public async Task<bool> GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync()
        {
            return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(
                MustSelectOnePersonsCategoryOfResultsOnLaunchKey,
                DefaultMustSelectOnePersonsCategoryOfResultsOnLaunch);
        }

        public async Task<bool> SaveMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync(bool valueToBeSaved)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(
                MustSelectOnePersonsCategoryOfResultsOnLaunchKey, valueToBeSaved);
        }

        #endregion

        #region MustUsePreviewDataOnLaunch

        private const string MustUsePreviewDataOnLaunchKey =
            "MustUseStagingDataOnLaunchKey";

        private const bool DefaultMustUsePreviewDataOnLaunch = false;

        public async Task<bool> GetMustUsePreviewDataOnLaunchAsync()
        {
            return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(
                MustUsePreviewDataOnLaunchKey,
                DefaultMustUsePreviewDataOnLaunch);
        }

        public async Task<bool> SaveMustUsePreviewDataOnLaunchAsync(bool valueToBeSaved)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(
                MustUsePreviewDataOnLaunchKey, valueToBeSaved);
        }

        #endregion

        #region MustDisplayConciseLeaderboardColumnsOnly

        private const string MustDisplayConciseLeaderboardColumnsOnlyKey =
            "MustDisplayConciseLeaderboardColumnsOnlyKey";

        private const bool DefaultMustDisplayConciseLeaderboardColumnsOnly = false;

        public async Task<bool> GetMustDisplayConciseLeaderboardColumnsOnlyAsync()
        {
            return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(
                MustDisplayConciseLeaderboardColumnsOnlyKey,
                DefaultMustDisplayConciseLeaderboardColumnsOnly);
        }

        public async Task<bool> SaveMustDisplayConciseLeaderboardColumnsOnlyAsync(bool valueToBeSaved)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(
                MustDisplayConciseLeaderboardColumnsOnlyKey, valueToBeSaved);
        }

        #endregion

        #region TargetBibNumberForSelectedCategoryOfResultsOnLaunch

        private const string TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey =
            "TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey";

        private const string DefaultTargetBibNumberForSelectedCategoryOfResultsOnLaunch = "";

        public async Task<string> GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync()
        {
            return await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(
                TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey,
                DefaultTargetBibNumberForSelectedCategoryOfResultsOnLaunch);
        }

        public async Task<bool> SaveTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync(string converterCodeName)
        {
            return await _settingsStorageService.AddOrUpdateSettingAsync(TargetBibNumberForSelectedCategoryOfResultsOnLaunchKey, converterCodeName);
        }

        #endregion

        #region FavoritesListIdentities

        private const string FavoritesListIdentitiesKey = "FavoritesListIdentitiesKey";

        private readonly ThingWithNamesItem[] _defaultFavoritesListIdentities = Array.Empty<ThingWithNamesItem>();

        public async Task<ThingWithNamesItem[]> GetFavoritesListIdentitiesAsync()
        {
            var kvpArrayRetrieved = await _settingsStorageService.GetSettingOrFailingThatResortToDefaultAsync(FavoritesListIdentitiesKey, _defaultFavoritesListIdentities);

            var answer = kvpArrayRetrieved.Where(z => z.IsValid()).ToArray();

            return answer;
        }

        public async Task<bool> SaveFavoritesListIdentitiesAsync(ThingWithNamesItem[] thingsWithNames)
        {
            var thingsToBeStored = thingsWithNames.Where(z => z.IsValid()).ToArray();

            var outcome = await _settingsStorageService.AddOrUpdateSettingAsync(FavoritesListIdentitiesKey, thingsToBeStored);

            return outcome;
        }

        #endregion


        public async Task<bool> ClearAllSettingsAsync()
        {
            return await _settingsStorageService.ClearAllSettingsAsync();
        }

        #endregion
    }
}