using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.OnBoardServices01.July2018.Settings
{
    /// <summary>
    ///     A platform-agnostic utility founded on file storage.
    ///     (Only async!! Therefore inadequate for Rezultz which requires a sync access to NavigationContextQuerySting setting)
    ///     It stores all settings in a simple JSON file saved in IsolatedStorage. This sole object in this file
    ///     is a serialised dictionary of string, string key/value pairs.
    ///     Key is a string and value is any serialisable object. The nature of underlying persistence
    ///     can be any form of storage that is amenable to the purposes of ILocalStorageService
    ///     and injectable by way of ctor injection. This class has limitations. For example, the
    ///     absence of a synchronous way of GetSettingOrFailingThatResortToDefault, which is essential in user classes.
    ///     And inability to do a proper type check in ValidSettingDoesExistAsync. Use of a platform specific settings service
    ///     is therefore recommended.
    /// </summary>
    /// <seealso cref="ISettingsService" />
    public class PartialSettingsServiceNetStd : ISettingsService
    {
        private const string Locus2 = nameof(PartialSettingsServiceNetStd);
        private const string Locus3 = "[NetStd.OnBoardServices01.July2018]";


        #region fields

        public IStorageDirectoryPaths StorageDirectoryPaths { get; set; } // relevant in this implementation for WPF because the implementation is built on top of the file system

        private readonly ILocalStorageService _localStorageService;

        private const string SettingsFileName = "UserSettingsFile";

        #endregion

        #region ctor
        
        public PartialSettingsServiceNetStd(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        #endregion

        #region methods


        /// <summary>
        ///     Update a setting for our application.
        ///     If the setting does not exist, then add the setting.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">any serialisable type</param>
        /// <returns>true if the write operation took place without fault, false if not</returns>
        /// <exception cref="System.ArgumentException">key is null or empty, or value is null.</exception>
        public async Task<bool> AddOrUpdateSettingAsync(string key, object value)
        {
            const string failure = "Unable to execute an insertion or update of a data-object into application data.";
            const string locus = "[AddOrUpdateSetting]";

            var mutex = new JghAsyncLock();

            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Key is null or empty. Unable to proceed.");

                if (value is null)
                    throw new ArgumentException(
                        "Object representing the value of a setting is null. Insertion of null into settings is forbidden. Unable to proceed.");

                #region insert the object

                var valueChanged = false;

                // because these are not atomic operations we should lock them. but they are async, so we can't easily. read more here https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx

                using (await mutex.LockAsync())
                {
                    var newSettingAsString = JghSerialisation.ToJsonFromObject(value);

                    var localSettings = await GetAllSettingsSafelyAsync();

                    // If the key exists
                    if (localSettings.ContainsKey(key))
                    {
                        // If the value has changed
                        if (localSettings[key] != newSettingAsString)
                        {
                            // update the new value
                            localSettings[key] = newSettingAsString;
                            valueChanged = true;
                        }
                    }
                    // Otherwise add the new the key/value
                    else
                    {
                        localSettings.Add(key, newSettingAsString);
                        valueChanged = true;
                    }

                    await SaveAllSettingsSafelyAsync(localSettings);
                }

                return valueChanged;

                #endregion
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Get the current value of a setting.
        /// </summary>
        /// <typeparam name="T">the expected type of the setting in storage</typeparam>
        /// <param name="key">the key</param>
        /// <param name="defaultValue">
        ///     the default to be used if the setting is not found or the type of the setting object found
        ///     in storage is unexpected
        /// </param>
        /// <returns>the value of the setting or the failing that the default</returns>
        /// <exception cref="System.ArgumentException">Key is null or empty.</exception>
        public async Task<T> GetSettingOrFailingThatResortToDefaultAsync<T>(string key, T defaultValue)
        {
            const string failure = "Unable to execute a retrieval operation from application data.";
            const string locus = "[GetSettingOrFailingThatResortToDefaultAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("Key must be a non-null, non-empty string.");

                // because these are not atomic operations we should lock them. but they are async, so we can't easily. read more here https://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx

                T value;

                using (await mutex.LockAsync())
                {
                    // If the key exists, retrieve the value, or else resort to the default value provided for T

                    if (await ValidSettingDoesExistAsync<T>(key))
                        value = await GetSettingAsync<T>(key);
                    else
                        value = defaultValue;
                }

                return value;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Get the current value of a setting synchronously. For use on rare occasions, such as
        ///     in containing methods that are prohibited from being Async Tasks
        ///     e.g. RezultzSingleEventLeaderboardPageViewModel.CboListOfMoreInfoOnSelectionChanged()
        /// </summary>
        /// <typeparam name="T">the expected type of the setting in storage</typeparam>
        /// <param name="key">the key</param>
        /// <param name="defaultValue">
        ///     the default to be used if the setting is not found or the type of the setting object found
        ///     in storage is unexpected
        /// </param>
        /// <returns>the value of the setting or the failing that the default</returns>
        /// <exception cref="System.ArgumentException">Key is null or empty.</exception>
        public T GetSettingOrFailingThatResortToDefault<T>(string key, T defaultValue)
        {
            throw new NotImplementedException(
                "public T GetSettingOrFailingThatResortToDefault<T>(string key, T defaultValue)");
        }

        public async Task<bool> ClearAllSettingsAsync()
        {
            // overwrite whatever JSON might be there. all the settings are in a single file

            await _localStorageService.DeleteFileAsync(GetFullyQualifiedDirectoryPathForThisStore(), SettingsFileName);

            return true;
        }

        #endregion

        #region helpers - the meat

        /// <summary>
        ///     Get the current value of a setting from the settings store.
        /// </summary>
        /// <typeparam name="T">the expected value type of the stored setting</typeparam>
        /// <param name="key">the key</param>
        /// <returns>value of the setting</returns>
        /// <exception cref="System.ArgumentException">Valid setting does not exist</exception>
        private async Task<T> GetSettingAsync<T>(string key)
        {
            var mutex = new JghAsyncLock();

            T answer;

            using (await mutex.LockAsync())
            {
                if (!await ValidSettingDoesExistAsync<T>(key))
                    throw new InvalidOperationException($"Specified key not found in setting. key = '{key}'");

                var localSettings = await GetAllSettingsSafelyAsync();

                var answerAsJson = localSettings[key];

                answer = JghSerialisation.ToObjectFromJson<T>(answerAsJson);
            }

            return answer;
        }

        /// <summary>
        ///     Confirm if a valid key/value already exists in the settings store.
        /// </summary>
        /// <typeparam name="T">the expected value type of the stored setting</typeparam>
        /// <param name="key">the key</param>
        /// <returns>false if the key isn't present, else true</returns>
        // ReSharper disable once UnusedTypeParameter
        private async Task<bool> ValidSettingDoesExistAsync<T>(string key)
        {
            const string failure = "Unable to determine if a valid setting exists in the store of application data.";
            const string locus = "[ValidSettingDoesExistAsync]";

            var mutex = new JghAsyncLock();

            try
            {
                if (string.IsNullOrWhiteSpace(key)) return false;

                using (await mutex.LockAsync())
                {
                    var localSettings = await GetAllSettingsSafelyAsync();

                    if (!localSettings.ContainsKey(key)) return false;

                    try
                    {
                        // unfortunately we have no way that i can think of to check whether the Json string
                        // purportedly representing the value of T in settings actually does represent a T.
                        // Unlike in Silverlight, a trial cast achieves the desired result. We have no option
                        // but to do nothing to verify the type. In other words, if the key exists, we are assuming
                        // that the corresponding setting is of type T

                        //var outcomeOfTrialCast = (T)localSettings[key];
                    }
                    catch (Exception)
                    {
                        // if we get here, it's because the cast threw an exception i.e. the type of object we were expecting is not the type we found.
                        return false;
                    }
                }

                return true;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private async Task<Dictionary<string, string>> GetAllSettingsSafelyAsync()
        {
            #region get the settings as a string Dictionary

            var json = await _localStorageService.ReadTextFileAsync(GetFullyQualifiedDirectoryPathForThisStore(), SettingsFileName);

            // ReadTextFileAsync() returns null if file not found, which is commonplace

            var localSettings =
                JghSerialisation.ToObjectFromJson<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();

            #endregion

            return localSettings;
        }

        /// <summary>
        ///     Saves settings safely. If an exception occurs it will be because
        ///     the space quota in IsolatedStorage has been exceeded. In which
        ///     case, the exception is caught and handled in the only way possible.
        ///     All settings are cleared.
        /// </summary>
        /// <returns>true if settings were saved, false if an exception occurred and all settings had to be deleted</returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task<bool> SaveAllSettingsSafelyAsync(Dictionary<string, string> localSettings)
        {
            const string failure = "Unable to save settings.";
            const string locus = "[SaveAllSettingsSafelyAsync]";


            try
            {
                if (localSettings is null) return false;

                var json = JghSerialisation.ToJsonFromObject(localSettings);

                await _localStorageService.WriteTextFileAsync(GetFullyQualifiedDirectoryPathForThisStore(), SettingsFileName, json);
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return true;
        }

        private string GetFullyQualifiedDirectoryPathForThisStore()
        {
            return StorageDirectoryPaths.GetFullyQualifiedDirectoryPathForThisStore();

        }
        #endregion
    }
}