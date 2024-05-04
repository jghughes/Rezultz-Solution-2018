using Microsoft.AspNetCore.Mvc;
using RezultzSvc.WebApp02.Controller_Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace RezultzSvc.WebApp02.Controller_Interfaces
{
    public interface ILeaderboardResultsController : IControllerBaseJgh
    {
        Task<IActionResult> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string seasonId);
        
        Task<IActionResult> GetSeasonProfileAsync(string seasonId);

        Task<IActionResult> GetAllSeasonProfilesAsync();

        Task<IActionResult> PopulateSingleEventWithResultsAsync(byte[] eventItemAsJsonAsCompressedBytes);

        Task<IActionResult> PopulateAllEventsInSingleSeriesWithAllResultsAsync(byte[] seriesItemAsJsonAsCompressedBytes);
    }
}