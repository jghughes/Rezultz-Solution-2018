﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using RezultzPortal.Uwp.Activation;
using RezultzPortal.Uwp.DependencyInjection;
using RezultzPortal.Uwp.Helpers;

namespace RezultzPortal.Uwp.In_app_services
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        public static readonly KeyboardAccelerator AltLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);

        public static readonly KeyboardAccelerator BackKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        private readonly App _app;
        private readonly Type _defaultNavItem;
        private readonly Lazy<UIElement> _shell;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources["DependencyInjectionLocator"] as DependencyInjectionLocator;

        private static NavigationServiceEx NavigationService => DependencyLocator.NavigationService;

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // BeInitialisedOrchestrateAsync things like registering background task before the app is loaded
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content is null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Window.Current.Content = _shell?.Value ?? new Frame();
                    NavigationService.NavigationFailed += (sender, e) => { throw e.Exception; };
                    NavigationService.Navigated += Frame_Navigated;
                    if (SystemNavigationManager.GetForCurrentView() is not null) SystemNavigationManager.GetForCurrentView().BackRequested += ActivationService_BackRequested;
                }
            }

            var activationHandler = GetActivationHandlers()
                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler is not null) await activationHandler.HandleAsync(activationArgs);

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs)) await defaultHandler.HandleAsync(activationArgs);

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            await ThemeSelectorService.InitializeAsync();
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();

            // TODO WTS: Configure and enable Azure Notification Hub integration.
            //  1. Go to the AzureNotificationsService class, in the InitializeAsync() method, provide the Hub Name and DefaultListenSharedAccessSignature.
            //  2. Uncomment the following line (an exception will be thrown if it is executed and the above information is not provided).
            // await Singleton<AzureNotificationsService>.Instance.InitializeAsync();

            //await Singleton<DevCenterNotificationsService>.Instance.InitializeAsync();
            //await FirstRunDisplayService.ShowIfAppropriateAsync();
            //await WhatsNewDisplayService.ShowIfAppropriateAsync();

            // Our Plan B for the WhatsNew information is to display it on the Settings Page alongside the accompanying
            // application version information, this in order to be consistent with the design approach in Rezultz.
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<DevCenterNotificationsService>.Instance;
            yield return Singleton<AzureNotificationsService>.Instance;
            yield return Singleton<ToastNotificationsService>.Instance;
            yield return Singleton<SuspendAndResumeService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationService.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator {Key = key};
            if (modifiers.HasValue) keyboardAccelerator.Modifiers = modifiers.Value;

            ToolTipService.SetToolTip(keyboardAccelerator, string.Empty);
            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }

        private void ActivationService_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var result = NavigationService.GoBack();
            e.Handled = result;
        }
    }
}
