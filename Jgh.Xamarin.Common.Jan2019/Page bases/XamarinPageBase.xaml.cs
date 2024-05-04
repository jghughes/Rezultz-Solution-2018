using System;
using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.OnBoardServices01.July2018.AlertMessages;
using Xamarin.Forms.Xaml;

namespace Jgh.Xamarin.Common.Jan2019.Page_bases
{
	/// <summary>
	///     Iff using the approach that Xamarin pages should provide the IAlertMessageService, pages must register themselves
	///     as the IAlertMessageService provider with an Ioc Container when they Load in OnAppearing(). Because pages are
	///     transient, the service should be injected into viewmodels by means of property injection rather than constructor
	///     injection. Xamarin pages intrinsically implement IDialogService
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class XamarinPageBase : IAlertMessageService, IDialogService
	{
        private readonly AlertMessageService _alertMessageService;

        public XamarinPageBase(AlertMessageService alertMessageService)
        {
            _alertMessageService = alertMessageService;
            InitializeComponent();
        }

        #region Implementation of IAlertMessageService 

        public async Task<int> ShowOkAsync(string message)
        {
            await _alertMessageService.ShowMessageAsync(message);

            return 0;
        }

        public async Task<int> ShowOkAsync(Exception ex)
        {
            await _alertMessageService.ShowMessageAsync(ex);

            return 0;
        }

        public async Task<int> ShowOkAsync(string message, Exception ex)
        {
            await _alertMessageService.ShowMessageAsync(message, ex);

            return 0;
        }

        public async Task<int> ShowErrorOkAsync(Exception ex)
        {
            await _alertMessageService.ShowErrorMessageAsync(ex);

            return 0;
        }

        public async Task<int> ShowErrorOkAsync(string message, Exception ex)
        {
            await _alertMessageService.ShowErrorMessageAsync(message, ex);

            return 0;
        }

        public async Task<int> ShowOkCancelAsync(string message, string messageCaption, string secondaryMessage)
        {
            var answer =
                await _alertMessageService.ShowMessageWithOKorCancelOptionsAsync(message, messageCaption,
                    secondaryMessage);

            return answer switch
            {
                AlertMessageEnum.Ok => 1,
                AlertMessageEnum.Cancel => 2,
                AlertMessageEnum.None => 0,
                _ => 0
            };
        }

        public async Task<int> ShowErrorOkCancelAsync(string message, string messageCaption, Exception ex)
        {
            var answer =
                await _alertMessageService.ShowErrorMessageWithOKorCancelOptionsAsync(message, messageCaption, ex);

            return answer switch
            {
                AlertMessageEnum.Ok => 1,
                AlertMessageEnum.Cancel => 2,
                AlertMessageEnum.None => 0,
                _ => 0
            };
        }

        public async Task<int> ShowNotificationOrErrorMessageAsync(string failure, string locus, string locus2,
            string locus3,
            Exception ex)
        {
            await _alertMessageService.ShowMessageOrErrorMessageAsCaseMayBeAsync(failure, locus, locus2, locus3, ex);

            return 0;
        }

        #endregion

        #region Implementation of IDialogService 

        public Task DisplayAlertAsync(string title, string message, string okButtonLabel)
        {
            return DisplayAlert(title, message, okButtonLabel);
        }

        public Task<bool> DisplayAlertAsync(string title, string message, string acceptButtonLabel,
            string cancelButtonLabel)
        {
            return DisplayAlert(title, message, acceptButtonLabel, cancelButtonLabel);
        }

        public Task<string> DisplayActionSheetAsync(string title, string cancelButtonLabel, string destruction,
            params string[] buttons)
        {
            return DisplayActionSheet(title, cancelButtonLabel, destruction, buttons);
        }

        #endregion

    }
}