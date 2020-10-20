using System;
using System.Threading;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.RemoteData;
using ChesterDevs.Core.Aspnet.App.RemoteData.EventData;
using ChesterDevs.Core.Aspnet.App.RemoteData.YouTube;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChesterDevs.Core.Aspnet.BackgroundServices
{
    public class RefreshRemoteDataHostedService : IHostedService, IDisposable
    {
        private const int RefreshIntervalMinutes = 10;

        private readonly IRemoteDataSource _eventListingData;
        private readonly IRemoteDataSource _youTubeChannelVideo;
        private readonly ILogger<RefreshRemoteDataHostedService> _logger;
        private Timer _timer;

        public RefreshRemoteDataHostedService(ILogger<RefreshRemoteDataHostedService> logger, IEventListingData eventListingData, IYouTubeChannelVideos youTubeChannelVideo)
        {
            _logger = logger;
            _eventListingData = eventListingData;
            _youTubeChannelVideo = youTubeChannelVideo;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(RefreshRemoteDataHostedService)} running.");

            _timer = new Timer(RefreshData, null, TimeSpan.Zero, TimeSpan.Zero);

            return Task.CompletedTask;
        }

        private void RefreshData(object state)
        {
            _eventListingData.RefreshData();
            _youTubeChannelVideo.RefreshData();
            
            _timer.Change(TimeSpan.FromMinutes(RefreshIntervalMinutes), TimeSpan.Zero);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(RefreshRemoteDataHostedService)} stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}