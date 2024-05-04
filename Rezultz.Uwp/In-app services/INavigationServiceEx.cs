using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Rezultz.Uwp.In_app_services
{
    public interface INavigationServiceEx
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        Frame Frame { get; set; }

        event NavigatedEventHandler Navigated;
        event NavigationFailedEventHandler NavigationFailed;

        void Configure(string key, Type pageType);
        string GetNameOfRegisteredPage(Type page);
        bool GoBack();
        void GoForward();
        bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null);
    }
}
