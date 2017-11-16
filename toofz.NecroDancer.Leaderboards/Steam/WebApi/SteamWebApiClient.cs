﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.ApplicationInsights;
using Polly;
using toofz.NecroDancer.Leaderboards.Steam.WebApi.ISteamRemoteStorage;
using toofz.NecroDancer.Leaderboards.Steam.WebApi.ISteamUser;

namespace toofz.NecroDancer.Leaderboards.Steam.WebApi
{
    public sealed class SteamWebApiClient : ISteamWebApiClient
    {
        /// <summary>
        /// Gets a retry strategy for <see cref="SteamWebApiClient"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="PolicyBuilder"/> configured with a retry strategy appropriate for <see cref="SteamWebApiClient"/>.
        /// </returns>
        public static PolicyBuilder GetRetryStrategy()
        {
            return Policy
                .Handle<HttpRequestStatusException>(ex =>
                {
                    // https://partner.steamgames.com/doc/webapi_overview/responses#status_codes
                    switch ((int)ex.StatusCode)
                    {
                        case 408:   // Request Timeout
                        case 429:   // Too Many Requests        You are being rate limited.
                        case 500:   // Internal Server Error    An unrecoverable error has occurred, please try again.
                        case 502:   // Bad Gateway
                        case 503:   // Service Unavailable      Server is temporarily unavailable, or too busy to respond. Please wait and try again later.
                        case 504:   // Gateway Timeout
                            return true;
                        default:
                            return false;
                    }
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamWebApiClient"/> class with a specific handler.
        /// </summary>
        /// <param name="handler">The HTTP handler stack to use for sending requests.</param>
        /// <param name="telemetryClient">The telemetry client to use for reporting telemetry.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="telemetryClient"/> is null.
        /// </exception>
        public SteamWebApiClient(HttpMessageHandler handler, TelemetryClient telemetryClient)
        {
            http = new ProgressReporterHttpClient(handler, true, telemetryClient) { BaseAddress = new Uri("https://api.steampowered.com/") };
        }

        private readonly ProgressReporterHttpClient http;

        /// <summary>
        /// A Steam Web API key. This is required by some API endpoints.
        /// </summary>
        public string SteamWebApiKey { get; set; }

        #region GetPlayerSummaries

        /// <summary>
        /// The maximum number of Steam IDs allowed per request by <see cref="GetPlayerSummariesAsync"/>.
        /// </summary>
        public const int MaxPlayerSummariesPerRequest = 100;

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
        /// Unable to request more than <see cref="MaxPlayerSummariesPerRequest"/> player summaries.
        /// </exception>
        public async Task<PlayerSummariesEnvelope> GetPlayerSummariesAsync(
            IEnumerable<long> steamIds,
            IProgress<long> progress = default,
            CancellationToken cancellationToken = default)
        {
            if (SteamWebApiKey == null)
                throw new InvalidOperationException($"{nameof(GetPlayerSummariesAsync)} requires {nameof(SteamWebApiKey)} to be set to a valid Steam Web API Key.");
            if (steamIds == null)
                throw new ArgumentNullException(nameof(steamIds), $"{nameof(steamIds)} is null.");
            if (steamIds.Count() > MaxPlayerSummariesPerRequest)
                throw new ArgumentException($"Unable to request more than {MaxPlayerSummariesPerRequest} player summaries.", nameof(steamIds));

            var requestUri = "ISteamUser/GetPlayerSummaries/v0002"
                .SetQueryParams(new
                {
                    key = SteamWebApiKey,
                    steamids = string.Join(",", steamIds),
                });
            var response = await http.GetAsync("Get player summaries", requestUri, progress, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadAsAsync<PlayerSummariesEnvelope>().ConfigureAwait(false);
        }

        #endregion

        #region GetUGCFileDetails

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
        public async Task<UgcFileDetailsEnvelope> GetUgcFileDetailsAsync(
            uint appId,
            long ugcId,
            IProgress<long> progress = default,
            CancellationToken cancellationToken = default)
        {
            if (SteamWebApiKey == null)
                throw new InvalidOperationException($"{nameof(GetUgcFileDetailsAsync)} requires {nameof(SteamWebApiKey)} to be set to a valid Steam Web API Key.");

            var requestUri = "ISteamRemoteStorage/GetUGCFileDetails/v1"
                .SetQueryParams(new
                {
                    key = SteamWebApiKey,
                    appid = appId,
                    ugcid = ugcId,
                });
            var response = await http.GetAsync("Get UGC file details", requestUri, progress, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadAsAsync<UgcFileDetailsEnvelope>().ConfigureAwait(false);
        }

        #endregion

        #region IDisposable Implementation

        private bool disposed;

        public void Dispose()
        {
            if (disposed) { return; }

            http.Dispose();

            disposed = true;
        }

        #endregion
    }
}