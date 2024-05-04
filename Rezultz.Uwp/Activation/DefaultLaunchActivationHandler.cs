﻿using NetStd.ServiceLocation.Aug2022;
using Rezultz.Uwp.In_app_services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Rezultz.Uwp.Activation
{
    internal class DefaultLaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly string _navElement;

        private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        //return CommonServiceLocator.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
        public DefaultLaunchActivationHandler(Type navElement)
        {
            _navElement = navElement.FullName;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            // When the navigation stack isn't restored, navigate to the first page and configure
            // the new page by passing required information in the navigation parameter
            NavigationService.Navigate(_navElement, args.Arguments);

            // TODO WTS: Remove or change this sample which shows a toast notification when the app is launched.
            // You can use this sample to create toast notifications where needed in your app.
            // Remove it here if you are showing a First Run dialog because it is not premitted to open more than one content dialog at a time
            //Singleton<ToastNotificationsService>.Instance.ShowToastNotificationSample();

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return NavigationService.Frame.Content == null;
        }
    }
}
