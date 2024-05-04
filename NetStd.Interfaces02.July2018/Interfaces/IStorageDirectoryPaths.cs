namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IStorageDirectoryPaths
    {
        string GetFullyQualifiedDirectoryPathForThisStore(); // intended to return a combination of platform and app-specific paths for KindOfInventory

        string KindOfInventory { get; set; } // intended to specify the kind of inventory e.g. Files or UserSettings 
    }
}