namespace NetStd.Interfaces02.July2018.Interfaces
{
    /// <summary>
    ///     The design intent is that this interface will be implemented by page viewmodels
    ///     in a manner that does not block the UI. Each vm can implement the service
    ///     in its own way, or derive from a standard implementation in a base class.
    ///     The vm approach is safer than implementing the service by each page
    ///     because calling the service isn't confined to the UI thread in this way.
    ///     The risk of cross-threading erors is mitigated.
    /// </summary>
    public interface IProgressIndicatorService
    {
        //void StartProgressRing();
        //void StopProgressRing();
        void OpenProgressIndicator(string descriptionOfWhatsHappening);
        void FreezeProgressIndicator();
        void CloseProgressIndicator();
    }
}