using System.Collections.Generic;
using System.Linq;
using NetStd.Goodies.Mar2022;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class HeaderOrFooterViewModel : BindableBase
    {

        #region props

        public TextBlockControlViewModel Heading1TextVm { get; } = new();
        public TextBlockControlViewModel Heading2TextVm { get; } = new();
        public TextBlockControlViewModel Heading3TextVm { get; } = new();
        public TextBlockControlViewModel Heading4TextVm { get; } = new();
        public TextBlockControlViewModel Heading5TextVm { get; } = new();
        public TextBlockControlViewModel Heading6TextVm { get; } = new();
        public TextBlockControlViewModel Heading7TextVm { get; } = new();
        public TextBlockControlViewModel Heading8TextVm { get; } = new();


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

        private bool _backingstoreIsVisible = true;

        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #region IsEmpty

        // not intended to be used as a bindable INPC property
        public bool IsEmpty => Heading1TextVm.IsEmpty && Heading2TextVm.IsEmpty && Heading3TextVm.IsEmpty && Heading4TextVm.IsEmpty && Heading5TextVm.IsEmpty && Heading6TextVm.IsEmpty && Heading7TextVm.IsEmpty && Heading8TextVm.IsEmpty;

        #endregion

        #region methods

        public void Zeroise()
        {
            Text = string.Empty;

            Heading1TextVm.Zeroise();
            Heading2TextVm.Zeroise();
            Heading3TextVm.Zeroise();
            Heading4TextVm.Zeroise();
            Heading5TextVm.Zeroise();
            Heading6TextVm.Zeroise();
            Heading7TextVm.Zeroise();
            Heading8TextVm.Zeroise();
        }

        public bool Populate(params string[] headings)
        {
            Zeroise();

            if (headings is null) return true;

            var nonBlankTitles = new Dictionary<int, string>();

            var i = 1;

            foreach (var title in headings.Where(z => !string.IsNullOrWhiteSpace(z)))
            {
                nonBlankTitles[i] = title;
                i++;
            }

            if (nonBlankTitles.ContainsKey(1))
                Heading1TextVm.Initialise(nonBlankTitles[1]);

            if (nonBlankTitles.ContainsKey(2))
                Heading2TextVm.Initialise(nonBlankTitles[2]);

            if (nonBlankTitles.ContainsKey(3))
                Heading3TextVm.Initialise(nonBlankTitles[3]);

            if (nonBlankTitles.ContainsKey(4))
                Heading4TextVm.Initialise(nonBlankTitles[4]);

            if (nonBlankTitles.ContainsKey(5))
                Heading5TextVm.Initialise(nonBlankTitles[5]);

            if (nonBlankTitles.ContainsKey(6))
                Heading6TextVm.Initialise(nonBlankTitles[6]);

            if (nonBlankTitles.ContainsKey(7))
                Heading7TextVm.Initialise(nonBlankTitles[7]);

            if (nonBlankTitles.ContainsKey(8))
                Heading8TextVm.Initialise(nonBlankTitles[8]);

            Text = JghString.ConcatWithSeparator("  ", Heading1TextVm.Text, Heading2TextVm.Text, Heading3TextVm.Text, Heading4TextVm.Text, Heading5TextVm.Text, Heading6TextVm.Text, Heading7TextVm.Text, Heading8TextVm.Text);

            return true;
        }

        public void SaveAsLastKnownGood()
        {
            Heading1TextVm.SaveCurrentTextAsLastKnownGood();
            Heading2TextVm.SaveCurrentTextAsLastKnownGood();
            Heading3TextVm.SaveCurrentTextAsLastKnownGood();
            Heading4TextVm.SaveCurrentTextAsLastKnownGood();
            Heading5TextVm.SaveCurrentTextAsLastKnownGood();
            Heading6TextVm.SaveCurrentTextAsLastKnownGood();
            Heading7TextVm.SaveCurrentTextAsLastKnownGood();
            Heading8TextVm.SaveCurrentTextAsLastKnownGood();
        }

        public void RestoreToLastKnownGood()
        {
            Heading1TextVm.RestoreTextToLastKnownGood();
            Heading2TextVm.RestoreTextToLastKnownGood();
            Heading3TextVm.RestoreTextToLastKnownGood();
            Heading4TextVm.RestoreTextToLastKnownGood();
            Heading5TextVm.RestoreTextToLastKnownGood();
            Heading7TextVm.RestoreTextToLastKnownGood();
            Heading8TextVm.RestoreTextToLastKnownGood();
        }

        #endregion
    }
}