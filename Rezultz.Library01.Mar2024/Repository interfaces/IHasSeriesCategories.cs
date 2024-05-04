using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    public interface IHasSeriesCategories
    {
        Task<string[]> GetRacesFoundInPointsAsync();
        Task<string[]> GetGendersFoundInPointsAsync();
        Task<string[]> GetAgeGroupsFoundInPointsAsync();
        Task<string[]> GetCitiesFoundInPointsAsync();
        Task<string[]> GetTeamsFoundInPointsAsync();
        Task<string[]> GetUtilityClassificationsFoundInPointsAsync();


        Task<string[]> GetRacesFoundInTourAsync();
        Task<string[]> GetGendersFoundInTourAsync();
        Task<string[]> GetAgeGroupsFoundInTourAsync();
        Task<string[]> GetCitiesFoundInTourAsync();
        Task<string[]> GetTeamsFoundInTourAsync();
        Task<string[]> GetUtilityClassificationsFoundInTourAsync();

        Task<SearchQueryItem[]> GetPointsSearchSuggestionsAsync();

        Task<SearchQueryItem[]> GetTourSearchSuggestionsAsync();

    }
}