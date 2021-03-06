﻿using System;
using System.Diagnostics.CodeAnalysis;
using SteamKit2;

namespace toofz.Steam.ClientApi
{
    internal sealed class ProgressDebugNetworkListener : IDebugNetworkListener
    {
        public IProgress<long> Progress { get; set; }

        /// <summary>
        /// Called when a packet is received from the Steam server
        /// </summary>
        /// <param name="msgType">Network message type of this packet message.</param>
        /// <param name="data">Raw packet data that was received.</param>
        public void OnIncomingNetworkMessage(EMsg msgType, byte[] data)
        {
            Progress?.Report(data.Length);
        }

        /// <summary>
        /// Called when a packet is about to be sent to the Steam server.
        /// </summary>
        /// <param name="msgType">Network message type of this packet message.</param>
        /// <param name="data">Raw packet data that will be sent.</param>
        [ExcludeFromCodeCoverage]
        public void OnOutgoingNetworkMessage(EMsg msgType, byte[] data)
        {
            // Do nothing
        }
    }
}
