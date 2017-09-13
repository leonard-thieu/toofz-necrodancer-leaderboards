﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using toofz.NecroDancer.Leaderboards.Steam.WebApi.ISteamUser;
using toofz.NecroDancer.Leaderboards.Tests.Properties;

namespace toofz.NecroDancer.Leaderboards.Tests.Steam.WebApi.ISteamUser
{
    class PlayerSummaryTests
    {
        [TestClass]
        public class Deserialization
        {
            [TestMethod]
            public void Deserializes()
            {
                // Arrange
                var json = Resources.PlayerSummaries;

                // Act
                var playerSummaries = JsonConvert.DeserializeObject<PlayerSummariesEnvelope>(json);
                var response = playerSummaries.Response;
                var players = response.Players;

                // Assert
                Assert.AreEqual(1, players.Count());
                var player = players.First();
                Assert.AreEqual(76561197960435530, player.SteamId);
                Assert.AreEqual("Robin", player.PersonaName);
                Assert.AreEqual("https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4.jpg", player.Avatar);
            }
        }
    }
}