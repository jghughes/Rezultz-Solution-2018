using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.Library02.Mar2024.PageViewModelBases;

namespace RezultzPortal.Uwp.PageViewModels
{
    public class HomePageViewModel : BaseViewViewModel
    {

        #region empty ctor

        #endregion

        #region page-loaded event

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
        {

            // i've considered all the alternatives and decided we should do nothing here

            return await Task.FromResult(string.Empty);
        }

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
            return [];
        }

        #endregion

        #region GenesisAsLastKnownGood

/*
    private void SaveGenesisOfThisViewModelAsLastKnownGood()
    {
    }
*/

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
        {

            return false;
        }

        #endregion
    }
}
