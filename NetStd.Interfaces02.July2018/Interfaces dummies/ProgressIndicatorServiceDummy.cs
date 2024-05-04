using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.Interfaces02.July2018.Interfaces_dummies
{
    /// <summary>
    ///     Does nothing. this is just a dummy placeholder
    /// </summary>
    /// <seealso cref="IProgressIndicatorService" />
    public class ProgressIndicatorServiceDummy : IProgressIndicatorService
    {
        public void StartProgressRing()
        {
            // do nothing
        }

        public void StopProgressRing()
        {
            // do nothing
        }

        public void OpenProgressIndicator(string descriptionOfWhatsHappening)
        {
            // do nothing
        }

        public void FreezeProgressIndicator()
        {
            // do nothing
        }

        public void CloseProgressIndicator()
        {
            // do nothing
        }
    }
}