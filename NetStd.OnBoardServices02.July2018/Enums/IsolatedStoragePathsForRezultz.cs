/*
 * Very important note. For Unix and Xamarin the directory path separator is a forward slash (/) only.
 * In UWP it is a backward slash(\) only. For WPF it is either. For universally reliable code, never hard-code your directory separators.
 * To insert the correct separator for the platform at runtime, use Path.DirectorySeparatorChar.
 * For static constants such as these, therefore, you do not have the option to use a path separator
 */

namespace NetStd.OnBoardServices02.July2018.Enums
{
    public static class IsolatedStoragePathsForRezultz
    {
        public const string NameOfFolderForPersistedFiles = "PersistedFilesFolder";
        public const string NameOfFolderForUserSettings = "UserSettingsFolder";
    }
}