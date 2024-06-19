using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace RezultzPortal.Uwp.In_app_services
{
    public class NavigationServiceEx
    {
        private const string Locus = "[RezultzPortal.Uwp.Services]";

        private readonly Dictionary<string, Type> _pages = new();

        private Frame _frame;
        private object _lastParamUsed;

        public Frame Frame
        {
            get
        {
            if (_frame is null)
            {
                _frame = Window.Current.Content as Frame;
                RegisterFrameEvents();
            }

            return _frame;
        }

            set
        {
            UnregisterFrameEvents();
            _frame = value;
            RegisterFrameEvents();
        }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public bool CanGoForward => Frame.CanGoForward;

        public event NavigatedEventHandler Navigated;

        public event NavigationFailedEventHandler NavigationFailed;

        public bool GoBack()
    {
        if (CanGoBack)
        {
            Frame.GoBack();
            return true;
        }

        return false;
    }

        public void GoForward()
    {
        Frame.GoForward();
    }

        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
    {
        Type page;

        lock (_pages)
        {
            if (!_pages.TryGetValue(pageKey, out page))
                throw new ArgumentException($"Error. Failed to navigate to XAML page in the class NavigationServiceEx. The name of the page is missing or incorrectly hardcoded in the Shell.NavigationView.MenuItems.\r\n" +
                                            $"Alternatively, the page key <{pageKey}> is not found in the page dictionary in the navigation service where it should be.\r\n" +
                                            $"Did you add it to the dictionary in the method DependencyInjectionLocator.ConfigureNavigationServiceExSingleton()?\r\n" +
                                            $"This method should have been properly called in the constructor of your DependencyInjectionLocator.\r\n" +
                                            $"This is where instantiation of the navigation service is meant to be done." +
                                            $"\r\n" +
                                            $"{Locus}");
        }

        if (Frame.Content?.GetType() != page || (parameter is not null && !parameter.Equals(_lastParamUsed)))
        {
            var navigationResult = Frame.Navigate(page, parameter, infoOverride);

            if (navigationResult) _lastParamUsed = parameter;

            return navigationResult;
        }

        return false;
    }

        public void Configure(string key, Type pageType)
    {
        lock (_pages)
        {
            if (_pages.ContainsKey(key))
                throw new ArgumentException($"Error. Mistake in configuring the XAML page navigation service.\r\n" +
                                            $"You are attempting to add pageKey <{key}> to the key list in the page dictionary in the navigation service.\r\n" +
                                            $"Duplicates are forbidden. Please investigate the method DependencyInjectionLocator.ConfigureNavigationServiceExSingleton()\r\n" +
                                            $"called in the constructor of your DependencyInjectionLocator where the page dictionary is instantiated.\r\n" +
                                            $"\r\n" +
                                            $"{Locus}");

            if (_pages.Any(p => p.Value == pageType))
                throw new ArgumentException($"Error. Mistake in configuring the XAML page navigation service.\r\n" +
                                            $"You are attempting to add page <{nameof(pageType)}> for a second time to the page list in the page dictionary in the navigation service.\r\n" +
                                            $"Duplicates are forbidden. Please investigate the method DependencyInjectionLocator.ConfigureNavigationServiceExSingleton()\r\n" +
                                            $"called in the constructor of your DependencyInjectionLocator where the page dictionary is instantiated.\r\n" +
                                            $"To help you find the first entry in the dictionary, the pageKey is <{_pages.First(p => p.Value == pageType).Key}>.\r\n" +
                                            $"\r\n" +
                                            $"{Locus}");

            _pages.Add(key, pageType);
        }
    }

        public string GetNameOfRegisteredPage(Type page)
    {
        lock (_pages)
        {
            if (_pages.ContainsValue(page))
                return _pages.FirstOrDefault(p => p.Value == page).Key;

            throw new ArgumentException($"Error. Failed to find the name of the page in the page dictionary in the navigation service.\r\n" +
                                        $"The page <{page.Name}> is unknown to the navigation service.It was not registered properly in NavigationServiceEx.\r\n" +
                                        $"Please investigate the method DependencyInjectionLocator.ConfigureNavigationServiceExSingleton()\r\n" +
                                        $"called in the constructor of your DependencyInjectionLocator where the page dictionary is instantiated.\r\n" +
                                        $"TIt should have been added to the dictionary in that method.\r\n" +
                                        $"\r\n" +
                                        $"{Locus}");
        }
    }

        private void RegisterFrameEvents()
    {
        if (_frame is not null)
        {
            _frame.Navigated += Frame_Navigated;
            _frame.NavigationFailed += Frame_NavigationFailed;
        }
    }

        private void UnregisterFrameEvents()
    {
        if (_frame is not null)
        {
            _frame.Navigated -= Frame_Navigated;
            _frame.NavigationFailed -= Frame_NavigationFailed;
        }
    }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        NavigationFailed?.Invoke(sender, e);
    }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
    {
        Navigated?.Invoke(sender, e);
    }
    }
}
