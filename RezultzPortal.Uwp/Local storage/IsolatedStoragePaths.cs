using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.OnBoardServices02.July2018.Enums;

/*
 * Very important note. For Unix and Xamarin the directory path separator is a forward slash (/) only.
 * In UWP it is a backward slash(\) only. For WPF it is either. For universally reliable code, never hard-code your directory separators.
 * To insert the correct separator for the platform at runtime, use Path.DirectorySeparatorChar, or Path.Combine.
 */

namespace RezultzPortal.Uwp.Local_storage
{
    /// <summary>
    ///     Provides app-specific local storage paths for in isolated storage.
    ///     Every app requires its own version of copy of this class
    /// </summary>
    public class IsolatedStoragePaths : IStorageDirectoryPaths
    {
        #region method

        public string GetFullyQualifiedDirectoryPathForThisStore()
        {
            return KindOfInventory; // the only thing needed to uniquely identify the purpose of this store (such as Files or Settings or Data or somesuch)
        }

        #endregion

        #region helpers

        //private static string GetPlatformIsolatedStorageDirectoryPath()
        //{
        //    return string.Empty; // superfluous. IsolatedStorageDirectoryPath is intrinsically baked into isolated storage
        //}

        #endregion

        #region props

        public string ThisAppName { get; } = @"RezultzPortal"; // redundant

        /// <summary>
        ///     This is intentionally a variable (OTHER THAN IN A TEST DATA ENVIRONMENT).
        ///     Name of different uses of LocalStorage in a project, typically for UserSettingsStorage
        ///     and FileStorage. For file storage, the default constant employed here
        ///     is IsolatedStoragePathsForRezultz.NameOfFolderForPersistedFiles
        /// </summary>
        public string KindOfInventory { get; set; } =
            IsolatedStoragePathsForRezultz.NameOfFolderForPersistedFiles; // default

        #endregion
    }
}
