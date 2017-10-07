﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace toofz.NecroDancer.Leaderboards.Steam.CommunityData
{
    public interface ISteamCommunityDataClient : IDisposable
    {
        Task<LeaderboardEntriesEnvelope> GetLeaderboardEntriesAsync(
            string communityGameName,
            int leaderboardId,
            int? startRange = null,
            int? endRange = null,
            IProgress<long> progress = null,
            CancellationToken cancellationToken = default);
        Task<LeaderboardEntriesEnvelope> GetLeaderboardEntriesAsync(
            uint appId,
            int leaderboardId,
            int? startRange = null,
            int? endRange = null,
            IProgress<long> progress = null,
            CancellationToken cancellationToken = default);
        Task<LeaderboardsEnvelope> GetLeaderboardsAsync(
            string communityGameName,
            IProgress<long> progress = null,
            CancellationToken cancellationToken = default);
        Task<LeaderboardsEnvelope> GetLeaderboardsAsync(
            uint appId,
            IProgress<long> progress = null,
            CancellationToken cancellationToken = default);
    }
}