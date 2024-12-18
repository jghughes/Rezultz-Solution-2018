﻿using System;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NetStd.Prism.July2018;
using NetStd.ServiceLocation.Aug2022;
using RezultzPortal.Uwp.Helpers;
using RezultzPortal.Uwp.In_app_services;
using RezultzPortal.Uwp.Pages;

namespace RezultzPortal.Uwp.PageViewModels
{
    public class ShellViewModel : BindableBase
    {
        private ICommand _itemInvokedCommand;
        private NavigationView _navigationView;
        private NavigationViewItem _selected;

        public NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public NavigationViewItem Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand ItemInvokedCommand => _itemInvokedCommand ??= new DelegateCommand<NavigationViewItemInvokedEventArgs>(OnItemInvoked);

        public void Initialize(Frame frame, NavigationView navigationView)
        {
            _navigationView = navigationView;
            NavigationService.Frame = frame;
            NavigationService.Navigated += Frame_Navigated;
        }

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // changed by Jgh to the page we want to navigate to when Settings icon is clicked 

                NavigationService.Navigate(typeof(AboutPage).FullName);
                return;
            }

            var item = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .First(menuItem => (string) menuItem.Content == (string) args.InvokedItem);

            var pageKey = item.GetValue(NavHelper.NavigateToProperty) as string;
            NavigationService.Navigate(pageKey);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(AboutPage))
            {
                Selected = _navigationView.SettingsItem as NavigationViewItem;
                return;
            }

            Selected = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var navigatedPageKey = NavigationService.GetNameOfRegisteredPage(sourcePageType);
            var pageKey = menuItem.GetValue(NavHelper.NavigateToProperty) as string;
            return pageKey == navigatedPageKey;
        }
    }
}
