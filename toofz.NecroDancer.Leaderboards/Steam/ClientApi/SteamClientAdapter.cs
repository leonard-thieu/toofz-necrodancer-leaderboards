﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using SteamKit2;
using static SteamKit2.SteamClient;
using static SteamKit2.SteamUser;

namespace toofz.NecroDancer.Leaderboards.Steam.ClientApi
{
    sealed class SteamClientAdapter : ISteamClientAdapter
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(SteamClientAdapter));

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamClientAdapter"/> class.
        /// </summary>
        /// <param name="steamClient">
        /// The Steam client.
        /// </param>
        /// <param name="manager">
        /// The callback manager associated with <paramref name="steamClient"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="steamClient"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="manager"/> is null.
        /// </exception>
        public SteamClientAdapter(ISteamClient steamClient, ICallbackManager manager)
        {
            this.steamClient = steamClient ?? throw new ArgumentNullException(nameof(steamClient), $"{nameof(steamClient)} is null.");
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager), $"{nameof(manager)} is null.");
            MessageLoop = new Thread(() =>
            {
                while (true)
                {
                    this.manager.RunWaitCallbacks();
                }
            });
            MessageLoop.Start();
        }

        readonly ISteamClient steamClient;
        readonly ICallbackManager manager;

        internal Thread MessageLoop { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is logged on to the remote CM server.
        /// </summary>
        public bool IsLoggedOn => steamClient.SessionID != null;

        /// <summary>
        /// Gets or sets the network listening interface.
        /// </summary>
        public ProgressDebugNetworkListener ProgressDebugNetworkListener
        {
            get => steamClient.DebugNetworkListener as ProgressDebugNetworkListener;
            set => steamClient.DebugNetworkListener = value;
        }

        /// <summary>
        /// Connects this client to a Steam3 server. This begins the process of connecting
        /// and encrypting the data channel between the client and the server. Results are
        /// returned asynchronously in a <see cref="ConnectedCallback"/>. If the
        /// server that SteamKit attempts to connect to is down, a <see cref="DisconnectedCallback"/>
        /// will be posted instead. SteamKit will not attempt to reconnect to Steam, you
        /// must handle this callback and call Connect again preferrably after a short delay.
        /// </summary>
        /// <param name="cmServer">
        /// The <see cref="IPEndPoint"/> of the CM server to connect to. If null, SteamKit will
        /// randomly select a CM server from its internal list.
        /// </param>
        public Task<ConnectedCallback> ConnectAsync(IPEndPoint cmServer = null)
        {
            var tcs = new TaskCompletionSource<ConnectedCallback>();

            IDisposable onConnected = null;
            IDisposable onDisconnected = null;
            onConnected = manager.Subscribe<ConnectedCallback>(response =>
            {
                switch (response.Result)
                {
                    case EResult.OK:
                        {
                            Log.Info("Connected to Steam.");
                            tcs.TrySetResult(response);
                            break;
                        }
                    default:
                        {
                            tcs.TrySetException(new SteamClientApiException($"Unable to connect to Steam.", response.Result));
                            break;
                        }
                }

                onConnected.Dispose();
                onDisconnected.Dispose();

                onDisconnected = manager.Subscribe<DisconnectedCallback>(_ =>
                {
                    Log.Info("Disconnected from Steam.");
                    onDisconnected.Dispose();
                });
            });
            onDisconnected = manager.Subscribe<DisconnectedCallback>(response =>
            {
                tcs.TrySetException(new SteamClientApiException("Unable to connect to Steam."));
                onConnected.Dispose();
                onDisconnected.Dispose();
            });

            steamClient.Connect(cmServer);

            return tcs.Task;
        }

        /// <summary>
        /// Logs the client into the Steam3 network. The client should already have been
        /// connected at this point. Results are returned in a <see cref="LoggedOnCallback"/>.
        /// </summary>
        /// <param name="details">The details to use for logging on.</param>
        /// <exception cref="ArgumentNullException">
        /// No logon details were provided.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Username or password are not set within details.
        /// </exception>
        public Task<LoggedOnCallback> LogOnAsync(LogOnDetails details)
        {
            var tcs = new TaskCompletionSource<LoggedOnCallback>();

            IDisposable onLoggedOn = null;
            IDisposable onDisconnected = null;
            onLoggedOn = manager.Subscribe<LoggedOnCallback>(response =>
            {
                switch (response.Result)
                {
                    case EResult.OK:
                        {
                            Log.Info("Logged on to Steam.");
                            tcs.TrySetResult(response);
                            break;
                        }
                    default:
                        {
                            var ex = new SteamClientApiException("Unable to logon to Steam.", response.Result);
                            tcs.TrySetException(ex);
                            break;
                        }
                }

                onLoggedOn.Dispose();
                onDisconnected.Dispose();
            });
            onDisconnected = manager.Subscribe<DisconnectedCallback>(response =>
            {
                tcs.TrySetException(new SteamClientApiException("Unable to connect to Steam."));
                onLoggedOn.Dispose();
                onDisconnected.Dispose();
            });

            steamClient.GetHandler<SteamUser>().LogOn(details);

            return tcs.Task;
        }

        /// <summary>
        /// Returns a registered handler for <see cref="SteamUserStats"/>.
        /// </summary>
        /// <returns>A registered handler on success, or null if the handler could not be found.</returns>
        public ISteamUserStats GetSteamUserStats() => steamClient.GetHandler<SteamUserStats>();

        /// <summary>
        /// Gets a value indicating whether this instance is connected to the remote CM server.
        /// </summary>
        public bool IsConnected => steamClient.IsConnected;

        /// <summary>
        /// Disconnects this client.
        /// </summary>
        public void Disconnect() => steamClient.Disconnect();
    }
}
