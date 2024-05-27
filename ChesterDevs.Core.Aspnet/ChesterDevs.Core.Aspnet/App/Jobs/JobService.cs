using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChesterDevs.Core.Aspnet.App.Jobs
{
    public interface IJobService
    {
        Task<JobListPage> GetJobsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<JobResponse> GetJobAsync(int jobId, CancellationToken cancellationToken);
        Task SubscribeAsync(string emailAddress, CancellationToken cancellationToken);

        Task AskARecruiterAsync(AskARecruiter askARecruiter, CancellationToken cancellationToken);
    }

    public class JobService : IJobService
    {
        //private const string API_URL = "https://localhost:5011";
        private const string API_URL = "https://cd-jobs.bluejumper.com";
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly ILogger<JobService> _logger;

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        public async Task<JobListPage> GetJobsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                using (var response = await
                    _httpClient.GetAsync($"{API_URL}/api/jobs?pageNumber={pageNumber}&pageSize={pageSize}",
                        cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    return DeserializeFromStream<JobListPage>(await response.Content.ReadAsStreamAsync());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error calling jobs api for list");
                return null;
            }
        }
        
        public async Task<JobResponse> GetJobAsync(int jobId, CancellationToken cancellationToken)
        {
            try
            {
                using (var response = await
                    _httpClient.GetAsync($"{API_URL}/api/jobs/{jobId}", cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    return DeserializeFromStream<JobResponse>(await response.Content.ReadAsStreamAsync());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error calling jobs api for detail");
                return null;
            }
        }

        
        public async Task SubscribeAsync(string emailAddress, CancellationToken cancellationToken)
        {
            try
            {
                var data = new NewSubscription()
                {
                    EmailAddress = emailAddress
                };

                using (var response = await
                    _httpClient.PostAsJsonAsync($"{API_URL}/api/email/subscribe", data, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error subscribing email");
            }
        }
        
        public async Task AskARecruiterAsync(AskARecruiter askARecruiter, CancellationToken cancellationToken)
        {
            try
            {
                using (var response = await
                    _httpClient.PostAsJsonAsync($"{API_URL}/api/email/askarecruiter", askARecruiter, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error subscribing email");
            }
        }


        private T DeserializeFromStream<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return _serializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}