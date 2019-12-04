using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.IO;

namespace sample_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await DoSomeWorkAsync();
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task DoSomeWorkAsync()
        {
            const string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://blob:10000/devstoreaccount1;";
            var blobServiceClient = new BlobServiceClient(connectionString);
            
            var containerName = "sample" + Guid.NewGuid();
            var blobContainerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            _logger.LogInformation($"Creating blob container:\n\t{containerName}");
            
            const string localPath = "./data/";
            const string fileName = "sample.txt";
            var localFilePath = Path.Combine(localPath, fileName);
            var blobClient = blobContainerClient.Value.GetBlobClient(fileName);
            _logger.LogInformation($"Uploading to blob storage as blob:\n\t{blobClient.Uri}");
            await blobClient.UploadAsync(localFilePath);            
        }
    }
}
