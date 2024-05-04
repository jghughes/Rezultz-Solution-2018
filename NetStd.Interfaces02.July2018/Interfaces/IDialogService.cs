using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    /// <summary>
    /// Interface for passing around references to types such as Page and Shell that are derived from Xamarin.Forms.ContentPage base class
    /// so that we can access their methods for displaying user alerts. Used in PageBase and AppShellBase.
    /// </summary>

    public interface IDialogService
    {
        Task DisplayAlertAsync(string title, string message, string okButtonLabel);

        Task<bool> DisplayAlertAsync(string title, string message, string acceptButtonLabel, string cancelButtonLabel);

        Task<string> DisplayActionSheetAsync(string title, string cancelLabel, string destruction,
            params string[] buttons);
    }
}