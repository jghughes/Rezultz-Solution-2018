using Microsoft.AspNetCore.Mvc;
using RezultzSvc.Mvc.Bases;

namespace RezultzSvc.Mvc.Controller_Interfaces
{
    public interface IRegistrationController : IControllerBaseJgh
    {
        Task<IActionResult> GetIfContainerExistsAsync(string databaseAccount, string dataContainer);

        Task<IActionResult> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, string participantItemAsJson);
   
        Task<IActionResult> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, byte[] participantItemArrayAsJsonAsCompressedBytes);

        Task<IActionResult> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey);

        Task<IActionResult> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer);

    }
}