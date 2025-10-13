using System;
using System.Collections.Generic;
using System.IO;
using ChesterDevs.Core.Aspnet.App;
using ChesterDevs.Core.Aspnet.App.RemoteData;
using ChesterDevs.Core.Aspnet.App.RemoteData.EventData;
using ChesterDevs.Core.Aspnet.App.Secrets;
using FluentAssertions;
using LazyCache;
using LazyCache.Mocks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.App.RemoteData
{
    public class EventbriteEventListingDataTests
    {
        [Fact]
        public void Given_CacheIsEmpty_When_GetData_Then_ReturnsNull()
        {
            var builder = new Builder();

            var objectUnderTest = builder.Build();

            var result = objectUnderTest.GetData();

            result.Should().BeNull();
        }

        [Fact]
        public void Given_CacheIsNotEmpty_When_GetData_Then_ReturnsData()
        {
            var builder = new Builder()
                .SetupCache();

            var objectUnderTest = builder.Build();

            var result = objectUnderTest.GetData();

            result.Should().NotBeNull();
        }

        [Fact]
        public void Given_AllIsWell_When_RefreshDataCalled_Then_CorrectDataIsAddedToCache()
        {
            var builder = new Builder()
                .SetupHttpWrapper()
                .SetupSecretManager();

            var objectUnderTest = builder.Build();

            objectUnderTest.RefreshData();

            builder.CacheMock.Verify(m => m.Add(
                    nameof(EventbriteEventListingData), 
                It.Is<List<EventItem>>(o => 
                    o != null
                    && o.Count == 2),
                It.IsAny<MemoryCacheEntryOptions>()), 
                Times.Once, 
                "Parsed event list should not be null or empty");

            builder.CacheMock.Verify(m => m.Add(
                    nameof(EventbriteEventListingData),
                It.Is<List<EventItem>>(o =>
                    o[0].Name == "Event 1 Name"
                    && o[0].Link == "https://www.eventbrite.co.uk/e/link1"
                    && o[0].Description == "Event 1 summary"
                    && o[0].EventDate == new DateTime(2020, 02, 05)
                    && o[0].Time == "19:00"
                    && o[0].PhotoLink == "https://img.evbuc.com/image1"),
                It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Event 1 has not been parsed correctly");
            
            builder.CacheMock.Verify(m => m.Add(
                    nameof(EventbriteEventListingData),
                    It.Is<List<EventItem>>(o =>
                        o[1].Name == "Event 2 Name"
                        && o[1].Link == "https://www.eventbrite.co.uk/e/link2"
                        && o[1].Description == "Event 2 summary"
                        && o[1].EventDate == new DateTime(2020, 03, 04)
                        && o[1].Time == "19:00"
                        && o[1].PhotoLink == "https://img.evbuc.com/image2"),
                    It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Event 2 has not been parsed correctly");

            builder.FileSystemWrapperMock.Verify(m => m.SaveFile(It.IsAny<string>(), It.IsAny<string>()));
        }

        class Builder
        {
            public Mock<IAppCache> CacheMock = new Mock<IAppCache>();
            public HttpWrapperFake HttpWrapperFake = new HttpWrapperFake();
            public Mock<ILogger<EventbriteEventListingData>> LoggerMock = new Mock<ILogger<EventbriteEventListingData>>();
            public Mock<IFileSystemWrapper> FileSystemWrapperMock = new Mock<IFileSystemWrapper>();
            public Mock<ISecretManager> SecretManager = new Mock<ISecretManager>();

            public Builder SetupHttpWrapper()
            {
                HttpWrapperFake.SetupGetData(File.OpenRead("Data/events.json"));
                return this;
            }

            public Builder SetupCache()
            {
                CacheMock.Setup(m => m.GetOrAdd<List<EventItem>>(nameof(EventbriteEventListingData), It.IsAny<Func<ICacheEntry, List<EventItem>>>()))
                    .Returns(new List<EventItem>());

                return this;
            }

            public Builder SetupSecretManager()
            {
                SecretManager.SetupGet(m => m.Secrets)
                    .Returns(new SecretData() { EventBriteApiKey = "EventBriteApiKey" });
                return this;
            }

            public EventbriteEventListingData Build()
                => new EventbriteEventListingData(HttpWrapperFake, CacheMock.Object, LoggerMock.Object, FileSystemWrapperMock.Object, SecretManager.Object);
        }

    }
}