using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class TextBlockControlViewModel : BindableBase
    {

        #region fields last known good

        private string _lastKnownGoodText;

        #endregion

        #region ctor

        public TextBlockControlViewModel()
        {
        }

        public TextBlockControlViewModel(string text)
        {
            Text = text;
        }

        public TextBlockControlViewModel(string label, string text)
        {
            Label = label;
            Text = text;
        }

        #endregion

        #region props
       
        #region Label

        private string _backingstoreLabel;

        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
        }

        #endregion

        #region Text
       
        private string _backingstoreText;

        public string Text
        {
            get => _backingstoreText ??= string.Empty;
            set => SetProperty(ref _backingstoreText, value);
        }

        #endregion

        #region IsVisible

        private bool _backingstoreIsVisible;

        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #region IsEmpty

        public bool IsEmpty => !string.IsNullOrWhiteSpace(Text);

        #endregion

        #endregion

        #region methods

        public void Append(string text)
        {
            if (text is null)
                return;

            var sb = new StringBuilder(Text);

            sb.Append(text);

            Text = sb.ToString();
        }

        public void AppendLine(string text)
        {
            if (text is null)
                return;

            var sb = new StringBuilder(Text);

            sb.AppendLine(text);

            Text = sb.ToString();
        }

        public void AppendLines(IEnumerable<string> linesOfText)
        {
            if (linesOfText is null)
                return;

            var textsAsArray = linesOfText.ToArray();

            var sb = new StringBuilder(Text);

            foreach (var lineOfText in textsAsArray)
            {
                if (lineOfText !=null)
                    sb.AppendLine(lineOfText);
            }

            Text = sb.ToString();
        }

        public bool Initialise(string text)
        {
            Label = string.Empty;

            Text = text ?? string.Empty;

            IsVisible = !string.IsNullOrWhiteSpace(text);

            _lastKnownGoodText = Text;

            return true;

        }

        public bool Zeroise()
        {
            Initialise(string.Empty);

            return true;
        }

        public void SaveCurrentTextAsLastKnownGood()
        {
            _lastKnownGoodText = Text;
        }

        public void RestoreTextToLastKnownGood()
        {
            Text = _lastKnownGoodText;
        }

        public override string ToString()
        {
            return Text;
        }


        #endregion

    }
}