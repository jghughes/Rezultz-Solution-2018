using System;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using Xamarin.Essentials;

namespace Jgh.Xamarin.Common.Jan2019.OnBoardServices
{
    /// <summary>
    ///     A Xamarin utility designed to add/change key/value pairs in a persisted store.
    ///     Key is a string and value is any serialisable object. The nature of underlying persistence
    ///     are the preferences/settings of the native platform, whatever those might be. This class
    ///     is based on Xamarin.Essentials.Preferences to interact with the native platform. 
    /// </summary>
    public class SettingsServiceXamarin : ISettingsService
    {
        private const string Locus2 = nameof(SettingsServiceXamarin);
        private const string Locus3 = "[Jgh.Xamarin.Common.Jan2019]";


        private static readonly object Padlock = new();

        #region methods

        public IStorageDirectoryPaths StorageDirectoryPaths { get; set; } // irrelevant in this implementation for Xamarin. not used

        /// <summary>
        ///     Update a setting for our application.
        ///     If the setting does not exist, then add the setting.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">any serialisable type</param>
        /// <returns>true if the write operation took place without fault, false if not</returns>
        /// <exception cref="ArgumentException">key is null or empty, or value is null.</exception>
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
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("Key is null or empty. Unable to proceed.");

                if (value is null)
                    throw new ArgumentException(
                        "Data-object is null. Insertion of null is forbidden. Unable to proceed.");

                #region serialise the object to Json

                var json = JghSerialisation.ToJsonFromObject(value);

                #endregion

                #region insert the Json string if the key is new or the setting has changed

                bool settingChanged;

                lock (Padlock)
                {
                    if (Preferences.ContainsKey(key))
                    {
                        // The key exists

                        if (Preferences.Get(key, string.Empty) != json)
                        {
                            // If the Json has changed, update the setting

                            Preferences.Set(key, json);

                            settingChanged = true;
                        }
                        else
                        {
                            // do nothing
                            settingChanged = false;
                        }
                    }
                    else
                    {
                        // The key is new. Add the new key/Json

                        Preferences.Set(key, json);

                        settingChanged = true;
                    }
                }

                return settingChanged;

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
        /// <exception cref="ArgumentException">Key is null or empty.</exception>
        public async Task<T> GetSettingOrFailingThatResortToDefaultAsync<T>(string key, T defaultValue)
        {
            var answer = GetSettingOrFailingThatResortToDefault(key, defaultValue);

            return await Task.FromResult(answer);
        }

        /// <summary>
        ///     Get the current value of a setting synchronously. For use on rare occasions, such as 
        ///     in containing methods that are prohibited from being Async Tasks and/or anywhere other than on the GUI thread
        /// </summary>
        /// <typeparam name="T">the expected type of the setting in storage</typeparam>
        /// <param name="key">the key</param>
        /// <param name="defaultValue">
        ///     the default to be used if the setting is not found or the type of the setting object found
        ///     in storage is unexpected
        /// </param>
        /// <returns>the value of the setting or the failing that the default</returns>
        /// <exception cref="ArgumentException">Key is null or empty.</exception>
        public T GetSettingOrFailingThatResortToDefault<T>(string key, T defaultValue)
        {
            const string failure = "Unable to execute a retrieval operation from application data.";
            const string locus = "[GetSettingOrFailingThatResortToDefault]";

            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("Key must be a non-null, non-empty string.");

                string settingAsString;

                lock (Padlock)
                {
                    // if key not found, return the default value 

                    if (!Preferences.ContainsKey(key))
                        return defaultValue;

                    // key exists, retrieve the value (which in my implementation is always a string) 

                    try
                    {
                        // empirically i have learned that this can throw unexpectedly in edge cases
                        settingAsString = Preferences.Get(key, string.Empty);

                    }
                    catch (Exception)
                    {
                        settingAsString = string.Empty;
                    }

                }

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

        public Task<bool> ClearAllSettingsAsync()
        {
            Preferences.Clear();

            return Task.FromResult(true);
        }

        #endregion

    }
}