using System.Threading.Tasks;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    public interface IHasSingleEventCategories
    {
        Task<string[]> GetRacesFoundAsync();
        Task<string[]> GetGendersFoundAsync();
        Task<string[]> GetAgeGroupsFoundAsync();
        Task<string[]> GetCitiesFoundAsync();
        Task<string[]> GetTeamsFoundAsync();
        Task<string[]> GetUtilityClassificationsFoundAsync();

        //Task<SearchQueryItem[]> GetSearchQuerySuggestionsAsync();
    }

}
