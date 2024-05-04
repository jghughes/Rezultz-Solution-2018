using System.Collections.Generic;
using System.Threading.Tasks;

/* this class cut and pasted from SmartHotel360 Xamarin/.Net Core reference project by Microsoft on GitHub
 Many classes are cut and pasted from microsoft's reference sample SmartHotel360. Repo is on GitHub at: - 
 https://github.com/Microsoft/SmartHotel360-Mobile For boiler plating and infrastructure, go to : -
https://github.com/Microsoft/SmartHotel360-Mobile/tree/master/Source/SmartHotel.Clients/SmartHotel.Clients
For file handling, also read https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/files?tabs=windows
 */

namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IDialogServiceAllanRitchie
    {
        Task ShowAlertAsync(string message, string title, string okLabel);

        void ShowToast(string message, int duration = 5000);

        Task<bool> ShowConfirmAsync(string message, string title, string okLabel, string cancelLabel);

        Task<string> SelectActionAsync(string message, string title, IEnumerable<string> options);

        Task<string> SelectActionAsync(string message, string title, string cancelLabel, IEnumerable<string> options);
    }
}
