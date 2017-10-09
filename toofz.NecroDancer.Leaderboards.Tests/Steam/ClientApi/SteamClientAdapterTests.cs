﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SteamKit2;
using toofz.NecroDancer.Leaderboards.Steam.ClientApi;

namespace toofz.NecroDancer.Leaderboards.Tests.Steam.ClientApi
{
    class SteamClientAdapterTests
    {
        [TestClass]
        public class Constructor
        {
            [TestMethod]
            public void SteamClientIsNull_ThrowsArgumentNullException()
            {
                // Arrange
                ISteamClient steamClient = null;
                var manager = Mock.Of<ICallbackManager>();

                // Act -> Assert
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new SteamClientAdapter(steamClient, manager);
                });
            }

            [TestMethod]
            public void ManagerIsNull_ThrowsArgumentNullException()
            {
                // Arrange
                var steamClient = Mock.Of<ISteamClient>();
                ICallbackManager manager = null;

                // Act -> Assert
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new SteamClientAdapter(steamClient, manager);
                });
            }

            [TestMethod]
            public void ReturnsInstance()
            {
                // Arrange
                var steamClient = Mock.Of<ISteamClient>();
                var manager = Mock.Of<ICallbackManager>();

                // Act
                var client = new SteamClientAdapter(steamClient, manager);

                // Assert
                Assert.IsInstanceOfType(client, typeof(SteamClientAdapter));
            }

            [TestMethod]
            public void StartsMessageLoop()
            {
                // Arrange
                var steamClient = Mock.Of<ISteamClient>();
                var manager = Mock.Of<ICallbackManager>();

                // Act
                var client = new SteamClientAdapter(steamClient, manager);

                // Assert
                Assert.IsTrue(client.MessageLoop.IsAlive);
            }
        }

        [TestClass]
        public class IsLoggedOnProperty
        {
            public IsLoggedOnProperty()
            {
                mockSteamClient = new Mock<ISteamClient>();
                var steamClient = mockSteamClient.Object;
                var manager = Mock.Of<ICallbackManager>();
                client = new SteamClientAdapter(steamClient, manager);
            }

            Mock<ISteamClient> mockSteamClient;
            SteamClientAdapter client;

            [TestMethod]
            public void SessionIDIsNull_ReturnsFalse()
            {
                // Arrange
                mockSteamClient.SetupGet(s => s.SessionID).Returns((int?)null);

                // Act
                var isLoggedOn = client.IsLoggedOn;

                // Assert
                Assert.IsFalse(isLoggedOn);
            }

            [TestMethod]
            public void SessionIDIsNotNull_ReturnsTrue()
            {
                // Arrange
                mockSteamClient.SetupGet(s => s.SessionID).Returns(1);

                // Act
                var isLoggedOn = client.IsLoggedOn;

                // Assert
                Assert.IsTrue(isLoggedOn);
            }
        }

        [TestClass]
        public class IsConnectedProperty
        {
            public IsConnectedProperty()
            {
                mockSteamClient = new Mock<ISteamClient>();
                var steamClient = mockSteamClient.Object;
                var manager = Mock.Of<ICallbackManager>();
                client = new SteamClientAdapter(steamClient, manager);
            }

            Mock<ISteamClient> mockSteamClient;
            SteamClientAdapter client;

            [TestMethod]
            public void IsNotConnected_ReturnsFalse()
            {
                // Arrange
                mockSteamClient.SetupGet(s => s.IsConnected).Returns(false);

                // Act
                var isConnected = client.IsConnected;

                // Assert
                Assert.IsFalse(isConnected);
            }

            [TestMethod]
            public void IsConnected_ReturnsTrue()
            {
                // Arrange
                mockSteamClient.SetupGet(s => s.IsConnected).Returns(true);

                // Act
                var isConnected = client.IsConnected;

                // Assert
                Assert.IsTrue(isConnected);
            }
        }
    }
}
