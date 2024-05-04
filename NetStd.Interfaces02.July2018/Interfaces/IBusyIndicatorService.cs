namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IBusyIndicatorService
    {
        void StartSpinner();

        void StopSpinner();

        void OpenBusyIndicator();

        void OpenBusyIndicator(string desriptionOfWhatsHappening);

        void OpenBusyIndicator(string titleOfBusyIndicatorUserControl, string desriptionOfWhatsHappening);

        void CollapseBusyIndicator();
    }
}