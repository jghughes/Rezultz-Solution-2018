using Microsoft.AspNetCore.Mvc;
using RezultzSvc.WebApp02.Controller_Base;

namespace RezultzSvc.WebApp02.Controller_Interfaces
{
    public interface IParticipantRegistrationController : IControllerBaseJgh
    {
        Task<IActionResult> GetIfContainerExistsAsync(string databaseAccount, string dataContainer);

        Task<IActionResult> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, string participantItemAsJson);
   
        Task<IActionResult> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, byte[] participantItemArrayAsJsonAsCompressedBytes);

        Task<IActionResult> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey);

        Task<IActionResult> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer);

    }
}