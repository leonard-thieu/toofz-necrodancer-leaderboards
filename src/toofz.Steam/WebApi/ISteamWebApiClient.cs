﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using toofz.Steam.WebApi.ISteamRemoteStorage;
using toofz.Steam.WebApi.ISteamUser;

namespace toofz.Steam.WebApi
{
    /// <summary>
    /// HTTP client used for interacting with Steam Web API.
    /// </summary>
    public interface ISteamWebApiClient : IDisposable
    {
        /// <summary>
        /// A Steam Web API key. This is required by some API endpoints.
        /// </summary>
        string SteamWebApiKey { get; set; }

        /// <summary>
        /// Returns basic profile information for a list of 64-bit Steam IDs.
        /// </summary>
        /// <param name="steamIds">
        /// List of 64 bit Steam IDs to return profile information for. Up to 100 Steam IDs can be requested.
        /// </param>
        /// <param name="progress">
        /// A progress provider that will be called with total bytes requested. <paramref name="progress"/> may be null.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="GetPlayerSummariesAsync"/> requires <see cref="SteamWebApiKey"/> to be set to a valid Steam Web API Key.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="steamIds"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Unable to request more than 100 player summaries.
        /// </exception>
        Task<PlayerSummariesEnvelope> GetPlayerSummariesAsync(
            IEnumerable<long> steamIds,
            IProgress<long> progress = default,
            CancellationToken cancellationToken = default);
        /// <summary>
        /// Returns file details for a UGC ID.
        /// </summary>
        /// <param name="appId">
        /// The ID of the product of the UGC.
        /// </param>
        /// <param name="ugcId">
        /// The ID of the UGC to get file details for.
        /// </param>
        /// <param name="progress">
        /// A progress provider that will be called with total bytes requested. <paramref name="progress"/> may be null.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="GetUgcFileDetailsAsync"/> requires <see cref="SteamWebApiKey"/> to be set to a valid Steam Web API Key.
        /// </exception>
        Task<UgcFileDetailsEnvelope> GetUgcFileDetailsAsync(
            uint appId,
            long ugcId,
            IProgress<long> progress = default,
            CancellationToken cancellationToken = default);
    }
}