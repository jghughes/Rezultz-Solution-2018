using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.ValidationViewModels;
using Rezultz.Uwp.UserSettings;

namespace Rezultz.Uwp.PageViewModels
{
    public class HomePageViewModel : BaseViewViewModel
    {
        private const string Locus2 = nameof(HomePageViewModel);
        private const string Locus3 = "[Rezultz.Uwp]";

        #region ctor

        public HomePageViewModel(ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationViewModel,
             IUserSettingsServiceViewModel userSettingsServiceViewModel)
        {
            GlobalUserSettingsServiceVm = userSettingsServiceViewModel;

            GlobalSeasonProfileAndIdentityValidationVm = globalSeasonProfileAndIdentityValidationViewModel;

            GlobalSeasonProfileAndIdentityValidationVm.CurrentRequiredWorkRole = EnumStringsForTimingSystemWorkRoles.Publishing;

        }

        #endregion

        #region page-loaded event

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
        {
            var failure = StringsForXamlPages.UnableToInitialiseViewmodel;
            const string locus = nameof(BeInitialisedFromPageCodeBehindOrchestrateAsync);

            try
            {
                if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                    return string.Empty;

                DeadenGui();

                await GlobalUserSettingsServiceVm.BeInitialisedOrchestrateAsync(); // we do this here because this is the landing page

                await GlobalSeasonProfileAndIdentityValidationVm.BeInitialisedForRezultzOrchestrateAsync(); // long running

                EnlivenGui();

                SaveGenesisOfThisViewModelAsLastKnownGood();

                ThisViewModelIsInitialised = true;

                var messageOk = "Welcome to Rezultz. Choose a season from the drop-down list.";

                return messageOk;
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

        protected readonly IUserSettingsServiceViewModel GlobalUserSettingsServiceVm;

        #endregion

        #region prop

        public bool ThisViewModelIsInitialised { get; protected set; }

        #endregion

        #region Gui stuff

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

        private SeasonProfileItem _lastKnownGoodSeasonProfileItem;

        private void SaveGenesisOfThisViewModelAsLastKnownGood()
        {
            _lastKnownGoodSeasonProfileItem = GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem;
        }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
        {
            if (_lastKnownGoodSeasonProfileItem == null)
                return true;

            return _lastKnownGoodSeasonProfileItem != GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem;
        }

        #endregion
    }
}
