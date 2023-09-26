using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Magnus.Futbot.Storage
{
    public class AzureStorage
    {
        private readonly IConfiguration _configuration;

        public AzureStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadImage(Stream file, string saveAsFileName, string containerName)
        {
            var path = $"C:\\futbot\\images\\{containerName}";

            Directory.CreateDirectory(path);

            using var fileStream = File.Create($"{path}\\{saveAsFileName}");
            file.Seek(0, SeekOrigin.Begin);
            file.CopyTo(fileStream);

            return $"{containerName}\\{saveAsFileName}";


            var connectionString = _configuration["AzureStorageConnectionString"];
            var container = new BlobContainerClient(connectionString, containerName);

            try
            {
                await container.CreateIfNotExistsAsync();
                var blob = container.GetBlobClient(saveAsFileName);
                await blob.UploadAsync(file, true);

                return blob.Uri.AbsoluteUri;
            }
            catch
            {
                Console.WriteLine($"Error with uploading image {saveAsFileName} - {containerName}");
                return string.Empty;
            }
        }

        public async Task<List<string>> GetImagesByContainerName(string containerName)
        {
            return new List<string>();

            var connectionString = _configuration["AzureStorageConnectionString"];
            var container = new BlobContainerClient(connectionString, containerName);
            var blobs = new List<string>();

            try
            {
                await container.CreateIfNotExistsAsync();
                await foreach (BlobItem blobItem in container.GetBlobsAsync())
                    blobs.Add(blobItem.Name);
            }
            catch
            {
            }

            return blobs;
        }
    }
}