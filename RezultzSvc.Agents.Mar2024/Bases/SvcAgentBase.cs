using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Interfaces03.Apr2022;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;

namespace RezultzSvc.Agents.Mar2024.Bases;

public class SvcAgentBase : ISvcAgentBase
{
    private const string Locus2 = nameof(SvcAgentBase);
    private const string Locus3 = "[RezultzSvc.Agents.Mar2024]";


    #region ctor stuff

    public SvcAgentBase()
    {
        const string failure = "Unable to instantiate service agent.";
        const string locus = "[SvcAgentBase]";

        try
        {
            // dummy
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

    #region fields

    internal IServiceClientBase ClientBase { get; set; }

    #endregion

    #region methods

    public async Task<bool> ThrowIfNoServiceConnectionAsync(CancellationToken ct = default)
    {
        const string failure = "Testing service availability.";
        const string locus = "[ThrowIfNoServiceConnectionAsync]";

        try
        {
            var answer = await ClientBase.GetIfServiceIsAnsweringAsync(ct); // throws if anything whatsoever fails, whether at this end or the far end

            if (answer)
                return true;

            throw new JghCommunicationFailureException(StringsSvcAgents.UnableToEstablishConnectionWithServer); // belt and braces. can't really foresee how we could ever get here
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to determine if remote server is answering.";
        const string locus = "[GetIfServiceIsAnsweringAsync]";

        try
        {
            var answer = await ClientBase.GetIfServiceIsAnsweringAsync(ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to obtain details of service endpoints on server.";
        const string locus = "[GetServiceEndpointsInfoAsync]";

        try
        {
            var answer = await ClientBase.GetServiceEndpointsInfoAsync(CancellationToken.None);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion
}