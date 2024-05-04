using Microsoft.AspNetCore.Mvc;
using RezultzSvc.WebApp02.Controller_Base;

namespace RezultzSvc.WebApp02.Controller_Interfaces
{
	public interface ITimeKeepingController : IControllerBaseJgh
	{
        Task<IActionResult> GetIfContainerExistsAsync(string databaseAccount, string dataContainer);

        Task<IActionResult> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, string timeStampItemAsJson);

        Task<IActionResult> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, byte[] timeStampItemArrayAsJsonAsCompressedBytes);

        Task<IActionResult> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey);
        
        Task<IActionResult> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer);
    }
}