using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;


// ReSharper disable UnassignedGetOnlyAutoProperty

namespace RezultzSvc.Agents.Mar2024.Dummies
{
    public  class LeaderboardResultsSvcAgentDummy : ILeaderboardResultsSvcAgent
    {
        public  Task<bool> ThrowIfNoServiceConnectionAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public  Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public  Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public  Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public  Task<SeasonProfileItem> GetSeasonProfileAsync(string profileFileNameFragment, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<SeasonProfileItem[]> GetAllSeasonProfilesAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<EventProfileItem> PopulateSingleEventWithResultsAsync(string databaseAccount, string dataContainer, EventProfileItem eventProfile, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<SeriesProfileItem> PopulateAllEventsInSingleSeriesWithAllResultsAsync(string databaseAccount, string dataContainer, SeriesProfileItem seriesProfile, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}