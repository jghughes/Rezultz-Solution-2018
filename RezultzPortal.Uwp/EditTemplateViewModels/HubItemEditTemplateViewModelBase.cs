using System;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Xamarin.Common.Jan2019;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.Library02.Mar2024.PageViewModelBases;

// ReSharper disable InconsistentNaming

namespace RezultzPortal.Uwp.EditTemplateViewModels
{
    // Important notes:

    // See SynchroniseIsAuthorisedToOperateValueOfConstituentControls() inside setter of IsAuthorisedToOperate.
    // This is how we keep IsAuthorisedToOperateValue of the template in sync with vms embedded in the template
    // that employ their own IsAuthorisedToOperateValue props to bind with GUI 

    // Action is what we use to transmit the value of IsAuthorisedToOperate to the containing type of this vm. Must be assigned in the containing type.

    // Do not use Guid. Merely here to satisfy interface.

    public abstract class HubItemEditTemplateViewModelBase : BaseViewViewModel, IHasGuid, IHasWasTouched, IHasAnyEditTemplateEntryChangedExecuteAction, IHasIsAuthorisedToOperate
    {
        private const string Locus2 = nameof(HubItemEditTemplateViewModelBase);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        #region fields

        protected bool IsFirstTimeThrough = true;

        private bool _capturedIsAuthorisedToOperateValue;

        protected string AsInitiallyPopulatedSemanticValue = string.Empty;

        protected string _kindOfTimeStampEnum;

        #endregion

        #region ctor

        protected HubItemEditTemplateViewModelBase()
        {
            CopyHubItemButtonVm = new ButtonControlViewModel(CopyHubItemButtonOnClickExecute, CopyHubItemButtonOnClickCanExecute) { IsAuthorisedToOperate = false, IsVisible = true };
            PasteHubItemButtonVm = new ButtonControlViewModel(PasteHubItemButtonOnClickExecute, PasteHubItemButtonOnClickCanExecute) { IsAuthorisedToOperate = false, IsVisible = true };

        }

        #endregion

        #region strings

        public const string Copying____ = "Copying...";
        public const string Pasting____ = "Pasting..";
        public const string Copied = "Copied.";
        public const string Pasted = "Pasted.";

        public const string Unable_to_retrieve_instance = "Coding error. Unable to retrieve instance of interface or object from dependency injection container. Object is not registered there.";

        #endregion

        #region props

        #region global props

        private static IAlertMessageService AlertMessageService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IAlertMessageService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(Unable_to_retrieve_instance,
                            $"'{nameof(IAlertMessageService)}");

                    var locus = $"Property getter of '{nameof(AlertMessageService)}]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region ButtonVms

        public ButtonControlViewModel CopyHubItemButtonVm { get; set;  } 

        public ButtonControlViewModel PasteHubItemButtonVm { get; set; }


        #endregion

        #region other props

        public ProgressIndicatorViewModelXamarin EditProgressIndicatorVm { get; } = new();

        public Action AnyEditTemplateEntryChangedExecuteAction { get; set; }

        private bool _backingstoreIsAuthorisedToOperate;

        public bool IsAuthorisedToOperate
        {
            get => _backingstoreIsAuthorisedToOperate;
            set => SetProperty(ref _backingstoreIsAuthorisedToOperate, value, SynchroniseIsAuthorisedToOperateValueOfConstituentControls);
        }

        public bool WasTouched { get; set; }

        public string Guid { get; set; }


        #endregion

        #region TextBlock

        private string _backingStoreKindOfTimeStampEnumText;

        public string KindOfTimeStampEnumText
        {
            get => _backingStoreKindOfTimeStampEnumText ?? string.Empty;

            set => SetProperty(ref _backingStoreKindOfTimeStampEnumText, value);
        }

        #endregion

        #region EditTemplate INPC entries - all setters invoke AnyEditTemplateEntryChanged

        private string _backingstoreBib = string.Empty;

        public string Bib
        {
            get => _backingstoreBib ?? string.Empty;
            set => SetProperty(ref _backingstoreBib, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreRfid = string.Empty;

        public string Rfid
        {
            get => _backingstoreRfid ?? string.Empty;
            set => SetProperty(ref _backingstoreRfid, value, AnyEditTemplateEntryChanged);
        }

        private bool _backingstoreMustDitchOriginatingItem;

        public bool MustDitchOriginatingItem
        {
            get => _backingstoreMustDitchOriginatingItem;
            set => SetProperty(ref _backingstoreMustDitchOriginatingItem, value, AnyEditTemplateEntryChanged);
        }

        #endregion

        #endregion

        #region commands

        #region CopyHubItemButtonOnClickAsync

        protected virtual bool CopyHubItemButtonOnClickCanExecute()
        {
            return CopyHubItemButtonVm.IsAuthorisedToOperate;
        }

        protected async void CopyHubItemButtonOnClickExecute()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[CopyHubItemButtonOnClickExecute]";

            try
            {
                if (!CopyHubItemButtonOnClickCanExecute())
                    return;

                ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

                OpenProgressIndicator(Copying____);

                var messageOk = await CopyHubItemButtonOnClickAsync();

                ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                EvaluateGui();

                FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(messageOk);
            }

            #region try catch

            catch (Exception ex)
            {
                ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                    Locus3, ex);
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        protected abstract Task<string> CopyHubItemButtonOnClickAsync();

        #endregion

        #region PasteHubItemButtonOnClick

        protected virtual bool PasteHubItemButtonOnClickCanExecute()
        {
            return PasteHubItemButtonVm.IsAuthorisedToOperate;
        }

        protected async void PasteHubItemButtonOnClickExecute()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[PasteHubItemButtonOnClickExecute]";

            try
            {
                if (!PasteHubItemButtonOnClickCanExecute())
                    return;

                ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

                OpenProgressIndicator(Pasting____);

                var messageOk = await PasteHubItemButtonOnClickAsync();

                ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                EvaluateGui();

                FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(messageOk); // placeholder
            }

            #region try catch

            catch (Exception ex)
            {
                ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                    Locus3, ex);
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        protected abstract Task<string> PasteHubItemButtonOnClickAsync();

        #endregion

        #endregion

        #region methods

        public void AnyEditTemplateEntryChanged()
        {
            AnyEditTemplateEntryChangedExecuteAction?.Invoke();

            WasTouched = true;
        }

        public virtual void CaptureIsAuthorisedToOperateValue()
        {
            _capturedIsAuthorisedToOperateValue = IsAuthorisedToOperate;
        }

        public virtual void RestoreCapturedIsAuthorisedToOperateValue()
        {
            IsAuthorisedToOperate = _capturedIsAuthorisedToOperateValue;
        }

        protected abstract void SynchroniseIsAuthorisedToOperateValueOfConstituentControls();

        public virtual async Task<bool> ZeroiseAsync()
        {
            IsAuthorisedToOperate = false;

            KindOfTimeStampEnumText = string.Empty;

            return await Task.FromResult(true);
        }

        #endregion

        #region progress indicating

        protected void OpenProgressIndicator(string descriptionOfWhatsHappening)
        {
            EditProgressIndicatorVm.OpenProgressIndicator(descriptionOfWhatsHappening);
        }

        protected void FreezeProgressIndicator()
        {
            EditProgressIndicatorVm.FreezeProgressIndicator();
        }

        protected void CloseProgressIndicator()
        {
            EditProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }
}
