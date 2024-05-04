using System;
using NetStd.DataTypes.Mar2024;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class ButtonWithHyperlinkControlViewModel : ButtonControlViewModel
    {
        #region ctor

        public ButtonWithHyperlinkControlViewModel(Action buttonOnClickExecuteAction,
            Func<bool> buttonOnClickCanExecuteFunc)
            : base(buttonOnClickExecuteAction, buttonOnClickCanExecuteFunc)
        {
            NavigateUri = new Uri(@"https://unknown/");
            TargetName = @"_blank";
        }

        #endregion

        #region props

        #region NavigateUri

        private Uri _backingstoreNavigateUri;

        public Uri NavigateUri
        {
            get => _backingstoreNavigateUri ??= new Uri(@"http://unknown/");
            set
            {
                SetProperty(ref _backingstoreNavigateUri, value);
                AbsoluteUriAsString = _backingstoreNavigateUri.AbsoluteUri;
            }
        }

        private string _backingstoreAbsoluteUriAsString;

        public string AbsoluteUriAsString
        {
            get => _backingstoreAbsoluteUriAsString ??= string.Empty;
            set => SetProperty(ref _backingstoreAbsoluteUriAsString, value);
        }

        #endregion

        #region TargetName

        private string _backingstoreTargetName;

        public string TargetName
        {
            get => _backingstoreTargetName ??= @"_blank";
            set => SetProperty(ref _backingstoreTargetName, value);
        }

        #endregion

        #endregion

        #region methods

        public void Populate(UriItem inputUriItem)
        {
            if (inputUriItem == null) inputUriItem = new UriItem();

            NavigateUri = new Uri(inputUriItem.ReferenceUriString ?? "https://unknown/");
            Content = inputUriItem.SourceUriString ?? string.Empty;
          
        }

        public new bool Zeroise()
        {
            base.Zeroise();

            NavigateUri = new Uri(@"https://unknown/");
            TargetName = @"_blank";
            Content = @"https://unknown/";
            return true;
        }

        #endregion

    }
}