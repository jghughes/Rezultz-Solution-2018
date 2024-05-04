using NetStd.Goodies.Mar2022;
using Rezultz.Uwp.In_app_services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Rezultz.Uwp.Pages;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

// ReSharper disable UnusedMember.Local

// ReSharper disable RedundantNameQualifier

namespace Rezultz.Uwp
{
    public sealed partial class App
    {
        private readonly Lazy<ActivationService> _activationService;

        private ActivationService ActivationService => _activationService.Value;

        public App()
        {
            InitializeComponent();

            UnhandledException += App_UnhandledException;

            EnteredBackground += App_EnteredBackground;

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // refactored by JGH to introduce Xamarin.Forms! groovy.

            if (args.PrelaunchActivated)
                return;

            Xamarin.Forms.Forms.Init(args);

            await ActivationService.ActivateAsync(args);

            //if (!args.PrelaunchActivated)
            //{
            //    await ActivationService.ActivateAsync(args);
            //}
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {

            // refactored by JGH to introduce Xamarin.Forms! groovy.

            Xamarin.Forms.Forms.Init(args);

            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            // this is where we instantiate an Activation Service, specify which page is the default launch page, and instantiate a Shell to serve as the navigation frame

            // N.B.unlike Windows Template Studio - we've enhanced/improved the registration of our NavigationServiceEx in DependencyInjectionLocator
            // (see CreateNavigationServiceEx()) so that it uses the FullName of the page as the pagekey as opposed to the fullname of its associated viewmodel
            // Hence the pagekey HomePage instead of HomePageViewModel for example.
            // we do this so we are not prevented from using a viewmodel as datacontext for more than one page, or from having
            // several viewmodels for a single page, or from having a page without a viewmodel

            return new ActivationService(this, typeof(HomePage), new Lazy<UIElement>(CreateShell));

            // At time of review in Jan 2021, not sure if the following pertains. NB. you need to search throughout all the Windows Template Studio generated files for ".Navigate(" and be sure to modify
            // the code to accept a string argument as opposed to a viewmodel e.g. see the method ShellVm.OnItemInvoked().
        }

        private UIElement CreateShell()
        {
            return new Shell();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }


        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // this method added by Jgh

            await App_UnhandledExceptionAsync(sender, e);
        }

        private async Task App_UnhandledExceptionAsync(object sender, UnhandledExceptionEventArgs e)
        {
            const string caption = "Unhandled Exception";

            const string failure =
                "An exception occurred that was unhandled, maybe or maybe not encountered by the Xaml parser or Windows Runtime.";

            const string instructionsOnHowToReportAnError =
                "Sorry about this difficulty. If you care to report the problem, take a screen shot of this error message and paste it into an email with some supporting narrative. In Windows, press PrntScr to take the shot and do Ctrl+V to paste it into your email.  On an Apple, press Command-Shift-4, move the crosshair pointer to where you want to start the screenshot, drag to select an area and then release your mouse or trackpad button. The screen shot will be on your desktop as a .png file.";

            var consolidatedMessage = JghString.ConcatAsSentences(e.Message, failure,
                instructionsOnHowToReportAnError);

            Debug.WriteLine(
                $"App_UnhandledException event was invoked. {sender} {consolidatedMessage}"); // this can be informative if you encounter problems during instantiation of resource dictionaries

            if (Debugger.IsAttached)
            {
                e.Handled = true;

                // intercept the exception and report it using a message dialogue (i.e. don't leave it to the debugger to handle during debugging because that unfortunately leads to uninformative error messaging)
                var messageDialog = new MessageDialog(consolidatedMessage, caption);
                await messageDialog.ShowAsync();
                // on the other hand if for some bizarre reason you want to leave it to the debugger to deal with, comment out the lines above

                Exit();
            }
            else
            {
                e.Handled = true;

                // If the app is running outside of the debugger then report the exception using a message dialogue.
                // ToDo For production applications this error handling should be replaced with something that will report or log the error and stop the application.
                var messageDialog = new MessageDialog(consolidatedMessage, caption);
                await messageDialog.ShowAsync();

                Exit();
            }
        }


    }
}
