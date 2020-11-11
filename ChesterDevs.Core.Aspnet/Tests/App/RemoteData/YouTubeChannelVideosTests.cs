using System;
using System.Collections.Generic;
using System.IO;
using ChesterDevs.Core.Aspnet.App;
using ChesterDevs.Core.Aspnet.App.RemoteData;
using ChesterDevs.Core.Aspnet.App.RemoteData.EventData;
using ChesterDevs.Core.Aspnet.App.RemoteData.YouTube;
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
    public class YouTubeChannelVideosTests
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
        public void Given_ApiKeyIsEmpty_When_RefreshDataCalled_Then_ApiIsNotCalled()
        {
            var builder = new Builder()
                .SetupHttpWrapper()
                .SetupSecrets();
            
            var objectUnderTest = builder.Build();

            objectUnderTest.RefreshData();

            builder.HttpWrapperFake.UrlRequests.Should().BeEmpty();
        }

        [Fact]
        public void Given_ApiKeyExists_When_RefreshDataCalled_Then_ApiCallContainsApiKep()
        {
            var builder = new Builder()
                .SetupHttpWrapper()
                .SetupSecrets("TEST_API_KEY");
            
            var objectUnderTest = builder.Build();

            objectUnderTest.RefreshData();

            builder.HttpWrapperFake.UrlRequests.Should().Contain(s => s.Contains("TEST_API_KEY"));
        }

        [Fact]
        public void Given_AllIsWell_When_RefreshDataCalled_Then_CorrectDataIsAddedToCache()
        {
            var builder = new Builder()
                .SetupHttpWrapper()
                .SetupSecrets("key");

            var objectUnderTest = builder.Build();

            objectUnderTest.RefreshData();

            builder.CacheMock.Verify(m => m.Add(
                    nameof(YouTubeChannelVideos),
                It.Is<List<YouTubeVideo>>(o =>
                    o != null
                    && o.Count == 2),
                It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Parsed video list should contain 2 videos");

            builder.CacheMock.Verify(m => m.Add(
                    nameof(YouTubeChannelVideos),
                It.Is<List<YouTubeVideo>>(o =>
                    o[0].VideoId == "YxJesR9Gp9s"
                    && o[0].Title == "Title 1"
                    && o[0].ThumbnailUrl == "https://i.ytimg.com/vi/YxJesR9Gp9s/hqdefault.jpg"),
                It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Video 1 has not been parsed correctly");

            builder.CacheMock.Verify(m => m.Add(
                    nameof(YouTubeChannelVideos),
                    It.Is<List<YouTubeVideo>>(o =>
                        o[1].VideoId == "FpBp6txmu-M"
                        && o[1].Title == "Title 2"
                        && o[1].ThumbnailUrl == "https://i.ytimg.com/vi/FpBp6txmu-M/hqdefault.jpg"),
                    It.IsAny<MemoryCacheEntryOptions>()),
                Times.Once,
                "Video 2 has not been parsed correctly");

            builder.FileSystemWrapperMock.Verify(m => m.SaveFile(It.IsAny<string>(), It.IsAny<string>()));
        }

        class Builder
        {
            public Mock<IAppCache> CacheMock = new Mock<IAppCache>();
            public HttpWrapperFake HttpWrapperFake = new HttpWrapperFake();
            public Mock<ILogger<YouTubeChannelVideos>> LoggerMock = new Mock<ILogger<YouTubeChannelVideos>>();
            public Mock<IFileSystemWrapper> FileSystemWrapperMock = new Mock<IFileSystemWrapper>();
            public Mock<ISecretManager> SecretManagerMock = new Mock<ISecretManager>();

            public Builder SetupHttpWrapper()
            {
                HttpWrapperFake.SetupGetData(File.OpenRead("Data/videos.json"));
                return this;
            }

            public Builder SetupCache()
            {
                CacheMock.Setup(m => m.GetOrAdd<List<YouTubeVideo>>(nameof(YouTubeChannelVideos), It.IsAny<Func<ICacheEntry, List<YouTubeVideo>>>()))
                    .Returns(new List<YouTubeVideo>());

                return this;
            }

            public Builder SetupSecrets(string apiKey = null)
            {
                SecretManagerMock.Setup(m => m.Secrets)
                    .Returns(new SecretData()
                    {
                        GoogleApiKey = apiKey
                    });
                return this;
            }

            public YouTubeChannelVideos Build()
                => new YouTubeChannelVideos(HttpWrapperFake, CacheMock.Object, LoggerMock.Object, FileSystemWrapperMock.Object, SecretManagerMock.Object);
        }

    }
}