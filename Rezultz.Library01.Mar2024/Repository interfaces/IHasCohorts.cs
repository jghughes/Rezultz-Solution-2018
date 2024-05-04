using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.RezultzItems;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    public interface IHasCohorts
    {
        Task<PopulationCohortItem[]> GetRaceCohortsFoundAsync();
        Task<PopulationCohortItem[]> GetGenderCohortsFoundAsync();
        Task<PopulationCohortItem[]> GetAgeGroupCohortsFoundAsync();
        Task<PopulationCohortItem[]> GetCityCohortsFoundAsync();

    }
}