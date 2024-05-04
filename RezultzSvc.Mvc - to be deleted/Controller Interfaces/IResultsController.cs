using Microsoft.AspNetCore.Mvc;
using RezultzSvc.Mvc.Bases;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace RezultzSvc.Mvc.Controller_Interfaces
{
    public interface IResultsController : IControllerBaseJgh
    {
        Task<IActionResult> GetIfSeasonIdIsRecognisedAsync(string seasonId);
        
        Task<IActionResult> GetSeasonItemAsync(string seasonId);

        Task<IActionResult> GetSeasonItemArrayAsync();

        Task<IActionResult> PopulateSingleEventWithPreprocessedResultsAsync(byte[] eventItemAsJsonAsCompressedBytes);

        Task<IActionResult> PopulateAllEventsInSeriesWithAllPreprocessedResultsAsync(byte[] seriesItemAsJsonAsCompressedBytes);
    }
}