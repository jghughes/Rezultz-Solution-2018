First of all, be sure to download the Template Studio for UWP from the Visual Studio Marketplace. This is a Visual Studio extension that provides a wizard for creating a UWP app with pages, page viewmodels, and services. The app creation wizard provides a Shell page with a NavigationView control that allows you to navigate between pages from a static menu bar. The NavigationView control uses the provided NavHelper class to register the pages to navigate to for each NavigationViewItem. The wizard generates a page for each item in the NavigationViewItem. The pages are created with a corresponding viewmodel.  The wizard also generates an ActivationService class. The ActivationSerice class creates the shell or frame to act as the navigation context.  The provided NavigationService class is then used to navigate between pages. 

These are the alterations to generated code you need to make to navigation and page/page viewmodel databinding that I have developed as an improvement. I enhance the navigation system to key off page names not viewmodel names.I do this to break what is otherwise a forced one-to-one correspondence between page and viewmodel. I do this so I are not prevented from using a viewmodel as datacontext for more than one page, or from using more than one viewmodel for a single page, or from being able to navigate to a page that doesn't have a viewmodel. I do it so that I can navigate to pages progammatically as opposed to just from the Shell menu. I make extensive use of a global DependencyInjectionLocator to tie everything together. This is instantated in App.xaml as one of the very first things that is done on loading the app.  Here are the steps you need to follow to make the generated code work with my system.

Modify the ActivationService class to wire it up to DependencyInjectionLocator and in turn to use the NavigationServiceEx singleton for navigation. NavigationServiceEx is my version of the NavigationService class, which is referred to throughout the ActivationService class 

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources["DependencyInjectionLocator"] as DependencyInjectionLocator;
        private static NavigationServiceEx NavigationService => DependencyLocator.NavigationService;

Modify the NavigationService class Navigate() method to use page names not viewmodel names as the navigation item key and then modify the Configure() method to use page name as a string not viewmodel names as the navigation item key. 

        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            Type page;

            lock (_pages)
            {
                if (!_pages.TryGetValue(pageKey, out page))
                {
                    throw new ArgumentException($"Unable to navigate to page. The name of the page is missing or incorrectly hardcoded in the [Shell.NavigationView.MenuItems] and/or the page key <{pageKey}> is not in the page dictionary in the navigation service. Did you call ConfigureNavigationServiceExSingleton() in the constructor of your DependencyInjectionLocator to instantiate the navigation service and did you configure the page inside the called method? {Locus}");
                }
            }

            // Don't open the same page multiple times
            if (Frame.Content?.GetType() != page || (parameter != null && !parameter.Equals(_lastParamUsed)))
            {
                var navigationResult = Frame.Navigate(page, parameter, infoOverride);

                if (navigationResult)
                {
                    _lastParamUsed = parameter;
                }

                return navigationResult;
            }
            else
            {
                return false;
            }

            // The lock statement is there to serialize navigation steps and make the app behave properly when the user is wildly clicking around 
        }



        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"Error. The page key <{key}> is already in the page dictionary in the navigation service. {Locus}");
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException($"Error. This page <{nameof(pageType)}> is already in the page dictionary in the navigation service. The duplicate page key is <{_pages.First(p => p.Value == pageType).Key}>. {Locus}");
                }

                _pages.Add(key, pageType);
            }
        }



The Configure() method is used to register the page names in the NavigationServiceEx class which we instantiate in the DependencyInjectionLocatorlike. In Rezultz, there are Pages we wish to navigate to programmatically by pressing a button, as opposed via a navigation menu item on the Shell. To accomplish this we create an interface in Rezultz.Library02.Mar2024.PageNavigation. In each app we provide a customised implementation, such as Rezultz.Uwp.NavigationServiceJgh.NavigationServiceExForRezultzViewModels. The purpose of this service is to provide an implementation of INavigationServiceForRezultz that can be registered in the DependencyInjectionLocator alongside navigationServiceExInstance like so:


        var navigationServiceExInstance = ConfigureNavigationServiceExSingleton(); 
        _dependencyContainer.Register(() => navigationServiceExInstance); 
        _dependencyContainer.Register<INavigationServiceForRezultz, NavigationServiceExForRezultzViewModels>();

        private static NavigationServiceEx ConfigureNavigationServiceExSingleton()
        {
            var svc = new NavigationServiceEx();

            svc.Configure(typeof(Shell).FullName, typeof(Shell));

            svc.Configure(typeof(HomePage).FullName, typeof(HomePage));
            svc.Configure(typeof(AboutPage).FullName, typeof(AboutPage));
            svc.Configure(typeof(PreferencesPage).FullName, typeof(PreferencesPage));

            svc.Configure(typeof(SingleEventLeaderboardPage).FullName, typeof(SingleEventLeaderboardPage));
            svc.Configure(typeof(SingleEventAverageSplitTimesPage).FullName, typeof(SingleEventAverageSplitTimesPage));
            svc.Configure(typeof(SingleEventPopulationCohortsPage).FullName, typeof(SingleEventPopulationCohortsPage));

            svc.Configure(typeof(SeriesStandingsLeaderboardPage).FullName, typeof(SeriesStandingsLeaderboardPage));
            svc.Configure(typeof(SeriesPopulationCohortsPage).FullName, typeof(SeriesPopulationCohortsPage));

            return svc;
        }


Now that we have the infrastructure, for each Page class that you write and wish to be able to navigate to via the Navigation view menuitems on the Shell.xaml menu, you need to ensure that the hard coded NavigationViewItem NavigateTo string exactly matches the fully-qualified page names:

        <NavigationViewItem Content="Home" helpers:NavHelper.NavigateTo="Rezultz.Uwp.Pages.HomePage"/>

In the locator we now create a public property for each viewmodel that we wish to bind as the whole or partial datacontext somewhere associated with a page (or other view), but the viewmodels are no longer used for navigation:

        public AboutPageViewModel AboutPageVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<AboutPageViewModel>();
                }
                catch (Exception ex)
                {
                    var msg = JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, "[AboutPageViewModel]");
                    const string locus = "Property getter of [AboutPageVm]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

Hey presto.

2nd April 2024         

