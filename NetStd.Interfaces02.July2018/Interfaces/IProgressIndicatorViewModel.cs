namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IProgressIndicatorViewModel
    {
        string ProgressMessage { get; set; }

        string ProgressMessageOrWhiteOut { get; set; }

        bool IsBusy { get; }

        bool IsVisible { get; set; }

        //bool SpinnerIsVisible { get; set; }

        //void StartProgressRing();

        //void StopProgressRing();

        //void OpenProgressIndicator();

        void OpenProgressIndicator(string descriptionOfWhatsHappening);

        void FreezeProgressIndicator();

        void CloseProgressIndicator();
    }
}