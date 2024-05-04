using System;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class SocialMediaConnectionsViewModel : BindableBase
    {
        #region ctor

        public SocialMediaConnectionsViewModel()
        {
            IsVisible = true;

            LaunchEmailButtonVm = new ButtonControlViewModel(ExecuteNothing, CannotExecute);
            NavigateToFacebookHyperlinkButtonVm = new ButtonWithHyperlinkControlViewModel(ExecuteNothing, CannotExecute);
            NavigateToInstagramHyperlinkButtonVm = new ButtonWithHyperlinkControlViewModel(ExecuteNothing, CannotExecute);
            NavigateToTwitterHyperlinkButtonVm = new ButtonWithHyperlinkControlViewModel(ExecuteNothing, CannotExecute);
        }

        #endregion

        #region properties

        public ButtonControlViewModel LaunchEmailButtonVm{get;}

        public ButtonWithHyperlinkControlViewModel NavigateToFacebookHyperlinkButtonVm {get;}

        public ButtonWithHyperlinkControlViewModel NavigateToInstagramHyperlinkButtonVm {get;}

        public ButtonWithHyperlinkControlViewModel NavigateToTwitterHyperlinkButtonVm {get;}

        #region IsVisible

        private bool _backingstoreIsVisible;

        //[DataMember]
        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #endregion


        #region methods

        public void Zeroise()
        {
            IsVisible = true;

            LaunchEmailButtonVm.Zeroise();
            NavigateToFacebookHyperlinkButtonVm.Zeroise();
            NavigateToInstagramHyperlinkButtonVm.Zeroise();
            NavigateToTwitterHyperlinkButtonVm.Zeroise();

        }



        public void PopulateConnections(string email, string facebookUri, string instagramUri, string twitterUri)
        {
            IsVisible = true;

            LaunchEmailButtonVm.Tag = string.IsNullOrWhiteSpace(email) ? "placeholder" : email;  // todo
            NavigateToFacebookHyperlinkButtonVm.NavigateUri = new Uri( string.IsNullOrWhiteSpace(facebookUri) ? "https://placeholder" : facebookUri);
            NavigateToInstagramHyperlinkButtonVm.NavigateUri = new Uri( string.IsNullOrWhiteSpace(instagramUri) ? "https://placeholder" : instagramUri);
            NavigateToTwitterHyperlinkButtonVm.NavigateUri = new Uri( string.IsNullOrWhiteSpace(twitterUri) ? "https://placeholder" : twitterUri);
        }

        #endregion

        #region prism delegate command helpers

        private static void ExecuteNothing()
        {
            // do nothing
        }

        private static bool CannotExecute()
        {
            return false;
        }

        #endregion

    }
}