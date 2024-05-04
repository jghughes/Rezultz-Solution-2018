using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface ISettingsService
    {
        #region props

        IStorageDirectoryPaths StorageDirectoryPaths { get ; set; }

        #endregion

        #region methods

        /// <summary>
        ///     Update a setting for our application.
        ///     If the setting does not exist, then add the setting.
        ///     To add or update a setting, just do so by setting it's property, never by using this method.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">any serialisable type</param>
        /// <returns>true if the write operation took place without fault, false if not</returns>
        /// <exception cref="System.ArgumentException">key is null or empty, or value is null.</exception>
        Task<bool> AddOrUpdateSettingAsync(string key, object value);

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
        Task<T> GetSettingOrFailingThatResortToDefaultAsync<T>(string key, T defaultValue);

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
        T GetSettingOrFailingThatResortToDefault<T>(string key, T defaultValue);

        /// <summary>
        ///     Clears all settings.
        /// </summary>
        /// <returns></returns>
        Task<bool> ClearAllSettingsAsync();

        #endregion
    }
}