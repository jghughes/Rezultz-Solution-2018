using System;
using System.Threading.Tasks;
using Windows.Storage;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;

namespace Jgh.Uwp.Common.July2018.OnBoardServices
{
    // 
    /// Settings are a convenient way of storing small pieces of configuration data
    /// for your application. Care should be taken to guard against an excessive volume of data being
    /// stored in settings.  Settings are not intended to be used as a database.
    /// Large data sets will take longer to load from disk during your application's launch.
    /// This helper utilises ApplicationData.Current.LocalSettings, where settings are stored in Win81.
    /// ApplicationData provides local, roaming, and temporary storage for app data. 
    /// In Win81, the item saved can be any kind of value type, or a string. 
    /// To play safe, our strategy here is to manually serialize every setting using JSON, thus saving every setting as a string
    ///
    public class SettingsServiceUwp : ISettingsService
    {
        private const string Locus2 = nameof(SettingsServiceUwp);
        private const string Locus3 = "[Jgh.Uwp.Common.July2018]";

        #region methods

        public IStorageDirectoryPaths StorageDirectoryPaths { get; set; } // irrelevant in this implementation for UWP

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
            var answer = AddOrUpdateSetting(key, value);

            return await Task.FromResult(answer);
        }


        private static bool AddOrUpdateSetting(string key, object value)
        {
            const string failure = "Unable to execute an insertion or update of a data-object into application data.";
            const string locus = "[AddOrUpdateSetting]";

            try
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key is null or empty. Unable to proceed.");
                if (value is null) throw new ArgumentException("Data-object is null. Insertion of null is forbidden. Unable to proceed.");

                #region serialise the object to Json

                var valueAsJsonString = JghSerialisation.ToJsonFromObject(value);
                //var valueAsJsonString = await JghSerialisation.ToJsonFromObjectAsync(value);

                #endregion

                #region insert the Json string if it has changed

                var localSettings = ApplicationData.Current.LocalSettings;

                var valueChanged = false;

                // If the key exists
                if (localSettings.Values.ContainsKey(key))
                {
                    // If the JsonString has changed
                    if ((string)localSettings.Values[key] != valueAsJsonString)
                    {
                        // update the new JsonString
                        localSettings.Values[key] = valueAsJsonString;
                        valueChanged = true;
                    }
                }
                // Otherwise add the new the key/JsonString
                else
                {
                    localSettings.Values[key] = valueAsJsonString;
                    valueChanged = true;
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
            const string locus = "[GetSettingOrFailingThatResortToDefault]";

            try
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must be a non-null, non-empty string.");

                // If the key exists and the value is of the expected type, retrieve the value, or else resort to the default setting

                if (!ValidSettingDoesExist<string>(key))
                    return defaultValue;

                var settingAsString = GetSetting<string>(key);

                T answer;

                try
                {
                    answer = JghSerialisation.ToObjectFromJson<T>(settingAsString); // blows up if there is something unexpected in there

                }
                catch (Exception)
                {
                    AddOrUpdateSetting(key, defaultValue); // clear out the rubbish for the future

                    return defaultValue;
                }

                return await Task.FromResult(answer);
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
            const string failure = "Unable to execute a retrieval operation from application data.";
            const string locus = "[GetSettingOrFailingThatResortToDefault]";

            try
            {
                if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key must be a non-null, non-empty string.");

                // If the key exists and the value is of the expected type, retrieve the value, or else resort to the default setting

                if (!ValidSettingDoesExist<string>(key))
                    return defaultValue;

                var settingAsString = GetSetting<string>(key);

                T answer;

                try
                {
                    answer = JghSerialisation.ToObjectFromJson<T>(settingAsString); // blows up if there is something unexpected in there

                }
                catch (Exception)
                {
                    AddOrUpdateSetting(key, defaultValue); // clear out the rubbish for the future

                    return defaultValue;
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public async Task<bool> ClearAllSettingsAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            localSettings.Values.Clear();

            return await Task.FromResult(true);
        }

        #endregion

        #region helpers

        /// <summary>
        ///     Get the current value of a setting from the settings store.
        /// </summary>
        /// <typeparam name="T">the expected value type of the stored setting</typeparam>
        /// <param name="key">the key</param>
        /// <returns>value of the setting</returns>
        /// <exception cref="System.ArgumentException">Valid setting does not exist.</exception>
        private static T GetSetting<T>(string key)
        {
            if (ValidSettingDoesExist<T>(key))
            {
                var localSettings = ApplicationData.Current.LocalSettings;

                var answer = (T)localSettings.Values[key];

                return answer;
            }
            throw new InvalidOperationException("Invalid key/value combination for setting.");
        }

        /// <summary>
        ///     Confirm if a valid key/value exists in the settings store.
        /// </summary>
        /// <typeparam name="T">the expected value type of the stored setting</typeparam>
        /// <param name="key">the key</param>
        /// <returns>false if the key is invalid or doesn't exist or if the value type of the setting is invalid, else true</returns>
        private static bool ValidSettingDoesExist<T>(string key)
        {
            const string failure = "Unable to determine if a valid setting exists in the store of application data.";
            const string locus = "[ValidSettingDoesExist]";

            try
            {
                if (string.IsNullOrEmpty(key)) return false;

                var localSettings = ApplicationData.Current.LocalSettings;

                if (!localSettings.Values.ContainsKey(key)) return false;

                try
                {
                    // ReSharper disable once UnusedVariable
                    var outcomeOfTrialCast = (T)localSettings.Values[key];
                }
                catch (Exception)
                {
                    // if we get here, it's because the cast threw an exception i.e. the type of object we were expecting is not the type we found.
                    return false;
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

        #endregion
    }
}