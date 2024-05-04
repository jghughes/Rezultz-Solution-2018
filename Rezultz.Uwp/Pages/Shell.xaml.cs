using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Rezultz.Uwp.DependencyInjection;
using Rezultz.Uwp.PageViewModels;

namespace Rezultz.Uwp.Pages
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in Shell.xaml.
    public sealed partial class Shell
    {
        private ShellViewModel ViewModel => DataContext as ShellViewModel;

        private static DependencyInjectionLocator DependencyLocator => Application.Current.Resources["DependencyInjectionLocator"] as DependencyInjectionLocator;

        public Shell()
        {
            InitializeComponent();
            HideNavViewBackButton();
            DataContext = ViewModel;
            ViewModel.Initialize(XamlElementShellFrame, XamlElementNavigationView);

            // commenting the following out because at time of writing has an irritating unsolicited popup visual artifact - which the Windows Studio Template engineers
            // are twisting and turning to try resolve in the newer templates - but not totally successfully

            //KeyboardAccelerators.Add(ActivationService.AltLeftKeyboardAccelerator);
            //KeyboardAccelerators.Add(ActivationService.BackKeyboardAccelerator);

        }

        private void HideNavViewBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                XamlElementNavigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            }
        }
    }
}
