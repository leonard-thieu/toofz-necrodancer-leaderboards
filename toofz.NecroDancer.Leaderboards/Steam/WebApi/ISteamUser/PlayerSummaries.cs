﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace toofz.NecroDancer.Leaderboards.Steam.WebApi.ISteamUser
{
    public sealed class PlayerSummaries
    {
        /// <summary>
        /// A list of profile objects. Contained information varies depending on whether or not the user has their profile 
        /// set to Friends only or Private.
        /// </summary>
        [DataMember(Name = "players")]
        public IEnumerable<PlayerSummary> Players { get; } = new List<PlayerSummary>();
    }
}
