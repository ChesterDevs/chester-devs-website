using System;
using System.Collections.Generic;
using System.IO;
using ChesterDevs.Core.Aspnet.App;
using ChesterDevs.Core.Aspnet.App.RemoteData;
using ChesterDevs.Core.Aspnet.App.RemoteData.Models;
using FluentAssertions;
using LazyCache;
using LazyCache.Mocks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.App.RemoteData
{
    public class EventListingDataTests
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
                .SetupHttpWrapper();

            var objectUnderTest = builder.Build();

            objectUnderTest.RefreshData();

            builder.CacheMock.Verify(m => m.Add(
                    nameof(EventListingData), 
                It.Is<List<EventItem>>(o => 
                    o != null
                    && o.Count == 2),
                It.IsAny<MemoryCacheEntryOptions>()), 
                Times.Once, 
                "Parsed event list should not be null or empty");

            builder.CacheMock.Verify(m => m.Add(
                    nameof(EventListingData),
                It.Is<List<EventItem>>(o =>
                    o[0].Name == "Event 1 Name"
                    && o[0].Link == "https://www.meetup.com/Chester-Devs/events/267928336/"
                    && o[0].Description == "Event 1 description"
                    && o[0].EventDate == new DateTime(2020,02, 05)
                    && o[0].Time == "19:00"
                    && o[0].PhotoLink == "https://secure.meetupstatic.com/photos/event/3/7/f/2/600_488174322.jpeg"
                    && o[0].VenueName == "Venue 1 Name"
                    && o[0].VenueCity == "Venue 1 Town"),
                It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Event 1 has not been parsed correctly");
            
            builder.CacheMock.Verify(m => m.Add(
                    nameof(EventListingData),
                    It.Is<List<EventItem>>(o =>
                        o[1].Name == "Event 2 name"
                        && o[1].Link == "https://www.meetup.com/Chester-Devs/events/268297314/"
                        && o[1].Description == "Event 2 description"
                        && o[1].EventDate == new DateTime(2020, 03, 04)
                        && o[1].Time == "19:00"
                        && o[1].PhotoLink == null
                        && o[1].VenueName == "Venue 2 name"
                        && o[1].VenueCity == "Venue 2 town"),
                    It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Event 2 has not been parsed correctly");

            builder.FileSystemWrapperMock.Verify(m => m.SaveFile(It.IsAny<string>(), It.IsAny<string>()));
        }

        class Builder
        {
            public Mock<IAppCache> CacheMock = new Mock<IAppCache>();
            public HttpWrapperFake HttpWrapperFake = new HttpWrapperFake();
            public Mock<ILogger<EventListingData>> LoggerMock = new Mock<ILogger<EventListingData>>();
            public Mock<IFileSystemWrapper> FileSystemWrapperMock = new Mock<IFileSystemWrapper>();

            public Builder SetupHttpWrapper()
            {
                HttpWrapperFake.SetupGetData(File.OpenRead("Data/events.json"));
                return this;
            }

            public Builder SetupCache()
            {
                CacheMock.Setup(m => m.GetOrAdd<List<EventItem>>(nameof(EventListingData), It.IsAny<Func<ICacheEntry, List<EventItem>>>()))
                    .Returns(new List<EventItem>());

                return this;
            }

            public Builder SetupFileSystem()
            {
                FileSystemWrapperMock.Setup(m => m.ReadJson<List<EventItem>>(It.IsAny<string>()))
                    .Returns(new List<EventItem>());

                return this;
            }

            public EventListingData Build()
                => new EventListingData(HttpWrapperFake, CacheMock.Object, LoggerMock.Object, FileSystemWrapperMock.Object);
        }

    }
}