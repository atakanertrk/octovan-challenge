using System.Collections.Generic;
using System.Threading.Tasks;

namespace OctovanAPI.Services
{
    public interface IBlobService
    {
        Task CreateContainerForTask(string taskId);
        Task DeleteBlobAsync(string blobName, string containerName);
        Task DeleteContainer(string containerName);
        Task<BlobInfo> GetBlobAsync(string blobName, string containerName);
        Task<IEnumerable<string>> ListBlobsAsync(string containerName);
        Task<IEnumerable<string>> ListBlobsUrlAsync(string containerName);
        Task UploadFileBlobAsync(string filePath, string fileName, string containerName);
    }
}