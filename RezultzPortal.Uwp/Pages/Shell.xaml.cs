using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;
using RezultzPortal.Uwp.PageViewModels;

namespace RezultzPortal.Uwp.Pages
{
    public sealed partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
            HideNavViewBackButton();
            DataContext = ViewModel;
            ViewModel.Initialize(ShellFrame, XamlElementNavigationView);

            // commenting this out because at time of writing has an irritating unsolicited popup visual artifact
            //    KeyboardAccelerators.Add(ActivationService.AltLeftKeyboardAccelerator);
            //      KeyboardAccelerators.Add(ActivationService.BackKeyboardAccelerator);
        }

        private ShellViewModel ViewModel => DataContext as ShellViewModel;

        private void HideNavViewBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
                XamlElementNavigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        }
    }
}
