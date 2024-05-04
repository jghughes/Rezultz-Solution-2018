namespace NetStd.ViewModels01.April2022.Dialogs
{
    public interface IDialogUserControlViewModel
    {
        bool? DialogResult { get; set; }

        void InitialiseOkDialogue(string primaryMessage, string primaryMessageCaption, string secondaryMessage,
            string secondaryMessageCaption);

        void InitialiseOkCancelDialogue(string primaryMessage, string primaryMessageCaption, string secondaryMessage,
            string secondaryMessageCaption);
    }
}