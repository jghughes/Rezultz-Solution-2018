using Microsoft.AspNetCore.Mvc;
using RezultzSvc.WebApp02.Controller_Base;

// ReSharper disable InconsistentNaming

namespace RezultzSvc.WebApp02.Controller_Interfaces
{
    public interface IAzureStorageController : IControllerBaseJgh

    {
        Task<IActionResult> GetIfContainerExistsAsync(string account, string container);

        Task<IActionResult> GetNamesOfBlobsInContainerAsync(string account, string container, string requiredSubstring, bool mustPrettyPrintMetadataNotOnlyBlobName);

        Task<IActionResult> GetIfBlobExistsAsync(string account, string container, string blob);

        Task<IActionResult> GetAbsoluteUriOfBlockBlobAsync(string account, string container, string blob);

        Task<IActionResult> DeleteBlockBlobIfExistsAsync(string account, string container, string blob);

        Task<IActionResult> UploadStringToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, string stringToUpload);

        Task<IActionResult> UploadBytesToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, byte[] bytes);

        Task<IActionResult> DownloadBlockBlobAsBytesAsync(string account, string container, string blob);



    }
}