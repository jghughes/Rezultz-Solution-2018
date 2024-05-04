using System;

namespace NetStd.ServiceLocation.Aug2022
{
    public static class ServiceLocator
    {
        private static ServiceLocatorProvider _currentProvider;

        public static IServiceLocator Current
        {
            get
            {
                if (!IsLocationProviderSet)
                    throw new InvalidOperationException("Your ServiceLocationProvider has not yet been set. You need to call ServiceLocator.SetLocatorProvider(ServiceLocatorProvider newProvider)");

                return _currentProvider();
            }
        }

        public static void SetLocatorProvider(ServiceLocatorProvider newProvider) => _currentProvider = newProvider;

        public static bool IsLocationProviderSet => _currentProvider != null;
    }
}