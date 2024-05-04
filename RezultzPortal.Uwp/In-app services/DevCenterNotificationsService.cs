using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Microsoft.Services.Store.Engagement;
using RezultzPortal.Uwp.Activation;

namespace RezultzPortal.Uwp.In_app_services
{
    internal class DevCenterNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        public async Task InitializeAsync()
        {
            var engagementManager = StoreServicesEngagementManager.GetDefault();
            await engagementManager.RegisterNotificationChannelAsync();
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args)
        {
            var toastActivationArgs = args;

            var engagementManager = StoreServicesEngagementManager.GetDefault();
            var originalArgs = engagementManager.ParseArgumentsAndTrackAppLaunch(toastActivationArgs.Argument);

            //// Use the originalArgs variable to access the original arguments passed to the app.

            await Task.CompletedTask;
        }
    }
}
