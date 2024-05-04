using System.Threading.Tasks;
using NetStd.ViewModels01.April2022.UserControls;

namespace Rezultz.Uwp.UserSettings
{
    public interface IUserSettingsServiceViewModel
    {
        public bool ThisViewModelIsInitialised { get; }

        #region method called on first time load and initialisation

        Task<string> BeInitialisedOrchestrateAsync();

        #endregion

        #region textbox

        TextBoxControlViewModel TextBoxForEnteringTargetParticipantIdVm { get; }

        #endregion

        #region buttons

        ButtonControlViewModel SaveTargetParticipantIdInStorageButtonVm { get; }
        ButtonControlViewModel ClearTargetParticipantIdSavedInStorageButtonVm { get; }

        #endregion

        #region ToggleSwitches

        ButtonControlViewModel MustShowOnlySingleCategoryOfResultsOnLaunchButtonVm { get; }

        ButtonControlViewModel MustUsePreviewNotPublishedDataOnLaunchButtonVm { get; }

        ButtonControlViewModel MustDisplayConciseLeaderboardColumnsOnlyButtonVm { get; }

        #endregion
    }
}
