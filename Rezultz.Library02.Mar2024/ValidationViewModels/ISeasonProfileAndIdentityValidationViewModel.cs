using System.Threading.Tasks;
using NetStd.ViewModels01.April2022.Collections;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;

namespace Rezultz.Library02.Mar2024.ValidationViewModels
{
    // this interface is a tiny subset of all the props/methods on the vm. these are the only ones we use in Rezultz.Uwp
// which is the only app where we use the interface in the dependency injection container to create a single global vm
// that is common to all pages and their vms
    public interface ISeasonProfileAndIdentityValidationViewModel
    {
        bool ThisViewModelIsInitialised { get; }

        SeasonProfileItem CurrentlyValidatedSeasonProfileItem { get; }
        IdentityItem CurrentlyAuthenticatedIdentityItem { get; }

        IndexDrivenCollectionViewModel<SeriesItemDisplayObject> CboLookupSeriesVm { get; }
        IndexDrivenCollectionViewModel<EventItemDisplayObject> CboLookupEventVm { get; }

        string CurrentRequiredWorkRole { get; set; } // set in the page viewmodel that owns this viewmodel upon instantiation

        Task<string> BeInitialisedForRezultzOrchestrateAsync();
        bool GetIfCurrentlyAuthenticatedIdentityUserIsAuthorisedForRequiredWorkRole();








    }
}