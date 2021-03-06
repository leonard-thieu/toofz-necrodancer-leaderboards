﻿using System;
using System.Net;
using Xunit;

namespace toofz.Steam.Tests
{
    public class HttpRequestStatusExceptionTests
    {
        public class Constructor
        {
            [DisplayFact("RequestUri", nameof(ArgumentNullException))]
            public void RequestUriIsNull_ThrowsArgumentNullException()
            {
                // Arrange
                var statusCode = HttpStatusCode.BadGateway;
                Uri requestUri = null;

                // Act -> Assert
                Assert.Throws<ArgumentNullException>(() =>
                {
                    var ex = new HttpRequestStatusException(statusCode, requestUri);
                });
            }

            [DisplayFact(nameof(HttpRequestStatusException.StatusCode))]
            public void SetsStatusCode()
            {
                // Arrange
                var statusCode = HttpStatusCode.BadGateway;
                var requestUri = new Uri("http://localhost/");

                // Act
                var ex = new HttpRequestStatusException(statusCode, requestUri);

                // Assert
                Assert.Equal(statusCode, ex.StatusCode);
            }

            [DisplayFact(nameof(HttpRequestStatusException.RequestUri))]
            public void SetsRequestUri()
            {
                // Arrange
                var statusCode = HttpStatusCode.BadGateway;
                var requestUri = new Uri("http://localhost/");

                // Act
                var ex = new HttpRequestStatusException(statusCode, requestUri);

                // Assert
                Assert.Equal(requestUri, ex.RequestUri);
            }

            [DisplayFact(nameof(HttpRequestStatusException))]
            public void ReturnsHttpRequestStatusException()
            {
                // Arrange
                var statusCode = HttpStatusCode.BadGateway;
                var requestUri = new Uri("http://localhost/");

                // Act
                var ex = new HttpRequestStatusException(statusCode, requestUri);

                // Assert
                Assert.IsAssignableFrom<HttpRequestStatusException>(ex);
            }
        }
    }
}
