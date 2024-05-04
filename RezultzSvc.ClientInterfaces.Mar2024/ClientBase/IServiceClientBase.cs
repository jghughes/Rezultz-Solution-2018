using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.JghExceptions;

namespace RezultzSvc.ClientInterfaces.Mar2024.ClientBase
{
    public interface IServiceClientBase
    {
        /// <summary>
        ///     Either returns True or throws an innermost JghCommunicationFailureException
        ///     with an explanatory message if unable to connect to service for any reason at all.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>Task type bool if AOK</returns>
        /// <exception cref="AggregateException">Async exception wrapper for all exceptions</exception>
        /// <exception cref="JghCommunicationFailureException">
        ///     innermost exception type used for handled exceptions and commonplace
        ///     communication exceptions
        /// </exception>
        Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct);

        Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct);

    }
}