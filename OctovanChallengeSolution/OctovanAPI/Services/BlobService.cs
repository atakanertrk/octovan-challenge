using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _blobStorageRootPath;
        public BlobService(BlobServiceClient blobServiceClient, IConfiguration config)
        {
            _blobServiceClient = blobServiceClient;
            _blobStorageRootPath = config.GetValue<string>("BlobStorageRootPath");
        }

        public async Task<BlobInfo> GetBlobAsync(string blobName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("taskid-" + containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var blobDownloadInfo = await blobClient.DownloadAsync();

            return new BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
        }

        public async Task DeleteBlobAsync(string blobName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("taskid-" + containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<IEnumerable<string>> ListBlobsAsync(string containerName)
        {
            string container = "taskid-" + containerName;
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var items = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                items.Add(blobItem.Name);
            }
            return items;
        }

        public async Task<IEnumerable<string>> ListBlobsUrlAsync(string containerName)
        {
            string container = "taskid-" + containerName;
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var items = new List<string>();
            string fullUrlOfBlob = _blobStorageRootPath + container;
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                items.Add(fullUrlOfBlob + @"/" + blobItem.Name);
            }
            return items;
        }

        public async Task UploadFileBlobAsync(string filePath, string fileName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("taskid-" + containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = "application/octet-stream" });
        }

        public async Task CreateContainerForTask(string taskId)
        {
            BlobServiceClient blobServiceClient = _blobServiceClient;
            string containerName = "taskid-" + taskId;
            // Create the container
            BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);
            container.SetAccessPolicy(PublicAccessType.BlobContainer);
            await container.ExistsAsync();
        }

    }
}
