using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace NetStd.OnBoardServices02.July2018.UserSettingsForRezultz
{
	public interface IThingsPersistedInLocalStorage
    {
        Task<int> GetSeasonMetadataIdAsync();
        Task<bool> SaveSeasonMetadataIdAsync(int id);


        // only for portal

        Task<string> GetTimingDataPreProcessorIdAsync();
        Task<bool> SaveTimingDataPreProcessorIdAsync(string converterCodeName);


        // this non-async method is a special case. we can't use any async methods inside Navigation methods. Navigation MUST be done on the UI thread and not from within a Task on a non-UI thread 
        Dictionary<string, string> GetNavigationContextQueryString();
        Task<bool> SaveNavigationContextQueryStringAsync(Dictionary<string, string> queryStringDictionary);

        Task<bool> GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync();
        Task<bool> SaveMustSelectAllRacesOnFirstTimeThroughForAnEventAsync(bool valueToBeSaved);

        Task<bool> GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync();
        Task<bool> SaveMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync(bool valueToBeSaved);

        Task<bool> GetMustUsePreviewDataOnLaunchAsync();
        Task<bool> SaveMustUsePreviewDataOnLaunchAsync(bool valueToBeSaved);

        Task<bool> GetMustDisplayConciseLeaderboardColumnsOnlyAsync();
        Task<bool> SaveMustDisplayConciseLeaderboardColumnsOnlyAsync(bool valueToBeSaved);

        Task<string> GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync();
        Task<bool> SaveTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync(string converterCodeName);


        Task<ThingWithNamesItem[]> GetFavoritesListIdentitiesAsync();
        Task<bool> SaveFavoritesListIdentitiesAsync(ThingWithNamesItem[] thingsWithNames);


        Task<bool> ClearAllSettingsAsync();

    }
}