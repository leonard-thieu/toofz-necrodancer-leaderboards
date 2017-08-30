﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using toofz.NecroDancer.Leaderboards.Steam.WebApi.ISteamRemoteStorage;
using toofz.NecroDancer.Leaderboards.Tests.Properties;

namespace toofz.NecroDancer.Leaderboards.Tests.Steam.WebApi.ISteamRemoteStorage
{
    class UgcFileDetailsTests
    {
        [TestClass]
        public class Deserialization
        {
            [TestMethod]
            public void Deserializes()
            {
                // Arrange
                var json = Resources.UgcFileDetails;

                // Act
                var ugcFileDetails = JsonConvert.DeserializeObject<UgcFileDetailsEnvelope>(json);
                var data = ugcFileDetails.Data;

                // Assert
                Assert.IsInstanceOfType(data, typeof(UgcFileDetails));
                Assert.AreEqual("2/9/2014_score191_zone1_level2", data.FileName);
                Assert.AreEqual("http://cloud-3.steamusercontent.com/ugc/22837952671856412/756063F4E07B686916257652BBEB972C3C9E6F8D/", data.Url);
                Assert.AreEqual(1558, data.Size);
            }
        }
    }
}
