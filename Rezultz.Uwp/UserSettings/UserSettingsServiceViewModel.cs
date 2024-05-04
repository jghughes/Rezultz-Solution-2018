using Jgh.Xamarin.Common.Jan2019;
using NetStd.Goodies.Mar2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.UserControls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Interfaces02.July2018.Interfaces;
using Rezultz.Library02.Mar2024.PageViewModelBases;

// ReSharper disable InconsistentNaming

namespace Rezultz.Uwp.UserSettings
{
    public class UserSettingsServiceViewModel : BaseViewViewModel, IUserSettingsServiceViewModel
    {
        private const string Locus2 = nameof(UserSettingsServiceViewModel);
        private const string Locus3 = "[Rezultz.Uwp]";

        #region ctor

        public UserSettingsServiceViewModel(IThingsPersistedInLocalStorage thingsPersistedInLocalStorage)
        {
            const string failure = "Unable to construct object UserSettingsServiceViewModel.";
            const string locus = "[ctor]";

            try
            {
                #region assign ctor IOC injections

                _thingsPersistedInLocalStorage = thingsPersistedInLocalStorage;

                #endregion

                #region instantiate Textbox

                TextBoxForEnteringTargetParticipantIdVm = new TextBoxControlViewModel(TextBoxForEnteringTargetParticipantIdOnTextChangedExecute, TextBoxForEnteringTargetParticipantIdOnTextChangedCanExecute);

                #endregion

                #region instantiate ButtonVms

                SaveTargetParticipantIdInStorageButtonVm = new ButtonControlViewModel(SaveTargetParticipantIdInStorageButtonOnClickExecuteAsync, SaveTargetParticipantIdInStorageButtonOnClickCanExecute);
                ClearTargetParticipantIdSavedInStorageButtonVm = new ButtonControlViewModel(ClearTargetParticipantIdSavedInStorageButtonOnClickExecuteAsync, ClearTargetParticipantIdSavedInStorageButtonOnClickCanExecute);


                #endregion

                #region instantiate toggle switches

                MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm = new ButtonControlViewModel(MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickExecute, MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickCanExecute);
                MustUsePreviewNotPublishedDataOnLaunchButtonVm = new ButtonControlViewModel(MustUsePreviewNotPublishedDataOnLaunchButtonOnClickExecute, MustUsePreviewNotPublishedDataOnLaunchButtonOnClickCanExecute);
                MustDisplayConciseLeaderboardColumnsOnlyButtonVm = new ButtonControlViewModel(MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickExecute, MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickCanExecute);


                #endregion

            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        #endregion

        #region method called on first time load and initialisation


        public async Task<string> BeInitialisedOrchestrateAsync()
        {
            const string failure = "Unable to load object UserSettingsServiceViewModel.";
            const string locus = "[BeInitialisedOrchestrateAsync]";

            try
            {
                if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                    return string.Empty;

                DeadenGui();

                await TextBoxForEnteringTargetParticipantIdVm.ChangeTextAsync(await _thingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync());

                MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm.IsChecked = await _thingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync();

                MustUsePreviewNotPublishedDataOnLaunchButtonVm.IsChecked = await _thingsPersistedInLocalStorage.GetMustUsePreviewDataOnLaunchAsync();

                MustDisplayConciseLeaderboardColumnsOnlyButtonVm.IsChecked = await _thingsPersistedInLocalStorage.GetMustDisplayConciseLeaderboardColumnsOnlyAsync();   

                EnlivenGui();

                ThisViewModelIsInitialised = true;

                return string.Empty;

            }

            #region try catch handling

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    EvaluateGui();

                    ThisViewModelIsInitialised = true;
                }
                else
                {
                    EvaluateGui(); // somewhat unusual, but special case

                    ThisViewModelIsInitialised = false;
                }

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

        }

        #endregion

        #region strings

        private const string Target_bib_number_will_be_blank = "The target participant ID will be blank.";
        private const string Will_be_used_as_target_bib_number = "will be used as the target particpant ID.";
        private const string Saving_target_Id = "Saving target ID...";
        private const string Id_cleared = "Id cleared.";

        private const string Unable_to_retrieve_instance = "Coding error. Unable to retrieve instance of interface or object from dependency injection container. Item is not registered there.";

        #endregion

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

        private readonly IThingsPersistedInLocalStorage _thingsPersistedInLocalStorage;



        #endregion

        #region props

        public bool ThisViewModelIsInitialised { get; set; }

        public TextBoxControlViewModel TextBoxForEnteringTargetParticipantIdVm { get; }

        public ButtonControlViewModel SaveTargetParticipantIdInStorageButtonVm { get; }

        public ButtonControlViewModel ClearTargetParticipantIdSavedInStorageButtonVm { get; }

        public ButtonControlViewModel MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm { get; }

        public ButtonControlViewModel MustUsePreviewNotPublishedDataOnLaunchButtonVm { get; }

        public ButtonControlViewModel MustDisplayConciseLeaderboardColumnsOnlyButtonVm { get; }

        public ProgressIndicatorViewModelXamarin UserSettingsProgressIndicatorVm { get; } = new();

        #endregion

        #region commands

        #region MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickAsync

        protected virtual bool MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickCanExecute()
        {
            return MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm.IsAuthorisedToOperate;
        }

        private async void MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickExecute()
        {
            const string failure = "Unable save setting.";
            const string locus = "[MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickExecute]";

            try
            {
                if (!MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickCanExecute())
                    return;

                DeadenGui();

                var messageOk = await MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickAsync();

                EnlivenGui();

                await AlertMessageService.ShowOkAsync(messageOk);

            }
            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    EvaluateGui();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
                else
                {
                    FreezeProgressIndicator();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        private async Task<string> MustShowOnlySingleCategoryOfResultsOnLaunchButtonOnClickAsync()
        {
            await _thingsPersistedInLocalStorage.SaveMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync(
                MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm.IsChecked);

            return "Preference noted. Go ahead and enter your desired ID.";
        }

        #endregion

        #region ForEnteringTargetParticipantIdTextBoxOnTextChangedAsync

        private bool TextBoxForEnteringTargetParticipantIdOnTextChangedCanExecute()
        {
            return TextBoxForEnteringTargetParticipantIdVm.IsAuthorisedToOperate;
        }

        private void TextBoxForEnteringTargetParticipantIdOnTextChangedExecute()
        {
            SaveTargetParticipantIdInStorageButtonVm.IsAuthorisedToOperate = true;
        }

        #endregion

        #region SaveTargetParticipantIdInStorageButtonOnClickAsync

        protected virtual bool SaveTargetParticipantIdInStorageButtonOnClickCanExecute()
        {
            return SaveTargetParticipantIdInStorageButtonVm.IsAuthorisedToOperate;
        }

        private async void SaveTargetParticipantIdInStorageButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[SaveTargetParticipantIdInStorageButtonOnClickExecuteAsync]";

            try
            {

                OpenProgressIndicator(Saving_target_Id);

                var outcome = await SaveTargetParticipantIdInStorageButtonOnClickOrchestrateAsync();

                FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(outcome);
            }

            #region try catch
            catch (Exception ex)
            {
                FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        public async Task<string> SaveTargetParticipantIdInStorageButtonOnClickOrchestrateAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[SaveTargetParticipantIdInStorageButtonOnClickOrchestrateAsync]";

            try
            {
                if (!SaveTargetParticipantIdInStorageButtonOnClickCanExecute())
                    return string.Empty;

                DeadenGui();

                var outcome = await SaveTargetParticipantIdInStorageButtonOnClickAsync();

                EnlivenGui();

                return outcome;
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private async Task<string> SaveTargetParticipantIdInStorageButtonOnClickAsync()
        {
            const string failure = "Unable to save participant ID.";
            const string locus = "[SaveTargetParticipantIdInStorageButtonOnClickAsync]";

            try
            {
                #region do work

                await _thingsPersistedInLocalStorage.SaveTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync(TextBoxForEnteringTargetParticipantIdVm.Text); // always save - there is no validity check to be done

                var confirmedId = await _thingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync();

                var message = string.IsNullOrWhiteSpace(confirmedId)
                    ? $"{Target_bib_number_will_be_blank}"
                    : $"{confirmedId} {Will_be_used_as_target_bib_number}";

                var messageOk = "ID saved.";

                messageOk = JghString.ConcatAsSentences(messageOk, message);

                return messageOk;

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region ClearTargetParticipantIdSavedInStorageButtonOnClickAsync

        protected virtual bool ClearTargetParticipantIdSavedInStorageButtonOnClickCanExecute()
        {
            return ClearTargetParticipantIdSavedInStorageButtonVm.IsAuthorisedToOperate;
        }

        private async void ClearTargetParticipantIdSavedInStorageButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[ClearTargetParticipantIdSavedInStorageButtonOnClickExecuteAsync]";

            try
            {
                if (!ClearTargetParticipantIdSavedInStorageButtonOnClickCanExecute())
                    return;

                DeadenGui();

                OpenProgressIndicator("Clearing ID...");

                var messageOk = await ClearTargetParticipantIdSavedInStorageButtonOnClick();

                EnlivenGui();

                FreezeProgressIndicator();


                await AlertMessageService.ShowOkAsync(messageOk);
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    EvaluateGui();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
                else
                {
                    FreezeProgressIndicator();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        private async Task<string> ClearTargetParticipantIdSavedInStorageButtonOnClick()
        {
            const string failure = "Unable to do what this method does ....";
            const string locus = "[ClearTargetParticipantIdSavedInStorageButtonOnClick]";


            try
            {
                #region do work

                await _thingsPersistedInLocalStorage.SaveTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync(string.Empty);

                await TextBoxForEnteringTargetParticipantIdVm.ChangeTextAsync(string.Empty);

                return Id_cleared;

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region MustUsePreviewNotPublishedDataOnLaunchButtonOnClickAsync

        protected virtual bool MustUsePreviewNotPublishedDataOnLaunchButtonOnClickCanExecute()
        {
            return MustUsePreviewNotPublishedDataOnLaunchButtonVm.IsAuthorisedToOperate;
        }

        private async void MustUsePreviewNotPublishedDataOnLaunchButtonOnClickExecute()
        {
            const string failure = "Unable save setting.";
            const string locus = "[MustUsePreviewNotPublishedDataOnLaunchButtonOnClickExecute]";

            try
            {
                if (!MustUsePreviewNotPublishedDataOnLaunchButtonOnClickCanExecute())
                    return;

                DeadenGui();

                var messageOk = await MustUsePreviewNotPublishedDataOnLaunchButtonOnClickAsync();

                EnlivenGui();

                await AlertMessageService.ShowOkAsync(messageOk);


            }
            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    EvaluateGui();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
                else
                {
                    FreezeProgressIndicator();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        private async Task<string> MustUsePreviewNotPublishedDataOnLaunchButtonOnClickAsync()
        {
            await _thingsPersistedInLocalStorage.SaveMustUsePreviewDataOnLaunchAsync(
                MustUsePreviewNotPublishedDataOnLaunchButtonVm.IsChecked);

            return "Preference saved.";
        }

        #endregion

        #region MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickAsync

        protected virtual bool MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickCanExecute()
        {
            return MustDisplayConciseLeaderboardColumnsOnlyButtonVm.IsAuthorisedToOperate;
        }

        private async void MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickExecute()
        {
            const string failure = "Unable to save setting.";
            const string locus = "[MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickExecute]";

            try
            {
                if (!MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickCanExecute())
                    return;

                DeadenGui();

                var messageOk = await MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickAsync();

                EnlivenGui();

                await AlertMessageService.ShowOkAsync(messageOk);


            }
            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    EvaluateGui();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
                else
                {
                    FreezeProgressIndicator();

                    await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
                }
            }
            finally
            {
                CloseProgressIndicator();
            }

            #endregion
        }

        private async Task<string> MustDisplayConciseLeaderboardColumnsOnlyButtonOnClickAsync()
        {
            await _thingsPersistedInLocalStorage.SaveMustDisplayConciseLeaderboardColumnsOnlyAsync(
                MustDisplayConciseLeaderboardColumnsOnlyButtonVm.IsChecked);

            return "Preference saved.";
        }

        #endregion

        #endregion

        #region ProgressIndicating

        private void OpenProgressIndicator(string descriptionOfWhatsHappening)
        {
            UserSettingsProgressIndicatorVm.OpenProgressIndicator(descriptionOfWhatsHappening);
        }

        private void FreezeProgressIndicator()
        {
            UserSettingsProgressIndicatorVm.FreezeProgressIndicator();
        }

        private void CloseProgressIndicator()
        {
            UserSettingsProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion

        #region Gui stuff

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
        {
            MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm.IsAuthorisedToOperate = true; // always enabled

            TextBoxForEnteringTargetParticipantIdVm.IsAuthorisedToOperate = MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm.IsChecked;

            SaveTargetParticipantIdInStorageButtonVm.IsAuthorisedToOperate = false;

            ClearTargetParticipantIdSavedInStorageButtonVm.IsAuthorisedToOperate = true; // ditto

            MustUsePreviewNotPublishedDataOnLaunchButtonVm.IsAuthorisedToOperate = true; // ditto

            MustDisplayConciseLeaderboardColumnsOnlyButtonVm.IsAuthorisedToOperate = true; // ditto
        }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
        {
            MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm.IsVisible = true;

            TextBoxForEnteringTargetParticipantIdVm.IsVisible = true;

            SaveTargetParticipantIdInStorageButtonVm.IsVisible = true;

            ClearTargetParticipantIdSavedInStorageButtonVm.IsVisible = true;

            MustUsePreviewNotPublishedDataOnLaunchButtonVm.IsVisible =  true;

            MustDisplayConciseLeaderboardColumnsOnlyButtonVm.IsVisible = true;

        }

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
        {
            var answer = new List<object>();

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringTargetParticipantIdVm);

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, SaveTargetParticipantIdInStorageButtonVm);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, ClearTargetParticipantIdSavedInStorageButtonVm);


            AddToCollectionIfIHasIsAuthorisedToOperate(answer, MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, MustUsePreviewNotPublishedDataOnLaunchButtonVm);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, MustDisplayConciseLeaderboardColumnsOnlyButtonVm);

            return answer;
        }

        #endregion

        #region GenesisAsLastKnownGood

        protected void SaveGenesisOfThisViewModelAsLastKnownGood()
        {
            // nothing required
        }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
        {
            return false;
        }

        #endregion

    }
}
