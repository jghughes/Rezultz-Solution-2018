using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.Interfaces02.July2018.Interfaces_dummies
{
    public class SettingsServiceDummy : ISettingsService
    {
        public IStorageDirectoryPaths StorageDirectoryPaths { get; set; }

        public Task<bool> AddOrUpdateSettingAsync(string key, object value)
        {
            return Task.FromResult(false);
        }

        public Task<T> GetSettingOrFailingThatResortToDefaultAsync<T>(string key, T defaultValue)
        {
            return Task.FromResult(default (T));
        }

        public T GetSettingOrFailingThatResortToDefault<T>(string key, T defaultValue)
        {
            return default;
        }

        public Task<bool> ClearAllSettingsAsync()
        {
            return Task.FromResult(false);
        }
    }
}