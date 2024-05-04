using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.ValidationViewModels;
using Rezultz.Uwp.Strings;

// simple viewmodels i.e. ones that inherit from GenericViewViewModel (only) drive their BusyIndicator off IsBusy, not off
// the ProgressViewModel in LeaderboardStylePageViewModelBase. NB this governs the choice of FullScreenLoadingTemplate
// as opposed to RezultzContentPageLoadingTemplate for page controls in Xamarin depending on which vm they have as their datacontext


namespace Rezultz.Uwp.PageViewModels
{
    public class PreferencesPageViewModel : BaseViewViewModel
    {
        private const string Locus2 = nameof(PreferencesPageViewModel);
        private const string Locus3 = "[Rezultz.Uwp]";

        #region ctor

        public PreferencesPageViewModel(ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationViewModel)
        {
            #region assign ctor IOC injections

            GlobalSeasonProfileAndIdentityValidationVm = globalSeasonProfileAndIdentityValidationViewModel;

            GlobalSeasonProfileAndIdentityValidationVm.CurrentRequiredWorkRole = EnumStringsForTimingSystemWorkRoles.Publishing; // so that we can post the html results document

            #endregion
        }

        #endregion

        #region methods called on arrival to page to which this vm is the data context each time by page-loaded event

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
        {
            var failure = StringsForXamlPages.UnableToInitialiseViewmodel;
            const string locus = nameof(BeInitialisedFromPageCodeBehindOrchestrateAsync);

            try
            {
                if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                    return string.Empty;

                FootersVm.Populate(StringsRezultz.Welcome);

                EvaluateGui();

                SaveGenesisOfThisViewModelAsLastKnownGood();

                ThisViewModelIsInitialised = true;

                return await Task.FromResult(string.Empty);
            }

            #region try catch handling

            catch (Exception ex)
            {
                ThisViewModelIsInitialised = false;

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region global props

        protected readonly ISeasonProfileAndIdentityValidationViewModel GlobalSeasonProfileAndIdentityValidationVm;

        #endregion

        #region props

        public bool ThisViewModelIsInitialised;

        public HeaderOrFooterViewModel FootersVm { get; } = new();

        #endregion

        #region Gui stuff to satisfy base class

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
        {
        }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
        {
        }

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
        {
            return new List<object>();
        }

        #endregion

        #region GenesisAsLastKnownGood

        private SeasonProfileItem _lastKnownGoodSeasonProfile;

        private void SaveGenesisOfThisViewModelAsLastKnownGood()
        {
            _lastKnownGoodSeasonProfile = GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem;
        }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
        {
            if (_lastKnownGoodSeasonProfile == null)
                return true;

            return _lastKnownGoodSeasonProfile != GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem;
        }

        #endregion
    }
}
