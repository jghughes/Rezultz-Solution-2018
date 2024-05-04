using System;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Prism.July2018;
using NetStd.ViewModels01.April2022.Dialogs;

// ReSharper disable UnusedMethodReturnValue.Local

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class DialogViewModel : BindableBase, IDialogUserControlViewModel
    {
        private const string Locus2 = nameof(DialogViewModel);
        private const string Locus3 = "[NetStd.ViewModels01.April2022]";

        #region ctor

        public DialogViewModel()
        {
            OkButtonVm = new ButtonControlViewModel(
                OkButtonOnClickExecute,
                OkButtonOnClickCanExecute
            )
            {
                Content = DefaultOkButtonText,
                Tag = "Ok"
            };


            CancelButtonVm = new ButtonControlViewModel(
                CancelButtonOnClickExecute, CancelButtonOnClickCanExecute)
            {
                Content = DefaultCancelButtonText,
                Tag = "Cancel operation"
            };

            PrimaryMessageVm = new TextBlockControlViewModel()
            {
                Label = DefaultWindowTitleText,
                Text = PrimaryMessageTextInitialiser
            };

            SecondaryMessageVm = new TextBlockControlViewModel
            {
                Label = SecondaryMessageTitleInitialiser,
                Text = SecondaryMessageTextInitialiser
            };
        }

        #endregion

        #region helpers

        private void AssignDialogResult(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }

        #endregion

        #region constants

        private const string DefaultOkButtonText = "OK";

        private const string DefaultCancelButtonText = "Cancel";

        private const string DefaultWindowTitleText = "Message";

        private const string PrimaryMessageTextInitialiser = "Contents of primary message go here";

        private const string SecondaryMessageTitleInitialiser = "Secondary message box title goes here";

        private const string SecondaryMessageTextInitialiser = "Contents of secondary message go here";

        #endregion

        #region Props

        public ButtonControlViewModel OkButtonVm { get;  }
        public ButtonControlViewModel CancelButtonVm { get;  }
        public TextBlockControlViewModel PrimaryMessageVm { get;  }
        public TextBlockControlViewModel SecondaryMessageVm { get; }
        public bool? DialogResult { get; set; }

        #endregion

        #region Dialog Methods

        public void InitialiseOkDialogue(string primaryMessage, string primaryMessageCaption, string secondaryMessage,
            string secondaryMessageCaption)
        {
            PrimaryMessageVm.Label = primaryMessageCaption ?? "";
            PrimaryMessageVm.Text = primaryMessage ?? "";
            SecondaryMessageVm.Text = secondaryMessage ?? "";
            SecondaryMessageVm.Label = secondaryMessageCaption ?? "";

            SecondaryMessageVm.IsVisible = !string.IsNullOrWhiteSpace(secondaryMessage);

            OkButtonVm.IsVisible = true;
            OkButtonVm.IsAuthorisedToOperate = true;

            AssignDialogResult(null);

            CancelButtonVm.IsVisible = false;
            CancelButtonVm.IsAuthorisedToOperate = false;
        }

        public void InitialiseOkCancelDialogue(string primaryMessage, string primaryMessageCaption,
            string secondaryMessage, string secondaryMessageCaption)
        {
            InitialiseOkDialogue(primaryMessage, primaryMessageCaption, secondaryMessage, secondaryMessageCaption);

            CancelButtonVm.IsVisible = true;
            CancelButtonVm.IsAuthorisedToOperate = true;
        }

        //public void OnDialogClose(IDialogUserControlViewModel obj)
        //{
        //    // don't understand this. what's the placeholder do. what's envisaged?
        //}

        #endregion

        #region command handlers

        #region  OkButtonOnClickAsync

        protected virtual bool OkButtonOnClickCanExecute()
        {
            return OkButtonVm.IsAuthorisedToOperate;

            //if (OkButtonVm.OnClickCommand.IsActive)
            //    return false;
        }

        private async void OkButtonOnClickExecute()
        {
            //const string failure = "Unable to complete ICommand Execute action.";
            //const string locus = "[OkButtonOnClickExecute]";

            try
            {
                #region handle canexecute

                OkButtonVm.IsAuthorisedToOperate = false;
                CancelButtonVm.IsAuthorisedToOperate = false;

                #endregion

                #region disable Gui command senders and related controls

                OkButtonVm.OnClickCommand?.RaiseCanExecuteChanged();
                CancelButtonVm.OnClickCommand?.RaiseCanExecuteChanged();

                #endregion

                await OkButtonOnClickAsync();
            }

            #region try catch

            catch (Exception)
            {
                //        // todo need some suitable exception handling here
                //        // AlertMesssageServiceInstance.ShowSimpleMessageOrFullRedactedExceptionMessage(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                #region handle canexecute

                OkButtonVm.IsAuthorisedToOperate = true; // todo or do we want to end up with this as false?
                CancelButtonVm.IsAuthorisedToOperate = true;

                #endregion

                #region restore Gui command senders and related controls

                OkButtonVm.OnClickCommand?.RaiseCanExecuteChanged();
                CancelButtonVm.OnClickCommand?.RaiseCanExecuteChanged();

                #endregion
            }

            #endregion
        }

        private async Task<bool> OkButtonOnClickAsync()
        {
            const string failure = "Unable to complete operation";
            const string locus = "[OkButtonOnClickAsync]";

            try
            {
                #region do work

                // whatever work is required, if any. otherwise just set the dialog result to true

                AssignDialogResult(true);

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return await Task.FromResult(true);
            //return TaskEx.FromResult(true);
        }

        #endregion


        #region  CancelButtonOnClickAsync

        protected virtual bool CancelButtonOnClickCanExecute()
        {
            return CancelButtonVm.IsAuthorisedToOperate;

            //if (CancelButtonVm.OnClickCommand.IsActive)
            //    return false;
        }

        private async void CancelButtonOnClickExecute()
        {
            //const string failure = "Unable to complete action.";
            //const string locus = "[CancelButtonOnClickExecuteAsync]";

            try
            {
                #region handle canexecute

                CancelButtonVm.IsAuthorisedToOperate = false;
                OkButtonVm.IsAuthorisedToOperate = false;

                #endregion

                #region update Gui command senders and related controls

                OkButtonVm.OnClickCommand?.RaiseCanExecuteChanged();
                CancelButtonVm.OnClickCommand?.RaiseCanExecuteChanged();

                #endregion

                await CancelButtonOnClickAsync();
            }

            #region try catch

            catch (Exception)
            {
                // todo need some suitable exception handling here
                // AlertMesssageServiceInstance.ShowSimpleMessageOrFullRedactedExceptionMessage(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                #region handle canexecute

                CancelButtonVm.IsAuthorisedToOperate = true;
                OkButtonVm.IsAuthorisedToOperate = true;

                #endregion

                #region update Gui command senders and related controls

                OkButtonVm.OnClickCommand?.RaiseCanExecuteChanged();
                CancelButtonVm.OnClickCommand?.RaiseCanExecuteChanged();

                #endregion
            }

            #endregion
        }

        private async Task<bool> CancelButtonOnClickAsync()
        {
            const string failure = "Unable to do what this method does ....";
            const string locus = "[CancelButtonOnClickAsync]";

            try
            {
                #region do work

                // todo  i.e. await some Task<bool> method
                AssignDialogResult(false);
                // whatever work is required, if any. otherwise just set the dialog result to false

                #endregion

                return await Task.FromResult(true);
                //return TaskEx.FromResult(true);
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #endregion
    }
}